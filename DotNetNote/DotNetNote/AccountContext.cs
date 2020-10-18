using DotNetNote.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetNote
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {

        }

        public DbSet<SignBase> SignBases { get; set; }
    }
}
