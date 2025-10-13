using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Azunt.Pages.TextMessagePages.Codes
{
    /// <summary>
    /// 각 테넌트 DB에 dbo.TextMessages 테이블을 보장하는 Enhancer.
    /// </summary>
    public class TenantSchemaEnhancerCreateTextMessagesTable
    {
        private readonly string _masterConnectionString;
        private readonly ILogger<TenantSchemaEnhancerCreateTextMessagesTable> _logger;

        // 커맨드 타임아웃(초)
        private const int TimeoutCheckSeconds = 30;
        private const int TimeoutCreateSeconds = 120;

        public TenantSchemaEnhancerCreateTextMessagesTable(
            string masterConnectionString,
            ILogger<TenantSchemaEnhancerCreateTextMessagesTable> logger)
        {
            if (string.IsNullOrWhiteSpace(masterConnectionString))
                throw new ArgumentException("Master connection string is null or empty.", nameof(masterConnectionString));

            _masterConnectionString = masterConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// 모든 테넌트 DB에 TextMessages 테이블이 없으면 생성.
        /// </summary>
        public async Task EnhanceAllTenantDatabasesAsync(CancellationToken ct = default)
        {
            var tenantConnInfos = await GetTenantConnectionStringsAsync(ct);

            foreach (var info in tenantConnInfos)
            {
                if (ct.IsCancellationRequested) break;

                // 1) C# 측 가드: null/공백/형식오류는 스킵 + 로그
                if (!TryValidateConnectionString(info.ConnectionString, out var validationError))
                {
                    _logger.LogWarning("[Skip] TenantId={TenantId} invalid connection string: {Error}",
                        info.TenantId, validationError);
                    continue;
                }

                try
                {
                    await CreateTextMessagesTableIfNotExistsAsync(info.ConnectionString, ct);
                    _logger.LogInformation("[OK]   TenantId={TenantId} TextMessages ensured.", info.TenantId);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Enhancement cancelled while processing TenantId={TenantId}.", info.TenantId);
                    throw;
                }
                catch (Exception ex)
                {
                    // 2) 테넌트별로 예외를 삼켜서 전체 진행 계속
                    _logger.LogError(ex, "[Fail] TenantId={TenantId}: {Message}", info.TenantId, ex.Message);
                }
            }
        }

        private sealed record TenantConnInfo(long TenantId, string ConnectionString);

        private async Task<List<TenantConnInfo>> GetTenantConnectionStringsAsync(CancellationToken ct)
        {
            var result = new List<TenantConnInfo>();

            using (var connection = new SqlConnection(_masterConnectionString))
            {
                await connection.OpenAsync(ct);

                // 3) DB 측 필터: NULL/공백, (있으면) 비활성 제외
                var sql = @"
SELECT Id, ConnectionString
FROM dbo.Tenants WITH (NOLOCK)
WHERE ConnectionString IS NOT NULL
  AND LTRIM(RTRIM(ConnectionString)) <> ''
  -- AND IsActive = 1  -- 컬럼 있으면 권장
";

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = TimeoutCheckSeconds;

                    using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        while (await reader.ReadAsync(ct))
                        {
                            var id = reader.GetInt64(0);
                            var cs = reader.GetString(1)?.Trim();
                            if (!string.IsNullOrWhiteSpace(cs))
                                result.Add(new TenantConnInfo(id, cs));
                        }
                    }
                }
            }

            return result;
        }

        private static bool TryValidateConnectionString(string cs, out string? error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(cs))
            {
                error = "empty";
                return false;
            }

            try
            {
                var b = new SqlConnectionStringBuilder(cs);

                // 필수 키 존재 여부(환경에 맞게 강화 가능)
                if (string.IsNullOrWhiteSpace(b.DataSource?.ToString()))
                {
                    error = "Data Source missing";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(b.InitialCatalog))
                {
                    error = "Initial Catalog missing";
                    return false;
                }
                // 통합인증 또는 SQL 인증 중 하나가 유효하면 OK
                // (필요 시 UserID/Password 필수화 가능)
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private async Task CreateTextMessagesTableIfNotExistsAsync(string tenantConnectionString, CancellationToken ct)
        {
            using (var connection = new SqlConnection(tenantConnectionString))
            {
                await connection.OpenAsync(ct);

                // 존재 여부: OBJECT_ID 사용 → 가볍고 명확
                const string checkSql = @"SELECT CASE WHEN OBJECT_ID(N'dbo.TextMessages', N'U') IS NULL THEN 0 ELSE 1 END;";

                using (var cmdCheck = new SqlCommand(checkSql, connection))
                {
                    cmdCheck.CommandType = CommandType.Text;
                    cmdCheck.CommandTimeout = TimeoutCheckSeconds;

                    var exists = Convert.ToInt32(await cmdCheck.ExecuteScalarAsync(ct)) == 1;
                    if (exists) return;
                }

                // 없으면 생성(기본값 + 인덱스 포함)
                var createSql = @"
IF OBJECT_ID(N'dbo.TextMessages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TextMessages
    (
        ID               BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TextMessages PRIMARY KEY,
        EmployeeID       BIGINT NOT NULL,
        [Message]        NVARCHAR(MAX) NOT NULL,
        DateSent         DATETIMEOFFSET(7) NOT NULL 
                         CONSTRAINT DF_TextMessages_DateSent DEFAULT (SYSUTCDATETIME()),
        TextMessageType  INT NULL
    );

    CREATE INDEX IX_TextMessages_EmployeeID ON dbo.TextMessages(EmployeeID);
    CREATE INDEX IX_TextMessages_DateSent   ON dbo.TextMessages(DateSent);
END
";

                using (var cmdCreate = new SqlCommand(createSql, connection))
                {
                    cmdCreate.CommandType = CommandType.Text;
                    cmdCreate.CommandTimeout = TimeoutCreateSeconds;
                    await cmdCreate.ExecuteNonQueryAsync(ct);
                }
            }
        }
    }
}
