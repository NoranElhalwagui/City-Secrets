using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



// Find this section in your Program.cs:
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    // Your existing category seeding code...
    
    // ADD THIS NEW SECTION:
    // Seed test users (admin and regular user)
    if (!context.Users.Any())
    {
        context.Users.AddRange(
            // ADMIN USER - can add/delete places
            new CitySecrets.Models.User
            {
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = "admin123",  // Password: admin123
                FullName = "Admin User",
                IsAdmin = true,   // THIS MAKES THEM ADMIN
                IsActive = true,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow
            },
            // REGULAR USER - cannot add/delete places
            new CitySecrets.Models.User
            {
                Username = "user",
                Email = "user@test.com",
                PasswordHash = "user123",  // Password: user123
                FullName = "Regular User",
                IsAdmin = false,  // THIS MAKES THEM REGULAR USER
                IsActive = true,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        context.SaveChanges();
    }
}