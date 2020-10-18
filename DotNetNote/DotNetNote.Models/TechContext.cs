using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace DotNetNote.Models
{
    public class TechContext : DbContext
    {
        public TechContext()
        {
            // Empty
        }

        public TechContext(DbContextOptions<TechContext> options)
            : base(options)
        {
            // 공식과 같은 코드 
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager
                    .ConnectionStrings["ConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /// <summary>
        /// 기술 리스트: [실습] Teches 테이블부터 Angular 앱 또는 Blazor 앱까지
        /// </summary>
        public DbSet<Tech> Teches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tech>().HasData(
                new Tech { Id = 1, Title = "ASP.NET" }, 
                new Tech { Id = 2, Title = "Blazor" });
        }
    }
}
