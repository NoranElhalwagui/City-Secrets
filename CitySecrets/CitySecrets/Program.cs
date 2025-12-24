using System.Text;
using CitySecrets.Services.Implementations;
using CitySecrets.Services.Interfaces;
using CitySecrets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CitySecrets.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
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

// Always enable Swagger in Development, can also enable in Production if desired
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Secrets API v1");
    c.RoutePrefix = string.Empty; // Show at root
});

app.UseCors("AllowFrontend");
app.UseCors("AllowReact");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// --- Database Seeding ---

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

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

app.Run();
