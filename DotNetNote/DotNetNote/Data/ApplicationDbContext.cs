using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DotNetNote.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<CabinetType> CabinetTypes { get; set; } = null!;

    #region Cascading DropDownList 
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Sublocation> Sublocations { get; set; } = null!;
    #endregion

    public DbSet<KnownUser> KnownUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnownUser>().ToTable("KnownUsers");
    }
}
