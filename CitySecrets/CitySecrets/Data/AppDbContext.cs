
using CitySecrets.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Place> Place { get; set; }
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
}

