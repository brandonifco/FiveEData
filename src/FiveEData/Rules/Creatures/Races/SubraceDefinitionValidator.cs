using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Creatures.Races;

internal static class SubraceDefinitionValidator
{
    public static IReadOnlyList<string> Validate(SubraceDefinition subrace)
    {
        ArgumentNullException.ThrowIfNull(subrace);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(subrace.Id.Value))
        {
            errors.Add("Subrace ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subrace.Name))
        {
            errors.Add("Subrace name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subrace.RaceId.Value))
        {
            errors.Add("Subrace race ID must not be empty.");
        }

        if (subrace.Speed is { } speed && speed.Feet <= 0)
        {
            errors.Add(
                "Subrace speed must be greater than zero when specified.");
        }

        if (subrace.Sources.Count == 0)
        {
            errors.Add("Subrace must have at least one source reference.");
        }

        var seenAbilities = new HashSet<AbilityId>();

        foreach (RaceAbilityScoreIncrease increase in subrace.AbilityScoreIncreases)
        {
            if (string.IsNullOrWhiteSpace(increase.AbilityId.Value))
            {
                errors.Add(
                    "Subrace ability score increase ability ID must not be empty.");
                continue;
            }

            if (!seenAbilities.Add(increase.AbilityId))
            {
                errors.Add(
                    $"Subrace ability score increase for ability '{increase.AbilityId}' is duplicated.");
            }
        }

        if (subrace.AdditionalLanguageChoiceCount < 0)
        {
            errors.Add(
                "Subrace additional language choice count cannot be negative.");
        }

        var seenTraitRules = new HashSet<RuleId>();

        foreach (RuleId traitRuleId in subrace.TraitRuleIds)
        {
            if (string.IsNullOrWhiteSpace(traitRuleId.Value))
            {
                errors.Add("Subrace trait rule ID must not be empty.");
                continue;
            }

            if (!seenTraitRules.Add(traitRuleId))
            {
                errors.Add(
                    $"Subrace trait rule '{traitRuleId}' is duplicated.");
            }
        }

        return errors;
    }

    public static void EnsureValid(SubraceDefinition subrace)
    {
        IReadOnlyList<string> errors = Validate(subrace);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Subrace definition '{subrace.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
