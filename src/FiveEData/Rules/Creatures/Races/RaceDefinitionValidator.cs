using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Races.BreathWeapon;

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

        if (race.DarkvisionRangeFeet is { } darkvisionRangeFeet &&
            darkvisionRangeFeet <= 0)
        {
            errors.Add(
                "Race darkvision range must be greater than zero feet " +
                "when specified.");
        }

        var seenResistedDamageTypes = new HashSet<DamageTypeId>();

        foreach (DamageTypeId damageTypeId in race.ResistedDamageTypeIds)
        {
            if (string.IsNullOrWhiteSpace(damageTypeId.Value))
            {
                errors.Add(
                    "Race resisted damage type ID must not be empty.");
                continue;
            }

            if (!seenResistedDamageTypes.Add(damageTypeId))
            {
                errors.Add(
                    $"Race resisted damage type '{damageTypeId}' is " +
                    "duplicated.");
            }
        }

        if (race.TranceDurationHours is { } tranceDurationHours &&
            tranceDurationHours <= 0)
        {
            errors.Add(
                "Race trance duration must be greater than zero hours " +
                "when specified.");
        }

        if (race.BreathWeaponProgression is { } breathWeaponProgression)
        {
            ValidateBreathWeaponProgression(
                breathWeaponProgression,
                errors);
        }

        return errors;
    }

    private static void ValidateBreathWeaponProgression(
        BreathWeaponProgressionDetail breathWeaponProgression,
        ICollection<string> errors)
    {
        if (breathWeaponProgression.DamageByLevel.Count == 0)
        {
            errors.Add(
                "Breath weapon progression must grant at least one " +
                "damage increase.");
        }

        var seenSides = new HashSet<int>();
        var seenLevels = new HashSet<int>();
        int? previousCount = null;

        foreach (
            BreathWeaponDamageGrant grant
            in breathWeaponProgression.DamageByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            seenSides.Add(grant.Damage.Sides);

            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Breath weapon damage character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousCount is { } previous &&
                grant.Damage.Count <= previous)
            {
                errors.Add(
                    $"Breath weapon damage at level {grant.CharacterLevel} " +
                    "must be greater than the value at the previous grant " +
                    "level.");
            }

            previousCount = grant.Damage.Count;
        }

        if (seenSides.Count > 1)
        {
            errors.Add(
                "Breath weapon progression must use the same damage die " +
                "size at every grant level.");
        }
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
