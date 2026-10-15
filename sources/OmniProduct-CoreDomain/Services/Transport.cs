using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.TransportContext;

namespace OmniProduct_CoreDomain.Services;

public sealed class Transport
{
    private readonly List<ShippableProduct> _shippables = [];

    public ShippableProduct Register(ProductId productId, double weight, string dimensions)
    {
        var shippable = new ShippableProduct(productId, weight, dimensions);
        _shippables.Add(shippable);
        return shippable;
    }

    public ShippableProduct Get(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return _shippables.FirstOrDefault(s => s.Id == productId)
               ?? throw new InvalidOperationException($"Product {productId.Value} is not registered for transport");
    }
}
