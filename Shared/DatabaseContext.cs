using Microsoft.EntityFrameworkCore;
using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Domain.Entities.Auth;

namespace net_core_web_api_clean_ddd.Shared;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OtpEntity> Otps { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
}