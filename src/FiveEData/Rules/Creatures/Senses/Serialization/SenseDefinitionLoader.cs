using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Senses.Serialization;

internal static class SenseDefinitionLoader
{
    public static IReadOnlyList<SenseDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SenseDefinition> LoadFromJson(
        string json)
    {
        SenseDefinitionData[] data =
            StrictJson.DeserializeArray<SenseDefinitionData>(
                json,
                "Sense");

        var definitions =
            new List<SenseDefinition>(data.Length);
        var ids = new HashSet<SenseId>();

        for (int index = 0; index < data.Length; index++)
        {
            SenseDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid sense definition at index {index}.");
            }

            SenseDefinition definition;

            try
            {
                definition = Map(itemData);
                SenseDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid sense definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate sense ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static SenseDefinition Map(
        SenseDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SenseId(
            data.Id
            ?? throw new ArgumentException(
                "Sense ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Sense name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Sense sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new SenseDefinition(
            id,
            name,
            sources);
    }
}
