using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
    Justification = "Root namespace OmniProduct_CoreDomain is the contract the Cupid.Harness rules match on.",
    Scope = "namespaceanddescendants", Target = "~N:OmniProduct_CoreDomain")]

[assembly: SuppressMessage("Naming", "CA1724:Type names should not match namespaces",
    Justification = "Storage is the bounded-context name from the ubiquitous language; the clash is with EF Core's internal Microsoft.EntityFrameworkCore.Sqlite.Storage namespace.",
    Scope = "type", Target = "~T:OmniProduct_CoreDomain.Services.Storage")]
