using System;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Services;

// Brings a product into (and out of) the range we sell, by registering it in every bounded context.
public sealed class Assortment
{
    private readonly Lifecycle _lifecycle;
    private readonly Catalog _catalog;
    private readonly Pricing _pricing;
    private readonly Storage _storage;
    private readonly Suppliers _suppliers;
    private readonly Transport _transport;
    private readonly Notifications _notifications;

    public Assortment(Lifecycle lifecycle, Catalog catalog, Pricing pricing, Storage storage,
                      Suppliers suppliers, Transport transport, Notifications notifications)
    {
        _lifecycle = lifecycle;
        _catalog = catalog;
        _pricing = pricing;
        _storage = storage;
        _suppliers = suppliers;
        _transport = transport;
        _notifications = notifications;
    }

    public ProductId AddProduct(string name, string region, decimal supplierPrice, string currency)
    {
        if (_suppliers.FindSupplierInRegion(region) == null)
            throw new InvalidOperationException($"No supplier found for region {region}");

        var warehouse = _storage.WarehouseServing(region)
                        ?? throw new InvalidOperationException($"No warehouse found for region {region}");

        var productId = ProductId.New();
        _lifecycle.Launch(productId);
        _catalog.AddToCatalog(productId, name);
        _pricing.SetPrice(productId, new Price(supplierPrice, currency));
        _storage.OpenStock(productId, warehouse.Id);
        _suppliers.AddSupplierToRegion(productId, region);
        _transport.Register(productId, weight: 0, dimensions: "");
        return productId;
    }

    public void DeprecateProduct(ProductId productId)
    {
        _lifecycle.Deprecate(productId);
        _storage.WriteOffStock(productId);
        _notifications.AnnounceDeprecation(productId, _catalog.Get(productId).Name, _suppliers.GetSuppliersOf(productId));
    }
}
