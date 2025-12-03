using Microsoft.EntityFrameworkCore;
using CitySecrets.Models;

namespace CitySecrets.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favourites { get; set; }
        public DbSet<AdminActionLog> AdminActionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.PlaceId })
                .IsUnique();
        }

        public override int SaveChanges()
        {
            // Call the base SaveChanges method to persist changes to the database
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Async version of SaveChanges for async operations
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
