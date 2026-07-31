using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment;
using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Tests;

public sealed class ArmorUsageRulesValidatorTests
{
    [Fact]
    public void Validator_RejectsZeroShieldHandsRequired()
    {
        ArmorUsageRules rules = CreateRules(shieldHandsRequired: 0);

        Assert.Throws<InvalidOperationException>(
            () => ArmorUsageRulesValidator.EnsureValid(rules));
    }

    [Fact]
    public void Validator_RejectsNonReducingHelpDivisor()
    {
        ArmorUsageRules rules = CreateRules(doffingWithHelpDivisor: 1);

        Assert.Throws<InvalidOperationException>(
            () => ArmorUsageRulesValidator.EnsureValid(rules));
    }

    [Fact]
    public void Validator_RejectsDefaultChangeTiming()
    {
        ArmorUsageRules rules = CreateRules(
            lightArmorChangeTiming:
                new EquipmentChangeTiming(default, default));

        Assert.Throws<InvalidOperationException>(
            () => ArmorUsageRulesValidator.EnsureValid(rules));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        ArmorUsageRules rules = CreateRules(sources: []);

        Assert.Throws<InvalidOperationException>(
            () => ArmorUsageRulesValidator.EnsureValid(rules));
    }

    private static ArmorUsageRules CreateRules(
        int shieldHandsRequired = 1,
        int doffingWithHelpDivisor = 2,
        EquipmentChangeTiming? lightArmorChangeTiming = null,
        IEnumerable<SourceReference>? sources = null)
    {
        var minute = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Minute);
        var action = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Action);

        return new ArmorUsageRules(
            new RuleId("dnd5e2014.armor-rule.proficiency"),
            new RuleId("dnd5e2014.armor-rule.strength-speed"),
            new RuleId("dnd5e2014.armor-rule.stealth"),
            new RuleId("dnd5e2014.armor-rule.shield"),
            new RuleId("dnd5e2014.armor-rule.don-doff"),
            new ArmorProficiencyConsequences(true, true, true, true),
            new Distance(10),
            shieldHandsRequired,
            maximumBenefitingShields: 1,
            requiresFullDonDurationForArmorClassBenefit: true,
            doffingWithHelpDivisor: doffingWithHelpDivisor,
            lightArmorChangeTiming ?? new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(action, action),
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 144)
            ]);
    }
}
