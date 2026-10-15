using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Infrastructure;

public sealed class ProductIdConverter() : ValueConverter<ProductId, Guid>(id => id.Value, value => new ProductId(value));

public sealed class SupplierIdConverter() : ValueConverter<SupplierId, Guid>(id => id.Value, value => new SupplierId(value));

public sealed class WarehouseIdConverter() : ValueConverter<WarehouseId, Guid>(id => id.Value, value => new WarehouseId(value));

public sealed class NotificationIdConverter() : ValueConverter<NotificationId, Guid>(id => id.Value, value => new NotificationId(value));
