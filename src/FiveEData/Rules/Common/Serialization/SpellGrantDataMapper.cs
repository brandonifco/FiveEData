using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Common.Serialization;

internal static class SpellGrantDataMapper
{
    public static SpellGrant Map(SpellGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string grantedSpellIdValue = data.GrantedSpellId
            ?? throw new ArgumentException(
                "Spell grant granted spell ID is required.",
                nameof(data));

        return new SpellGrant(
            new SpellId(grantedSpellIdValue),
            data.MinimumCharacterLevel,
            data.Frequency,
            data.CastAtSpellLevel);
    }
}
