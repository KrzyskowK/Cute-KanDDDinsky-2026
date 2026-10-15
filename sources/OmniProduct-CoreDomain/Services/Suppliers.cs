using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.SuppliersContext;

namespace OmniProduct_CoreDomain.Services;

public sealed class Suppliers
{
    private readonly List<Supplier> _suppliers = [];
    private readonly List<SuppliedProduct> _supplied = [];

    public Supplier AddSupplier(string name, string email, string region)
    {
        var supplier = new Supplier(SupplierId.New(), name, email, region);
        _suppliers.Add(supplier);
        return supplier;
    }

    public Supplier? FindSupplierInRegion(string region) => _suppliers.FirstOrDefault(s => s.Region == region);

    public void AddSupplierToRegion(ProductId productId, string region)
    {
        var supplier = FindSupplierInRegion(region)
                       ?? throw new InvalidOperationException($"No supplier found for region {region}");

        var supplied = _supplied.FirstOrDefault(s => s.Id == productId);
        if (supplied == null)
        {
            supplied = new SuppliedProduct(productId);
            _supplied.Add(supplied);
        }

        supplied.AssignSupplier(region, supplier.Id);
    }

    public bool IsSuppliedInRegion(ProductId productId, string region) =>
        _supplied.Any(s => s.Id == productId && s.IsSuppliedInRegion(region));

    public IReadOnlyList<Supplier> GetSuppliersOf(ProductId productId)
    {
        var supplied = _supplied.FirstOrDefault(s => s.Id == productId);
        if (supplied == null)
            return [];

        return [.. supplied.SuppliersByRegion.Values.Select(supplierId => _suppliers.First(s => s.Id == supplierId))];
    }
}
