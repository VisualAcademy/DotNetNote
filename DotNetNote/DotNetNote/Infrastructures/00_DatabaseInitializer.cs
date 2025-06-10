using Azunt.Web.Infrastructures._Initializers;
using Azunt.Web.Infrastructures.Initializers;

namespace Azunt.Web.Infrastructures;

public static class DatabaseInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseInitializer");

        try
        {
            // 0. 시스템
            SystemSchemaInitializer.Initialize(services);
            logger.LogInformation("시스템 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "시스템 테이블 초기화 중 오류 발생");
        }

        try
        {
            // 1. 인증 및 사용자 초기화
            AuthSchemaInitializer.Initialize(services);
            logger.LogInformation("인증 및 사용자 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "인증 및 사용자 초기화 중 오류 발생");
        }

        try
        {
            // 2. 보안(Security) 관련 테이블 초기화
            SecurityInitializer.Initialize(services, forMaster: true);
            logger.LogInformation("보안 관련 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "보안 관련 테이블 초기화 중 오류 발생");
        }

        try
        {
            // 3. 공통 테이블 초기화
            SchemaInitializer.Initialize(services);
            logger.LogInformation("공통 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "공통 테이블 초기화 중 오류 발생");
        }

        try
        {
            // 4. 자산(Asset) 관련 테이블 초기화
            AssetSchemaInitializer.Initialize(services);
            logger.LogInformation("자산 관련 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "자산 관련 테이블 초기화 중 오류 발생");
        }

        try
        {
            // 5. 커뮤니티 관련 테이블 초기화
            CommunitySchemaInitializer.Initialize(services);
            logger.LogInformation("커뮤니티 관련 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "커뮤니티 관련 테이블 초기화 중 오류 발생");
        }

        //try
        //{
        //    // 6. 직원(Employee) 관련 테이블 초기화
        //    EmployeeSchemaInitializer.Initialize(services);
        //    logger.LogInformation("직원 관련 테이블 초기화 완료");
        //}
        //catch (Exception ex)
        //{
        //    logger.LogError(ex, "직원 관련 테이블 초기화 중 오류 발생");
        //}

        try
        {
            // 7. 보고서(ReportWriter) 관련 테이블 초기화
            ReportWriterSchemaInitializer.Initialize(services);
            logger.LogInformation("보고서 관련 테이블 초기화 완료");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "보고서 관련 테이블 초기화 중 오류 발생");
        }

        logger.LogInformation("전체 데이터베이스 초기화 완료");
    }
}
