namespace FiveEData.Rules.Classes.SpellsKnown;

public sealed record SpellsKnownProgressionDetail
{
    public SpellsKnownProgressionDetail(
        IEnumerable<SpellsKnownGrant> spellsKnownByLevel)
    {
        ArgumentNullException.ThrowIfNull(spellsKnownByLevel);

        SpellsKnownByLevel =
            Array.AsReadOnly(spellsKnownByLevel.ToArray());
    }

    public IReadOnlyList<SpellsKnownGrant> SpellsKnownByLevel { get; }
}
