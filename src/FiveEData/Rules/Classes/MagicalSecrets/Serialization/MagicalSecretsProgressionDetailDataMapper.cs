namespace FiveEData.Rules.Classes.MagicalSecrets.Serialization;

internal static class MagicalSecretsProgressionDetailDataMapper
{
    public static MagicalSecretsProgressionDetail Map(
        MagicalSecretsProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        MagicalSecretsChoiceGrantData[] grantData =
            data.SpellsKnownByLevel
            ?? throw new ArgumentException(
                "Magical Secrets progression spells known by level is " +
                "required.",
                nameof(data));

        MagicalSecretsChoiceGrant[] spellsKnownByLevel = grantData
            .Select(MapGrant)
            .ToArray();

        return new MagicalSecretsProgressionDetail(
            spellsKnownByLevel,
            data.CountsAgainstSpellsKnown);
    }

    private static MagicalSecretsChoiceGrant MapGrant(
        MagicalSecretsChoiceGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new MagicalSecretsChoiceGrant(
            data.CharacterLevel,
            data.SpellsKnown);
    }
}
