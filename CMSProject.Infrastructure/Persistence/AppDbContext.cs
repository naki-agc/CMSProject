using CMSProject.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Content> Contents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContentVariant> ContentVariants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Contents)
                .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<Content>()
                .HasMany(c => c.Variants)
                .WithOne(v => v.Content)
                .HasForeignKey(v => v.ContentId);

            modelBuilder.Entity<Content>()
                .HasMany(u => u.User)
                .WithMany(c => c.Contents);
        }
    }
}
