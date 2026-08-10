using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Common;

public readonly record struct SpellGrant
{
    public SpellGrant(
        SpellId grantedSpellId,
        int minimumCharacterLevel,
        SpellGrantFrequency frequency,
        int? castAtSpellLevel)
    {
        if (string.IsNullOrWhiteSpace(grantedSpellId.Value))
        {
            throw new ArgumentException(
                "Spell grant granted spell ID is required.",
                nameof(grantedSpellId));
        }

        if (minimumCharacterLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumCharacterLevel),
                minimumCharacterLevel,
                "Spell grant minimum character level must be greater " +
                "than zero.");
        }

        if (castAtSpellLevel is { } level && level <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(castAtSpellLevel),
                castAtSpellLevel,
                "Spell grant cast-at spell level must be greater than " +
                "zero when specified.");
        }

        GrantedSpellId = grantedSpellId;
        MinimumCharacterLevel = minimumCharacterLevel;
        Frequency = frequency;
        CastAtSpellLevel = castAtSpellLevel;
    }

    public SpellId GrantedSpellId { get; }
    public int MinimumCharacterLevel { get; }
    public SpellGrantFrequency Frequency { get; }
    public int? CastAtSpellLevel { get; }
}
