using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.TransportContext;

// Transport BC: what a carrier needs to know to move the product.
public sealed class ShippableProduct : IDentifiable
{
    public ShippableProduct(ProductId id, double weight, string dimensions)
    {
        Id = id;
        Weight = weight;
        Dimensions = dimensions;
    }

    public ProductId Id { get; }
    public double Weight { get; }
    public string Dimensions { get; }

    EntityId IDentifiable.Id => new(Id.Value);
}
