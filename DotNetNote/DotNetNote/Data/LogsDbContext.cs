namespace DotNetNote.Data
{
    public class LogsDbContext : DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }
        public DbSet<AppLog> AppLogs => Set<AppLog>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<AppLog>(e =>
            {
                e.ToTable("AppLogs");   // dbo.AppLogs
                e.HasNoKey();           // 키 없음
                e.HasIndex(nameof(AppLog.TimeStamp));
                e.HasIndex(nameof(AppLog.Level));
            });
        }
    }
}
