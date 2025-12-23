
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
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }
    public DbSet<PlaceView> PlaceViews { get; set; }
    public DbSet<ReviewHelpfulness> ReviewHelpfulness { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PlaceImage> PlaceImages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }  // 🔄 For staying logged in
    public DbSet<LoginAttempt> LoginAttempts { get; set; }  // 🛡️ For tracking login attempts

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- UNIQUE CONSTRAINTS ---
        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.PlaceId })
            .IsUnique();

        builder.Entity<ReviewHelpfulness>()
            .HasIndex(rh => new { rh.UserId, rh.ReviewId })
            .IsUnique();

        builder.Entity<UserPreference>()
            .HasIndex(up => up.UserId)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // --- PERFORMANCE INDEXES ---
        builder.Entity<PlaceView>()
            .HasIndex(pv => new { pv.PlaceId, pv.ViewedAt });

        builder.Entity<SearchHistory>()
            .HasIndex(sh => new { sh.UserId, sh.SearchedAt });

        builder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });

        builder.Entity<Place>()
            .HasIndex(p => new { p.CategoryId, p.IsActive, p.IsDeleted });

        builder.Entity<Place>()
            .HasIndex(p => new { p.Latitude, p.Longitude });

        builder.Entity<Review>()
            .HasIndex(r => new { r.PlaceId, r.IsApproved, r.IsDeleted });

        builder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.CreatedAt });

        builder.Entity<PlaceImage>()
            .HasIndex(pi => new { pi.PlaceId, pi.IsPrimary, pi.DisplayOrder });

        // --- FIX CASCADE DELETE ISSUES ---
        builder.Entity<Review>()
         .HasOne(r => r.Place)
         .WithMany(p => p.Reviews)
         .HasForeignKey(r => r.PlaceId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ReviewHelpfulness>()
            .HasOne(rh => rh.User)
            .WithMany(u => u.ReviewHelpfulness)
            .HasForeignKey(rh => rh.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserPreference>()
            .HasOne(up => up.User)
            .WithOne(u => u.UserPreference)
            .HasForeignKey<UserPreference>(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SearchHistory>()
            .HasOne(sh => sh.User)
            .WithMany(u => u.SearchHistories)
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LoginAttempt>()
            .HasOne(la => la.User)
            .WithMany(u => u.LoginAttempts)
            .HasForeignKey(la => la.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Place>()
       .HasOne(p => p.Category)
       .WithMany(c => c.Places)
       .HasForeignKey(p => p.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);
    }

}

