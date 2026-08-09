using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Adventuring.DowntimeActivities.Serialization;

internal static class DowntimeActivityDefinitionLoader
{
    public static IReadOnlyList<DowntimeActivityDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<DowntimeActivityDefinition> LoadFromJson(
        string json)
    {
        DowntimeActivityDefinitionData[] data =
            StrictJson.DeserializeArray<DowntimeActivityDefinitionData>(
                json,
                "DowntimeActivity");

        var definitions = new List<DowntimeActivityDefinition>(data.Length);
        var ids = new HashSet<DowntimeActivityId>();

        for (int index = 0; index < data.Length; index++)
        {
            DowntimeActivityDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid downtime activity definition at index " +
                    $"{index}.");
            }

            DowntimeActivityDefinition definition;

            try
            {
                definition = Map(itemData);
                DowntimeActivityDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is
                    ArgumentException or
                    InvalidOperationException)
            {
                string identity =
                    string.IsNullOrWhiteSpace(itemData.Id)
                        ? $"index {index}"
                        : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid downtime activity definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate downtime activity ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static DowntimeActivityDefinition Map(
        DowntimeActivityDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new DowntimeActivityId(
            data.Id
            ?? throw new ArgumentException(
                "Downtime activity ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Downtime activity name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Downtime activity sources are required.",
                nameof(data));

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new DowntimeActivityDefinition(
            id,
            name,
            data.RequiredDays,
            data.CostPerDayGoldPieces,
            savingThrowAbilityId,
            data.SavingThrowDC,
            data.MarketValueProgressPerDayGoldPieces,
            sources);
    }
}
