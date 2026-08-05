using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Alignments.Serialization;

internal static class AlignmentDefinitionLoader
{
    public static IReadOnlyList<AlignmentDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<AlignmentDefinition> LoadFromJson(
        string json)
    {
        AlignmentDefinitionData[] data =
            StrictJson.DeserializeArray<AlignmentDefinitionData>(
                json,
                "Alignment");

        var definitions =
            new List<AlignmentDefinition>(data.Length);
        var ids = new HashSet<AlignmentId>();

        for (int index = 0; index < data.Length; index++)
        {
            AlignmentDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid alignment definition at index {index}.");
            }

            AlignmentDefinition definition;

            try
            {
                definition = Map(itemData);
                AlignmentDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid alignment definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate alignment ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static AlignmentDefinition Map(
        AlignmentDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new AlignmentId(
            data.Id
            ?? throw new ArgumentException(
                "Alignment ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Alignment name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Alignment sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new AlignmentDefinition(
            id,
            name,
            data.Ethic,
            data.Morality,
            sources);
    }
}
