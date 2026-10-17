using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_qrmenu.Entity;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qrmenu.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ProductName).HasMaxLength(150).IsRequired();
                entity.Property(p => p.Image).HasMaxLength(500).IsRequired();
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.CategoryName).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Slug).HasMaxLength(50).IsRequired();
                entity.HasIndex(c => c.Slug).IsUnique();
            });
        }
    }
}