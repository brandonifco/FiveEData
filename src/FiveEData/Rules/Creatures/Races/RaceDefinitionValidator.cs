using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Languages;

namespace FiveEData.Rules.Creatures.Races;

internal static class RaceDefinitionValidator
{
    public static IReadOnlyList<string> Validate(RaceDefinition race)
    {
        ArgumentNullException.ThrowIfNull(race);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(race.Id.Value))
        {
            errors.Add("Race ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(race.Name))
        {
            errors.Add("Race name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(race.Size.Value))
        {
            errors.Add("Race size ID must not be empty.");
        }

        if (race.Speed.Feet <= 0)
        {
            errors.Add("Race speed must be greater than zero.");
        }

        if (race.Sources.Count == 0)
        {
            errors.Add("Race must have at least one source reference.");
        }

        var seenAbilities = new HashSet<AbilityId>();

        foreach (RaceAbilityScoreIncrease increase in race.AbilityScoreIncreases)
        {
            if (string.IsNullOrWhiteSpace(increase.AbilityId.Value))
            {
                errors.Add(
                    "Race ability score increase ability ID must not be empty.");
                continue;
            }

            if (!seenAbilities.Add(increase.AbilityId))
            {
                errors.Add(
                    $"Race ability score increase for ability '{increase.AbilityId}' is duplicated.");
            }
        }

        if (race.ChoosableAbilityScoreIncreaseCount < 0)
        {
            errors.Add(
                "Race choosable ability score increase count cannot be negative.");
        }

        var seenLanguages = new HashSet<LanguageId>();

        foreach (LanguageId languageId in race.LanguageIds)
        {
            if (string.IsNullOrWhiteSpace(languageId.Value))
            {
                errors.Add("Race language ID must not be empty.");
                continue;
            }

            if (!seenLanguages.Add(languageId))
            {
                errors.Add(
                    $"Race language '{languageId}' is duplicated.");
            }
        }

        if (race.AdditionalLanguageChoiceCount < 0)
        {
            errors.Add(
                "Race additional language choice count cannot be negative.");
        }

        var seenTraitRules = new HashSet<RuleId>();

        foreach (RuleId traitRuleId in race.TraitRuleIds)
        {
            if (string.IsNullOrWhiteSpace(traitRuleId.Value))
            {
                errors.Add("Race trait rule ID must not be empty.");
                continue;
            }

            if (!seenTraitRules.Add(traitRuleId))
            {
                errors.Add(
                    $"Race trait rule '{traitRuleId}' is duplicated.");
            }
        }

        return errors;
    }

    public static void EnsureValid(RaceDefinition race)
    {
        IReadOnlyList<string> errors = Validate(race);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Race definition '{race.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
