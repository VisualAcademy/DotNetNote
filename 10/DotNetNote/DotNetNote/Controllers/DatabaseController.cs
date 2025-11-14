using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DotNetNote.Controllers
{
    [ApiController]
    [Route("database")]
    public class DatabaseController : ControllerBase
    {
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=DotNetNote;Trusted_Connection=True;";

        public class Book
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [HttpGet("ado")]
        public async Task<IActionResult> GetByAdoNet()
        {
            var list = new List<Book>();

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            const string sql = "SELECT Id, Name FROM dbo.Books ORDER BY Id;";
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                list.Add(new Book
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return Ok(list); // JSON 반환
        }

        // Dapper
        [HttpGet("dapper")]
        public async Task<IActionResult> GetByDapper()
        {
            await using var conn = new SqlConnection(ConnectionString);
            var books = await conn.QueryAsync<Book>(
                "SELECT Id, Name FROM dbo.Books ORDER BY Id;");
            return Ok(books); // JSON 반환
        }

        // EF Core용 경량 DbContext
        public class BooksDbContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(ConnectionString);

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Book>(e =>
                {
                    e.ToTable("Books");
                    e.HasKey(x => x.Id);
                    e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                });
            }

            public DbSet<Book> Books => Set<Book>();
        }

        [HttpGet("ef")]
        public async Task<IActionResult> GetByEfCore()
        {
            await using var db = new BooksDbContext();
            var books = await db.Books
                .OrderBy(b => b.Id)
                .AsNoTracking()
                .ToListAsync();

            return Ok(books); // JSON 반환
        }
    }
}
