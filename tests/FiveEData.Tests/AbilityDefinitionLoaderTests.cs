using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Abilities.Serialization;

namespace FiveEData.Tests;

public sealed class AbilityDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        AbilityDefinition definition = Assert.Single(
            AbilityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "dnd5e2014.ability.test",
                    "name": "Test",
                    "sources": [
                      {
                        "documentId": "dnd5e2014.source.phb-first-printing",
                        "page": 173,
                        "section": "Chapter 7"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "dnd5e2014.ability.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    AbilityDefinitionLoader.LoadFromJson(
                        "[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json =
            """
            [
              {
                "id": "dnd5e2014.ability.test",
                "name": "Test",
                "sources": [],
                "unexpected": true
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => AbilityDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        string json =
            """
            [
              {
                "id": "dnd5e2014.ability.test",
                "name": "Test"
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => AbilityDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """
            {
              "id": "dnd5e2014.ability.test",
              "name": "Test",
              "sources": [
                {
                  "documentId": "dnd5e2014.source.phb-first-printing",
                  "page": 173,
                  "section": "Chapter 7"
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => AbilityDefinitionLoader.LoadFromJson(json));
    }
}
