using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Vehicles.Serialization;

namespace FiveEData.Tests;

public sealed class VehicleDefinitionLoaderTests
{
    [Fact]
    public void ValidLandDefinition_LoadsStrictly()
    {
        VehicleDefinition definition = Assert.Single(
            VehicleDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Tack, Harness, and Drawn Vehicles"}]}]"""));

        Assert.Equal("dnd5e2014.vehicle.test", definition.Id.Value);
        Assert.Equal(VehicleKind.Land, definition.Kind);
        Assert.Equal(100m, definition.ListedWeight?.Pounds);
        Assert.Null(definition.ListedSpeed);
    }

    [Fact]
    public void ValidWaterDefinition_LoadsStrictly()
    {
        VehicleDefinition definition = Assert.Single(
            VehicleDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Water","cost":{"copperPieces":100},"listedWeight":null,"listedSpeedMilesPerHour":1.5,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Waterborne Vehicles"}]}]"""));

        Assert.Equal(VehicleKind.Water, definition.Kind);
        Assert.Null(definition.ListedWeight);
        Assert.Equal(1.5m, definition.ListedSpeed?.MilesPerHour);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownEnumValue_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Air","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void IntegerEnumValue_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":1,"cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingListedWeightMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingListedSpeedMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Water","cost":{"copperPieces":100},"listedWeight":null,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSpecialRuleIdsMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSourcesMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void LandVehicleWithNullListedWeight_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":null,"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void WaterVehicleWithNullListedSpeed_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Water","cost":{"copperPieces":100},"listedWeight":null,"listedSpeedMilesPerHour":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void LandVehicleWithWaterborneListedSpeed_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Land","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":1,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void WaterVehicleWithDrawnVehicleListedWeight_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.vehicle.test","name":"Test vehicle","kind":"Water","cost":{"copperPieces":100},"listedWeight":{"pounds":100},"listedSpeedMilesPerHour":1,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => VehicleDefinitionLoader.LoadFromJson(json));
    }
}
