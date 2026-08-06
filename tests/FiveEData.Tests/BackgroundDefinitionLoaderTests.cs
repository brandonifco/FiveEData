using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Backgrounds.Serialization;

namespace FiveEData.Tests;

public sealed class BackgroundDefinitionLoaderTests
{
    private const string ValidDefinition =
        """
        [
          {
            "id": "extension.background.test",
            "name": "Test",
            "skillProficiencyIds": [
              "dnd5e2014.skill.insight",
              "dnd5e2014.skill.religion"
            ],
            "languageChoiceCount": 2,
            "featureRuleId": "extension.background-rule.test",
            "sources": [
              {
                "documentId": "extension.source.test",
                "page": 1,
                "section": "Test section"
              }
            ]
          }
        ]
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        BackgroundDefinition definition = Assert.Single(
            BackgroundDefinitionLoader.LoadFromJson(ValidDefinition));

        Assert.Equal("extension.background.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            [
                "dnd5e2014.skill.insight",
                "dnd5e2014.skill.religion"
            ],
            definition.SkillProficiencyIds
                .Select(id => id.Value)
                .ToArray());
        Assert.Equal(2, definition.LanguageChoiceCount);
        Assert.Equal(
            "extension.background-rule.test",
            definition.FeatureRuleId.Value);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => BackgroundDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => BackgroundDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                BackgroundDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.background.test",
                        "name": "Test",
                        "skillProficiencyIds": [],
                        "languageChoiceCount": 0,
                        "featureRuleId": "extension.background-rule.test",
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
            () =>
                BackgroundDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.background.test",
                        "name": "Test",
                        "name": "Other",
                        "skillProficiencyIds": [],
                        "languageChoiceCount": 0,
                        "featureRuleId": "extension.background-rule.test",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Theory]
    [InlineData(
        """
        [{ "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test" }]
        """)]
    public void MissingRequiredMember_IsRejected(string json)
    {
        Assert.Throws<InvalidDataException>(
            () => BackgroundDefinitionLoader.LoadFromJson(json));
    }

    [Theory]
    [InlineData(
        """
        [{ "id": null, "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": null, "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "skillProficiencyIds": null, "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": null, "sources": [] }]
        """)]
    [InlineData(
        """
        [{ "id": "extension.background.test", "name": "Test", "skillProficiencyIds": [], "languageChoiceCount": 0, "featureRuleId": "extension.background-rule.test", "sources": null }]
        """)]
    public void NullRequiredMember_IsRejected(string json)
    {
        Assert.Throws<InvalidDataException>(
            () => BackgroundDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        const string one =
            """
            {
              "id": "extension.background.test",
              "name": "Test",
              "skillProficiencyIds": [
                "dnd5e2014.skill.insight",
                "dnd5e2014.skill.religion"
              ],
              "languageChoiceCount": 0,
              "featureRuleId": "extension.background-rule.test",
              "sources": [
                {
                  "documentId": "extension.source.test",
                  "page": 1
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => BackgroundDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void WrongSkillCount_IsRejectedAsDataError()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                BackgroundDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.background.test",
                        "name": "Test",
                        "skillProficiencyIds": ["dnd5e2014.skill.insight"],
                        "languageChoiceCount": 0,
                        "featureRuleId": "extension.background-rule.test",
                        "sources": [
                          {
                            "documentId": "extension.source.test",
                            "page": 1
                          }
                        ]
                      }
                    ]
                    """));
    }
}
