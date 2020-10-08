//Install-Package Microsoft.EntityFrameworkCore
//Install-Package Microsoft.EntityFrameworkCore.SqlServer
//Install-Package Microsoft.EntityFrameworkCore.Tools
//Install-Package System.Configuration.ConfigurationManager
//Install-Package Microsoft.Data.SqlClient
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace DotNetNote.Models
{
    public class CompanyContext : DbContext
    {
        public CompanyContext()
        {
            // Empty
        }

        public CompanyContext(DbContextOptions<CompanyContext> options)
            : base(options)
        {
            // 공식과 같은 코드 
        }

        //// 강의에서 사용한 형태 
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DotNetNote;Integrated Security=True;");
        //}
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            // 닷넷 프레임워크 기반에서 호출되는 코드 영역: 
            // App.Config 또는 Web.Config의 연결 문자열 사용
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager.ConnectionStrings[
                    "DefaultConnection"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<CompanyModel> Companies { get; set; }
    }
}
