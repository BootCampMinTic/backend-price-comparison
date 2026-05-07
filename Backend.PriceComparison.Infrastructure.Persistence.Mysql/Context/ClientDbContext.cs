using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context
{
    public class ClientDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<ClientLegalPosEntity> ClientLegalPos { get; set; }
        public DbSet<ClientNaturalPosEntity> ClientNaturalPos { get; set; }
        public DbSet<DocumentTypeEntity> DocumentTypes { get; set; }

        public DbSet<StateEntity> States { get; set; }
        public DbSet<TypeUserEntity> TypeUsers { get; set; }
        public DbSet<CategoryProductEntity> CategoryProducts { get; set; }
        public DbSet<CategoryStoreEntity> CategoryStores { get; set; }
        public DbSet<StoreEntity> Stores { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<SaleEntity> Sales { get; set; }
        public DbSet<ProductSaleEntity> ProductSales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
     base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
            StoreEntityConfiguration(modelBuilder);
        }

        private static void EntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentTypeEntity>().HasData(
                new DocumentTypeEntity { Id = 1, Name = "Cédula de Ciudadanía", DocumentType = "CC", HelpTextHeader = "Ingrese su cédula", HelpText = "Número de cédula sin puntos ni espacios", Regex = "^[0-9]{6,10}$" },
                new DocumentTypeEntity { Id = 2, Name = "Cédula de Extranjería", DocumentType = "CE", HelpTextHeader = "Ingrese su cédula de extranjería", HelpText = "Número de cédula de extranjería", Regex = "^[0-9]{6,12}$" },
                new DocumentTypeEntity { Id = 3, Name = "NIT", DocumentType = "NIT", HelpTextHeader = "Ingrese el NIT", HelpText = "Número de Identificación Tributario sin dígito de verificación", Regex = "^[0-9]{6,12}$" },
                new DocumentTypeEntity { Id = 4, Name = "Pasaporte", DocumentType = "PP", HelpTextHeader = "Ingrese su pasaporte", HelpText = "Número de pasaporte", Regex = "^[A-Za-z0-9]{6,20}$" },
                new DocumentTypeEntity { Id = 5, Name = "Registro Civil", DocumentType = "RC", HelpTextHeader = "Ingrese el registro civil", HelpText = "Número de registro civil", Regex = "^[0-9]{6,10}$" }
            );
        }

        private static void StoreEntityConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StateEntity>(entity =>
            {
                entity.ToTable("state");
                entity.Property(e => e.Id).HasColumnName("id_state");
                entity.Property(e => e.Description).HasColumnName("description");
            });

            modelBuilder.Entity<TypeUserEntity>(entity =>
            {
                entity.ToTable("type_user");
                entity.Property(e => e.Id).HasColumnName("id_type_user");
                entity.Property(e => e.Description).HasColumnName("description");
            });

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

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("user");
                entity.Property(e => e.Id).HasColumnName("id_user");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.TypeUserId).HasColumnName("id_type_user");
                entity.HasOne(e => e.TypeUser).WithMany().HasForeignKey(e => e.TypeUserId);
            });

            modelBuilder.Entity<SaleEntity>(entity =>
            {
                entity.ToTable("sale");
                entity.Property(e => e.Id).HasColumnName("id_sale");
                entity.Property(e => e.UserId).HasColumnName("id_user");
                entity.Property(e => e.StoreId).HasColumnName("id_store");
                entity.Property(e => e.StateId).HasColumnName("id_state");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Store).WithMany().HasForeignKey(e => e.StoreId);
                entity.HasOne(e => e.State).WithMany().HasForeignKey(e => e.StateId);
                entity.HasMany(e => e.ProductSales).WithOne(ps => ps.Sale).HasForeignKey(ps => ps.SaleId);
            });

            modelBuilder.Entity<ProductSaleEntity>(entity =>
            {
                entity.ToTable("product_sale");
                entity.Property(e => e.Id).HasColumnName("id_product_sale");
                entity.Property(e => e.ProductId).HasColumnName("id_product");
                entity.Property(e => e.SaleId).HasColumnName("id_sale");
                entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
            });
        }
    }
}
