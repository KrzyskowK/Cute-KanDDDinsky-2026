using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.SuppliersContext;

public sealed class Supplier : IDentifiable
{
    public Supplier(SupplierId id, string name, string email, string region)
    {
        Id = id;
        Name = name;
        Email = email;
        Region = region;
    }

    public SupplierId Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string Region { get; }

    EntityId IDentifiable.Id => new(Id.Value);
}
