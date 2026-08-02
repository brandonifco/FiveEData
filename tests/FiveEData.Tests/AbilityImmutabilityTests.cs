using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Tests;

public sealed class AbilityImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 173)
        };

        var definition = new AbilityDefinition(
            new AbilityId(
                "dnd5e2014.ability.strength"),
            "Strength",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }
}
