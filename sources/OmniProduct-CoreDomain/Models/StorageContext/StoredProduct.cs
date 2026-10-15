using System;
using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.StorageContext;

// Storage BC: only what matters to stock the merchandise in a warehouse.
public sealed class StoredProduct : IDentifiable
{
    public StoredProduct(ProductId id, WarehouseId warehouseId)
    {
        Id = id;
        WarehouseId = warehouseId;
    }

    public ProductId Id { get; }
    public WarehouseId WarehouseId { get; private set; }

    // units currently on the shelves
    public int Stock { get; private set; }

    // units ever received
    public int Quantity { get; private set; }

    EntityId IDentifiable.Id => new(Id.Value);

    public bool IsOutOfStock => Stock == 0;

    public void ReceiveStock(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        Stock += quantity;
        Quantity += quantity;
    }

    public void ReleaseStock(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        if (Stock < quantity)
            throw new InvalidOperationException("Not enough stock");

        Stock -= quantity;
    }

    public void WriteOffStock()
    {
        Stock = 0;
    }
}
