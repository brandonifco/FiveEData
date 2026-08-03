using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class RuleDefinitionValidatorTests
{
    [Fact]
    public void RepresentativeRule_IsValid()
    {
        RuleDefinition rule = Create(
            new RuleId("dnd5e2014.rule.test"));

        RuleDefinitionValidator.EnsureValid(rule);
    }

    [Fact]
    public void DefaultRuleId_IsRejected()
    {
        RuleDefinition rule = Create(default);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () =>
                    RuleDefinitionValidator.EnsureValid(
                        rule));

        Assert.Contains(
            "Rule ID must not be empty.",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void MissingSourceReference_RemainsRejected()
    {
        var rule = new RuleDefinition(
            new RuleId("dnd5e2014.rule.test"),
            "Test Rule",
            []);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () =>
                    RuleDefinitionValidator.EnsureValid(
                        rule));

        Assert.Contains(
            "must have at least one source reference",
            exception.Message,
            StringComparison.Ordinal);
    }

    private static RuleDefinition Create(RuleId id)
    {
        return new RuleDefinition(
            id,
            "Test Rule",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 1,
                    section: "Test Section")
            ]);
    }
}
