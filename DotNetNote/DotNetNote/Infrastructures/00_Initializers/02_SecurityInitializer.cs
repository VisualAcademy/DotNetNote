using Azunt.Infrastructures.Auth;
using Azunt.Web.Infrastructures.All;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azunt.Web.Infrastructures._Initializers;

public static class SecurityInitializer
{
    public static void Initialize(IServiceProvider services, bool forMaster)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SecurityInitializer");

        InitializeResourcesTable(services, logger, forMaster);
        //InitializeRulesTable(services, logger, forMaster);
        //InitializeAllowedIpRangesTable(services, logger, forMaster);
    }

    private static void InitializeResourcesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";
        try
        {
            Azunt.ResourceManagement.ResourcesTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 Resources 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 Resources 테이블 초기화 중 오류 발생");
        }
    }

    //private static void InitializeRulesTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";
    //    try
    //    {
    //        TenantSchemaEnhancerEnsureRulesTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 Rules 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 Rules 테이블 초기화 중 오류 발생");
    //    }
    //}

    //private static void InitializeAllowedIpRangesTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";
    //    try
    //    {
    //        TenantSchemaEnhancerEnsureAllowedIpRangesTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 AllowedIpRanges 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 AllowedIpRanges 테이블 초기화 중 오류 발생");
    //    }
    //}
}
