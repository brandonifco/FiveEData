using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DiscipleOfTheElements;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.ImprovedCritical;
using FiveEData.Rules.Classes.Portent;
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

        if (subclass.DiscipleOfTheElementsProgression is
            { } discipleOfTheElementsProgression)
        {
            ValidateDiscipleOfTheElementsProgression(
                discipleOfTheElementsProgression,
                errors);
        }

        if (subclass.MagicalSecretsProgression is
            { } magicalSecretsProgression)
        {
            ValidateMagicalSecretsProgression(
                magicalSecretsProgression,
                errors);
        }

        if (subclass.PortentProgression is { } portentProgression)
        {
            ValidatePortentProgression(portentProgression, errors);
        }

        if (subclass.ImprovedCriticalProgression is
            { } improvedCriticalProgression)
        {
            ValidateImprovedCriticalProgression(
                improvedCriticalProgression,
                errors);
        }

        return errors;
    }

    // The only progression in the codebase whose value falls as level
    // rises: a lower critical-hit threshold is the improvement, so 19 at 3rd
    // becomes 18 at 15th. Do not "fix" this to the ascending check every
    // other progression uses.
    private static void ValidateImprovedCriticalProgression(
        ImprovedCriticalProgressionDetail improvedCriticalProgression,
        ICollection<string> errors)
    {
        if (improvedCriticalProgression.MinimumRollByLevel.Count == 0)
        {
            errors.Add(
                "Improved Critical progression must grant at least one " +
                "critical hit threshold.");
        }

        var seenLevels = new HashSet<int>();
        int? previousMinimumRoll = null;

        foreach (
            CriticalHitThresholdGrant grant
            in improvedCriticalProgression.MinimumRollByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Improved Critical threshold character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousMinimumRoll is { } previous &&
                grant.MinimumRoll >= previous)
            {
                errors.Add(
                    "Improved Critical minimum roll at level " +
                    $"{grant.CharacterLevel} must be lower than the value " +
                    "at the previous grant level.");
            }

            previousMinimumRoll = grant.MinimumRoll;
        }
    }

    private static void ValidateMagicalSecretsProgression(
        MagicalSecretsProgressionDetail magicalSecretsProgression,
        ICollection<string> errors)
    {
        if (magicalSecretsProgression.SpellsKnownByLevel.Count == 0)
        {
            errors.Add(
                "Magical Secrets progression must grant at least one " +
                "spells known increase.");
        }

        var seenLevels = new HashSet<int>();
        int? previousSpellsKnown = null;

        foreach (
            MagicalSecretsChoiceGrant grant
            in magicalSecretsProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Magical Secrets spells known character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousSpellsKnown is { } previous &&
                grant.SpellsKnown <= previous)
            {
                errors.Add(
                    "Magical Secrets spells known at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousSpellsKnown = grant.SpellsKnown;
        }
    }

    private static void ValidatePortentProgression(
        PortentProgressionDetail portentProgression,
        ICollection<string> errors)
    {
        if (portentProgression.ForetellingRollsByLevel.Count == 0)
        {
            errors.Add(
                "Portent progression must grant at least one foretelling " +
                "rolls increase.");
        }

        var seenLevels = new HashSet<int>();
        int? previousForetellingRolls = null;

        foreach (
            PortentRollGrant grant
            in portentProgression.ForetellingRollsByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Portent foretelling rolls character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousForetellingRolls is { } previous &&
                grant.ForetellingRolls <= previous)
            {
                errors.Add(
                    "Portent foretelling rolls at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousForetellingRolls = grant.ForetellingRolls;
        }
    }

    private static void ValidateDiscipleOfTheElementsProgression(
        DiscipleOfTheElementsProgressionDetail
            discipleOfTheElementsProgression,
        ICollection<string> errors)
    {
        if (discipleOfTheElementsProgression.DisciplinesKnownByLevel.Count ==
            0)
        {
            errors.Add(
                "Disciple of the Elements progression must grant at " +
                "least one disciplines known increase.");
        }

        var seenDisciplinesKnownLevels = new HashSet<int>();
        int? previousDisciplinesKnown = null;

        foreach (
            DiscipleOfTheElementsDisciplinesKnownGrant grant
            in discipleOfTheElementsProgression.DisciplinesKnownByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenDisciplinesKnownLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Disciple of the Elements disciplines known " +
                    $"character level {grant.CharacterLevel} is " +
                    "duplicated.");
                continue;
            }

            if (previousDisciplinesKnown is { } previous &&
                grant.DisciplinesKnown <= previous)
            {
                errors.Add(
                    "Disciple of the Elements disciplines known at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousDisciplinesKnown = grant.DisciplinesKnown;
        }

        if (discipleOfTheElementsProgression.MaxKiPointsPerSpellByLevel
                .Count == 0)
        {
            errors.Add(
                "Disciple of the Elements progression must grant at " +
                "least one max ki points increase.");
        }

        var seenMaxKiPointsLevels = new HashSet<int>();
        int? previousMaxKiPoints = null;

        foreach (
            DiscipleOfTheElementsMaxKiPointsGrant grant
            in discipleOfTheElementsProgression.MaxKiPointsPerSpellByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            if (!seenMaxKiPointsLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Disciple of the Elements max ki points character " +
                    $"level {grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousMaxKiPoints is { } previous &&
                grant.MaxKiPoints <= previous)
            {
                errors.Add(
                    "Disciple of the Elements max ki points at level " +
                    $"{grant.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousMaxKiPoints = grant.MaxKiPoints;
        }
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
