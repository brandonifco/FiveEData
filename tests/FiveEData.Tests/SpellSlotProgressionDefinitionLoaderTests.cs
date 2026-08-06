using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.Spellcasting.Serialization;

namespace FiveEData.Tests;

public sealed class SpellSlotProgressionDefinitionLoaderTests
{
    private const string ValidProgression =
        """
        {
          "id": "extension.spell-slot-progression.test",
          "name": "Test",
          "recoveryRest": "LongRest",
          "levelEntries": LEVEL_ENTRIES_TOKEN,
          "sources": [
            {
              "documentId": "extension.source.test",
              "page": 1,
              "section": "Test section"
            }
          ]
        }
        """;

    private static string BuildLevelEntries(
        int levelWithSlots = 1)
    {
        var entries = new List<string>();

        for (int level = 1; level <= 20; level++)
        {
            string slots =
                level == levelWithSlots
                    ? "[{\"spellLevel\": 1, \"count\": 2}]"
                    : "[]";

            entries.Add(
                "{\"characterLevel\": " + level +
                ", \"slots\": " + slots + "}");
        }

        return "[" + string.Join(",", entries) + "]";
    }

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        string json = ValidProgression.Replace(
            "LEVEL_ENTRIES_TOKEN",
            BuildLevelEntries());

        SpellSlotProgressionDefinition definition = Assert.Single(
            SpellSlotProgressionDefinitionLoader.LoadFromJson($"[{json}]"));

        Assert.Equal(
            "extension.spell-slot-progression.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            SpellSlotRecoveryRest.LongRest,
            definition.RecoveryRest);
        Assert.Equal(20, definition.LevelEntries.Count);

        SpellSlotLevelEntry firstLevel = definition.LevelEntries[0];
        Assert.Equal(1, firstLevel.CharacterLevel);

        SpellSlotCount slot = Assert.Single(firstLevel.Slots);
        Assert.Equal(1, slot.SpellLevel);
        Assert.Equal(2, slot.Count);

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsShortOrLongRestRecovery()
    {
        string json = ValidProgression
            .Replace("\"recoveryRest\": \"LongRest\"", "\"recoveryRest\": \"ShortOrLongRest\"")
            .Replace("LEVEL_ENTRIES_TOKEN", BuildLevelEntries());

        SpellSlotProgressionDefinition definition = Assert.Single(
            SpellSlotProgressionDefinitionLoader.LoadFromJson($"[{json}]"));

        Assert.Equal(
            SpellSlotRecoveryRest.ShortOrLongRest,
            definition.RecoveryRest);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    SpellSlotProgressionDefinitionLoader.LoadFromJson(
                        "[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.spell-slot-progression.test",
                    "name": "Test",
                    "recoveryRest": "LongRest",
                    "levelEntries": [],
                    "sources": [],
                    "unexpected": true
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.spell-slot-progression.test",
                    "name": "Test",
                    "name": "Other",
                    "recoveryRest": "LongRest",
                    "levelEntries": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredLevelEntriesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.spell-slot-progression.test",
                    "name": "Test",
                    "recoveryRest": "LongRest",
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "recoveryRest": "LongRest",
                    "levelEntries": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SpellSlotProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.spell-slot-progression.test",
                    "name": "Test",
                    "recoveryRest": "LongRest",
                    "levelEntries": [],
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = ValidProgression.Replace(
            "LEVEL_ENTRIES_TOKEN",
            BuildLevelEntries());

        Assert.Throws<InvalidDataException>(
            () =>
                SpellSlotProgressionDefinitionLoader.LoadFromJson(
                    $"[{json},{json}]"));
    }
}
