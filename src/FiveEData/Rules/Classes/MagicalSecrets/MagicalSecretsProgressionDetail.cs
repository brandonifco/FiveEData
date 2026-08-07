namespace FiveEData.Rules.Classes.MagicalSecrets;

public sealed record MagicalSecretsProgressionDetail
{
    public MagicalSecretsProgressionDetail(
        IEnumerable<MagicalSecretsChoiceGrant> spellsKnownByLevel,
        bool countsAgainstSpellsKnown)
    {
        ArgumentNullException.ThrowIfNull(spellsKnownByLevel);

        SpellsKnownByLevel = Array.AsReadOnly(spellsKnownByLevel.ToArray());
        CountsAgainstSpellsKnown = countsAgainstSpellsKnown;
    }

    public IReadOnlyList<MagicalSecretsChoiceGrant> SpellsKnownByLevel { get; }

    public bool CountsAgainstSpellsKnown { get; }
}
