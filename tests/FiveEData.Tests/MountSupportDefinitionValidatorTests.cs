using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountSupport;

namespace FiveEData.Tests;

public sealed class MountSupportDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        MountSupportDefinition definition = Create(default);

        Assert.Contains(
            MountSupportDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCost_IsRejected()
    {
        MountSupportDefinition definition = Create(
            new MountSupportId("dnd5e2014.mount-support.test"),
            cost: new Money(0));

        Assert.Contains(
            MountSupportDefinitionValidator.Validate(definition),
            error => error.Contains(
                "cost",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ZeroListedWeight_IsRejected()
    {
        MountSupportDefinition definition = Create(
            new MountSupportId("dnd5e2014.mount-support.test"),
            listedWeight: new Weight(0));

        Assert.Contains(
            MountSupportDefinitionValidator.Validate(definition),
            error => error.Contains(
                "greater than zero",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        var ruleId = new RuleId("dnd5e2014.mount-support-rule.test");

        MountSupportDefinition definition = Create(
            new MountSupportId("dnd5e2014.mount-support.test"),
            specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            MountSupportDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NoSources_IsRejected()
    {
        MountSupportDefinition definition = Create(
            new MountSupportId("dnd5e2014.mount-support.test"),
            sources: []);

        Assert.Contains(
            MountSupportDefinitionValidator.Validate(definition),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    private static MountSupportDefinition Create(
        MountSupportId id,
        Money? cost = null,
        Weight? listedWeight = null,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new MountSupportDefinition(
            id,
            "Test mount support",
            cost ?? new Money(100),
            listedWeight ?? new Weight(1),
            specialRuleIds ?? [],
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);
    }
}
