using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.PricingContext;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Services;

public sealed class Pricing
{
    private readonly List<PricedProduct> _prices = [];

    public void SetPrice(ProductId productId, Price price) => _prices.Add(new PricedProduct(productId, price));

    public decimal GetResellerPrice(ProductId productId) => Find(productId).ResellerPrice;

    public void SetMargin(ProductId productId, decimal marginPercent) => Find(productId).SetMargin(marginPercent);

    private PricedProduct Find(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        return _prices.FirstOrDefault(p => p.Id == productId)
               ?? throw new InvalidOperationException($"No price for product {productId.Value}");
    }
}
