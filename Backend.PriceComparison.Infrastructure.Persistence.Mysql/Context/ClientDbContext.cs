using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context
{
    public class ClientDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<CategoryProductEntity> CategoryProducts { get; set; }
        public DbSet<CategoryStoreEntity> CategoryStores { get; set; }
        public DbSet<StoreEntity> Stores { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<SaleEntity> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            StoreEntityConfiguration(modelBuilder);
        }

        private static void StoreEntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryProductEntity>(entity =>
            {
                entity.ToTable("category_product");
                entity.Property(e => e.Id).HasColumnName("id_category_product");
                entity.Property(e => e.Description).HasColumnName("descrption");
            });

            modelBuilder.Entity<CategoryStoreEntity>(entity =>
            {
                entity.ToTable("category_store");
                entity.Property(e => e.Id).HasColumnName("id_category_store");
                entity.Property(e => e.Description).HasColumnName("description");
            });

            modelBuilder.Entity<StoreEntity>(entity =>
            {
                entity.ToTable("store");
                entity.Property(e => e.Id).HasColumnName("id_store");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.CategoryStoreId).HasColumnName("id_category_store");
                entity.HasOne(e => e.CategoryStore).WithMany().HasForeignKey(e => e.CategoryStoreId);
            });

            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.ToTable("product");
                entity.Property(e => e.Id).HasColumnName("id_product");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.StoreId).HasColumnName("id_store");
                entity.Property(e => e.CategoryProductId).HasColumnName("id_category_product");
                entity.HasOne(e => e.Store).WithMany().HasForeignKey(e => e.StoreId);
                entity.HasOne(e => e.CategoryProduct).WithMany().HasForeignKey(e => e.CategoryProductId);
            });

            modelBuilder.Entity<SaleEntity>(entity =>
            {
                entity.ToTable("sale");
                entity.Property(e => e.Id).HasColumnName("id_sale");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.UserId).HasColumnName("id_user");
                entity.Property(e => e.StateId).HasColumnName("id_state");
            });
        }
    }
}
