namespace MemberManagement.Data;

public class MemberDbContext(DbContextOptions<MemberDbContext> options) : DbContext(options)
{
    // Member 모델 클래스를 바탕으로 Members 테이블을 생성
    public DbSet<Member> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
