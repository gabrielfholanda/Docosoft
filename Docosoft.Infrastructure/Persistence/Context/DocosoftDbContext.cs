using Docosoft.Domain.Entities;
using Docosoft.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Infrastructure.Persistence.Context
{
    public class DocosoftDbContext : DbContext
    {
        public DocosoftDbContext(DbContextOptions<DocosoftDbContext> options)
       : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
