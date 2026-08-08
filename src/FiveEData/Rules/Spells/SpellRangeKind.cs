namespace FiveEData.Rules.Spells;

public enum SpellRangeKind
{
    Self,
    Touch,
    Distance,

    /// <summary>
    /// No range limit at all, as in Sending's "Range: Unlimited" - distinct
    /// from a large but bounded <see cref="Distance"/>.
    /// </summary>
    Unlimited
}
