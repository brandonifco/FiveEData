using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.Serialization;

namespace FiveEData.Tests;

public sealed class SpellDefinitionLoaderTests
{
    private const string ValidJson =
        """
            [
              {
                "id": "extension.spell.test",
                "name": "Test",
                "level": 0,
                "schoolId": "dnd5e2014.magic-school.evocation",
                "castingTime": { "amount": 1, "unit": "Action" },
                "range": {
                  "kind": "Distance", "distanceFeet": 30,
                  "areaShape": null, "areaSizeFeet": null
                },
                "components": {
                  "verbal": true, "somatic": true,
                  "material": false, "materialDescription": null,
                  "materialCostGoldPieces": null
                },
                "duration": {
                  "isInstantaneous": true, "requiresConcentration": false,
                  "isUpTo": false, "amount": null, "unit": null
                },
                "isRitual": false,
                "availableToClassIds": ["dnd5e2014.class.wizard"],
                "sources": [{
                  "documentId": "extension.source.test",
                  "page": 1, "section": "Test section"
                }]
              }
            ]
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(ValidJson));

        Assert.Equal("extension.spell.test", definition.Id.Value);
        Assert.Equal(0, definition.Level);
        Assert.True(definition.IsCantrip);
        Assert.Equal(SpellRangeKind.Distance, definition.Range.Kind);
        Assert.Equal(30, definition.Range.DistanceFeet);
        Assert.True(definition.Duration.IsInstantaneous);
        Assert.Single(definition.AvailableToClassIds);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("[null]")]
    public void NullRootOrElement_IsRejected(string json)
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"name\": \"Test\"",
                    "\"name\": \"Test\", \"unexpected\": true",
                    StringComparison.Ordinal)));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"name\": \"Test\"",
                    "\"name\": \"Test\", \"name\": \"Other\"",
                    StringComparison.Ordinal)));
    }

    [Theory]
    [InlineData("\"id\": \"extension.spell.test\"")]
    [InlineData("\"level\": 0")]
    [InlineData("\"schoolId\": \"dnd5e2014.magic-school.evocation\"")]
    [InlineData("\"availableToClassIds\": [\"dnd5e2014.class.wizard\"]")]
    public void MissingRequiredMember_IsRejected(string member)
    {
        string json = ValidJson.Replace(
            member + ",",
            string.Empty,
            StringComparison.Ordinal);

        Assert.NotEqual(ValidJson, json);
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UndefinedEnumValue_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"unit\": \"Action\"",
                    "\"unit\": \"Fortnight\"",
                    StringComparison.Ordinal)));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = ValidJson
            .TrimEnd()
            .TrimEnd(']')
            .TrimEnd()
            + "," + ValidJson.TrimStart().TrimStart('[');

        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MaterialWithoutDescription_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"material\": false",
                    "\"material\": true",
                    StringComparison.Ordinal)));
    }
}
