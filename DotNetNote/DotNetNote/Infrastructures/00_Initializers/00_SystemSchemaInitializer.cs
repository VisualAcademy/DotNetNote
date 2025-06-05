using Azunt.NoteManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Azunt.Web.Infrastructures.Initializers
{
    /// <summary>
    /// 시스템 관련 테이블 초기화 작업을 담당하는 초기화 클래스입니다.
    /// - 마스터 DB 및 테넌트 DB의 시스템 테이블을 생성 및 보강합니다.
    /// - 기본 데이터를 삽입합니다.
    /// </summary>
    public static class SystemSchemaInitializer
    {
        /// <summary>
        /// 초기화 진입점입니다. Program.cs 또는 Startup.cs에서 호출됩니다.
        /// - Tenants 테이블 초기화
        /// </summary>
        /// <param name="services">DI 컨테이너 서비스 프로바이더</param>
        public static void Initialize(IServiceProvider services)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SystemSchemaInitializer");

            // 마스터 DB 및 테넌트 DB에서 Tenants 테이블 생성 및 보강
            InitializeTenantsTable(services, logger, forMaster: true);
            InitializePostsTable(services, logger, forMaster: true);
            InitializeNotesTable(services, logger, forMaster: true);
        }

        /// <summary>
        /// Tenants 테이블을 초기화합니다.
        /// </summary>
        /// <param name="services">서비스 프로바이더</param>
        /// <param name="logger">로거</param>
        /// <param name="forMaster">마스터 DB 대상 여부</param>
        private static void InitializeTenantsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                TenantManagement.TenantsTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Tenants 테이블 초기화 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Tenants 테이블 초기화 중 오류 발생");
            }
        }

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
