using DotNetNote.Pages.TextMessagePages.Codes;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace VisualAcademy.Models.TextMessages
{
    /// <summary>
    /// TextMessageApp에서 사용하는 데이터베이스 컨텍스트 클래스입니다.
    /// 이 클래스는 Entity Framework Core와 데이터베이스 간의 브리지 역할을 합니다.
    /// </summary>
    public class TextMessageAppDbContext : DbContext
    {
        /// <summary>
        /// 기본 생성자. 쿼리 트래킹 동작을 NoTracking으로 설정합니다.
        /// </summary>
        public TextMessageAppDbContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// DbContextOptions을 인자로 받는 생성자입니다.
        /// 이 생성자는 Dependency Injection에 의해 호출되며, 
        /// 주로 Startup.cs에서 DBContext를 services에 등록할 때 사용됩니다.
        /// </summary>
        public TextMessageAppDbContext(DbContextOptions<TextMessageAppDbContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// 데이터베이스 연결 설정을 담당하는 메서드입니다.
        /// 이 메서드는 DbContext가 처음 생성될 때 호출됩니다.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // .NET Framework 시절에 Web.config 또는 App.config 파일에서 데이터베이스 연결 문자열 값 가져오는 기본 코드
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /// <summary>
        /// 데이터베이스 모델을 설정하는 메서드입니다.
        /// 이 메서드는 DbContext가 처음 생성될 때 호출됩니다.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TextMessages 테이블의 Created, PostDate 열은 자동으로 GetDate() 제약 조건을 부여하기 
            //modelBuilder.Entity<TextMessageModel>().Property(m => m.CreatedAt).HasDefaultValueSql("GetDate()");
        }

        /// <summary>
        /// TextMessageApp 솔루션 관련 모든 테이블에 대한 참조 
        /// </summary>
        public DbSet<TextMessageModel> TextMessages { get; set; } = null!;

        /// <summary>
        /// Contacts 테이블에 대한 참조
        /// </summary>
        public DbSet<ContactModelForTextMessage> Contacts { get; set; } = null!;

        /// <summary>
        /// Employees 테이블에 대한 참조
        /// </summary>
        public DbSet<EmployeeModelForTextMessage> Employees { get; set; } = null!;
    }
}
