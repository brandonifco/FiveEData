using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;

namespace FiveEData.Tests;

public sealed class AmmunitionDefinitionValidatorTests
{
    [Fact]
    public void RepresentativeDefinition_IsValid()
    {
        AmmunitionDefinition ammunition = Create(
            new AmmunitionTypeId(
                "dnd5e2014.ammunition.arrow"));

        Assert.Empty(
            AmmunitionDefinitionValidator.Validate(
                ammunition));
    }

    [Fact]
    public void DefaultAmmunitionId_IsRejected()
    {
        AmmunitionDefinition ammunition =
            Create(default);

        IReadOnlyList<string> errors =
            AmmunitionDefinitionValidator.Validate(
                ammunition);

        Assert.Contains(
            "Ammunition ID must not be empty.",
            errors);
    }

    private static AmmunitionDefinition Create(
        AmmunitionTypeId id)
    {
        return new AmmunitionDefinition(
            id,
            "Arrows",
            bundleQuantity: 20,
            new Money(100),
            new Weight(1),
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150,
                    section:
                        "Chapter 5: Equipment — Ammunition")
            ]);
    }
}
