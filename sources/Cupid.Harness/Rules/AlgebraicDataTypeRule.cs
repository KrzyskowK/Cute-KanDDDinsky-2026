using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cupid.Harness.Rules;

// HA4.8 - ADT (algebraic data structures): enforce the use of pseudo-union types via record
// hierarchies. A base record that has subtypes is a union: the base must be 'abstract' and every
// direct subtype must be 'sealed', so consumers get exhaustive pattern matching instead of open
// extension. Optional: OneOf<...> is the other accepted form.
public sealed class AlgebraicDataTypeRule : IHarnessRule
{
    public string Id => "HA4.8";
    public string Name => "ADT via record hierarchy (abstract base, sealed leaves)";

    public IReadOnlyList<Violation> Check(IReadOnlyList<SyntaxTree> trees, Compilation compilation)
    {
        var violations = new List<Violation>();

        var allRecords = trees
            .SelectMany(t => t.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>()
                .Select(r => (Tree: t, Record: r)))
            .ToList();

        var model = allRecords
            .Select(x => compilation.GetSemanticModel(x.Tree))
            .ToList();

        var recordSymbols = allRecords
            .Select((x, i) => (x.Tree, x.Record, Symbol: model[i].GetDeclaredSymbol(x.Record) as INamedTypeSymbol))
            .Where(x => x.Symbol != null)
            .ToList();

        var baseTypesWithSubtypes = recordSymbols
            .Select(x => x.Symbol!.BaseType)
            .Where(bt => bt is { SpecialType: SpecialType.None, TypeKind: TypeKind.Class })
            .Distinct(SymbolEqualityComparer.Default)
            .ToList();

        // No record hierarchy is fine: an ADT can equally be expressed with OneOf<...> (checked by HA4.3).
        // C# has no native union type yet, so both designs are accepted; this rule only checks the
        // record-hierarchy form when it is used.

        foreach (var baseType in baseTypesWithSubtypes)
        {
            var baseDecl = recordSymbols.FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.Symbol, baseType));
            if (baseDecl.Symbol == null)
                continue; // base type isn't a record declared in the analyzed sources - not our concern here.

            if (!baseDecl.Record.Modifiers.Any(m => m.Text == "abstract"))
            {
                violations.Add(new Violation(
                    baseDecl.Tree.FilePath,
                    baseDecl.Record.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    $"Record '{baseDecl.Record.Identifier.Text}' is a union base (has subtypes) but is not 'abstract'."));
            }

            var subtypes = recordSymbols
                .Where(x => SymbolEqualityComparer.Default.Equals(x.Symbol!.BaseType, baseType))
                .ToList();

            foreach (var subtype in subtypes)
            {
                if (!subtype.Record.Modifiers.Any(m => m.Text == "sealed"))
                {
                    violations.Add(new Violation(
                        subtype.Tree.FilePath,
                        subtype.Record.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        $"Record '{subtype.Record.Identifier.Text}' is a union case (derives from '{baseType!.Name}') but is not 'sealed'."));
                }
            }
        }

        return violations;
    }
}
