// Using directives must come first
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagementApp.Core.Interfaces;
using UserManagementApp.Infrastructure.Data;
using UserManagementApp.Infrastructure.Repositories;
using UserManagementApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.API.Middleware;
using Microsoft.OpenApi.Models;

// FIX: Enable legacy timestamp behavior for PostgreSQL (must come after using statements)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagementApp.API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "fallback-secret-key-at-least-32-characters-long-here";
var validIssuer = builder.Configuration["Jwt:Issuer"] ?? "UserManagementApp";
var validAudience = builder.Configuration["Jwt:Audience"] ?? "UserManagementAppUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Custom Middleware for user status check
app.UseMiddleware<UserStatusMiddleware>();

app.MapControllers();

// Serve static files (for the frontend)
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

// Ensure database is created
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Database setup completed successfully.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database setup failed: {ex.Message}");
}


app.Run();