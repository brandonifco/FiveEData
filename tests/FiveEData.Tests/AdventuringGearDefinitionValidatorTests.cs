using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class AdventuringGearDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_IsAccepted()
    {
        Assert.Empty(
            AdventuringGearDefinitionValidator.Validate(
                CreateDefinition()));
    }

    [Fact]
    public void DefaultId_IsRejected()
    {
        AdventuringGearDefinition definition = CreateDefinition(id: default(AdventuringGearId));

        Assert.Contains(
            AdventuringGearDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCostAndMissingSource_AreRejected()
    {
        AdventuringGearDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.invalid"),
            "Invalid",
            new Money(0),
            listedWeight: null,
            specialRuleIds: [],
            sources: []);

        IReadOnlyList<string> errors =
            AdventuringGearDefinitionValidator.Validate(definition);

        Assert.Contains(
            errors,
            error => error.Contains("cost", StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains("source", StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        var ruleId = new RuleId("dnd5e2014.adventuring-gear-rule.test");

        AdventuringGearDefinition definition =
            CreateDefinition(specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            AdventuringGearDefinitionValidator.Validate(definition),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    private static AdventuringGearDefinition CreateDefinition(
        AdventuringGearId? id = null,
        IEnumerable<RuleId>? specialRuleIds = null)
    {
        return new AdventuringGearDefinition(
            id ?? new AdventuringGearId(
                "dnd5e2014.adventuring-gear.test"),
            "Test gear",
            new Money(100),
            new ListedWeight(new Weight(1m)),
            specialRuleIds ?? [],
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150)
            ]);
    }
}
