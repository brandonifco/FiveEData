using FiveEData.Rules.Classes;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Spells.MagicSchools;

namespace FiveEData.Rules.Spells.Serialization;

internal static class SpellDefinitionLoader
{
    public static IReadOnlyList<SpellDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SpellDefinition> LoadFromJson(string json)
    {
        SpellDefinitionData[] data =
            StrictJson.DeserializeArray<SpellDefinitionData>(json, "Spell");

        var definitions = new List<SpellDefinition>(data.Length);
        var ids = new HashSet<SpellId>();

        for (int index = 0; index < data.Length; index++)
        {
            SpellDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid spell definition at index {index}.");
            }

            SpellDefinition definition;

            try
            {
                definition = Map(itemData);
                SpellDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is
                    ArgumentException or
                    ArgumentOutOfRangeException or
                    InvalidOperationException)
            {
                string identity =
                    string.IsNullOrWhiteSpace(itemData.Id)
                        ? $"index {index}"
                        : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid spell definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate spell ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static SpellDefinition Map(SpellDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SpellId(
            data.Id
            ?? throw Missing("ID"));

        string name = data.Name ?? throw Missing("name");
        int level = data.Level ?? throw Missing("level");
        var schoolId = new MagicSchoolId(
            data.SchoolId ?? throw Missing("school"));

        SpellCastingTimeData castingTimeData =
            data.CastingTime ?? throw Missing("casting time");
        SpellRangeData rangeData = data.Range ?? throw Missing("range");
        SpellComponentsData componentsData =
            data.Components ?? throw Missing("components");
        SpellDurationData durationData =
            data.Duration ?? throw Missing("duration");

        string[] classIdValues =
            data.AvailableToClassIds ?? throw Missing("class list");
        SourceReferenceData[] sourceData =
            data.Sources ?? throw Missing("sources");

        return new SpellDefinition(
            id,
            name,
            level,
            schoolId,
            MapCastingTime(castingTimeData),
            MapRange(rangeData),
            MapComponents(componentsData),
            MapDuration(durationData),
            data.IsRitual ?? throw Missing("ritual flag"),
            data.DamageEffect is { } damageEffectData
                ? MapDamageEffect(damageEffectData)
                : null,
            data.ConditionEffect is { } conditionEffectData
                ? MapConditionEffect(conditionEffectData)
                : null,
            classIdValues.Select(value => new ClassId(value)).ToArray(),
            sourceData.Select(SourceReferenceDataMapper.Map).ToArray());
    }

    private static SpellDamageEffect MapDamageEffect(
        SpellDamageEffectData data)
    {
        DamageTypeId? damageTypeId = data.DamageTypeId is { } damageTypeIdValue
            ? new DamageTypeId(damageTypeIdValue)
            : null;

        IEnumerable<DamageTypeId>? choosableDamageTypeIds =
            data.ChoosableDamageTypeIds?
                .Select(value => new DamageTypeId(value))
                .ToArray();

        SpellAttackRollType? attackRollType =
            data.AttackRollType is { } attackRollTypeValue
                ? ParseEnum<SpellAttackRollType>(
                    attackRollTypeValue,
                    "damage effect attack roll type")
                : null;

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        bool halfDamageOnSuccessfulSave =
            data.HalfDamageOnSuccessfulSave
            ?? throw Missing("damage effect half-damage-on-save flag");

        SpellDamageTierGrantData[]? tierData = data.DamageByCharacterLevel;

        IEnumerable<SpellDamageTierGrant>? damageByCharacterLevel =
            tierData?.Select(MapDamageTier).ToArray();

        DiceExpression? baseDamage = data.BaseDamage is { } baseDamageData
            ? new DiceExpression(baseDamageData.Count, baseDamageData.Sides)
            : null;

        return new SpellDamageEffect(
            damageTypeId,
            choosableDamageTypeIds,
            attackRollType,
            savingThrowAbilityId,
            halfDamageOnSuccessfulSave,
            damageByCharacterLevel,
            baseDamage,
            data.FlatDamageBonus);
    }

    private static SpellConditionEffect MapConditionEffect(
        SpellConditionEffectData data)
    {
        string[] conditionIdValues =
            data.ConditionIds ?? throw Missing("condition effect conditions");

        var savingThrowAbilityId = new AbilityId(
            data.SavingThrowAbilityId
            ?? throw Missing("condition effect saving throw ability"));

        return new SpellConditionEffect(
            conditionIdValues.Select(value => new ConditionId(value)).ToArray(),
            savingThrowAbilityId);
    }

    private static SpellDamageTierGrant MapDamageTier(
        SpellDamageTierGrantData data)
    {
        int characterLevel =
            data.CharacterLevel ?? throw Missing("damage tier character level");
        DiceExpressionData damageData =
            data.Damage ?? throw Missing("damage tier damage");

        return new SpellDamageTierGrant(
            characterLevel,
            new DiceExpression(damageData.Count, damageData.Sides));
    }

    private static SpellCastingTime MapCastingTime(
        SpellCastingTimeData data)
    {
        return new SpellCastingTime(
            data.Amount ?? throw Missing("casting time amount"),
            ParseEnum<SpellCastingTimeUnit>(
                data.Unit ?? throw Missing("casting time unit"),
                "casting time unit"));
    }

    private static SpellRange MapRange(SpellRangeData data)
    {
        var kind = ParseEnum<SpellRangeKind>(
            data.Kind ?? throw Missing("range kind"),
            "range kind");

        if (kind == SpellRangeKind.Self && data.AreaShape is not null)
        {
            return SpellRange.SelfWithArea(
                ParseEnum<SpellAreaShape>(data.AreaShape, "area shape"),
                data.AreaSizeFeet ?? throw Missing("area size"));
        }

        return kind switch
        {
            SpellRangeKind.Self => SpellRange.Self(),
            SpellRangeKind.Touch => SpellRange.Touch(),
            SpellRangeKind.Unlimited => SpellRange.Unlimited(),
            SpellRangeKind.Special => SpellRange.Special(),
            SpellRangeKind.Sight => SpellRange.Sight(),
            _ => SpellRange.Distance(
                data.DistanceFeet
                ?? throw Missing("range distance")),
        };
    }

    private static SpellComponents MapComponents(SpellComponentsData data)
    {
        return new SpellComponents(
            data.Verbal ?? throw Missing("verbal component"),
            data.Somatic ?? throw Missing("somatic component"),
            data.Material ?? throw Missing("material component"),
            data.MaterialDescription,
            data.MaterialCostGoldPieces,
            data.MaterialIsConsumed
            ?? throw Missing("material consumed flag"));
    }

    private static SpellDuration MapDuration(SpellDurationData data)
    {
        bool isInstantaneous =
            data.IsInstantaneous ?? throw Missing("duration kind");
        bool isUntilDispelled =
            data.IsUntilDispelled
            ?? throw Missing("duration until-dispelled flag");
        bool isSpecial =
            data.IsSpecial ?? throw Missing("duration special flag");
        bool requiresConcentration =
            data.RequiresConcentration
            ?? throw Missing("duration concentration flag");
        bool isUpTo = data.IsUpTo ?? throw Missing("duration up-to flag");

        if (new[] { isInstantaneous, isUntilDispelled, isSpecial }
            .Count(isKind => isKind) > 1)
        {
            throw new ArgumentException(
                "Spell duration must be at most one of instantaneous, " +
                "until dispelled, or special.",
                "data");
        }

        if (isInstantaneous)
        {
            return SpellDuration.Instantaneous();
        }

        if (isUntilDispelled)
        {
            return SpellDuration.UntilDispelled();
        }

        if (isSpecial)
        {
            return SpellDuration.Special();
        }

        int amount = data.Amount ?? throw Missing("duration amount");
        var unit = ParseEnum<SpellDurationUnit>(
            data.Unit ?? throw Missing("duration unit"),
            "duration unit");

        if (requiresConcentration)
        {
            return SpellDuration.Concentration(amount, unit);
        }

        return isUpTo
            ? SpellDuration.UpTo(amount, unit)
            : SpellDuration.Fixed(amount, unit);
    }

    private static TEnum ParseEnum<TEnum>(string value, string label)
        where TEnum : struct, Enum
    {
        if (!Enum.TryParse(value, ignoreCase: false, out TEnum parsed)
            || !Enum.IsDefined(parsed))
        {
            throw new ArgumentException(
                $"Spell {label} '{value}' is not a defined value.",
                nameof(value));
        }

        return parsed;
    }

    private static ArgumentException Missing(string label)
    {
        return new ArgumentException($"Spell {label} is required.", "data");
    }
}
