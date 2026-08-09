namespace FiveEData.Rules.Classes.SpellsKnown.Serialization;

internal static class SpellsKnownProgressionDetailDataMapper
{
    public static SpellsKnownProgressionDetail Map(
        SpellsKnownProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        SpellsKnownGrantData[] spellsKnownData =
            data.SpellsKnownByLevel
            ?? throw new ArgumentException(
                "Spells known progression spells known by level is " +
                "required.",
                nameof(data));

        SpellsKnownGrant[] spellsKnownByLevel =
            spellsKnownData
                .Select(
                    grant => new SpellsKnownGrant(
                        grant.CharacterLevel,
                        grant.SpellsKnown))
                .ToArray();

        return new SpellsKnownProgressionDetail(spellsKnownByLevel);
    }
}
