using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.CatalogContext;
using OmniProduct_CoreDomain.Models.LifecycleContext;
using OmniProduct_CoreDomain.Models.NotificationsContext;
using OmniProduct_CoreDomain.Models.PricingContext;
using OmniProduct_CoreDomain.Models.StorageContext;
using OmniProduct_CoreDomain.Models.SuppliersContext;
using OmniProduct_CoreDomain.Models.TransportContext;

namespace OmniProduct_CoreDomain.Infrastructure;

// All persistence knowledge lives here (fluent API), none of it on the domain models.
// Entities are reached through Set<T>() rather than one DbSet property per table.
public class OmniProductDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=omniproduct.db");
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
        configurationBuilder.Properties<SupplierId>().HaveConversion<SupplierIdConverter>();
        configurationBuilder.Properties<WarehouseId>().HaveConversion<WarehouseIdConverter>();
        configurationBuilder.Properties<NotificationId>().HaveConversion<NotificationIdConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(p => p.Id);
            e.Property(p => p.Status).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<CatalogProduct>(e =>
        {
            e.ToTable("CatalogProducts");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(256);
            e.Property(p => p.Slug).HasMaxLength(256);
            e.Property(p => p.Images).HasConversion(
                images => JsonSerializer.Serialize(images, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Dictionary<string, Uri>>(json, (JsonSerializerOptions?)null)!);
            e.Property(p => p.Discounts).HasConversion(
                discounts => string.Join(',', discounts),
                csv => csv.Split(',', StringSplitOptions.RemoveEmptyEntries));
        });

        modelBuilder.Entity<PricedProduct>(e =>
        {
            e.ToTable("Prices");
            e.HasKey(p => p.Id);
            e.ComplexProperty(p => p.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency).HasMaxLength(8);
                price.Property(p => p.Margin).HasColumnType("decimal(5,2)");
                price.Property(p => p.Vat).HasColumnType("decimal(5,2)");
            });
        });

        modelBuilder.Entity<StoredProduct>(e =>
        {
            e.ToTable("StoredProducts");
            e.HasKey(p => p.Id);
            e.HasOne<Warehouse>().WithMany().HasForeignKey(p => p.WarehouseId);
        });

        modelBuilder.Entity<Warehouse>(e =>
        {
            e.ToTable("Warehouses");
            e.HasKey(w => w.Id);
            e.Property(w => w.Name).IsRequired().HasMaxLength(256);
            e.Property(w => w.Address).HasMaxLength(512);
            e.Property(w => w.Region).HasMaxLength(8);
        });

        modelBuilder.Entity<Supplier>(e =>
        {
            e.ToTable("Suppliers");
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(256);
            e.Property(s => s.Email).IsRequired().HasMaxLength(256);
            e.Property(s => s.Region).HasMaxLength(8);
        });

        modelBuilder.Entity<SuppliedProduct>(e =>
        {
            e.ToTable("SuppliedProducts");
            e.HasKey(p => p.Id);
            e.Property(p => p.SuppliersByRegion).HasConversion(
                map => JsonSerializer.Serialize(map, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Dictionary<string, SupplierId>>(json, (JsonSerializerOptions?)null)!);
        });

        modelBuilder.Entity<ShippableProduct>(e =>
        {
            e.ToTable("ShippableProducts");
            e.HasKey(p => p.Id);
            e.Property(p => p.Dimensions).HasMaxLength(64);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(n => n.Id);
            e.Property(n => n.Recipient).IsRequired().HasMaxLength(256);
            e.Property(n => n.Channel).HasConversion<string>().HasMaxLength(16);
            e.ComplexProperty(n => n.Message, message =>
            {
                message.Property(m => m.Subject).HasMaxLength(256);
            });
        });
    }
}
