using AwesomeAssertions;
using Cupid.Harness.Rules;

namespace Cupid.Harness.Test;

public class AlgebraicDataTypeRuleTests
{
    private readonly AlgebraicDataTypeRule _rule = new();

    [Fact]
    public void NonAbstractUnionBase_ShouldBeFlagged()
    {
        var (trees, compilation) = RuleTestHarness.Compile("""
            namespace Sample;
            public record Shape;
            public sealed record Circle : Shape;
            public sealed record Square : Shape;
            """);

        var violations = _rule.Check(trees, compilation);

        violations.Should().Contain(v => v.Message.Contains("Shape") && v.Message.Contains("not 'abstract'"));
    }

    [Fact]
    public void NonSealedUnionCase_ShouldBeFlagged()
    {
        var (trees, compilation) = RuleTestHarness.Compile("""
            namespace Sample;
            public abstract record Shape;
            public record Circle : Shape;
            public sealed record Square : Shape;
            """);

        var violations = _rule.Check(trees, compilation);

        violations.Should().ContainSingle(v => v.Message.Contains("Circle") && v.Message.Contains("not 'sealed'"));
    }

    [Fact]
    public void WellFormedUnion_ShouldPass()
    {
        var (trees, compilation) = RuleTestHarness.Compile("""
            namespace Sample;
            public abstract record Shape;
            public sealed record Circle : Shape;
            public sealed record Square : Shape;
            """);

        var violations = _rule.Check(trees, compilation);

        violations.Should().BeEmpty();
    }

    // No hierarchy is fine: OneOf<...> is the other accepted way to express an ADT.
    [Fact]
    public void NoRecordHierarchyAtAll_ShouldPass()
    {
        var (trees, compilation) = RuleTestHarness.Compile("""
            namespace Sample;
            public class Price
            {
                public decimal Amount { get; set; }
            }
            """);

        var violations = _rule.Check(trees, compilation);

        violations.Should().BeEmpty();
    }
}
