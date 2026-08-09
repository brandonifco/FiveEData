using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Adventuring.Resting.Serialization;

internal static class RestTypeDefinitionLoader
{
    public static IReadOnlyList<RestTypeDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<RestTypeDefinition> LoadFromJson(
        string json)
    {
        RestTypeDefinitionData[] data =
            StrictJson.DeserializeArray<RestTypeDefinitionData>(
                json,
                "RestType");

        var definitions = new List<RestTypeDefinition>(data.Length);
        var ids = new HashSet<RestTypeId>();

        for (int index = 0; index < data.Length; index++)
        {
            RestTypeDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid rest type definition at index {index}.");
            }

            RestTypeDefinition definition;

            try
            {
                definition = Map(itemData);
                RestTypeDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid rest type definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate rest type ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static RestTypeDefinition Map(RestTypeDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new RestTypeId(
            data.Id
            ?? throw new ArgumentException(
                "Rest type ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Rest type name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Rest type sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new RestTypeDefinition(
            id,
            name,
            data.MinimumDurationHours,
            data.CooldownHours,
            data.MinimumHitPointsToBenefit,
            sources);
    }
}
