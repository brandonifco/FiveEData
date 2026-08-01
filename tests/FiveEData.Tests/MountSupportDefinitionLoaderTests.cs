using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountSupport.Serialization;

namespace FiveEData.Tests;

public sealed class MountSupportDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        MountSupportDefinition definition = Assert.Single(
            MountSupportDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":1},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Tack, Harness, and Drawn Vehicles"}]}]"""));

        Assert.Equal("dnd5e2014.mount-support.test", definition.Id.Value);
        Assert.Equal(100, definition.Cost.CopperPieces);
        Assert.Equal(1m, definition.ListedWeight?.Pounds);
    }

    [Fact]
    public void NullListedWeight_LoadsWhenMemberIsPresent()
    {
        MountSupportDefinition definition = Assert.Single(
            MountSupportDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Tack, Harness, and Drawn Vehicles"}]}]"""));

        Assert.Null(definition.ListedWeight);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":1},"specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingListedWeightMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSpecialRuleIdsMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":1},"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSourcesMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":1},"specialRuleIds":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":1},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void ZeroListedWeight_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":100},"listedWeight":{"pounds":0},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void ZeroCost_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.mount-support.test","name":"Test mount support","cost":{"copperPieces":0},"listedWeight":{"pounds":1},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":null}]}]""";

        Assert.Throws<InvalidDataException>(
            () => MountSupportDefinitionLoader.LoadFromJson(json));
    }
}
