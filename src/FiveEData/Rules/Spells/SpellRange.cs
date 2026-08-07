namespace FiveEData.Rules.Spells;

public readonly record struct SpellRange
{
    private SpellRange(SpellRangeKind kind, int? distanceFeet)
    {
        Kind = kind;
        DistanceFeet = distanceFeet;
    }

    public SpellRangeKind Kind { get; }

    public int? DistanceFeet { get; }

    public static SpellRange Self() => new(SpellRangeKind.Self, null);

    public static SpellRange Touch() => new(SpellRangeKind.Touch, null);

    public static SpellRange Distance(int feet)
    {
        if (feet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(feet),
                feet,
                "Spell range distance must be greater than zero.");
        }

        return new SpellRange(SpellRangeKind.Distance, feet);
    }
}
