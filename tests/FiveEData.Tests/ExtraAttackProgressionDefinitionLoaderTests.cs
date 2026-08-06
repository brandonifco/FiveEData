using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.ExtraAttack.Serialization;

namespace FiveEData.Tests;

public sealed class ExtraAttackProgressionDefinitionLoaderTests
{
    private const string ValidProgression =
        """
        {
          "id": "extension.extra-attack-progression.test",
          "name": "Test",
          "grants": [
            { "characterLevel": 5, "attackCount": 2 },
            { "characterLevel": 11, "attackCount": 3 }
          ],
          "sources": [
            {
              "documentId": "extension.source.test",
              "page": 1,
              "section": "Test section"
            }
          ]
        }
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ExtraAttackProgressionDefinition definition = Assert.Single(
            ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                $"[{ValidProgression}]"));

        Assert.Equal(
            "extension.extra-attack-progression.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(2, definition.Grants.Count);

        ExtraAttackGrant firstGrant = definition.Grants[0];
        Assert.Equal(5, firstGrant.CharacterLevel);
        Assert.Equal(2, firstGrant.AttackCount);

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    ExtraAttackProgressionDefinitionLoader.LoadFromJson(
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
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.extra-attack-progression.test",
                    "name": "Test",
                    "grants": [],
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
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.extra-attack-progression.test",
                    "name": "Test",
                    "name": "Other",
                    "grants": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredGrantsMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.extra-attack-progression.test",
                    "name": "Test",
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "grants": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.extra-attack-progression.test",
                    "name": "Test",
                    "grants": [],
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                    $"[{ValidProgression},{ValidProgression}]"));
    }
}
