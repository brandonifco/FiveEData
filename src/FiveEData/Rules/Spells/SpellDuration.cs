namespace FiveEData.Rules.Spells;

public readonly record struct SpellDuration
{
    private SpellDuration(
        bool isInstantaneous,
        bool isUntilDispelled,
        bool isSpecial,
        bool requiresConcentration,
        bool isUpTo,
        int? amount,
        SpellDurationUnit? unit)
    {
        IsInstantaneous = isInstantaneous;
        IsUntilDispelled = isUntilDispelled;
        IsSpecial = isSpecial;
        RequiresConcentration = requiresConcentration;
        IsUpTo = isUpTo;
        Amount = amount;
        Unit = unit;
    }

    public bool IsInstantaneous { get; }

    /// <summary>
    /// True for the PHB's "Until dispelled" duration, which has no span at
    /// all — Arcane Lock and Continual Flame. It is not an unbounded
    /// amount/unit pair, so it is its own flag rather than a sentinel
    /// amount.
    /// </summary>
    public bool IsUntilDispelled { get; }

    /// <summary>
    /// True for the PHB's "Duration: Special" header value, as in
    /// Creation, where the actual duration is a lookup table keyed by the
    /// material created (1 day to 1 minute) that lives entirely in the
    /// citation. Like <see cref="IsUntilDispelled"/>, it carries no
    /// amount/unit of its own.
    /// </summary>
    public bool IsSpecial { get; }

    /// <summary>
    /// True when the duration is the maximum the caster may sustain rather
    /// than a fixed span — the PHB's "up to" wording. Always true for a
    /// concentration duration, and independently true for a dismissible
    /// duration such as Prestidigitation's "Up to 1 hour".
    /// </summary>
    public bool IsUpTo { get; }

    public bool RequiresConcentration { get; }

    public int? Amount { get; }

    public SpellDurationUnit? Unit { get; }

    public static SpellDuration Instantaneous() =>
        new(true, false, false, false, false, null, null);

    public static SpellDuration UntilDispelled() =>
        new(false, true, false, false, false, null, null);

    public static SpellDuration Special() =>
        new(false, false, true, false, false, null, null);

    public static SpellDuration Fixed(int amount, SpellDurationUnit unit) =>
        Timed(amount, unit, isUpTo: false, requiresConcentration: false);

    public static SpellDuration UpTo(int amount, SpellDurationUnit unit) =>
        Timed(amount, unit, isUpTo: true, requiresConcentration: false);

    public static SpellDuration Concentration(
        int amount,
        SpellDurationUnit unit) =>
        Timed(amount, unit, isUpTo: true, requiresConcentration: true);

    private static SpellDuration Timed(
        int amount,
        SpellDurationUnit unit,
        bool isUpTo,
        bool requiresConcentration)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Spell duration amount must be greater than zero.");
        }

        if (!Enum.IsDefined(unit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(unit),
                unit,
                "Spell duration unit must be a defined value.");
        }

        return new SpellDuration(
            false,
            false,
            false,
            requiresConcentration,
            isUpTo,
            amount,
            unit);
    }
}
