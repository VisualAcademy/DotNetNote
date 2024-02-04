using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DotNetNote.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CabinetType> CabinetTypes { get; set; } = null!;

        #region Cascading DropDownList 
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Sublocation> Sublocations { get; set; } = null!; 
        #endregion
    }
}
