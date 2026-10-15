using System.Collections.Generic;
using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.SuppliersContext;

// Supplier BC: which supplier provides the product in which region. Suppliers are held by id only.
public sealed class SuppliedProduct : IDentifiable
{
    public SuppliedProduct(ProductId id)
    {
        Id = id;
    }

    public ProductId Id { get; }

    // key = region
    public IReadOnlyDictionary<string, SupplierId> SuppliersByRegion { get; private set; } = new Dictionary<string, SupplierId>();

    EntityId IDentifiable.Id => new(Id.Value);

    public bool IsSuppliedInRegion(string region) => SuppliersByRegion.ContainsKey(region);

    public void AssignSupplier(string region, SupplierId supplierId)
    {
        SuppliersByRegion = new Dictionary<string, SupplierId>(SuppliersByRegion) { [region] = supplierId };
    }
}
