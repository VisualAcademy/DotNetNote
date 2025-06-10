using Azunt.BannedCustomerManagement;
using Azunt.IncidentManagement;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Azunt.Web.Infrastructures.Initializers
{
    /// <summary>
    /// ReportWriter 관련 테이블 초기화 작업을 담당하는 초기화 클래스입니다.
    /// - 마스터 DB 및 테넌트 DB의 시스템 테이블을 생성 및 보강합니다.
    /// - 기본 데이터를 삽입합니다.
    /// </summary>
    public static class ReportWriterSchemaInitializer
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

            InitializeIncidentsTable(services, logger, forMaster: true);
            InitializeIncidentTableEnhancer(services, logger, forMaster: true);
            InitializeBannedTypesTable(services, logger, forMaster: true);
            InitializeBannedCustomersTable(services, logger, forMaster: true);
        }

        private static void InitializeIncidentsTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                IncidentsTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Incidents 테이블 생성 작업 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Incidents 테이블 생성 작업 중 오류 발생");
            }
        }

        private static void InitializeIncidentTableEnhancer(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                Azunt.IncidentManagement.IncidentTableEnhancer.Run(services, forMaster);
                logger.LogInformation($"{target}의 Incidents 테이블 확장 작업 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Incidents 테이블 확장 작업 중 오류 발생");
            }
        }

        private static void InitializeBannedTypesTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                BannedTypesTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 BannedTypes 테이블 초기화 작업 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 BannedTypes 테이블 초기화 작업 중 오류 발생");
            }
        }

        private static void InitializeBannedCustomersTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                BannedCustomersTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 BannedCustomers 테이블 초기화 작업 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 BannedCustomers 테이블 초기화 작업 중 오류 발생");
            }
        }
    }
}
