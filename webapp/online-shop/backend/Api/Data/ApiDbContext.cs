using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Data;

public class ApiDbContext(DbContextOptions options): DbContext(options)
{
    public DbSet<PanelUser> PanelUsers { get; init; }
    public DbSet<Category> Categories { get; init; }
    public DbSet<Product> Products { get; init; }
    public DbSet<ProductVariant> ProductVariants { get; init; }
    public DbSet<Color> Colors { get; init; }
    public DbSet<Size> Sizes { get; init; }
    public DbSet<Image> Images { get; init; }
    public DbSet<Brand> Brands { get; init; }
    public DbSet<State> States { get; init; }
    public DbSet<City> Cities { get; init; }
    public DbSet<Notification> Notifications { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<UserAddress> UserAddresses { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<PanelUser>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.CreatedById)
                .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("NOW()");
            entity.Property(n => n.NotificationSource)
                .HasConversion<string>();
            entity.Property(n => n.NotificationType)
                .HasConversion<string>();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.Property(e => e.CreatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.UpdatedById)
                  .HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
             .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()");
        });
    }
}
