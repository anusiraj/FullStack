using Microsoft.EntityFrameworkCore;
using NetCoreDemo.Models;
using Npgsql;

namespace NetCoreDemo.Db;

public class AppDbContext : DbContext
{
    static AppDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    private readonly IConfiguration _config;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options) => _config = config;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connString = _config.GetConnectionString("DefaultConnection");
        optionsBuilder
            .UseNpgsql(connString)
            .AddInterceptors(new AppDbContextSaveChangesInterceptor())
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<Product>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
         modelBuilder.Entity<Product>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    public DbSet<Product> Products { get; set; } = null!;
}