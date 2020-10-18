using DotNetNote.Models.Companies;
using Microsoft.EntityFrameworkCore;

namespace DotNetNote.Models.Companies
{
    public class CompanyContext : DbContext
    {
        //public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
        //{

        //}

        public DbSet<CompanyModel> Companies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DotNetNote;Integrated Security=True;");
        }
    }
}
