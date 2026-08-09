using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Spells;

/// <summary>
/// A spell that imposes one or more named Appendix A conditions on a
/// failed saving throw — Charm Person's <c>charmed</c>, Tasha's Hideous
/// Laughter's <c>prone</c> and <c>incapacitated</c> together. Independent
/// of <see cref="SpellDamageEffect"/>: a spell may carry both (Ray of
/// Sickness deals damage on a hit and separately poisons on a failed
/// Constitution save), one, or neither.
///
/// <see cref="SavingThrowAbilityId"/> is required rather than nullable —
/// every condition-imposing spell found so far gates on a save. A spell
/// whose condition has no save at all (Sleep's hit-point-pool targeting)
/// is a compound mechanic declined the same way Color Spray's hit-point
/// pool is, not a reason to relax this field.
/// </summary>
public sealed record SpellConditionEffect
{
    public SpellConditionEffect(
        IEnumerable<ConditionId> conditionIds,
        AbilityId savingThrowAbilityId)
    {
        ConditionId[] conditions = conditionIds?.ToArray()
            ?? throw new ArgumentNullException(nameof(conditionIds));

        if (conditions.Length == 0)
        {
            throw new ArgumentException(
                "A spell condition effect must impose at least one " +
                "condition.",
                nameof(conditionIds));
        }

        if (conditions.Distinct().Count() != conditions.Length)
        {
            throw new ArgumentException(
                "A spell condition effect must not repeat a condition.",
                nameof(conditionIds));
        }

        ConditionIds = Array.AsReadOnly(conditions);
        SavingThrowAbilityId = savingThrowAbilityId;
    }

    public IReadOnlyList<ConditionId> ConditionIds { get; }

    public AbilityId SavingThrowAbilityId { get; }
}
