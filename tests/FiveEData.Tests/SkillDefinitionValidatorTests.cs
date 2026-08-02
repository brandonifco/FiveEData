using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Tests;

public sealed class SkillDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        var definition = new SkillDefinition(
            default,
            "Test",
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            [CreateSource()]);

        Assert.Contains(
            SkillDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultNormallyAssociatedAbilityId_IsRejected()
    {
        var definition = new SkillDefinition(
            new SkillId("dnd5e2014.skill.test"),
            "Test",
            default,
            [CreateSource()]);

        Assert.Contains(
            SkillDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ability ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        var definition = new SkillDefinition(
            new SkillId("dnd5e2014.skill.test"),
            "Test",
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            []);

        Assert.Contains(
            SkillDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 174);
    }
}
