using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;

namespace FiveEData.Tests;

public sealed class ToolDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ToolDefinition definition = Assert.Single(
            ToolDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.tool.test","name":"Test tool","cost":{"copperPieces":100},"weight":{"pounds":1},"familyId":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":154,"section":"Chapter 5: Equipment — Tools"}]}]"""));

        Assert.Equal("dnd5e2014.tool.test", definition.Id.Value);
        Assert.Equal(100, definition.Cost.CopperPieces);
        Assert.Equal(1m, definition.Weight?.Pounds);
        Assert.Null(definition.FamilyId);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => ToolDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json = """[{"id":"dnd5e2014.tool.test","name":"Test","cost":{"copperPieces":100},"weight":null,"familyId":null,"specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => ToolDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredFamilyIdMember_IsRejected()
    {
        string json = """[{"id":"dnd5e2014.tool.test","name":"Test","cost":{"copperPieces":100},"weight":null,"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => ToolDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one = """{"id":"dnd5e2014.tool.test","name":"Test","cost":{"copperPieces":100},"weight":null,"familyId":null,"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":154,"section":null}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => ToolDefinitionLoader.LoadFromJson(json));
    }
}
