using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.ChannelDivinityOptions;

public sealed class ChannelDivinityOptionDefinition
{
    internal ChannelDivinityOptionDefinition(
        ChannelDivinityOptionId id,
        string name,
        int? rangeFeet,
        AbilityId? savingThrowAbilityId,
        int? durationMinutes,
        int? rollBonus,
        ConditionId? imposedConditionId,
        NextTurnDurationTrigger? conditionDurationTrigger,
        bool maximizesDamageRoll,
        SpellId? grantedSpellId,
        bool automaticallyFailsGrantedSpellSave,
        bool addsSpellcastingModifierToAttackRolls,
        int? brightLightRadiusFeet,
        int? dimLightRadiusFeet,
        IEnumerable<AbilityId> choosableSavingThrowAbilityIds,
        bool grantsAdvantageOnAttackRollsAgainstTarget,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(choosableSavingThrowAbilityIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        DurationMinutes = durationMinutes;
        RollBonus = rollBonus;
        ImposedConditionId = imposedConditionId;
        ConditionDurationTrigger = conditionDurationTrigger;
        MaximizesDamageRoll = maximizesDamageRoll;
        GrantedSpellId = grantedSpellId;
        AutomaticallyFailsGrantedSpellSave =
            automaticallyFailsGrantedSpellSave;
        AddsSpellcastingModifierToAttackRolls =
            addsSpellcastingModifierToAttackRolls;
        BrightLightRadiusFeet = brightLightRadiusFeet;
        DimLightRadiusFeet = dimLightRadiusFeet;
        ChoosableSavingThrowAbilityIds =
            Array.AsReadOnly(choosableSavingThrowAbilityIds.ToArray());
        GrantsAdvantageOnAttackRollsAgainstTarget =
            grantsAdvantageOnAttackRollsAgainstTarget;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ChannelDivinityOptionId Id { get; }
    public string Name { get; }
    public int? RangeFeet { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public int? DurationMinutes { get; }
    public int? RollBonus { get; }
    public ConditionId? ImposedConditionId { get; }
    public NextTurnDurationTrigger? ConditionDurationTrigger { get; }
    public bool MaximizesDamageRoll { get; }
    public SpellId? GrantedSpellId { get; }
    public bool AutomaticallyFailsGrantedSpellSave { get; }
    public bool AddsSpellcastingModifierToAttackRolls { get; }
    public int? BrightLightRadiusFeet { get; }
    public int? DimLightRadiusFeet { get; }
    public IReadOnlyList<AbilityId> ChoosableSavingThrowAbilityIds { get; }
    public bool GrantsAdvantageOnAttackRollsAgainstTarget { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
