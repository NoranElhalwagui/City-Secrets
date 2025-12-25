using System.Text;
using CitySecrets.Services.Implementations;
using CitySecrets.Services.Interfaces;
using CitySecrets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CitySecrets.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure for Railway deployment - listen on PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "5293";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// --- Services ---

// Database - Use PostgreSQL for production (Railway), SQLite for local dev
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// Railway provides DATABASE_URL in Postgres format: postgres://user:pass@host:port/db
// Convert it to .NET connection string format if needed
if (connectionString?.StartsWith("postgres://") == true)
{
    var uri = new Uri(connectionString);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString?.Contains("Host=") == true || connectionString?.Contains("Server=") == true)
    {
        options.UseNpgsql(connectionString); // PostgreSQL for production
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=CitySecrets.db"); // SQLite for local dev
    }
});

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISearchService, SearchService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CitySecrets",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CitySecretsUsers",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
    };
});

// 🛡️ AUTHORIZATION POLICIES
// These control who can access certain features
// "AdminOnly" = Only admin users can access
// "VerifiedUser" = Only users with verified email can access
builder.Services.AddAuthorization(options =>
{
    // Admin-only endpoints (like delete user, approve places)
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    // Verified users only (like write reviews, add places)
    options.AddPolicy("VerifiedUser", policy =>
        policy.RequireClaim("EmailVerified", "True"));
});

// CORS - Allow both local development and production frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new List<string> { "http://localhost:3000" };
        
        // Add production frontend URL from environment variable
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            allowedOrigins.Add(frontendUrl);
        }
        
        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "City Secrets API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Middleware ---

// Always enable Swagger for easy API testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Secrets API v1");
    c.RoutePrefix = string.Empty; // Swagger at root (/)
});

// Add a simple health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseCors("AllowFrontend");

// Only redirect to HTTPS in production with proper setup
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Database Migration ---
// Apply migrations on startup for Railway
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        
        // Seed data only if database is empty
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User
                {
                    Username = "admin",
                    Email = "admin@test.com",
                    PasswordHash = "admin123",
                    FullName = "Admin User",
                    IsAdmin = true,
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "user",
                    Email = "user@test.com",
                    PasswordHash = "user123",
                    FullName = "Regular User",
                    IsAdmin = false,
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration error: {ex.Message}");
    }
}

var portForLog = Environment.GetEnvironmentVariable("PORT") ?? "5293";
Console.WriteLine($"🚀 Server starting on port {portForLog}");
Console.WriteLine($"📍 Swagger UI available at: http://localhost:{portForLog}/");

app.Run();
