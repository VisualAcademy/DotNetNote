using DotNetNote.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetNote
{
    public class DashboardContext : DbContext
    {
        public DbSet<Twelve> Twelves { get; set; }

        public DashboardContext()
        {
            // Empty
        }

        /// <summary>
        /// 생성자 매개변수에 옵션값 전달
        /// </summary>
        public DashboardContext(DbContextOptions<DashboardContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
