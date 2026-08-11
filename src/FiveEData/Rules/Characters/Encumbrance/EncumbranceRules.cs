using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Characters.Encumbrance;

/// <summary>
/// The Size and Strength rule and the Variant: Encumbrance sidebar (p.176).
/// A singleton rules object rather than a catalog — a handful of flat
/// constants plus one small closed table, with no named entries of its own
/// to key.
///
/// "Ignore the Strength column of the Armor table in chapter 5" is
/// declined — it is an instruction about which other already-modelled rule
/// to disregard while this variant is active, not itself a game-mechanical
/// number, and stays in the citation.
///
/// Carrying capacity itself (Strength score x 15) and the push/drag/lift
/// multiplier (x2) stay declined too, the same "formula relative to a
/// value already modelled elsewhere" line the Long Rest's Hit Dice fraction
/// and Maneuvering Attack's half-speed movement already sit on — only the
/// size multiplier and the two encumbrance thresholds are genuinely
/// non-derivable facts.
/// </summary>
public sealed class EncumbranceRules
{
    internal EncumbranceRules(
        IEnumerable<CarryingCapacitySizeMultiplierGrant>
            sizeCarryingCapacityMultipliers,
        int encumberedCarryingCapacityMultiplier,
        int encumberedSpeedReductionFeet,
        int heavilyEncumberedCarryingCapacityMultiplier,
        int heavilyEncumberedSpeedReductionFeet,
        IEnumerable<AbilityId> heavilyEncumberedDisadvantageAbilityIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(sizeCarryingCapacityMultipliers);
        ArgumentNullException.ThrowIfNull(
            heavilyEncumberedDisadvantageAbilityIds);
        ArgumentNullException.ThrowIfNull(sources);

        SizeCarryingCapacityMultipliers =
            Array.AsReadOnly(sizeCarryingCapacityMultipliers.ToArray());
        EncumberedCarryingCapacityMultiplier =
            encumberedCarryingCapacityMultiplier;
        EncumberedSpeedReductionFeet = encumberedSpeedReductionFeet;
        HeavilyEncumberedCarryingCapacityMultiplier =
            heavilyEncumberedCarryingCapacityMultiplier;
        HeavilyEncumberedSpeedReductionFeet =
            heavilyEncumberedSpeedReductionFeet;
        HeavilyEncumberedDisadvantageAbilityIds =
            Array.AsReadOnly(
                heavilyEncumberedDisadvantageAbilityIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    /// <summary>
    /// "For each size category above Medium, double the creature's
    /// carrying capacity and the amount it can push, drag, or lift. For a
    /// Tiny creature, halve these weights." One entry per
    /// <c>CreatureSizeId</c> (Tiny 0.5, Small/Medium 1, Large 2, Huge 4,
    /// Gargantuan 8).
    /// </summary>
    public IReadOnlyList<CarryingCapacitySizeMultiplierGrant>
        SizeCarryingCapacityMultipliers
    { get; }

    /// <summary>
    /// "If you carry weight in excess of 5 times your Strength score, you
    /// are encumbered."
    /// </summary>
    public int EncumberedCarryingCapacityMultiplier { get; }

    public int EncumberedSpeedReductionFeet { get; }

    /// <summary>
    /// "If you carry weight in excess of 10 times your Strength score, up
    /// to your maximum carrying capacity, you are instead heavily
    /// encumbered."
    /// </summary>
    public int HeavilyEncumberedCarryingCapacityMultiplier { get; }

    public int HeavilyEncumberedSpeedReductionFeet { get; }

    /// <summary>
    /// "You have disadvantage on ability checks, attack rolls, and saving
    /// throws that use Strength, Dexterity, or Constitution" while heavily
    /// encumbered. Encumbered (the lighter tier) imposes no such
    /// disadvantage — only the speed reduction.
    /// </summary>
    public IReadOnlyList<AbilityId> HeavilyEncumberedDisadvantageAbilityIds
    { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
