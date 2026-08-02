using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Tests;

public sealed class AbilityDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        var definition = new AbilityDefinition(
            default,
            "Test",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 173)
            ]);

        Assert.Contains(
            AbilityDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        var definition = new AbilityDefinition(
            new AbilityId("dnd5e2014.ability.test"),
            "Test",
            []);

        Assert.Contains(
            AbilityDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static AbilityDefinition Create(
        AbilityId? id = null)
    {
        return new AbilityDefinition(
            id ?? new AbilityId(
                "dnd5e2014.ability.test"),
            "Test",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 173)
            ]);
    }
}
