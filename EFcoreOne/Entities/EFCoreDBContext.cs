using EFcoreOne.Models;
using Microsoft.EntityFrameworkCore;

namespace EFcoreOne.Entities
{
    public class EFCoreDBContext : DbContext
    {
        // Constructor that accepts DbContextOptions<EFCoreDbContext> as a parameter.
        // The options parameter contains the settings required by EF Core to configure the DbContext,
        // such as the connection string and provider.

        public EFCoreDBContext(DbContextOptions<EFCoreDBContext> options): base(options) // The base(options) call passes the options to the base DbContext class constructor.
        {
        }
        // OnConfiguring is an override method that allows configuring the DbContext options,
        // like setting the database provider and connection string.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the database provider and connection string.
            // UseSqlServer method configures the DbContext to use SQL Server as the database provider.
            // The provided connection string specifies the server, database name, and credentials.
            // Replace "Server=YourServerName;Database=YourDatabaseName;User Id=YourUsername;Password=YourPassword;"
            // with your actual SQL Server details.

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OneAtharvic;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        // DbSet<Student> Students represents a table in the database corresponding to the Student entity.
        // EF Core uses DbSet<TEntity> to track changes and execute queries related to the Student entity.
        //Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;
        //Encrypt=True;TrustServerCertificate=False;Application Name="SQL Server Management Studio";Command Timeout=30
        public DbSet<Student> Students { get; set; }
        // DbSet<Branch> Branches represents a table in the database corresponding to the Branch entity.
        // Similar to the Students DbSet, this property is used by EF Core to track and manage Branch entities.
        public DbSet<Branch> Branches { get; set; }
    }
}

