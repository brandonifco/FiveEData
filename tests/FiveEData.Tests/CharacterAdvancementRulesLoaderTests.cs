using FiveEData.Rules.Characters.CharacterAdvancement;
using FiveEData.Rules.Characters.CharacterAdvancement.Serialization;

namespace FiveEData.Tests;

public sealed class CharacterAdvancementRulesLoaderTests
{
    [Fact]
    public void ValidRules_LoadStrictly()
    {
        CharacterAdvancementRules rules =
            CharacterAdvancementRulesLoader.LoadFromJson(BuildJson());

        Assert.Equal(20, rules.Levels.Count);
        Assert.Equal(0, rules.Levels[0].ExperiencePointThreshold);
        Assert.Equal(2, rules.Levels[0].ProficiencyBonus);
        Assert.Single(rules.Sources);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson("null"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                BuildJson(extra: "\"unexpected\": true,")));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                BuildJson(extra: "\"sources\": [],")));
    }

    [Fact]
    public void MissingRequiredLevelsMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                """
                {
                  "sources": []
                }
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                """
                {
                  "levels": []
                }
                """));
    }

    [Fact]
    public void ShortTable_IsRejectedByTheValidator()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                """
                {
                  "levels": [
                    {
                      "level": 1,
                      "experiencePointThreshold": 0,
                      "proficiencyBonus": 2
                    }
                  ],
                  "sources": [
                    {
                      "documentId": "extension.source.test",
                      "page": 1,
                      "section": "Test section"
                    }
                  ]
                }
                """));
    }

    [Fact]
    public void OutOfRangeLevel_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => CharacterAdvancementRulesLoader.LoadFromJson(
                BuildJson(firstLevelOverride: 0)));
    }

    private static string BuildJson(
        string extra = "",
        int? firstLevelOverride = null)
    {
        IEnumerable<string> levels = Enumerable
            .Range(1, 20)
            .Select(
                level =>
                    $$"""
                        {
                          "level": {{(level == 1 && firstLevelOverride
                              is { } o ? o : level)}},
                          "experiencePointThreshold": {{(level - 1) * 100}},
                          "proficiencyBonus": {{2 + ((level - 1) / 4)}}
                        }
                      """);

        return $$"""
            {
              {{extra}}
              "levels": [
                {{string.Join(",", levels)}}
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
    }
}
