using AuthenticationServerOne.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace AuthenticationServerOne.DBContext

{
    public class ApplicationDbContext : DbContext
    {
        //constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SSOToken>()
                .HasOne (t=> t.User)
                .WithMany(u => u.SSOTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            public DbSet<SSOToken> SSOTokens { get; set; } = null!;
        }
    }
}
