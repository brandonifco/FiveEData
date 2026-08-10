using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.EldritchInvocations.Serialization;

namespace FiveEData.Tests;

public sealed class EldritchInvocationDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictlyWithNoPrerequisites()
    {
        EldritchInvocationDefinition definition = Assert.Single(
            EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "extension.eldritch-invocation.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.False(definition.RequiresEldritchBlastCantrip);
        Assert.Null(definition.RequiredMinimumLevel);
        Assert.Null(definition.RequiresPactBoon);
        Assert.Empty(definition.SkillProficiencyIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsAllPrerequisitesWhenPresent()
    {
        EldritchInvocationDefinition definition = Assert.Single(
            EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": true,
                    "requiredMinimumLevel": 12,
                    "requiresPactBoon": "Blade",
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.True(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(12, definition.RequiredMinimumLevel);
        Assert.Equal(WarlockPactBoon.Blade, definition.RequiresPactBoon);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        EldritchInvocationDefinition definition = Assert.Single(
            EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": "dnd5e2014.spell.mage-armor",
                    "castingFrequency": "AtWill",
                    "waivesMaterialComponents": true,
                    "addsSpellcastingModifierToDamage": true,
                    "extraDamageTypeId": "dnd5e2014.damage-type.necrotic",
                    "skillProficiencyIds": [
                      "dnd5e2014.skill.deception",
                      "dnd5e2014.skill.persuasion"
                    ],
                    "darknessVisionRangeFeet": 120,
                    "trueSightRangeFeet": 30,
                    "eldritchBlastRangeFeet": 300,
                    "eldritchBlastPushDistanceFeet": 10,
                    "canReadAllWriting": true,
                    "grantsSecondPactWeaponAttack": true,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "dnd5e2014.spell.mage-armor",
            definition.GrantedSpellId?.Value);
        Assert.Equal(
            EldritchInvocationCastingFrequency.AtWill,
            definition.CastingFrequency);
        Assert.True(definition.WaivesMaterialComponents);
        Assert.True(definition.AddsSpellcastingModifierToDamage);
        Assert.Equal(
            "dnd5e2014.damage-type.necrotic",
            definition.ExtraDamageTypeId?.Value);
        Assert.Equal(
            [
                "dnd5e2014.skill.deception",
                "dnd5e2014.skill.persuasion"
            ],
            definition.SkillProficiencyIds
                .Select(skillId => skillId.Value)
                .ToArray());
        Assert.Equal(120, definition.DarknessVisionRangeFeet);
        Assert.Equal(30, definition.TrueSightRangeFeet);
        Assert.Equal(300, definition.EldritchBlastRangeFeet);
        Assert.Equal(10, definition.EldritchBlastPushDistanceFeet);
        Assert.True(definition.CanReadAllWriting);
        Assert.True(definition.GrantsSecondPactWeaponAttack);
    }

    [Fact]
    public void InvalidRequiresPactBoonValue_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": "NotARealPactBoon",
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => EldritchInvocationDefinitionLoader.LoadFromJson(
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
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
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
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "name": "Other",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "grantedSpellId": null,
                    "castingFrequency": null,
                    "waivesMaterialComponents": false,
                    "addsSpellcastingModifierToDamage": false,
                    "extraDamageTypeId": null,
                    "skillProficiencyIds": [],
                    "darknessVisionRangeFeet": null,
                    "trueSightRangeFeet": null,
                    "eldritchBlastRangeFeet": null,
                    "eldritchBlastPushDistanceFeet": null,
                    "canReadAllWriting": false,
                    "grantsSecondPactWeaponAttack": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        const string one =
            """
            {
              "id": "extension.eldritch-invocation.test",
              "name": "Test",
              "requiresEldritchBlastCantrip": false,
              "requiredMinimumLevel": null,
              "requiresPactBoon": null,
              "grantedSpellId": null,
              "castingFrequency": null,
              "waivesMaterialComponents": false,
              "addsSpellcastingModifierToDamage": false,
              "extraDamageTypeId": null,
              "skillProficiencyIds": [],
              "darknessVisionRangeFeet": null,
              "trueSightRangeFeet": null,
              "eldritchBlastRangeFeet": null,
              "eldritchBlastPushDistanceFeet": null,
              "canReadAllWriting": false,
              "grantsSecondPactWeaponAttack": false,
              "sources": [
                {
                  "documentId": "extension.source.test",
                  "page": 1,
                  "section": "Test section"
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(json));
    }
}
