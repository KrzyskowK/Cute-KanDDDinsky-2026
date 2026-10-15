using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Models.PricingContext;

// Pricing BC: a product is nothing more than the price it is sold at.
public sealed class PricedProduct : IDentifiable
{
    public PricedProduct(ProductId id, Price price)
    {
        Id = id;
        Price = price;
    }

    public ProductId Id { get; }
    public Price Price { get; private set; }

    EntityId IDentifiable.Id => new(Id.Value);

    public decimal ResellerPrice => Price.CalculateResellerPrice();

    public void SetMargin(decimal marginPercent)
    {
        Price = Price with { Margin = marginPercent };
    }
}
