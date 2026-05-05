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
   // TEMPORARY FIX: Ignore DocumentCountry property until database schema is updated
            // 
            // Issue: The DocumentCountry property exists in both ClientNaturalPosEntity and 
            // ClientLegalPosEntity domain models, but the corresponding column doesn't exist 
  // in the database tables. This causes MySqlException: "Unknown column 'c.DocumentCountry' 
            // in 'field list'" when Entity Framework tries to generate SQL queries.
            //
    // Current Impact:
            // - GetAllNaturalAsync() and GetAllLegalAsync() queries fail
            // - GetByIdAsync() and GetByDocumentNumberAsync() queries fail  
            // - Create operations (CreateClientNaturalAsync/CreateClientLegalAsync) might fail
            // - All operations involving these entities that try to access DocumentCountry fail
   //
       // Affected Components:
     // - Controllers: ClientController endpoints for natural/legal clients
        // - Commands: CreateClientNaturalPosCommand, CreateClientLegalPosCommand 
            // - Queries: GetAllClientNaturalQuery, GetAllClientLegalQuery, GetClientByIdQuery, GetClientByDocumentNumberQuery
    // - Domain Services: ClientDomainService methods
 //
       // TODO: To permanently fix this issue, you need to:
       // 1. Create a database migration to add the DocumentCountry column to both tables:
   //    - ALTER TABLE ClientNaturalPos ADD COLUMN DocumentCountry VARCHAR(255) NULL;
     //    - ALTER TABLE ClientLegalPos ADD COLUMN DocumentCountry VARCHAR(255) NULL;
            // 2. Remove these Ignore() configurations below
            // 3. Test all affected endpoints to ensure they work correctly
      //
        // NOTE: Until the database schema is updated, the DocumentCountry property will:
      // - Accept values in API requests (Commands) but won't be persisted to database
   // - Always return NULL/empty in API responses (Queries) since it's not loaded from database
   // - Not cause runtime exceptions, allowing the application to function normally
            
            modelBuilder.Entity<ClientNaturalPosEntity>()
          .Ignore(e => e.DocumentCountry);
            
            modelBuilder.Entity<ClientLegalPosEntity>()
      .Ignore(e => e.DocumentCountry);
        }
    }
}
