using FiveEData.Rules.Classes;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
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
            classIdValues.Select(value => new ClassId(value)).ToArray(),
            sourceData.Select(SourceReferenceDataMapper.Map).ToArray());
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
        bool requiresConcentration =
            data.RequiresConcentration
            ?? throw Missing("duration concentration flag");
        bool isUpTo = data.IsUpTo ?? throw Missing("duration up-to flag");

        if (isInstantaneous && isUntilDispelled)
        {
            throw new ArgumentException(
                "Spell duration cannot be both instantaneous and until " +
                "dispelled.",
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
