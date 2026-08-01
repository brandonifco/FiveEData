using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Mounts;

namespace FiveEData.Tests;

public sealed class MountDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        MountDefinition definition = Create(id: default(MountId));

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCost_IsRejected()
    {
        MountDefinition definition = Create(cost: new Money(0));

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains(
                "cost",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ZeroSpeed_IsRejected()
    {
        MountDefinition definition = Create(speed: new Distance(0));

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains(
                "speed",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ZeroBaseCarryingCapacity_IsRejected()
    {
        MountDefinition definition =
            Create(baseCarryingCapacity: new Weight(0));

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains(
                "carrying capacity",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        var ruleId = new RuleId("dnd5e2014.mount-rule.test");
        MountDefinition definition =
            Create(specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NoSources_IsRejected()
    {
        MountDefinition definition = Create(sources: []);

        Assert.Contains(
            MountDefinitionValidator.Validate(definition),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    private static MountDefinition Create(
        MountId? id = null,
        Money? cost = null,
        Distance? speed = null,
        Weight? baseCarryingCapacity = null,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new MountDefinition(
            id ?? new MountId("dnd5e2014.mount.test"),
            "Test mount",
            cost ?? new Money(100),
            speed ?? new Distance(40),
            baseCarryingCapacity ?? new Weight(100),
            specialRuleIds ?? [],
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 155)
            ]);
    }
}
