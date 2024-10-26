using System.Text;
using Asp.Versioning;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using net_core_web_api_clean_ddd.Infrastructure.Mappers;
using net_core_web_api_clean_ddd.Infrastructure.Middlewares;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Dependencies injection
// Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Mappers
// builder.Services.AddScoped<IAuthMapper, AuthMapper>();
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<IProductMapper, ProductMapper>();
// System
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DatabaseContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Local"));
});

// IpRateLimiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Use secure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

// Versioning
builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Swagger
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1-user", new OpenApiInfo
    {
        Title = "MobileApp APIs",
        Version = "v1"
    });
    swagger.SwaggerDoc("v1-admin", new OpenApiInfo
    {
        Title = "AdminPanel APIs",
        Version = "v1"
    });
    swagger.DocInclusionPredicate((docName, apiDesc) =>
    {
        return docName switch
        {
            "v1-user" => !apiDesc.RelativePath!.Contains("api/v1/Admin"),
            "v1-admin" => apiDesc.RelativePath!.StartsWith("api/v1/Admin") /* ||
                          !apiDesc.RelativePath!.StartsWith("api/v1/User")*/,
            _ => false
        };
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter your token"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Jwt
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
IConfigurationSection jwtConfigs = builder.Configuration.GetSection("Jwt");
// JwtToken
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = [jwtConfigs["AdminIssuer"], jwtConfigs["UserIssuer"]],
            ValidAudiences = [jwtConfigs["AdminAudience"], jwtConfigs["UserAudience"]],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigs["Key"]!))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("iss", jwtConfigs["AdminIssuer"]!)
            .RequireClaim("aud", jwtConfigs["AdminAudience"]!))
    .AddPolicy("UserPolicy", policy =>
        policy.RequireClaim("iss", jwtConfigs["UserIssuer"]!)
            .RequireClaim("aud", jwtConfigs["UserAudience"]!));


WebApplication app = builder.Build();

//Middlewares
// app.UseMiddleware<TooManyRequestsMiddleware>();
app.UseMiddleware<ForbiddenMiddleware>();
app.UseMiddleware<UnauthorizedMiddleware>();
app.UseMiddleware<SecurityMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/swagger/v1-user/swagger.json", "MobileApp v1");
        swagger.SwaggerEndpoint("/swagger/v1-admin/swagger.json", "AdminPanel v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "NetCoreWebApiCleanDDD",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("Api-Key", "Your-Api-Key");
    await next();
});

await app.RunAsync();