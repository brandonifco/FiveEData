using FiveEData.Rules.Classes.TotemWarriorOptions;
using FiveEData.Rules.Classes.TotemWarriorOptions.Serialization;

namespace FiveEData.Tests;

public sealed class TotemWarriorOptionDefinitionLoaderTests
{
    private const string MinimalMembers =
        """
        "requiresRaging": false,
        "requiresNotWearingHeavyArmor": false,
        "resistsAllDamageExceptTypeId": null,
        "imposesDisadvantageOnOpportunityAttacksAgainstYou": false,
        "grantsDashAsBonusAction": false,
        "grantsAlliesAdvantageOnMeleeAttacksWithinFeet": null,
        "doublesCarryingCapacity": false,
        "grantsAdvantageOnStrengthChecksToMoveObjects": false,
        "clearSightRangeFeet": null,
        "clearSightDetailEquivalentRangeFeet": null,
        "ignoresDimLightPerceptionDisadvantage": false,
        "tracksAtTravelPaceId": null,
        "movesStealthilyAtTravelPaceId": null,
        "imposesDisadvantageOnAttacksAgainstOthersWithinFeet": null,
        "grantsFlyingSpeedEqualToWalkingSpeed": false,
        "imposedConditionId": null,
        "maximumTargetSizeId": null,
        "imposedConditionRequiresBonusAction": false
        """;

    private const string TestSources =
        """
        "sources": [
          {
            "documentId": "extension.source.test",
            "page": 1,
            "section": "Test section"
          }
        ]
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        TotemWarriorOptionDefinition definition = Assert.Single(
            TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.totem-warrior-option.test",
                    "name": "Test",
                    "requiredLevel": 3,
                    {{MinimalMembers}},
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal(
            "extension.totem-warrior-option.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(3, definition.RequiredLevel);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        TotemWarriorOptionDefinition definition = Assert.Single(
            TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.totem-warrior-option.test",
                    "name": "Test",
                    "requiredLevel": 14,
                    "requiresRaging": true,
                    "requiresNotWearingHeavyArmor": true,
                    "resistsAllDamageExceptTypeId":
                      "dnd5e2014.damage-type.psychic",
                    "imposesDisadvantageOnOpportunityAttacksAgainstYou": true,
                    "grantsDashAsBonusAction": true,
                    "grantsAlliesAdvantageOnMeleeAttacksWithinFeet": 5,
                    "doublesCarryingCapacity": true,
                    "grantsAdvantageOnStrengthChecksToMoveObjects": true,
                    "clearSightRangeFeet": 5280,
                    "clearSightDetailEquivalentRangeFeet": 100,
                    "ignoresDimLightPerceptionDisadvantage": true,
                    "tracksAtTravelPaceId": "dnd5e2014.travel-pace.fast",
                    "movesStealthilyAtTravelPaceId":
                      "dnd5e2014.travel-pace.normal",
                    "imposesDisadvantageOnAttacksAgainstOthersWithinFeet": 5,
                    "grantsFlyingSpeedEqualToWalkingSpeed": true,
                    "imposedConditionId": "dnd5e2014.condition.prone",
                    "maximumTargetSizeId": "dnd5e2014.creature-size.large",
                    "imposedConditionRequiresBonusAction": true,
                    {{TestSources}}
                  }
                ]
                """));

        Assert.True(definition.RequiresRaging);
        Assert.True(definition.RequiresNotWearingHeavyArmor);
        Assert.Equal(
            "dnd5e2014.damage-type.psychic",
            definition.ResistsAllDamageExceptTypeId?.Value);
        Assert.True(
            definition.ImposesDisadvantageOnOpportunityAttacksAgainstYou);
        Assert.True(definition.GrantsDashAsBonusAction);
        Assert.Equal(
            5,
            definition.GrantsAlliesAdvantageOnMeleeAttacksWithinFeet);
        Assert.True(definition.DoublesCarryingCapacity);
        Assert.True(definition.GrantsAdvantageOnStrengthChecksToMoveObjects);
        Assert.Equal(5280, definition.ClearSightRangeFeet);
        Assert.Equal(100, definition.ClearSightDetailEquivalentRangeFeet);
        Assert.True(definition.IgnoresDimLightPerceptionDisadvantage);
        Assert.Equal(
            "dnd5e2014.travel-pace.fast",
            definition.TracksAtTravelPaceId?.Value);
        Assert.Equal(
            "dnd5e2014.travel-pace.normal",
            definition.MovesStealthilyAtTravelPaceId?.Value);
        Assert.Equal(
            5,
            definition.ImposesDisadvantageOnAttacksAgainstOthersWithinFeet);
        Assert.True(definition.GrantsFlyingSpeedEqualToWalkingSpeed);
        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
        Assert.True(definition.ImposedConditionRequiresBonusAction);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
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
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.totem-warrior-option.test",
                    "name": "Test",
                    "requiredLevel": 3,
                    {{MinimalMembers}},
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
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.totem-warrior-option.test",
                    "name": "Test",
                    "name": "Other",
                    "requiredLevel": 3,
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.totem-warrior-option.test",
                    "name": "Test",
                    "requiredLevel": 3,
                    {{MinimalMembers}}
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": null,
                    "name": "Test",
                    "requiredLevel": 3,
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            $$"""
            {
              "id": "extension.totem-warrior-option.test",
              "name": "Test",
              "requiredLevel": 3,
              {{MinimalMembers}},
              {{TestSources}}
            }
            """;

        Assert.Throws<InvalidDataException>(
            () => TotemWarriorOptionDefinitionLoader.LoadFromJson(
                $"[{one},{one}]"));
    }
}
