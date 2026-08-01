using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Mounts.Serialization;

namespace FiveEData.Tests;

public sealed class MountDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        MountDefinition definition = Assert.Single(
            MountDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"speedFeet":40,"baseCarryingCapacity":{"pounds":100},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":155,"section":"Chapter 5: Equipment — Mounts and Vehicles"}]}]"""));

        Assert.Equal("dnd5e2014.mount.test", definition.Id.Value);
        Assert.Equal(100, definition.Cost.CopperPieces);
        Assert.Equal(40, definition.Speed.Feet);
        Assert.Equal(100m, definition.BaseCarryingCapacity.Pounds);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"speedFeet":40,"baseCarryingCapacity":{"pounds":100},"specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredSpeedMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"baseCarryingCapacity":{"pounds":100},"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredBaseCarryingCapacityMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"speedFeet":40,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredSpecialRuleIdsMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"speedFeet":40,"baseCarryingCapacity":{"pounds":100},"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """{"id":"dnd5e2014.mount.test","name":"Test mount","cost":{"copperPieces":100},"speedFeet":40,"baseCarryingCapacity":{"pounds":100},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":155,"section":null}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => MountDefinitionLoader.LoadFromJson(json));
    }
}
