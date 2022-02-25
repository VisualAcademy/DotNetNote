using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Acts.Models
{
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
}
