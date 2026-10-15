using System;

namespace OmniProduct_CoreDomain.Abstractions;

// Uniform identity every entity exposes, whatever its own typed id (ProductId, SupplierId, ...).
public interface IDentifiable
{
    EntityId Id { get; }
}

public sealed record EntityId(Guid Value);
