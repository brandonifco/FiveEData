using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

namespace FiveEData.Tests;

public sealed class ElementalDisciplineDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictlyWithKiCostAndLevel()
    {
        ElementalDisciplineDefinition definition = Assert.Single(
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 3,
                    "requiredMinimumLevel": 6,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null,
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
            "extension.elemental-discipline.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(3, definition.KiPointCost);
        Assert.Equal(6, definition.RequiredMinimumLevel);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsWithNoKiCostAndNoLevel()
    {
        ElementalDisciplineDefinition definition = Assert.Single(
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": null,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null,
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

        Assert.Null(definition.KiPointCost);
        Assert.Null(definition.RequiredMinimumLevel);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        ElementalDisciplineDefinition definition = Assert.Single(
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": "dnd5e2014.spell.thunderwave",
                    "savingThrowAbilityId": "dnd5e2014.ability.strength",
                    "baseDamage": { "count": 3, "sides": 10 },
                    "baseDamageTypeId":
                      "dnd5e2014.damage-type.bludgeoning",
                    "halfDamageOnSuccessfulSave": true,
                    "rangeFeet": 30,
                    "pushDistanceFeet": 20,
                    "imposedConditionId": "dnd5e2014.condition.prone",
                    "reachIncreaseFeet": 10,
                    "changesUnarmedDamageTypeId":
                      "dnd5e2014.damage-type.fire",
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
            "dnd5e2014.spell.thunderwave",
            definition.GrantedSpellId?.Value);
        Assert.Equal(
            "dnd5e2014.ability.strength",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(3, definition.BaseDamage?.Count);
        Assert.Equal(10, definition.BaseDamage?.Sides);
        Assert.Equal(
            "dnd5e2014.damage-type.bludgeoning",
            definition.BaseDamageTypeId?.Value);
        Assert.True(definition.HalfDamageOnSuccessfulSave);
        Assert.Equal(30, definition.RangeFeet);
        Assert.Equal(20, definition.PushDistanceFeet);
        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(10, definition.ReachIncreaseFeet);
        Assert.Equal(
            "dnd5e2014.damage-type.fire",
            definition.ChangesUnarmedDamageTypeId?.Value);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => ElementalDisciplineDefinitionLoader.LoadFromJson(
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null,
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "name": "Other",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "grantedSpellId": null,
                    "savingThrowAbilityId": null,
                    "baseDamage": null,
                    "baseDamageTypeId": null,
                    "halfDamageOnSuccessfulSave": false,
                    "rangeFeet": null,
                    "pushDistanceFeet": null,
                    "imposedConditionId": null,
                    "reachIncreaseFeet": null,
                    "changesUnarmedDamageTypeId": null,
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
              "id": "extension.elemental-discipline.test",
              "name": "Test",
              "kiPointCost": 2,
              "requiredMinimumLevel": null,
              "grantedSpellId": null,
              "savingThrowAbilityId": null,
              "baseDamage": null,
              "baseDamageTypeId": null,
              "halfDamageOnSuccessfulSave": false,
              "rangeFeet": null,
              "pushDistanceFeet": null,
              "imposedConditionId": null,
              "reachIncreaseFeet": null,
              "changesUnarmedDamageTypeId": null,
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(json));
    }
}
