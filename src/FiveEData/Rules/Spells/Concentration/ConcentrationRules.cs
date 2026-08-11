using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Spells.Concentration;

/// <summary>
/// The Concentration rules (p.203). A singleton rules object rather than a
/// catalog — a handful of flat constants with no named entries to key.
///
/// The saving throw DC is the one genuinely non-derivable constant here:
/// "10 or half the damage you take, whichever number is higher", stored as
/// <see cref="MinimumSavingThrowDC"/> plus
/// <see cref="DamageDivisorForSavingThrowDC"/> rather than as a formula.
///
/// Whether a given spell requires concentration is not stored here — that
/// lives on each spell's own <see cref="SpellDuration"/>.
/// </summary>
public sealed class ConcentrationRules
{
    internal ConcentrationRules(
        AbilityId savingThrowAbilityId,
        int minimumSavingThrowDC,
        int damageDivisorForSavingThrowDC,
        bool separateSavingThrowPerDamageSource,
        bool endsWhenCastingAnotherConcentrationSpell,
        bool endsOnDeath,
        bool canBeEndedVoluntarilyWithoutAnAction,
        IEnumerable<ConditionId> endedByConditionIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(endedByConditionIds);
        ArgumentNullException.ThrowIfNull(sources);

        SavingThrowAbilityId = savingThrowAbilityId;
        MinimumSavingThrowDC = minimumSavingThrowDC;
        DamageDivisorForSavingThrowDC = damageDivisorForSavingThrowDC;
        SeparateSavingThrowPerDamageSource = separateSavingThrowPerDamageSource;
        EndsWhenCastingAnotherConcentrationSpell =
            endsWhenCastingAnotherConcentrationSpell;
        EndsOnDeath = endsOnDeath;
        CanBeEndedVoluntarilyWithoutAnAction =
            canBeEndedVoluntarilyWithoutAnAction;
        EndedByConditionIds =
            Array.AsReadOnly(endedByConditionIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AbilityId SavingThrowAbilityId { get; }

    public int MinimumSavingThrowDC { get; }

    /// <summary>
    /// "half the damage you take" — the divisor the printed rule states,
    /// not a precomputed DC. A consumer takes the higher of
    /// <see cref="MinimumSavingThrowDC"/> and damage divided by this.
    /// </summary>
    public int DamageDivisorForSavingThrowDC { get; }

    /// <summary>
    /// "If you take damage from multiple sources, such as an arrow and a
    /// dragon's breath, you make a separate saving throw for each source."
    /// </summary>
    public bool SeparateSavingThrowPerDamageSource { get; }

    /// <summary>
    /// The same fact as "You can't concentrate on two spells at once" —
    /// the PHB states it as both a cause and a cap, so it is stored once.
    /// </summary>
    public bool EndsWhenCastingAnotherConcentrationSpell { get; }

    public bool EndsOnDeath { get; }

    /// <summary>"You can end concentration at any time (no action required)."</summary>
    public bool CanBeEndedVoluntarilyWithoutAnAction { get; }

    /// <summary>
    /// Named Appendix A conditions that end concentration outright — the
    /// PHB names <c>incapacitated</c> directly, so this is a real
    /// reference rather than an inferred tag.
    /// </summary>
    public IReadOnlyList<ConditionId> EndedByConditionIds { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
