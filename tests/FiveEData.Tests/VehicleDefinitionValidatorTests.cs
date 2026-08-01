using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class VehicleDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        VehicleDefinition definition = Create(
            default,
            VehicleKind.Land,
            new Weight(100),
            listedSpeed: null);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCost_IsRejected()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            new Weight(100),
            listedSpeed: null,
            cost: new Money(0));

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "cost",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DefaultKind_IsRejected()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            default,
            listedWeight: null,
            listedSpeed: null);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "kind",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LandVehicle_RequiresListedWeight()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            listedWeight: null,
            listedSpeed: null);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "listed weight",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LandVehicle_RejectsListedSpeed()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            new Weight(100),
            new VehicleSpeed(1));

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "listed speed",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LandVehicle_RejectsZeroListedWeight()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            new Weight(0),
            listedSpeed: null);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "greater than zero",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void WaterVehicle_RequiresListedSpeed()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Water,
            listedWeight: null,
            listedSpeed: null);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "listed speed",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void WaterVehicle_RejectsListedWeight()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Water,
            new Weight(100),
            new VehicleSpeed(1));

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "listed weight",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void WaterVehicle_RejectsZeroListedSpeed()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Water,
            listedWeight: null,
            listedSpeed: new VehicleSpeed(0));

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "greater than zero",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        var ruleId = new RuleId("dnd5e2014.vehicle-rule.test");
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            new Weight(100),
            listedSpeed: null,
            specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NoSources_IsRejected()
    {
        VehicleDefinition definition = Create(
            new VehicleId("dnd5e2014.vehicle.test"),
            VehicleKind.Land,
            new Weight(100),
            listedSpeed: null,
            sources: []);

        Assert.Contains(
            VehicleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    private static VehicleDefinition Create(
        VehicleId id,
        VehicleKind kind,
        Weight? listedWeight,
        VehicleSpeed? listedSpeed,
        Money? cost = null,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new VehicleDefinition(
            id,
            "Test vehicle",
            kind,
            cost ?? new Money(100),
            listedWeight,
            listedSpeed,
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
