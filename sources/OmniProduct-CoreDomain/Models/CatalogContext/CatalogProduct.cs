using System;
using System.Collections.Generic;
using System.Linq;
using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Models.CatalogContext;

// Catalog BC: what a customer browses - name, slug, pictures and discount codes.
public sealed class CatalogProduct : IDentifiable
{
    public CatalogProduct(ProductId id, string name)
    {
        Id = id;
        Name = name;
        Slug = SlugFor(name);
    }

    public ProductId Id { get; }
    public string Name { get; private set; }
    public string Slug { get; private set; }

    // key = context (e.g. "thumbnail", "hero"), value = url
    public IReadOnlyDictionary<string, Uri> Images { get; private set; } = new Dictionary<string, Uri>();
    public IReadOnlyList<string> Discounts { get; private set; } = [];

    EntityId IDentifiable.Id => new(Id.Value);

    public static string SlugFor(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return string.Concat(name.Select(c => c == ' ' ? '-' : char.ToLowerInvariant(c)));
    }

    public void AddImage(string context, Uri url)
    {
        Images = new Dictionary<string, Uri>(Images) { [context] = url };
    }

    public void AddDiscount(string discountCode)
    {
        Discounts = [.. Discounts, discountCode];
    }
}
