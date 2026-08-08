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
    Unlimited,

    /// <summary>
    /// The PHB's "Range: Special" header value, as in Dream, where the
    /// actual reach is a conditional rule (same plane of existence as the
    /// target) that lives in the citation rather than a distance.
    /// </summary>
    Special
}
