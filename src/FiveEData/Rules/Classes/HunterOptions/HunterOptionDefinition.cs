using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.HunterOptions;

public sealed class HunterOptionDefinition
{
    internal HunterOptionDefinition(
        HunterOptionId id,
        string name,
        int requiredLevel,
        DiceExpression? extraDamage,
        bool oncePerTurn,
        bool requiresTargetBelowHitPointMaximum,
        CreatureSizeId? minimumTargetSizeId,
        bool grantsExtraAttackAgainstDifferentTarget,
        int? secondaryTargetRangeFeet,
        bool imposesDisadvantageOnOpportunityAttacksAgainstYou,
        int? armorClassBonusAgainstSubsequentAttacks,
        ConditionId? grantsAdvantageOnSavingThrowsAgainstConditionId,
        int? attacksAnyNumberOfCreaturesWithinFeet,
        HunterMultiattackKind? multiattackKind,
        AbilityId? savingThrowAbilityId,
        bool negatesDamageOnSuccessfulSave,
        bool halfDamageOnFailedSave,
        bool halvesAttackDamageAsReaction,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RequiredLevel = requiredLevel;
        ExtraDamage = extraDamage;
        OncePerTurn = oncePerTurn;
        RequiresTargetBelowHitPointMaximum =
            requiresTargetBelowHitPointMaximum;
        MinimumTargetSizeId = minimumTargetSizeId;
        GrantsExtraAttackAgainstDifferentTarget =
            grantsExtraAttackAgainstDifferentTarget;
        SecondaryTargetRangeFeet = secondaryTargetRangeFeet;
        ImposesDisadvantageOnOpportunityAttacksAgainstYou =
            imposesDisadvantageOnOpportunityAttacksAgainstYou;
        ArmorClassBonusAgainstSubsequentAttacks =
            armorClassBonusAgainstSubsequentAttacks;
        GrantsAdvantageOnSavingThrowsAgainstConditionId =
            grantsAdvantageOnSavingThrowsAgainstConditionId;
        AttacksAnyNumberOfCreaturesWithinFeet =
            attacksAnyNumberOfCreaturesWithinFeet;
        MultiattackKind = multiattackKind;
        SavingThrowAbilityId = savingThrowAbilityId;
        NegatesDamageOnSuccessfulSave = negatesDamageOnSuccessfulSave;
        HalfDamageOnFailedSave = halfDamageOnFailedSave;
        HalvesAttackDamageAsReaction = halvesAttackDamageAsReaction;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public HunterOptionId Id { get; }
    public string Name { get; }
    public int RequiredLevel { get; }
    public DiceExpression? ExtraDamage { get; }
    public bool OncePerTurn { get; }
    public bool RequiresTargetBelowHitPointMaximum { get; }
    public CreatureSizeId? MinimumTargetSizeId { get; }
    public bool GrantsExtraAttackAgainstDifferentTarget { get; }
    public int? SecondaryTargetRangeFeet { get; }
    public bool ImposesDisadvantageOnOpportunityAttacksAgainstYou { get; }
    public int? ArmorClassBonusAgainstSubsequentAttacks { get; }
    public ConditionId? GrantsAdvantageOnSavingThrowsAgainstConditionId
    {
        get;
    }

    public int? AttacksAnyNumberOfCreaturesWithinFeet { get; }
    public HunterMultiattackKind? MultiattackKind { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public bool NegatesDamageOnSuccessfulSave { get; }
    public bool HalfDamageOnFailedSave { get; }
    public bool HalvesAttackDamageAsReaction { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
