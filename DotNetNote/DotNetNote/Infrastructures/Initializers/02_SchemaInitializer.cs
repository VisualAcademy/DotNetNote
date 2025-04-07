using Azunt.Infrastructures.Tenants;

namespace Hawaso.Infrastructures.Initializers;

public static class SchemaInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SchemaInitializer");

        var config = services.GetRequiredService<IConfiguration>();
        var masterConnectionString = config.GetConnectionString("DefaultConnection");

        // LicenseTypes 테이블 생성/수정
        InitializeLicenseTypesTable(services, logger, forMaster: true); // InitializeLicenseTypesTable(services, logger, forMaster: false);
        //InitializeContactTypesTable(services, logger, forMaster: true); // 또는 true
        //InitializeAllsTable(services, logger, forMaster: true); // Alls 테이블
        InitializeLicenseStatusesTable(services, logger, forMaster: true); // LicenseStatuses 테이블
    }

    private static void InitializeLicenseTypesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureLicenseTypesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 LicenseTypes 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 LicenseTypes 테이블 초기화 중 오류 발생");
        }
    }

    //private static void InitializeContactTypesTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";

    //    try
    //    {
    //        TenantSchemaEnhancerEnsureContactTypesTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 ContactTypes 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 ContactTypes 테이블 초기화 중 오류 발생");
    //    }
    //}

    //private static void InitializeAllsTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";

    //    try
    //    {
    //        TenantSchemaEnhancerEnsureAllsTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 Alls 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 Alls 테이블 초기화 중 오류 발생");
    //    }
    //}

    private static void InitializeLicenseStatusesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureVendorLicenseStatusesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 LicenseStatuses 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 LicenseStatuses 테이블 초기화 중 오류 발생");
        }
    }
}
