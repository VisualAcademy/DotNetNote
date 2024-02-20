namespace DotNetNote
{
    public class AccountContext(DbContextOptions<AccountContext> options) : DbContext(options)
    {
        public DbSet<SignBase> SignBases { get; set; }
    }
}
