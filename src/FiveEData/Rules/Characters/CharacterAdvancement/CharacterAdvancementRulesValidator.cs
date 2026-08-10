namespace FiveEData.Rules.Characters.CharacterAdvancement;

internal static class CharacterAdvancementRulesValidator
{
    public static IReadOnlyList<string> Validate(
        CharacterAdvancementRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        var errors = new List<string>();

        if (rules.Levels.Count != CharacterAdvancementRules.MaximumLevel)
        {
            errors.Add(
                $"Character advancement must have exactly " +
                $"{CharacterAdvancementRules.MaximumLevel} levels.");
            return errors;
        }

        for (int index = 0; index < rules.Levels.Count; index++)
        {
            CharacterAdvancementLevel entry = rules.Levels[index];

            if (entry.Level != index + 1)
            {
                errors.Add(
                    $"Character advancement levels must run 1 through " +
                    $"{CharacterAdvancementRules.MaximumLevel} without " +
                    $"gaps; found level {entry.Level} at position " +
                    $"{index + 1}.");
                continue;
            }

            if (index == 0)
            {
                if (entry.ExperiencePointThreshold != 0)
                {
                    errors.Add(
                        "Character advancement must start at 0 experience " +
                        "points.");
                }

                continue;
            }

            CharacterAdvancementLevel previous = rules.Levels[index - 1];

            if (entry.ExperiencePointThreshold <=
                previous.ExperiencePointThreshold)
            {
                errors.Add(
                    $"Character advancement experience point thresholds " +
                    $"must strictly ascend; level {entry.Level} does not " +
                    $"exceed level {previous.Level}.");
            }

            // The proficiency bonus plateaus for four levels at a time, so
            // it is checked as non-decreasing rather than ascending.
            if (entry.ProficiencyBonus < previous.ProficiencyBonus)
            {
                errors.Add(
                    $"Character advancement proficiency bonus must never " +
                    $"decrease; level {entry.Level} is lower than level " +
                    $"{previous.Level}.");
            }
        }

        if (rules.Sources.Count == 0)
        {
            errors.Add(
                "Character advancement must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(CharacterAdvancementRules rules)
    {
        IReadOnlyList<string> errors = Validate(rules);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Character advancement rules are invalid:" +
            $"{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
