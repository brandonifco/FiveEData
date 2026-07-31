using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment;

namespace FiveEData.Rules.Equipment.Armor;

public sealed class ArmorUsageRules
{
    private readonly IReadOnlyList<RuleId> _referencedRuleIds;

    internal ArmorUsageRules(
        RuleId armorProficiencyRuleId,
        RuleId strengthSpeedRuleId,
        RuleId stealthRuleId,
        RuleId shieldRuleId,
        RuleId donDoffRuleId,
        ArmorProficiencyConsequences armorProficiencyConsequences,
        Distance insufficientStrengthSpeedReduction,
        int shieldHandsRequired,
        int maximumBenefitingShields,
        bool requiresFullDonDurationForArmorClassBenefit,
        int doffingWithHelpDivisor,
        EquipmentChangeTiming lightArmorChangeTiming,
        EquipmentChangeTiming mediumArmorChangeTiming,
        EquipmentChangeTiming heavyArmorChangeTiming,
        EquipmentChangeTiming shieldChangeTiming,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);

        ArmorProficiencyRuleId = armorProficiencyRuleId;
        StrengthSpeedRuleId = strengthSpeedRuleId;
        StealthRuleId = stealthRuleId;
        ShieldRuleId = shieldRuleId;
        DonDoffRuleId = donDoffRuleId;
        ArmorProficiencyConsequences = armorProficiencyConsequences;
        InsufficientStrengthSpeedReduction = insufficientStrengthSpeedReduction;
        ShieldHandsRequired = shieldHandsRequired;
        MaximumBenefitingShields = maximumBenefitingShields;
        RequiresFullDonDurationForArmorClassBenefit =
            requiresFullDonDurationForArmorClassBenefit;
        DoffingWithHelpDivisor = doffingWithHelpDivisor;
        LightArmorChangeTiming = lightArmorChangeTiming;
        MediumArmorChangeTiming = mediumArmorChangeTiming;
        HeavyArmorChangeTiming = heavyArmorChangeTiming;
        ShieldChangeTiming = shieldChangeTiming;
        Sources = Array.AsReadOnly(sources.ToArray());
        _referencedRuleIds = Array.AsReadOnly(
            new[]
            {
                ArmorProficiencyRuleId,
                StrengthSpeedRuleId,
                StealthRuleId,
                ShieldRuleId,
                DonDoffRuleId
            });
    }

    public RuleId ArmorProficiencyRuleId { get; }
    public RuleId StrengthSpeedRuleId { get; }
    public RuleId StealthRuleId { get; }
    public RuleId ShieldRuleId { get; }
    public RuleId DonDoffRuleId { get; }
    public ArmorProficiencyConsequences ArmorProficiencyConsequences { get; }
    public Distance InsufficientStrengthSpeedReduction { get; }
    public int ShieldHandsRequired { get; }
    public int MaximumBenefitingShields { get; }
    public bool RequiresFullDonDurationForArmorClassBenefit { get; }
    public int DoffingWithHelpDivisor { get; }
    public EquipmentChangeTiming LightArmorChangeTiming { get; }
    public EquipmentChangeTiming MediumArmorChangeTiming { get; }
    public EquipmentChangeTiming HeavyArmorChangeTiming { get; }
    public EquipmentChangeTiming ShieldChangeTiming { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
    public IReadOnlyList<RuleId> ReferencedRuleIds => _referencedRuleIds;
}
