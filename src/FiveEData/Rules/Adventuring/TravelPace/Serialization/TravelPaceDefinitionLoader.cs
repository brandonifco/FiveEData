using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Adventuring.TravelPace.Serialization;

internal static class TravelPaceDefinitionLoader
{
    public static IReadOnlyList<TravelPaceDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<TravelPaceDefinition> LoadFromJson(
        string json)
    {
        TravelPaceDefinitionData[] data =
            StrictJson.DeserializeArray<TravelPaceDefinitionData>(
                json,
                "TravelPace");

        var definitions = new List<TravelPaceDefinition>(data.Length);
        var ids = new HashSet<TravelPaceId>();

        for (int index = 0; index < data.Length; index++)
        {
            TravelPaceDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid travel pace definition at index {index}.");
            }

            TravelPaceDefinition definition;

            try
            {
                definition = Map(itemData);
                TravelPaceDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid travel pace definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate travel pace ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static TravelPaceDefinition Map(TravelPaceDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new TravelPaceId(
            data.Id
            ?? throw new ArgumentException(
                "Travel pace ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Travel pace name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Travel pace sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new TravelPaceDefinition(
            id,
            name,
            data.FeetPerMinute,
            data.MilesPerHour,
            data.MilesPerDay,
            data.PassiveWisdomPerceptionPenalty,
            data.AllowsStealth,
            sources);
    }
}
