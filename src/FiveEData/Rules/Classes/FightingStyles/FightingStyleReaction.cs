using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.FightingStyles;

public readonly record struct FightingStyleReaction
{
    public FightingStyleReaction(
        Distance range,
        bool requiresShield)
    {
        if (range.Feet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(range),
                range.Feet,
                "Fighting style reaction range must be greater than zero.");
        }

        Range = range;
        RequiresShield = requiresShield;
    }

    public Distance Range { get; }

    public bool RequiresShield { get; }
}
