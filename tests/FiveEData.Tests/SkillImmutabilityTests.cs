using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Tests;

public sealed class SkillImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 174)
        };

        var definition = new SkillDefinition(
            new SkillId(
                "dnd5e2014.skill.acrobatics"),
            "Acrobatics",
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }
}
