using Azunt.NoteManagement;
using Azunt.SignInManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Azunt.Web.Infrastructures.Initializers
{
    /// <summary>
    /// 시스템 관련 테이블 초기화를 담당합니다.
    /// - 마스터/테넌트 DB의 필수 테이블을 생성/보강하고(없으면 생성, 누락 컬럼 추가),
    ///   필요 시 기본 데이터를 시드합니다.
    /// - 초기화 단계별 로깅/예외 처리/소요 시간 측정을 제공합니다.
    /// </summary>
    public static class SystemSchemaInitializer
    {
        private static readonly EventId EvStart = new(1000, nameof(SystemSchemaInitializer) + "_Start");
        private static readonly EventId EvDone = new(1001, nameof(SystemSchemaInitializer) + "_Done");
        private static readonly EventId EvStep = new(1010, nameof(SystemSchemaInitializer) + "_Step");
        private static readonly EventId EvErr = new(1099, nameof(SystemSchemaInitializer) + "_Error");

        /// <summary>
        /// 시스템 초기화 진입점입니다. Program.cs/Startup.cs 등에서 호출하세요.
        /// </summary>
        /// <param name="services">DI 컨테이너</param>
        public static void Initialize(IServiceProvider services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services), "services is null.");

            var loggerFactory = services.GetService<ILoggerFactory>()
                ?? throw new InvalidOperationException("ILoggerFactory is not registered.");
            var logger = loggerFactory.CreateLogger("SystemSchemaInitializer");

            logger.LogInformation(EvStart, "System schema initialization started.");

            // 마스터 DB 초기화
            InitializeTenantsTable(services, logger, forMaster: true);
            InitializePostsTable(services, logger, forMaster: true);
            InitializeNotesTable(services, logger, forMaster: true);
            InitializeSignInsTable(services, logger, forMaster: true);

            // ※ 필요 시 테넌트 DB에 대해서도 초기화를 수행하세요.
            // InitializeTenantsTable(services, logger, forMaster: false);
            // InitializePostsTable(services, logger, forMaster: false);
            // InitializeNotesTable(services, logger, forMaster: false);
            // InitializeSignInsTable(services, logger, forMaster: false);

            logger.LogInformation(EvDone, "System schema initialization completed.");
        }

        /// <summary>
        /// 공통 초기화 실행기. 소요 시간 측정/예외 처리/로깅을 일괄 제공합니다.
        /// </summary>
        private static void RunStep(ILogger logger, string stepName, bool forMaster, Action action)
        {
            var target = forMaster ? "마스터 DB" : "테넌트 DB";
            var sw = Stopwatch.StartNew();

            try
            {
                logger.LogInformation(EvStep, "[{Target}] {Step} 시작", target, stepName);
                action();
                sw.Stop();
                logger.LogInformation(EvStep, "[{Target}] {Step} 완료 (elapsed: {ElapsedMs} ms)", target, stepName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(EvErr, ex, "[{Target}] {Step} 중 오류 발생 (elapsed: {ElapsedMs} ms)", target, stepName, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Tenants 테이블 초기화(생성/보강 및 시드).
        /// </summary>
        private static void InitializeTenantsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            RunStep(logger, "Tenants 초기화", forMaster, () =>
            {
                TenantManagement.TenantsTableBuilder.Run(services, forMaster);

                // 마스터 DB 시 기본 테넌트 시드가 필요하다면 아래 호출을 해제하세요.
                // if (forMaster) TryInsertDefaultTenant(services, logger);
            });
        }

        /// <summary>
        /// 기본 테넌트 데이터 삽입을 시도합니다. (선택 사항)
        /// </summary>
        private static void TryInsertDefaultTenant(IServiceProvider services, ILogger logger)
        {
            var config = services.GetService<IConfiguration>();
            var connectionString = config?.GetConnectionString("DefaultConnection");
            var resolvedLogger = logger ?? services.GetService<ILoggerFactory>()?.CreateLogger("TenantSeeder");

            if (!string.IsNullOrWhiteSpace(connectionString) && resolvedLogger is not null)
            {
                TenantManagement.TenantSeeder.InsertDefaultTenant(connectionString, resolvedLogger);
                resolvedLogger.LogInformation("기본 테넌트 데이터 삽입 완료");
            }
            else
            {
                logger?.LogWarning("TenantSeeder 호출 실패: connectionString 또는 logger가 null입니다.");
            }
        }

        /// <summary>
        /// Posts 테이블 초기화(생성/보강 및 시드).
        /// </summary>
        private static void InitializePostsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            RunStep(logger, "Posts 초기화", forMaster, () =>
            {
                PostManagement.PostsTableBuilder.Run(services, forMaster);
            });
        }

        /// <summary>
        /// Notes 테이블 초기화(생성/보강 및 시드).
        /// </summary>
        private static void InitializeNotesTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            RunStep(logger, "Notes 초기화", forMaster, () =>
            {
                NotesTableBuilder.Run(services, forMaster);
            });
        }

        /// <summary>
        /// SignIns 테이블 초기화(생성/보강 및 시드).
        /// </summary>
        private static void InitializeSignInsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            RunStep(logger, "SignIns 초기화", forMaster, () =>
            {
                SignInsTableBuilder.Run(services, forMaster);
            });
        }
    }
}
