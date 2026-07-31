using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;

namespace FiveEData.Tests;

public sealed class ToolFamilyDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ToolFamilyDefinition definition = Assert.Single(
            ToolFamilyDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.tool-family.test","name":"Test family","specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":154,"section":"Chapter 5: Equipment — Tools"}]}]"""));

        Assert.Equal("dnd5e2014.tool-family.test", definition.Id.Value);
        Assert.Equal("Test family", definition.Name);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => ToolFamilyDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json = """[{"id":"dnd5e2014.tool-family.test","name":"Test","specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => ToolFamilyDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        string json = """[{"id":"dnd5e2014.tool-family.test","name":"Test","specialRuleIds":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => ToolFamilyDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one = """{"id":"dnd5e2014.tool-family.test","name":"Test","specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":154,"section":null}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => ToolFamilyDefinitionLoader.LoadFromJson(json));
    }
}
