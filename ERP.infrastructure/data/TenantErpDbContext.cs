using ERP.domain.entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.data;

public class TenantErpDbContext : DbContext
{
    public TenantErpDbContext(DbContextOptions<TenantErpDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Inventory> Inventories => Set<Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Category)
                .HasMaxLength(100);

            entity.Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            entity.HasIndex(p => p.ProductCode)
                .IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.CustomerId);

            entity.Property(c => c.CustomerCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(c => c.CustomerCode)
                .IsUnique();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.SupplierId);

            entity.Property(s => s.SupplierCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(s => s.SupplierName)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(s => s.SupplierCode)
                .IsUnique();
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(i => i.InventoryId);

            entity.Property(i => i.QuantityOnHand)
                .HasPrecision(18, 2);

            entity.Property(i => i.ReorderLevel)
                .HasPrecision(18, 2);

            entity.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}