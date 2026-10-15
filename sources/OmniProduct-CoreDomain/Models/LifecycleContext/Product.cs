using System;
using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Models.LifecycleContext;

// Lifecycle BC: a product only as something that is launched and, one day, deprecated.
public sealed class Product : IDentifiable
{
    public Product(ProductId id, DateTimeOffset createdAt)
    {
        Id = id;
        Status = ProductStatus.Active;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public ProductId Id { get; }
    public ProductStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    EntityId IDentifiable.Id => new(Id.Value);

    public bool IsDeprecated => Status == ProductStatus.Deprecated;

    public void Deprecate(DateTimeOffset at)
    {
        Status = ProductStatus.Deprecated;
        UpdatedAt = at;
    }
}
