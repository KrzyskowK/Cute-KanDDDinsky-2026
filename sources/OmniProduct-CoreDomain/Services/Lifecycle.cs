using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.LifecycleContext;

namespace OmniProduct_CoreDomain.Services;

public sealed class Lifecycle(TimeProvider clock)
{
    private readonly List<Product> _products = [];

    public Product Launch(ProductId productId)
    {
        var product = new Product(productId, clock.GetUtcNow());
        _products.Add(product);
        return product;
    }

    public Product Get(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return _products.FirstOrDefault(p => p.Id == productId)
               ?? throw new InvalidOperationException($"Product {productId.Value} not found");
    }

    public void Deprecate(ProductId productId) => Get(productId).Deprecate(clock.GetUtcNow());
}
