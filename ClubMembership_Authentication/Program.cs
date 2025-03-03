using ClubMembership_Authentication.Application.Services;
using ClubMembership_Authentication.Infrastructure.Persistence;
using ClubMembership_Authentication.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Add this line to define 'configuration'

// Load environment variables from .env
DotEnv.Load();
var Environment = DotEnv.Read();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Environment["DB_CONNECTION_STRING"])); // Fix the 'configuration' error

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(Environment["JWT_SECRET_KEY"] ?? "");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment["JWT_ISSUER"],
            ValidAudience = Environment["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Register Repositories and Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add gRPC services
builder.Services.AddGrpc();

var app = builder.Build();

// Seed the database
await DbInitializer.Initialize(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.UseExceptionHandler("/error"); // Handle errors in production
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<UserServiceGrpc>();

app.Run();
