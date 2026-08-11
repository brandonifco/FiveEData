using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Characters.Encumbrance;

/// <summary>
/// One row of the Size and Strength rule (p.176): "For each size category
/// above Medium, double the creature's carrying capacity and the amount it
/// can push, drag, or lift. For a Tiny creature, halve these weights." The
/// multiplier is stored directly per size rather than as a "count the steps
/// from Medium" formula, since <see cref="CreatureSizeId"/> already
/// enumerates the closed set of sizes this applies to.
/// </summary>
public readonly record struct CarryingCapacitySizeMultiplierGrant
{
    public CarryingCapacitySizeMultiplierGrant(
        CreatureSizeId sizeId,
        double multiplier)
    {
        if (string.IsNullOrWhiteSpace(sizeId.Value))
        {
            throw new ArgumentException(
                "Carrying capacity size multiplier grant requires a size " +
                "ID.",
                nameof(sizeId));
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(multiplier),
                multiplier,
                "Carrying capacity size multiplier must be positive.");
        }

        SizeId = sizeId;
        Multiplier = multiplier;
    }

    public CreatureSizeId SizeId { get; }

    public double Multiplier { get; }
}
