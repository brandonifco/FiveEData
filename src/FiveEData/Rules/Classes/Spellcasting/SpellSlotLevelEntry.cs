namespace FiveEData.Rules.Classes.Spellcasting;

public sealed record SpellSlotLevelEntry
{
    public SpellSlotLevelEntry(
        int characterLevel,
        IEnumerable<SpellSlotCount> slots)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        ArgumentNullException.ThrowIfNull(slots);

        CharacterLevel = characterLevel;
        Slots = Array.AsReadOnly(slots.ToArray());
    }

    public int CharacterLevel { get; }

    public IReadOnlyList<SpellSlotCount> Slots { get; }
}
