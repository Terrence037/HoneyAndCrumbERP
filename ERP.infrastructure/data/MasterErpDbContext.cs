using ERP.domain.entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.data;

public class MasterErpDbContext : IdentityDbContext<ApplicationUser>
{
    public MasterErpDbContext(DbContextOptions<MasterErpDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyDatabase> CompanyDatabases => Set<CompanyDatabase>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Required so ASP.NET Identity configures its tables and keys
        base.OnModelCreating(builder);

        builder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.CompanyId);

            entity.Property(c => c.CompanyCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(c => c.CompanyCode)
                .IsUnique();

            entity.Property(c => c.SubscriptionPlan)
                .IsRequired()
                .HasMaxLength(30);
        });

        builder.Entity<CompanyDatabase>(entity =>
        {
            entity.HasKey(cd => cd.CompanyDatabaseId);

            entity.Property(cd => cd.ServerName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(cd => cd.CredentialKey)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(cd => cd.DatabaseName)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(cd => cd.Company)
                .WithMany()
                .HasForeignKey(cd => cd.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Device>(entity =>
        {
            entity.HasKey(d => d.DeviceId);

            entity.Property(d => d.DeviceCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(d => d.DeviceName)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(d => new { d.CompanyId, d.DeviceCode })
                .IsUnique();

            entity.HasOne(d => d.Company)
                .WithMany(c => c.Devices)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}