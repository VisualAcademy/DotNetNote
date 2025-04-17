using Azunt.Web.Infrastructures.Assets.Tenants;

namespace Azunt.Web.Infrastructures._Initializers;

public static class AssetSchemaInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AssetSchemaInitializer");

        var config = services.GetRequiredService<IConfiguration>();
        var masterConnectionString = config.GetConnectionString("DefaultConnection");

        // 테이블마다 forMaster 지정 (유연하게)
        InitializeProjectsMachinesTable(services, logger, forMaster: true); // 또는 false
        InitializeProjectsMediasTable(services, logger, forMaster: true); // 또는 false
    }

    private static void InitializeProjectsMachinesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureProjectsMachinesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 ProjectsMachines 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 ProjectsMachines 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeProjectsMediasTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureProjectsMediasTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 ProjectsMedias 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 ProjectsMedias 테이블 초기화 중 오류 발생");
        }
    }
}
