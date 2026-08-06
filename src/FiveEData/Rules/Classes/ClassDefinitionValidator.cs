using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes;

internal static class ClassDefinitionValidator
{
    public static IReadOnlyList<string> Validate(ClassDefinition @class)
    {
        ArgumentNullException.ThrowIfNull(@class);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(@class.Id.Value))
        {
            errors.Add("Class ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(@class.Name))
        {
            errors.Add("Class name must not be empty.");
        }

        if (@class.Sources.Count == 0)
        {
            errors.Add("Class must have at least one source reference.");
        }

        if (@class.PrimaryAbilityIds.Count == 0)
        {
            errors.Add("Class must declare at least one primary ability.");
        }

        ValidateDistinct(
            @class.PrimaryAbilityIds,
            "primary ability",
            errors);

        if (@class.SavingThrowProficiencyIds.Count != 2)
        {
            errors.Add(
                "Class must declare exactly two saving throw proficiencies.");
        }

        ValidateDistinct(
            @class.SavingThrowProficiencyIds,
            "saving throw proficiency",
            errors);

        ValidateDistinctDefinedEnum(
            @class.ArmorProficiencyCategories,
            "armor proficiency category",
            errors);

        ValidateDistinctDefinedEnum(
            @class.WeaponProficiencyCategories,
            "weapon proficiency category",
            errors);

        ValidateDistinct(
            @class.WeaponProficiencyIds,
            "weapon proficiency",
            errors);

        if (@class.SkillChoiceCount < 0)
        {
            errors.Add("Class skill choice count cannot be negative.");
        }

        if (@class.SkillChoiceCount > @class.SkillChoiceOptionIds.Count)
        {
            errors.Add(
                "Class skill choice count cannot exceed the number of " +
                "skill choice options.");
        }

        ValidateDistinct(
            @class.SkillChoiceOptionIds,
            "skill choice option",
            errors);

        var seenLevelFeatures = new HashSet<(int Level, RuleId FeatureRuleId)>();

        foreach (ClassLevelFeature feature in @class.LevelFeatures)
        {
            if (!seenLevelFeatures.Add((feature.Level, feature.FeatureRuleId)))
            {
                errors.Add(
                    $"Class level feature '{feature.FeatureRuleId}' is " +
                    $"duplicated at level {feature.Level}.");
            }
        }

        if (@class.RageProgression is { } rageProgression)
        {
            ValidateRageProgression(rageProgression, errors);
        }

        if (@class.SneakAttackProgression is { } sneakAttackProgression)
        {
            ValidateSneakAttackProgression(
                sneakAttackProgression,
                errors);
        }

        if (@class.KiProgression is { } kiProgression)
        {
            ValidatePointsProgression(
                kiProgression.PointsByLevel
                    .Select(grant => (grant.CharacterLevel, grant.Points)),
                "Ki points",
                errors);
        }

        if (@class.SorceryPointsProgression is { } sorceryPointsProgression)
        {
            ValidatePointsProgression(
                sorceryPointsProgression.PointsByLevel
                    .Select(grant => (grant.CharacterLevel, grant.Points)),
                "Sorcery points",
                errors);
        }

        if (@class.WildShapeProgression is { } wildShapeProgression)
        {
            ValidateWildShapeProgression(wildShapeProgression, errors);
        }

        if (@class.BardicInspirationProgression is
            { } bardicInspirationProgression)
        {
            ValidateBardicInspirationProgression(
                bardicInspirationProgression,
                errors);
        }

        if (@class.ChannelDivinityProgression is
            { } channelDivinityProgression)
        {
            ValidatePointsProgression(
                channelDivinityProgression.UsesByLevel
                    .Select(
                        grant => (grant.CharacterLevel, grant.UsesPerRest)),
                "Channel Divinity uses",
                errors);
        }

        if (@class.MysticArcanumProgression is { } mysticArcanumProgression)
        {
            ValidatePointsProgression(
                mysticArcanumProgression.ArcanumByLevel
                    .Select(
                        grant => (grant.CharacterLevel, grant.SpellLevel)),
                "Mystic Arcanum spell level",
                errors);
        }

        if (@class.FontOfMagicConversion is { } fontOfMagicConversion)
        {
            ValidateFontOfMagicConversion(fontOfMagicConversion, errors);
        }

        if (@class.SongOfRestProgression is { } songOfRestProgression)
        {
            ValidateSongOfRestProgression(songOfRestProgression, errors);
        }

        if (@class.EldritchInvocationsKnownProgression is
            { } eldritchInvocationsKnownProgression)
        {
            ValidatePointsProgression(
                eldritchInvocationsKnownProgression.InvocationsKnownByLevel
                    .Select(
                        grant =>
                            (grant.CharacterLevel, grant.InvocationsKnown)),
                "Eldritch Invocations known",
                errors);
        }

        return errors;
    }

    private static void ValidateSongOfRestProgression(
        SongOfRestProgressionDetail songOfRestProgression,
        ICollection<string> errors)
    {
        if (songOfRestProgression.DieByLevel.Count == 0)
        {
            errors.Add(
                "Song of Rest progression must grant at least one die " +
                "increase.");
        }

        var seenCounts = new HashSet<int>();
        var seenLevels = new HashSet<int>();
        int? previousSides = null;

        foreach (
            SongOfRestDieGrant grant
            in songOfRestProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            seenCounts.Add(grant.Die.Count);

            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Song of Rest die character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousSides is { } previous && grant.Die.Sides <= previous)
            {
                errors.Add(
                    "Song of Rest die at level " +
                    $"{grant.CharacterLevel} must use a larger die than " +
                    "the value at the previous grant level.");
            }

            previousSides = grant.Die.Sides;
        }

        if (seenCounts.Count > 1)
        {
            errors.Add(
                "Song of Rest progression must grant the same number of " +
                "dice at every grant level.");
        }
    }

    private static void ValidateFontOfMagicConversion(
        FontOfMagicConversionDetail fontOfMagicConversion,
        ICollection<string> errors)
    {
        if (fontOfMagicConversion.SlotCostByLevel.Count == 0)
        {
            errors.Add(
                "Font of Magic conversion must grant at least one spell " +
                "slot cost.");
        }

        var seenSlotLevels = new HashSet<int>();
        int? previousCost = null;

        foreach (
            FontOfMagicSlotCostGrant grant
            in fontOfMagicConversion.SlotCostByLevel
                .OrderBy(grant => grant.SpellSlotLevel))
        {
            if (!seenSlotLevels.Add(grant.SpellSlotLevel))
            {
                errors.Add(
                    "Font of Magic spell slot level " +
                    $"{grant.SpellSlotLevel} is duplicated.");
                continue;
            }

            if (previousCost is { } previous &&
                grant.SorceryPointCost <= previous)
            {
                errors.Add(
                    "Font of Magic sorcery point cost at spell slot " +
                    $"level {grant.SpellSlotLevel} must be greater than " +
                    "the cost at the previous spell slot level.");
            }

            previousCost = grant.SorceryPointCost;
        }
    }

    private static void ValidateBardicInspirationProgression(
        BardicInspirationProgressionDetail bardicInspirationProgression,
        ICollection<string> errors)
    {
        if (bardicInspirationProgression.DieByLevel.Count == 0)
        {
            errors.Add(
                "Bardic Inspiration progression must grant at least one " +
                "die increase.");
        }

        var seenCounts = new HashSet<int>();
        var seenLevels = new HashSet<int>();
        int? previousSides = null;

        foreach (
            BardicInspirationDieGrant grant
            in bardicInspirationProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel))
        {
            seenCounts.Add(grant.Die.Count);

            if (!seenLevels.Add(grant.CharacterLevel))
            {
                errors.Add(
                    "Bardic Inspiration die character level " +
                    $"{grant.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousSides is { } previous && grant.Die.Sides <= previous)
            {
                errors.Add(
                    "Bardic Inspiration die at level " +
                    $"{grant.CharacterLevel} must use a larger die than " +
                    "the value at the previous grant level.");
            }

            previousSides = grant.Die.Sides;
        }

        if (seenCounts.Count > 1)
        {
            errors.Add(
                "Bardic Inspiration progression must grant the same " +
                "number of dice at every grant level.");
        }
    }

    private static void ValidateWildShapeProgression(
        WildShapeProgressionDetail wildShapeProgression,
        ICollection<string> errors)
    {
        if (wildShapeProgression.FormLimitsByLevel.Count == 0)
        {
            errors.Add(
                "Wild Shape progression must grant at least one form " +
                "limit.");
        }

        var seenLevels = new HashSet<int>();
        double? previousMaxChallengeRating = null;

        foreach (
            WildShapeFormLimit limit
            in wildShapeProgression.FormLimitsByLevel
                .OrderBy(limit => limit.CharacterLevel))
        {
            if (!seenLevels.Add(limit.CharacterLevel))
            {
                errors.Add(
                    "Wild Shape form limit character level " +
                    $"{limit.CharacterLevel} is duplicated.");
                continue;
            }

            if (previousMaxChallengeRating is { } previous &&
                limit.MaxChallengeRating <= previous)
            {
                errors.Add(
                    "Wild Shape max challenge rating at level " +
                    $"{limit.CharacterLevel} must be greater than the " +
                    "value at the previous grant level.");
            }

            previousMaxChallengeRating = limit.MaxChallengeRating;
        }
    }

    private static void ValidatePointsProgression(
        IEnumerable<(int CharacterLevel, int Points)> pointsByLevel,
        string label,
        ICollection<string> errors)
    {
        (int CharacterLevel, int Points)[] grants = pointsByLevel.ToArray();

        if (grants.Length == 0)
        {
            errors.Add($"{label} progression must grant at least one level.");
        }

        ValidateAscending(
            grants
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, (int?)grant.Points)),
            label,
            errors);
    }

    private static void ValidateSneakAttackProgression(
        SneakAttackProgressionDetail sneakAttackProgression,
        ICollection<string> errors)
    {
        if (sneakAttackProgression.DiceByLevel.Count == 0)
        {
            errors.Add(
                "Sneak Attack progression must grant at least one dice " +
                "increase.");
        }

        var seenSides = new HashSet<int>();

        foreach (
            SneakAttackDiceGrant grant
            in sneakAttackProgression.DiceByLevel)
        {
            seenSides.Add(grant.Damage.Sides);
        }

        if (seenSides.Count > 1)
        {
            errors.Add(
                "Sneak Attack progression must use the same damage die " +
                "size at every grant level.");
        }

        ValidateAscending(
            sneakAttackProgression.DiceByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, (int?)grant.Damage.Count)),
            "Sneak Attack dice",
            errors);
    }

    private static void ValidateRageProgression(
        RageProgressionDetail rageProgression,
        ICollection<string> errors)
    {
        if (rageProgression.UsesByLevel.Count == 0)
        {
            errors.Add(
                "Rage progression must grant at least one uses-per-long" +
                "-rest increase.");
        }

        ValidateAscending(
            rageProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.UsesPerLongRest)),
            "Rage uses",
            errors);

        if (rageProgression.DamageBonusByLevel.Count == 0)
        {
            errors.Add(
                "Rage progression must grant at least one damage bonus " +
                "increase.");
        }

        ValidateAscending(
            rageProgression.DamageBonusByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, (int?)grant.Bonus)),
            "Rage damage bonus",
            errors);

        ValidateDistinct(
            rageProgression.ResistedDamageTypeIds,
            "Rage resisted damage type",
            errors);
    }

    private static void ValidateAscending(
        IEnumerable<(int CharacterLevel, int? Value)> grants,
        string label,
        ICollection<string> errors)
    {
        var seenLevels = new HashSet<int>();
        int? previousValue = null;

        foreach ((int characterLevel, int? value) in grants)
        {
            if (!seenLevels.Add(characterLevel))
            {
                errors.Add(
                    $"{label} character level {characterLevel} is " +
                    "duplicated.");
                continue;
            }

            bool isIncrease =
                previousValue is null ||
                value is null ||
                value > previousValue;

            if (!isIncrease)
            {
                errors.Add(
                    $"{label} at level {characterLevel} must be greater " +
                    "than the value at the previous grant level, or " +
                    "unlimited.");
            }

            previousValue = value;
        }
    }

    private static void ValidateDistinct(
        IReadOnlyList<DamageTypeId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<DamageTypeId>();

        foreach (DamageTypeId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"{label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"{label} '{id}' is duplicated.");
            }
        }
    }

    public static void EnsureValid(ClassDefinition @class)
    {
        IReadOnlyList<string> errors = Validate(@class);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Class definition '{@class.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateDistinct(
        IReadOnlyList<AbilityId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<AbilityId>();

        foreach (AbilityId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinct(
        IReadOnlyList<WeaponId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<WeaponId>();

        foreach (WeaponId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinct(
        IReadOnlyList<SkillId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<SkillId>();

        foreach (SkillId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinctDefinedEnum(
        IReadOnlyList<ArmorCategory> values,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<ArmorCategory>();

        foreach (ArmorCategory value in values)
        {
            if (!Enum.IsDefined(value))
            {
                errors.Add($"Class {label} value '{value}' must be defined.");
                continue;
            }

            if (!seen.Add(value))
            {
                errors.Add($"Class {label} '{value}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinctDefinedEnum(
        IReadOnlyList<WeaponProficiencyCategory> values,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<WeaponProficiencyCategory>();

        foreach (WeaponProficiencyCategory value in values)
        {
            if (!Enum.IsDefined(value))
            {
                errors.Add($"Class {label} value '{value}' must be defined.");
                continue;
            }

            if (!seen.Add(value))
            {
                errors.Add($"Class {label} '{value}' is duplicated.");
            }
        }
    }
}
