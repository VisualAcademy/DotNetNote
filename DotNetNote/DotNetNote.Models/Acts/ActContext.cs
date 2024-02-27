using Microsoft.EntityFrameworkCore;

namespace Acts.Models;

public partial class ActContext : DbContext
{
    public ActContext()
    {
    }

    public ActContext(DbContextOptions<ActContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionCategory> ActionCategories { get; set; } = null!;

}
