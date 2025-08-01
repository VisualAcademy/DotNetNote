using Azunt.BackgroundCheckManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azunt.Web.Infrastructures.Initializers;

/// <summary>
/// 직원 관련 테이블을 초기화하는 클래스입니다.
/// 예: EligibilityTypes 등 직원 결정 및 자격 관련 정보 테이블 포함
/// </summary>
public static class EmployeeSchemaInitializer
{
    /// <summary>
    /// 직원 관련 테이블 초기화의 진입점입니다. Program.cs 또는 Startup.cs에서 호출됩니다.
    /// </summary>
    /// <param name="services">DI 컨테이너 서비스 프로바이더</param>
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("EmployeeSchemaInitializer");

        // forMaster: true 로 마스터 DB에만 적용 (필요 시 false 로 테넌트 확장 가능)
        //InitializeEligibilityTypesTable(services, logger, forMaster: true);
        InitializeBackgroundChecksTable(services, logger, forMaster: true);
        InitializeBranchesTable(services, logger, forMaster: true);

        InitializeDepartmentsTable(services, logger, forMaster: true);  // 마스터 DB
    }

    private static void InitializeBackgroundChecksTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";
        try
        {
            BackgroundChecksTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 BackgroundChecks 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 BackgroundChecks 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeBranchesTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            Azunt.BranchManagement.BranchesTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 Branches 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 Branches 테이블 초기화 중 오류 발생");
        }
    }

    private static void InitializeDepartmentsTable(IServiceProvider services, ILogger logger, bool forMaster)
    {
        string target = forMaster ? "마스터 DB" : "테넌트 DB";

        try
        {
            Azunt.DepartmentManagement.DepartmentsTableBuilder.Run(services, forMaster);
            logger.LogInformation($"{target}의 Departments 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{target}의 Departments 테이블 초기화 중 오류 발생");
        }
    }
}
