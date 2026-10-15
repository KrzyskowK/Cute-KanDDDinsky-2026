using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.CatalogContext;

namespace OmniProduct_CoreDomain.Services;

public sealed class Catalog(Lifecycle lifecycle, Storage storage, Suppliers suppliers)
{
    private readonly List<CatalogProduct> _entries = [];

    public CatalogProduct AddToCatalog(ProductId productId, string name)
    {
        var entry = new CatalogProduct(productId, name);
        _entries.Add(entry);
        return entry;
    }

    public CatalogProduct Get(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return _entries.FirstOrDefault(e => e.Id == productId)
               ?? throw new InvalidOperationException($"Product {productId.Value} not found");
    }

    public IReadOnlyList<CatalogProduct> GetCatalog(string region) =>
        [.. _entries.Where(e => suppliers.IsSuppliedInRegion(e.Id, region) && !lifecycle.Get(e.Id).IsDeprecated)];

    public string GetDisplayLabel(ProductId productId)
    {
        var name = Get(productId).Name;
        if (lifecycle.Get(productId).IsDeprecated)
            return $"[DISCONTINUED] {name}";
        if (storage.GetStock(productId).IsOutOfStock)
            return $"[OUT OF STOCK] {name}";
        return name;
    }

    public void AddImage(ProductId productId, string context, Uri url) => Get(productId).AddImage(context, url);

    public void AddDiscount(ProductId productId, string discountCode) => Get(productId).AddDiscount(discountCode);
}
