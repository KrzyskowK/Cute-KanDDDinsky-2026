using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.StorageContext;

namespace OmniProduct_CoreDomain.Services;

public sealed class Storage
{
    private readonly List<Warehouse> _warehouses = [];
    private readonly List<StoredProduct> _stock = [];

    public Warehouse AddWarehouse(string name, string address, string region)
    {
        var warehouse = new Warehouse(WarehouseId.New(), name, address, region);
        _warehouses.Add(warehouse);
        return warehouse;
    }

    public Warehouse? WarehouseServing(string region) => _warehouses.FirstOrDefault(w => w.Region == region);

    public StoredProduct OpenStock(ProductId productId, WarehouseId warehouseId)
    {
        var stored = new StoredProduct(productId, warehouseId);
        _stock.Add(stored);
        return stored;
    }

    public StoredProduct GetStock(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return _stock.FirstOrDefault(s => s.Id == productId)
               ?? throw new InvalidOperationException($"No stock for product {productId.Value}");
    }

    public void ReceiveStock(ProductId productId, int quantity) => GetStock(productId).ReceiveStock(quantity);

    public void ReleaseStock(ProductId productId, int quantity) => GetStock(productId).ReleaseStock(quantity);

    public void WriteOffStock(ProductId productId) => GetStock(productId).WriteOffStock();
}
