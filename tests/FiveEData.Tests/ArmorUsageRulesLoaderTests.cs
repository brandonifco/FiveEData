using FiveEData.Rules.Equipment;
using FiveEData.Rules.Equipment.Armor.Serialization;

namespace FiveEData.Tests;

public sealed class ArmorUsageRulesLoaderTests
{
    [Fact]
    public void LoadFromJson_MapsStructuredUsageRules()
    {
        var rules = ArmorUsageRulesLoader.LoadFromJson(ValidJson);

        Assert.Equal(10, rules.InsufficientStrengthSpeedReduction.Feet);
        Assert.Equal(1, rules.ShieldHandsRequired);
        Assert.Equal(1, rules.MaximumBenefitingShields);
        Assert.True(rules.RequiresFullDonDurationForArmorClassBenefit);
        Assert.Equal(2, rules.DoffingWithHelpDivisor);
        Assert.Equal(5, rules.MediumArmorChangeTiming.Don.Amount);
        Assert.Equal(
            EquipmentChangeTimeUnit.Minute,
            rules.MediumArmorChangeTiming.Don.Unit);
        Assert.Equal(
            EquipmentChangeTimeUnit.Action,
            rules.ShieldChangeTiming.Don.Unit);
    }

    [Fact]
    public void LoadFromJson_MapsArmorProficiencyConsequences()
    {
        var rules = ArmorUsageRulesLoader.LoadFromJson(ValidJson);
        var consequences = rules.ArmorProficiencyConsequences;

        Assert.True(consequences.DisadvantageOnStrengthOrDexterityAbilityChecks);
        Assert.True(consequences.DisadvantageOnStrengthOrDexteritySavingThrows);
        Assert.True(consequences.DisadvantageOnStrengthOrDexterityAttackRolls);
        Assert.True(consequences.PreventsSpellcasting);
    }

    [Fact]
    public void MissingProficiencyConsequences_IsRejected()
    {
        string json = ValidJson.Replace(
            "  \"armorProficiencyConsequences\": {\n" +
            "    \"disadvantageOnStrengthOrDexterityAbilityChecks\": true,\n" +
            "    \"disadvantageOnStrengthOrDexteritySavingThrows\": true,\n" +
            "    \"disadvantageOnStrengthOrDexterityAttackRolls\": true,\n" +
            "    \"preventsSpellcasting\": true\n" +
            "  },\n",
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorUsageRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingTiming_IsRejected()
    {
        string json = ValidJson.Replace(
            "  \"shieldChangeTiming\": {\n" +
            "    \"don\": { \"amount\": 1, \"unit\": \"Action\" },\n" +
            "    \"doff\": { \"amount\": 1, \"unit\": \"Action\" }\n" +
            "  },\n",
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorUsageRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownJsonMember_IsRejected()
    {
        string json = ValidJson.Replace(
            "  \"shieldHandsRequired\": 1,",
            "  \"shieldHandsRequired\": 1,\n  \"unexpected\": true,",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorUsageRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void InvalidDurationAmount_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"amount\": 10, \"unit\": \"Minute\"",
            "\"amount\": 0, \"unit\": \"Minute\"",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorUsageRulesLoader.LoadFromJson(json));
    }

    private const string ValidJson = """
    {
      "armorProficiencyRuleId": "dnd5e2014.armor-rule.proficiency",
      "strengthSpeedRuleId": "dnd5e2014.armor-rule.strength-speed",
      "stealthRuleId": "dnd5e2014.armor-rule.stealth",
      "shieldRuleId": "dnd5e2014.armor-rule.shield",
      "donDoffRuleId": "dnd5e2014.armor-rule.don-doff",
      "armorProficiencyConsequences": {
        "disadvantageOnStrengthOrDexterityAbilityChecks": true,
        "disadvantageOnStrengthOrDexteritySavingThrows": true,
        "disadvantageOnStrengthOrDexterityAttackRolls": true,
        "preventsSpellcasting": true
      },
      "insufficientStrengthSpeedReductionFeet": 10,
      "shieldHandsRequired": 1,
      "maximumBenefitingShields": 1,
      "requiresFullDonDurationForArmorClassBenefit": true,
      "doffingWithHelpDivisor": 2,
      "lightArmorChangeTiming": {
        "don": { "amount": 1, "unit": "Minute" },
        "doff": { "amount": 1, "unit": "Minute" }
      },
      "mediumArmorChangeTiming": {
        "don": { "amount": 5, "unit": "Minute" },
        "doff": { "amount": 1, "unit": "Minute" }
      },
      "heavyArmorChangeTiming": {
        "don": { "amount": 10, "unit": "Minute" },
        "doff": { "amount": 5, "unit": "Minute" }
      },
      "shieldChangeTiming": {
        "don": { "amount": 1, "unit": "Action" },
        "doff": { "amount": 1, "unit": "Action" }
      },
      "sources": [
        {
          "documentId": "dnd5e2014.source.phb-first-printing",
          "page": 144,
          "section": "Armor and Shields"
        },
        {
          "documentId": "dnd5e2014.source.phb-first-printing",
          "page": 146,
          "section": "Getting Into and Out of Armor"
        }
      ]
    }
    """;
}
