using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Domain.Enums;
using FurnitureStore.Infrastructure.Repositories;
using FurnitureStore.Persistence.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DotNetEnv;
using System.Text.Json;
using System.Text.Json.Serialization;
using ServiceStack.Text;
// Add services to the container

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();
// Add services to the container.

builder.Services.AddControllers();

// 1. DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."))
    );

//2.Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
// 3. JWT Authentication

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration.GetSection("Authentication:Jwt")["Issuer"],
        ValidAudience = builder.Configuration.GetSection("Authentication:Jwt")["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:Jwt")["Key"]))
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
        ?? throw new InvalidOperationException("Google ClientId is not configured.");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
        ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
   /* options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
    options.SaveTokens = true;
    options.Events.OnCreatingTicket = ctx =>
    {
        var picture = ctx.User.GetProperty("picture").GetString();
        if (!string.IsNullOrEmpty(picture))
        {
            ctx.Identity.AddClaim(new System.Security.Claims.Claim("picture", picture));
        }
        var locale = ctx.User.GetProperty("locale").GetString();
        if (!string.IsNullOrEmpty(locale))
        {
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
            ctx.Identity.AddClaim(new System.Security.Claims.Claim("locale", locale));
        }
        return Task.CompletedTask;
    };*/
});
// 4. Dependency Injection for Repositories and Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();// To access HttpContext in services ip address
builder.Services.AddHttpContextAccessor();
// 5. Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FurnitureStore API", Version = "v1" });

    // تعريف الـ Bearer JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\\r\\n\\r\\nExample: \\\"Bearer abc123\\"
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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));// Handle enums as strings in camelCase
});
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FurnitureStore API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
