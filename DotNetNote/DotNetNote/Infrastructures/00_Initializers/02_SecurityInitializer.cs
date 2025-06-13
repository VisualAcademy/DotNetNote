using Azunt.ResourceManagement;
using Azunt.Web.Infrastructures.All;

namespace Azunt.Web.Infrastructures.Initializers;

public static class SecurityInitializer
{
    public static void Initialize(IServiceProvider services, bool forMaster)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SecurityInitializer");

        InitializeResourcesTable(services, logger, forMaster);
        InitializeRulesTable(services, logger, forMaster);
        InitializeAllowedIpRangesTable(services, logger, forMaster);
    }

    private static void InitializeResourcesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";
        try
        {
            // 1. 테이블 생성 및 컬럼 보강
            ResourcesTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 Resources 테이블 스키마 보강 완료");

            // 2. 시드 데이터 삽입 (단일 appName 또는 전체)
            var configuration = services.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // appName을 구분하여 시드
            ResourceSeeder.InsertRequiredResources(connectionString, logger, appName: null); // 전체
            ResourceSeeder.InsertRequiredResources(connectionString, logger, appName: "VisualAcademy");

            logger.LogInformation($"{target}의 Resources 시드 데이터 삽입 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 Resources 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeRulesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";
        try
        {
            Azunt.RuleManagement.RulesTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 Rules 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 Rules 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeAllowedIpRangesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";
        try
        {
            TenantSchemaEnhancerEnsureAllowedIpRangesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 AllowedIpRanges 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 AllowedIpRanges 테이블 초기화 중 오류 발생");
        }
    }
}
