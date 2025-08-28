using Application.ApiHttpClient;
using Application.AppMapper;
using Application.Permissions;
using Application.Service;
using Application.ServiceInterface;
using AutoMapper;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Infrastructure.Repository;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure AutoMapper
var mappingConfig = new MapperConfiguration(options => options.AddProfile(new MapperProfile()));
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
.AddRoles<IdentityRole>() // Required for role management
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// DI Services
builder.Services.AddScoped<IServiceInfra, ServiceInfra>();
builder.Services.AddScoped<IServiceInfraRepo, ServiceInfraRepo>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

//builder.Services.AddTransient<IHttpClients, HttpClients>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Build the app
var app = builder.Build();

// Seed roles and admin after app is built
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DefaultRoles.SeedRolesAndAdminAsync(services);
}

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "NovelWaveTechAPI");
    });
}

app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
