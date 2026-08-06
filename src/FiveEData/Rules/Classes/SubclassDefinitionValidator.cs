using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes;

internal static class SubclassDefinitionValidator
{
    public static IReadOnlyList<string> Validate(SubclassDefinition subclass)
    {
        ArgumentNullException.ThrowIfNull(subclass);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(subclass.Id.Value))
        {
            errors.Add("Subclass ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subclass.Name))
        {
            errors.Add("Subclass name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(subclass.ClassId.Value))
        {
            errors.Add("Subclass class ID must not be empty.");
        }

        if (subclass.ChosenAtLevel is < 1 or > 20)
        {
            errors.Add(
                "Subclass chosen-at level must be between 1 and 20.");
        }

        if (subclass.Sources.Count == 0)
        {
            errors.Add("Subclass must have at least one source reference.");
        }

        var seenLevelFeatures = new HashSet<(int Level, RuleId FeatureRuleId)>();

        foreach (ClassLevelFeature feature in subclass.LevelFeatures)
        {
            if (!seenLevelFeatures.Add((feature.Level, feature.FeatureRuleId)))
            {
                errors.Add(
                    $"Subclass level feature '{feature.FeatureRuleId}' is " +
                    $"duplicated at level {feature.Level}.");
            }
        }

        if (subclass.DivineStrikeProgression is { } divineStrikeProgression)
        {
            ValidateDivineStrikeProgression(divineStrikeProgression, errors);
        }

        if (subclass.CircleFormsProgression is { } circleFormsProgression)
        {
            ValidateCircleFormsProgression(circleFormsProgression, errors);
        }

        if (subclass.CombatSuperiorityProgression is
            { } combatSuperiorityProgression)
        {
            ValidateCombatSuperiorityProgression(
                combatSuperiorityProgression,
                errors);
        }

        return errors;
    }

    private static void ValidateCombatSuperiorityProgression(
        CombatSuperiorityProgressionDetail combatSuperiorityProgression,
        ICollection<string> errors)
    {
        if (combatSuperiorityProgression.ManeuversKnownByLevel.Count == 0)
        {
            errors.Add(
                "Combat Superiority progression must grant at least one " +
                "maneuvers known increase.");
        }

        var seenManeuversKnownLevels = new HashSet<int>();
        int? previousManeuversKnown = null;

        foreach (
            CombatSuperiorityManeuversKnownGrant grant
            in combatSuperiorityProgression.ManeuversKnownByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenManeuversKnownLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Combat Superiority maneuvers known character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousManeuversKnown is { } previous &&
                grant.ManeuversKnown <= previous)
            {
                errors.Add(
                    "Combat Superiority maneuvers known at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousManeuversKnown = grant.ManeuversKnown;
        }

        if (combatSuperiorityProgression.DiceCountByLevel.Count == 0)
        {
            errors.Add(
                "Combat Superiority progression must grant at least one " +
                "superiority dice count increase.");
        }

        var seenDiceCountLevels = new HashSet<int>();
        int? previousDiceCount = null;

        foreach (
            CombatSuperiorityDiceCountGrant grant
            in combatSuperiorityProgression.DiceCountByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenDiceCountLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Combat Superiority dice count character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousDiceCount is { } previous &&
                grant.DiceCount <= previous)
            {
                errors.Add(
                    "Combat Superiority dice count at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousDiceCount = grant.DiceCount;
        }

        if (combatSuperiorityProgression.DieSizeByLevel.Count == 0)
        {
            errors.Add(
                "Combat Superiority progression must grant at least one " +
                "superiority die size increase.");
        }

        var seenDieSizeLevels = new HashSet<int>();
        var seenDieCounts = new HashSet<int>();
        int? previousSides = null;

        foreach (
            CombatSuperiorityDieSizeGrant grant
            in combatSuperiorityProgression.DieSizeByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            seenDieCounts.Add(grant.Die.Count);

            if (!seenDieSizeLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Combat Superiority die size character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousSides is { } previous && grant.Die.Sides <= previous)
            {
                errors.Add(
                    "Combat Superiority die size at level " +
                    $"{grant.CharacterLevel} must use a larger die than " +
                    "the value at the previous grant level.");
            }

            previousSides = grant.Die.Sides;
        }

        if (seenDieCounts.Count > 1)
        {
            errors.Add(
                "Combat Superiority progression must grant the same " +
                "number of dice at every grant level.");
        }
    }

    private static void ValidateCircleFormsProgression(
        CircleFormsProgressionDetail circleFormsProgression,
        ICollection<string> errors)
    {
        if (circleFormsProgression.MaxChallengeRatingByLevel.Count == 0)
        {
            errors.Add(
                "Circle Forms progression must grant at least one max " +
                "challenge rating increase.");
        }

        var seenLevels = new HashSet<int>();
        double? previousMaxChallengeRating = null;

        foreach (
            CircleFormsChallengeRatingGrant grant
            in circleFormsProgression.MaxChallengeRatingByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    $"Circle Forms max challenge rating character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousMaxChallengeRating is { } previous &&
                grant.MaxChallengeRating <= previous)
            {
                errors.Add(
                    "Circle Forms max challenge rating at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousMaxChallengeRating = grant.MaxChallengeRating;
        }
    }

    private static void ValidateDivineStrikeProgression(
        DivineStrikeProgressionDetail divineStrikeProgression,
        ICollection<string> errors)
    {
        if (divineStrikeProgression.DamageByLevel.Count == 0)
        {
            errors.Add(
                "Divine Strike progression must grant at least one " +
                "damage increase.");
        }

        var seenSides = new HashSet<int>();
        var seenLevels = new HashSet<int>();
        int? previousCount = null;

        foreach (
            DivineStrikeDamageGrant grant
            in divineStrikeProgression.DamageByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            seenSides.Add(grant.Damage.Sides);

            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    $"Divine Strike damage character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousCount is { } previous && grant.Damage.Count <= previous)
            {
                errors.Add(
                    $"Divine Strike damage at level {grant.CharacterLevel} " +
                    "must be greater than the value at the previous grant " +
                    "level.");
            }

            previousCount = grant.Damage.Count;
        }

        if (seenSides.Count > 1)
        {
            errors.Add(
                "Divine Strike progression must use the same damage die " +
                "size at every grant level.");
        }

        int mechanismCount =
            (divineStrikeProgression.FixedDamageTypeId is not null ? 1 : 0) +
            (divineStrikeProgression.ChoosableDamageTypeIds is not null ? 1 : 0) +
            (divineStrikeProgression.MatchesWeaponDamageType ? 1 : 0);

        if (mechanismCount != 1)
        {
            errors.Add(
                "Divine Strike progression must define exactly one of a " +
                "fixed damage type, a set of choosable damage types, or " +
                "matching the weapon's damage type.");
        }

        if (divineStrikeProgression.ChoosableDamageTypeIds is
            { Count: < 2 })
        {
            errors.Add(
                "Divine Strike progression's choosable damage types must " +
                "contain at least two options.");
        }
    }

    public static void EnsureValid(SubclassDefinition subclass)
    {
        IReadOnlyList<string> errors = Validate(subclass);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Subclass definition '{subclass.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
