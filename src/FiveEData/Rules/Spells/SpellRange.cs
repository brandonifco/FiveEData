namespace FiveEData.Rules.Spells;

public readonly record struct SpellRange
{
    private SpellRange(
        SpellRangeKind kind,
        int? distanceFeet,
        SpellAreaShape? areaShape,
        int? areaSizeFeet)
    {
        Kind = kind;
        DistanceFeet = distanceFeet;
        AreaShape = areaShape;
        AreaSizeFeet = areaSizeFeet;
    }

    public SpellRangeKind Kind { get; }

    public int? DistanceFeet { get; }

    /// <summary>
    /// Shape of the area a self-ranged spell originates, as in Burning
    /// Hands' "Self (15-foot cone)". Null when the spell has no area.
    /// </summary>
    public SpellAreaShape? AreaShape { get; }

    public int? AreaSizeFeet { get; }

    public static SpellRange Self() =>
        new(SpellRangeKind.Self, null, null, null);

    public static SpellRange SelfWithArea(
        SpellAreaShape shape,
        int sizeFeet)
    {
        if (!Enum.IsDefined(shape))
        {
            throw new ArgumentOutOfRangeException(
                nameof(shape),
                shape,
                "Spell area shape must be a defined value.");
        }

        if (sizeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sizeFeet),
                sizeFeet,
                "Spell area size must be greater than zero.");
        }

        return new SpellRange(SpellRangeKind.Self, null, shape, sizeFeet);
    }

    public static SpellRange Touch() =>
        new(SpellRangeKind.Touch, null, null, null);

    public static SpellRange Unlimited() =>
        new(SpellRangeKind.Unlimited, null, null, null);

    public static SpellRange Special() =>
        new(SpellRangeKind.Special, null, null, null);

    public static SpellRange Distance(int feet)
    {
        if (feet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(feet),
                feet,
                "Spell range distance must be greater than zero.");
        }

        return new SpellRange(SpellRangeKind.Distance, feet, null, null);
    }
}
