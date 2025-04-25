using Azunt.Infrastructures.Community;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Azunt.Web.Infrastructures._Initializers;

/// <summary>
/// 커뮤니티 관련 테이블 초기화를 담당하는 클래스입니다.
/// 예: Tabs, Menus, Navigation 등
/// </summary>
public static class CommunitySchemaInitializer
{
    /// <summary>
    /// 마스터 또는 테넌트 DB를 대상으로 커뮤니티 관련 테이블을 초기화합니다.
    /// </summary>
    /// <param name="services">DI 서비스 컨테이너</param>
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("CommunitySchemaInitializer");

        var config = services.GetRequiredService<IConfiguration>();
        var masterConnectionString = config.GetConnectionString("DefaultConnection");

        // 필요 시, forMaster: false로 변경하여 테넌트 대상 처리도 가능
        //InitializeTabsTable(services, logger, forMaster: true);

        InitializeAttachFilesTable(services, logger, forMaster: true);

        //InitializeMailListTable(services, logger, forMaster: true);
    }

    /// <summary>
    /// Tabs 테이블을 마스터 또는 테넌트 DB에 생성 및 초기화합니다.
    /// </summary>
    //private static void InitializeTabsTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";

    //    try
    //    {
    //        TenantSchemaEnhancerEnsureTabsTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 Tabs 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 Tabs 테이블 초기화 중 오류 발생");
    //    }
    //}

    private static void InitializeAttachFilesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureAttachFilesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 AttachFiles 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 AttachFiles 테이블 초기화 중 오류 발생");
        }
    }

    //private static void InitializeMailListTable(IServiceProvider services, ILogger logger, bool forMaster)
    //{
    //    string target = forMaster ? "마스터 DB" : "테넌트 DB";

    //    try
    //    {
    //        TenantSchemaEnhancerEnsureMailListTable.Run(services, forMaster);
    //        logger.LogInformation($"{target}의 MailList 테이블 초기화 완료");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"{target}의 MailList 테이블 초기화 중 오류 발생");
    //    }
    //}

}
