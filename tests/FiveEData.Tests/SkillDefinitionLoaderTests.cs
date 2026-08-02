using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Creatures.Skills.Serialization;

namespace FiveEData.Tests;

public sealed class SkillDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        SkillDefinition definition = Assert.Single(
            SkillDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "dnd5e2014.skill.test",
                    "name": "Test",
                    "normallyAssociatedAbilityId": "dnd5e2014.ability.dexterity",
                    "sources": [
                      {
                        "documentId": "dnd5e2014.source.phb-first-printing",
                        "page": 174,
                        "section": "Chapter 7"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "dnd5e2014.skill.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            definition.NormallyAssociatedAbilityId.Value);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    SkillDefinitionLoader.LoadFromJson(
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
                "id": "dnd5e2014.skill.test",
                "name": "Test",
                "normallyAssociatedAbilityId": "dnd5e2014.ability.dexterity",
                "sources": [],
                "unexpected": true
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => SkillDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredAbilityMember_IsRejected()
    {
        string json =
            """
            [
              {
                "id": "dnd5e2014.skill.test",
                "name": "Test",
                "sources": []
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => SkillDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullAbilityId_IsRejected()
    {
        string json =
            """
            [
              {
                "id": "dnd5e2014.skill.test",
                "name": "Test",
                "normallyAssociatedAbilityId": null,
                "sources": []
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => SkillDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """
            {
              "id": "dnd5e2014.skill.test",
              "name": "Test",
              "normallyAssociatedAbilityId": "dnd5e2014.ability.dexterity",
              "sources": [
                {
                  "documentId": "dnd5e2014.source.phb-first-printing",
                  "page": 174,
                  "section": "Chapter 7"
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => SkillDefinitionLoader.LoadFromJson(json));
    }
}
