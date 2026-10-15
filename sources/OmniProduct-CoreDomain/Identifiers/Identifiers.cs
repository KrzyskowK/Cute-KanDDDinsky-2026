using System;

namespace OmniProduct_CoreDomain.Identifiers;

public sealed record ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.CreateVersion7());
}

public sealed record SupplierId(Guid Value)
{
    public static SupplierId New() => new(Guid.CreateVersion7());
}

public sealed record WarehouseId(Guid Value)
{
    public static WarehouseId New() => new(Guid.CreateVersion7());
}

public sealed record NotificationId(Guid Value)
{
    public static NotificationId New() => new(Guid.CreateVersion7());
}
