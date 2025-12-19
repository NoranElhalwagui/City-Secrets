using CitySecrets.Services.Implementations;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CitySecrets.Models; // Adjust if your User model is in a different namespace

var builder = WebApplication.CreateBuilder(args);

// --- Services ---

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

<<<<<<< HEAD
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
=======
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
>>>>>>> a9eaf9b54d85cb3ea15a65cd7e57c4b3d8b65518

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

app.UseHttpsRedirection();

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
