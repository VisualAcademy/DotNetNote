namespace Azunt.Web.Infrastructures._00_Initializers
{
    public static class VisualAcademySchemaInitializer
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

            InitializeArticlesTable(services, logger, forMaster: true);
        }

        private static void InitializeArticlesTable(IServiceProvider services, ILogger logger, bool forMaster)
        {
            string target = forMaster ? "마스터 DB" : "테넌트 DB";

            try
            {
                Azunt.ArticleManagement.ArticlesSqlServerTableBuilder.Run(services, forMaster);
                logger.LogInformation($"{target}의 Articles 테이블 초기화 완료");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{target}의 Articles 테이블 초기화 중 오류 발생");
            }
        }
    }
}
