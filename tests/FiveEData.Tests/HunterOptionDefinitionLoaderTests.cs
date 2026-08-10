using FiveEData.Rules.Classes.HunterOptions;
using FiveEData.Rules.Classes.HunterOptions.Serialization;

namespace FiveEData.Tests;

public sealed class HunterOptionDefinitionLoaderTests
{
    private const string MinimalMembers =
        """
        "extraDamage": null,
        "oncePerTurn": false,
        "requiresTargetBelowHitPointMaximum": false,
        "minimumTargetSizeId": null,
        "grantsExtraAttackAgainstDifferentTarget": false,
        "secondaryTargetRangeFeet": null,
        "imposesDisadvantageOnOpportunityAttacksAgainstYou": false,
        "armorClassBonusAgainstSubsequentAttacks": null,
        "grantsAdvantageOnSavingThrowsAgainstConditionId": null,
        "attacksAnyNumberOfCreaturesWithinFeet": null,
        "multiattackKind": null,
        "savingThrowAbilityId": null,
        "negatesDamageOnSuccessfulSave": false,
        "halfDamageOnFailedSave": false,
        "halvesAttackDamageAsReaction": false
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
        HunterOptionDefinition definition = Assert.Single(
            HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
                    "name": "Test",
                    "requiredLevel": 3,
                    {{MinimalMembers}},
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal("extension.hunter-option.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(3, definition.RequiredLevel);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        HunterOptionDefinition definition = Assert.Single(
            HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
                    "name": "Test",
                    "requiredLevel": 15,
                    "extraDamage": { "count": 1, "sides": 8 },
                    "oncePerTurn": true,
                    "requiresTargetBelowHitPointMaximum": true,
                    "minimumTargetSizeId": "dnd5e2014.creature-size.large",
                    "grantsExtraAttackAgainstDifferentTarget": true,
                    "secondaryTargetRangeFeet": 5,
                    "imposesDisadvantageOnOpportunityAttacksAgainstYou": true,
                    "armorClassBonusAgainstSubsequentAttacks": 4,
                    "grantsAdvantageOnSavingThrowsAgainstConditionId":
                      "dnd5e2014.condition.frightened",
                    "attacksAnyNumberOfCreaturesWithinFeet": 10,
                    "multiattackKind": "Ranged",
                    "savingThrowAbilityId": "dnd5e2014.ability.dexterity",
                    "negatesDamageOnSuccessfulSave": true,
                    "halfDamageOnFailedSave": true,
                    "halvesAttackDamageAsReaction": true,
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal(1, definition.ExtraDamage?.Count);
        Assert.Equal(8, definition.ExtraDamage?.Sides);
        Assert.True(definition.OncePerTurn);
        Assert.True(definition.RequiresTargetBelowHitPointMaximum);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MinimumTargetSizeId?.Value);
        Assert.True(definition.GrantsExtraAttackAgainstDifferentTarget);
        Assert.Equal(5, definition.SecondaryTargetRangeFeet);
        Assert.True(
            definition.ImposesDisadvantageOnOpportunityAttacksAgainstYou);
        Assert.Equal(4, definition.ArmorClassBonusAgainstSubsequentAttacks);
        Assert.Equal(
            "dnd5e2014.condition.frightened",
            definition.GrantsAdvantageOnSavingThrowsAgainstConditionId?.Value);
        Assert.Equal(10, definition.AttacksAnyNumberOfCreaturesWithinFeet);
        Assert.Equal(
            HunterMultiattackKind.Ranged,
            definition.MultiattackKind);
        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            definition.SavingThrowAbilityId?.Value);
        Assert.True(definition.NegatesDamageOnSuccessfulSave);
        Assert.True(definition.HalfDamageOnFailedSave);
        Assert.True(definition.HalvesAttackDamageAsReaction);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => HunterOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => HunterOptionDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
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
            () => HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
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
            () => HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
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
            () => HunterOptionDefinitionLoader.LoadFromJson(
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
    public void UnknownMultiattackKind_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => HunterOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.hunter-option.test",
                    "name": "Test",
                    "requiredLevel": 11,
                    "extraDamage": null,
                    "oncePerTurn": false,
                    "requiresTargetBelowHitPointMaximum": false,
                    "minimumTargetSizeId": null,
                    "grantsExtraAttackAgainstDifferentTarget": false,
                    "secondaryTargetRangeFeet": null,
                    "imposesDisadvantageOnOpportunityAttacksAgainstYou": false,
                    "armorClassBonusAgainstSubsequentAttacks": null,
                    "grantsAdvantageOnSavingThrowsAgainstConditionId": null,
                    "attacksAnyNumberOfCreaturesWithinFeet": 10,
                    "multiattackKind": "Thrown",
                    "savingThrowAbilityId": null,
                    "negatesDamageOnSuccessfulSave": false,
                    "halfDamageOnFailedSave": false,
                    "halvesAttackDamageAsReaction": false,
                    {{TestSources}}
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
              "id": "extension.hunter-option.test",
              "name": "Test",
              "requiredLevel": 3,
              {{MinimalMembers}},
              {{TestSources}}
            }
            """;

        Assert.Throws<InvalidDataException>(
            () => HunterOptionDefinitionLoader.LoadFromJson(
                $"[{one},{one}]"));
    }
}
