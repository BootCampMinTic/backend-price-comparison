using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context
{
    public class ClientDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<ClientLegalPosEntity> ClientLegalPos { get; set; }
        public DbSet<ClientNaturalPosEntity> ClientNaturalPos { get; set; }
        public DbSet<DocumentTypeEntity> DocumentTypes { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
     base.OnModelCreating(modelBuilder);
        EntityConfiguration(modelBuilder);
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
    }
}
