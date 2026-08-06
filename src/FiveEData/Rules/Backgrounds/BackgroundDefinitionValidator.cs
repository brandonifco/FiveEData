using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Backgrounds;

internal static class BackgroundDefinitionValidator
{
    public static IReadOnlyList<string> Validate(BackgroundDefinition background)
    {
        ArgumentNullException.ThrowIfNull(background);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(background.Id.Value))
        {
            errors.Add("Background ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(background.Name))
        {
            errors.Add("Background name must not be empty.");
        }

        if (background.SkillProficiencyIds.Count != 2)
        {
            errors.Add(
                "Background must grant exactly two skill proficiencies.");
        }

        var seenSkills = new HashSet<SkillId>();

        foreach (SkillId skillId in background.SkillProficiencyIds)
        {
            if (string.IsNullOrWhiteSpace(skillId.Value))
            {
                errors.Add("Background skill ID must not be empty.");
                continue;
            }

            if (!seenSkills.Add(skillId))
            {
                errors.Add(
                    $"Background skill '{skillId}' is duplicated.");
            }
        }

        if (background.LanguageChoiceCount < 0)
        {
            errors.Add(
                "Background language choice count cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(background.FeatureRuleId.Value))
        {
            errors.Add("Background feature rule ID must not be empty.");
        }

        if (background.Sources.Count == 0)
        {
            errors.Add("Background must have at least one source reference.");
        }

        if (background.AdditionalPeopleFedPerDay is
                { } additionalPeopleFedPerDay &&
            additionalPeopleFedPerDay <= 0)
        {
            errors.Add(
                "Background additional people fed per day must be " +
                "greater than zero when specified.");
        }

        if (background.GuildDuesGoldPerMonth is { } guildDuesGoldPerMonth &&
            guildDuesGoldPerMonth <= 0)
        {
            errors.Add(
                "Background guild dues must be greater than zero gold " +
                "when specified.");
        }

        if (background.FastTravelSpeedMultiplier is
                { } fastTravelSpeedMultiplier &&
            fastTravelSpeedMultiplier <= 1)
        {
            errors.Add(
                "Background fast travel speed multiplier must be " +
                "greater than one when specified.");
        }

        return errors;
    }

    public static void EnsureValid(BackgroundDefinition background)
    {
        IReadOnlyList<string> errors = Validate(background);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Background definition '{background.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
