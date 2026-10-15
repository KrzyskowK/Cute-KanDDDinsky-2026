using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.StorageContext;

public sealed class Warehouse : IDentifiable
{
    public Warehouse(WarehouseId id, string name, string address, string region)
    {
        Id = id;
        Name = name;
        Address = address;
        Region = region;
    }

    public WarehouseId Id { get; }
    public string Name { get; }
    public string Address { get; }
    public string Region { get; }

    EntityId IDentifiable.Id => new(Id.Value);
}
