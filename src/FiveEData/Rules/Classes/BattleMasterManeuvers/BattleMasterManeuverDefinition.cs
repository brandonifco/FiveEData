using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Classes.BattleMasterManeuvers;

public sealed class BattleMasterManeuverDefinition
{
    internal BattleMasterManeuverDefinition(
        BattleMasterManeuverId id,
        string name,
        BattleMasterManeuverEffectTarget effectTarget,
        AbilityId? savingThrowAbilityId,
        ConditionId? imposedConditionId,
        CreatureSizeId? maximumTargetSizeId,
        int? pushDistanceFeet,
        int? reachIncreaseFeet,
        int? secondaryTargetRangeFeet,
        bool forcesDroppedItem,
        bool grantsAdvantageOnNextAttackRoll,
        bool grantsAdvantageToNextAttackAgainstTarget,
        bool imposesDisadvantageOnAttacksAgainstOthers,
        bool allowsAllyReactionMovement,
        NextTurnDurationTrigger? secondaryEffectDurationTrigger,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        EffectTarget = effectTarget;
        SavingThrowAbilityId = savingThrowAbilityId;
        ImposedConditionId = imposedConditionId;
        MaximumTargetSizeId = maximumTargetSizeId;
        PushDistanceFeet = pushDistanceFeet;
        ReachIncreaseFeet = reachIncreaseFeet;
        SecondaryTargetRangeFeet = secondaryTargetRangeFeet;
        ForcesDroppedItem = forcesDroppedItem;
        GrantsAdvantageOnNextAttackRoll = grantsAdvantageOnNextAttackRoll;
        GrantsAdvantageToNextAttackAgainstTarget =
            grantsAdvantageToNextAttackAgainstTarget;
        ImposesDisadvantageOnAttacksAgainstOthers =
            imposesDisadvantageOnAttacksAgainstOthers;
        AllowsAllyReactionMovement = allowsAllyReactionMovement;
        SecondaryEffectDurationTrigger = secondaryEffectDurationTrigger;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public BattleMasterManeuverId Id { get; }
    public string Name { get; }
    public BattleMasterManeuverEffectTarget EffectTarget { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public ConditionId? ImposedConditionId { get; }
    public CreatureSizeId? MaximumTargetSizeId { get; }
    public int? PushDistanceFeet { get; }
    public int? ReachIncreaseFeet { get; }
    public int? SecondaryTargetRangeFeet { get; }
    public bool ForcesDroppedItem { get; }
    public bool GrantsAdvantageOnNextAttackRoll { get; }
    public bool GrantsAdvantageToNextAttackAgainstTarget { get; }
    public bool ImposesDisadvantageOnAttacksAgainstOthers { get; }
    public bool AllowsAllyReactionMovement { get; }

    public NextTurnDurationTrigger?
        SecondaryEffectDurationTrigger
    { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
