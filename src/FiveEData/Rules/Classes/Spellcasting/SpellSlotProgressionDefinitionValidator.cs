namespace FiveEData.Rules.Classes.Spellcasting;

internal static class SpellSlotProgressionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        SpellSlotProgressionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Spell slot progression ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Spell slot progression name must not be empty.");
        }

        if (!Enum.IsDefined(definition.RecoveryRest))
        {
            errors.Add(
                "Spell slot progression recovery rest must be a " +
                "defined value.");
        }

        var seenLevels = new HashSet<int>();

        foreach (SpellSlotLevelEntry entry in definition.LevelEntries)
        {
            if (!seenLevels.Add(entry.CharacterLevel))
            {
                errors.Add(
                    "Spell slot progression character level " +
                    $"{entry.CharacterLevel} is duplicated.");
            }

            var seenSpellLevels = new HashSet<int>();

            foreach (SpellSlotCount slot in entry.Slots)
            {
                if (!seenSpellLevels.Add(slot.SpellLevel))
                {
                    errors.Add(
                        $"Spell slot progression character level " +
                        $"{entry.CharacterLevel} duplicates spell " +
                        $"level {slot.SpellLevel}.");
                }
            }
        }

        if (definition.LevelEntries.Count != 20 ||
            !Enumerable.Range(1, 20).All(seenLevels.Contains))
        {
            errors.Add(
                "Spell slot progression must define exactly one " +
                "entry for each character level from 1 to 20.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Spell slot progression must have at least one " +
                "source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        SpellSlotProgressionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Spell slot progression definition " +
            $"'{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
