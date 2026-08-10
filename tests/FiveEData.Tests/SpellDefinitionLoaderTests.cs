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
                  "materialCostGoldPieces": null,
                  "materialIsConsumed": false
                },
                "duration": {
                  "isInstantaneous": true, "isUntilDispelled": false,
                  "isSpecial": false,
                  "requiresConcentration": false,
                  "isUpTo": false, "amount": null, "unit": null
                },
                "isRitual": false,
                "damageEffect": null,
                "conditionEffect": null,
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

    [Fact]
    public void UntilDispelledDuration_LoadsWithNoAmountOrUnit()
    {
        SpellDefinition definition = Assert.Single(
            SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"isInstantaneous\": true, \"isUntilDispelled\": false",
                    "\"isInstantaneous\": false, \"isUntilDispelled\": true",
                    StringComparison.Ordinal)));

        Assert.True(definition.Duration.IsUntilDispelled);
        Assert.False(definition.Duration.IsInstantaneous);
        Assert.Null(definition.Duration.Amount);
        Assert.Null(definition.Duration.Unit);
    }

    [Fact]
    public void DurationThatIsBothInstantaneousAndUntilDispelled_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"isUntilDispelled\": false",
                    "\"isUntilDispelled\": true",
                    StringComparison.Ordinal)));
    }

    [Fact]
    public void SpecialDuration_LoadsWithNoAmountOrUnit()
    {
        string json = ValidJson
            .Replace(
                "\"isInstantaneous\": true, \"isUntilDispelled\": false",
                "\"isInstantaneous\": false, \"isUntilDispelled\": false",
                StringComparison.Ordinal)
            .Replace(
                "\"isSpecial\": false,",
                "\"isSpecial\": true,",
                StringComparison.Ordinal);

        SpellDefinition definition = Assert.Single(
            SpellDefinitionLoader.LoadFromJson(json));

        Assert.True(definition.Duration.IsSpecial);
        Assert.False(definition.Duration.IsInstantaneous);
        Assert.False(definition.Duration.IsUntilDispelled);
        Assert.Null(definition.Duration.Amount);
        Assert.Null(definition.Duration.Unit);
    }

    [Fact]
    public void DurationThatIsBothInstantaneousAndSpecial_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"isSpecial\": false,",
                    "\"isSpecial\": true,",
                    StringComparison.Ordinal)));
    }

    [Fact]
    public void DamageEffect_LoadsWithAttackRoll()
    {
        string json = ValidJson.Replace(
            "\"damageEffect\": null,",
            """
            "damageEffect": {
              "damageTypeId": "dnd5e2014.damage-type.fire",
              "choosableDamageTypeIds": null,
              "attackRollType": "Ranged",
              "savingThrowAbilityId": null,
              "halfDamageOnSuccessfulSave": false,
              "damageByCharacterLevel": [
                { "characterLevel": 1, "damage": { "count": 1, "sides": 10 } },
                { "characterLevel": 5, "damage": { "count": 2, "sides": 10 } }
              ],
              "baseDamage": null,
              "flatDamageBonus": null
            },
            """,
            StringComparison.Ordinal);

        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(json));

        Assert.NotNull(definition.DamageEffect);
        Assert.Equal(
            SpellAttackRollType.Ranged,
            definition.DamageEffect!.AttackRollType);
        Assert.Null(definition.DamageEffect.SavingThrowAbilityId);
        Assert.Equal(2, definition.DamageEffect.DamageByCharacterLevel.Count);
    }

    [Fact]
    public void DamageEffect_LoadsWithSavingThrow()
    {
        string json = ValidJson.Replace(
            "\"damageEffect\": null,",
            """
            "damageEffect": {
              "damageTypeId": "dnd5e2014.damage-type.acid",
              "choosableDamageTypeIds": null,
              "attackRollType": null,
              "savingThrowAbilityId": "dnd5e2014.ability.dexterity",
              "halfDamageOnSuccessfulSave": false,
              "damageByCharacterLevel": [
                { "characterLevel": 1, "damage": { "count": 1, "sides": 6 } }
              ],
              "baseDamage": null,
              "flatDamageBonus": null
            },
            """,
            StringComparison.Ordinal);

        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(json));

        Assert.Null(definition.DamageEffect!.AttackRollType);
        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            definition.DamageEffect.SavingThrowAbilityId!.Value.Value);
    }

    [Fact]
    public void DamageEffectWithBothAttackRollAndSavingThrow_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"damageEffect\": null,",
            """
            "damageEffect": {
              "damageTypeId": "dnd5e2014.damage-type.acid",
              "choosableDamageTypeIds": null,
              "attackRollType": "Ranged",
              "savingThrowAbilityId": "dnd5e2014.ability.dexterity",
              "halfDamageOnSuccessfulSave": false,
              "damageByCharacterLevel": [
                { "characterLevel": 1, "damage": { "count": 1, "sides": 6 } }
              ],
              "baseDamage": null,
              "flatDamageBonus": null
            },
            """,
            StringComparison.Ordinal);

        Assert.ThrowsAny<Exception>(
            () => SpellDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DamageEffect_LoadsWithChoosableDamageTypesAndBaseDamage()
    {
        string json = ValidJson.Replace(
            "\"damageEffect\": null,",
            """
            "damageEffect": {
              "damageTypeId": null,
              "choosableDamageTypeIds": [
                "dnd5e2014.damage-type.acid", "dnd5e2014.damage-type.fire"
              ],
              "attackRollType": "Ranged",
              "savingThrowAbilityId": null,
              "halfDamageOnSuccessfulSave": false,
              "damageByCharacterLevel": null,
              "baseDamage": { "count": 3, "sides": 8 },
              "flatDamageBonus": null
            },
            """,
            StringComparison.Ordinal);

        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(json));

        Assert.Null(definition.DamageEffect!.DamageTypeId);
        Assert.Equal(
            2,
            definition.DamageEffect.ChoosableDamageTypeIds!.Count);
        Assert.Equal(3, definition.DamageEffect.BaseDamage!.Value.Count);
        Assert.Empty(definition.DamageEffect.DamageByCharacterLevel);
    }

    [Fact]
    public void DamageEffect_LoadsWithHalfDamageOnSuccessfulSave()
    {
        string json = ValidJson.Replace(
            "\"damageEffect\": null,",
            """
            "damageEffect": {
              "damageTypeId": "dnd5e2014.damage-type.fire",
              "choosableDamageTypeIds": null,
              "attackRollType": null,
              "savingThrowAbilityId": "dnd5e2014.ability.dexterity",
              "halfDamageOnSuccessfulSave": true,
              "damageByCharacterLevel": null,
              "baseDamage": { "count": 3, "sides": 6 },
              "flatDamageBonus": null
            },
            """,
            StringComparison.Ordinal);

        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(json));

        Assert.True(definition.DamageEffect!.HalfDamageOnSuccessfulSave);
    }

    [Fact]
    public void ConditionEffect_Loads()
    {
        string json = ValidJson.Replace(
            "\"conditionEffect\": null,",
            """
            "conditionEffect": {
              "conditionIds": ["dnd5e2014.condition.charmed"],
              "savingThrowAbilityId": "dnd5e2014.ability.wisdom"
            },
            """,
            StringComparison.Ordinal);

        SpellDefinition definition =
            Assert.Single(SpellDefinitionLoader.LoadFromJson(json));

        Assert.NotNull(definition.ConditionEffect);
        Assert.Equal(
            "dnd5e2014.condition.charmed",
            Assert.Single(definition.ConditionEffect!.ConditionIds).Value);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.ConditionEffect.SavingThrowAbilityId.Value);
    }

    [Fact]
    public void SpecialRange_LoadsWithNoDistance()
    {
        SpellDefinition definition = Assert.Single(
            SpellDefinitionLoader.LoadFromJson(
                ValidJson.Replace(
                    "\"kind\": \"Distance\", \"distanceFeet\": 30,",
                    "\"kind\": \"Special\", \"distanceFeet\": null,",
                    StringComparison.Ordinal)));

        Assert.Equal(SpellRangeKind.Special, definition.Range.Kind);
        Assert.Null(definition.Range.DistanceFeet);
    }
}
