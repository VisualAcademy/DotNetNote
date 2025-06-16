using Azunt.NoteManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Azunt.Web.Infrastructures.Initializers
{
    /// <summary>
    /// 시스템 관련 테이블 초기화 작업을 담당하는 초기화 클래스입니다.
    /// - 마스터 DB 및 테넌트 DB의 시스템 테이블을 생성 및 보강합니다.
    /// - 필요한 경우 기본 데이터를 삽입합니다.
    /// </summary>
    public static class SystemSchemaInitializer
    {
        /// <summary>
        /// 시스템 초기화 진입점입니다.
        /// Program.cs 또는 Startup.cs에서 호출됩니다.
        /// </summary>
        /// <param name="services">DI 컨테이너 서비스 프로바이더</param>
        public static void Initialize(IServiceProvider services)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SystemSchemaInitializer");

            // 마스터 DB를 대상으로 테이블 초기화 수행
            InitializeTenantsTable(services, logger, forMaster: true);
            InitializePostsTable(services, logger, forMaster: true);
            InitializeNotesTable(services, logger, forMaster: true);
        }

        /// <summary>
        /// Tenants 테이블을 초기화합니다.
        /// 생성 및 컬럼 보강 후, 마스터 DB일 경우 기본 테넌트 데이터 삽입까지 수행합니다.
        /// </summary>
        private static void InitializeTenantsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                // 1. 테이블 생성 및 컬럼 보강
                TenantManagement.TenantsTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Tenants 테이블 초기화 완료");

                // 2. 마스터 DB일 경우 기본 테넌트 시드 데이터 삽입
                if (forMaster)
                {
                    TryInsertDefaultTenant(services, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Tenants 테이블 초기화 중 오류 발생");
            }
        }

        /// <summary>
        /// 기본 테넌트 데이터 삽입을 시도합니다.
        /// Insert 조건과 로거를 안전하게 처리합니다.
        /// </summary>
        private static void TryInsertDefaultTenant(IServiceProvider services, ILogger logger)
        {
            var config = services.GetService<IConfiguration>();
            var connectionString = config?.GetConnectionString("DefaultConnection");

            var resolvedLogger = logger ?? services.GetService<ILoggerFactory>()?.CreateLogger("TenantSeeder");

            if (!string.IsNullOrWhiteSpace(connectionString) && resolvedLogger != null)
            {
                TenantManagement.TenantSeeder.InsertDefaultTenant(connectionString, resolvedLogger);
            }
            else
            {
                logger?.LogWarning("TenantSeeder를 호출하지 못했습니다: connectionString 또는 logger가 null입니다.");
            }
        }

        /// <summary>
        /// Posts 테이블을 초기화합니다.
        /// </summary>
        private static void InitializePostsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                PostManagement.PostsTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Posts 테이블 초기화 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Posts 테이블 초기화 중 오류 발생");
            }
        }

        /// <summary>
        /// Notes 테이블을 초기화합니다.
        /// </summary>
        private static void InitializeNotesTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                NotesTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Notes 테이블 초기화 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Notes 테이블 초기화 중 오류 발생");
            }
        }
    }
}
