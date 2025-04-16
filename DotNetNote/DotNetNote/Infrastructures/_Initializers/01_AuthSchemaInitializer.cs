using Azunt.Infrastructures.Auth;

namespace Azunt.Web.Infrastructures._Initializers;

public static class AuthSchemaInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AuthSchemaInitializer");

        InitializeRolesTable(services, logger, forMaster: true); // InitializeRolesTable(services, logger, forMaster: false);

        InitializeUsersTable(services, logger, forMaster: true); // InitializeUsersTable(services, logger, forMaster: false);

        InitializeDefaultUsers(services, logger);
    }

    private static void InitializeRolesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureRolesTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 AspNetRoles 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 AspNetRoles 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeUsersTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            TenantSchemaEnhancerEnsureUsersTable.Run(services, forMaster);
            logger.LogInformation($"{target}의 AspNetUsers 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 AspNetUsers 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeDefaultUsers(IServiceProvider services, ILogger logger)
    {
        try
        {
            using (var scope = services.CreateScope())
            {
                TenantSchemaEnhancerEnsureDefaultUsers.RunAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                logger.LogInformation("기본 사용자 초기화 완료");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "기본 사용자 초기화 중 오류 발생");
        }
    }
}
