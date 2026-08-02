using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Sizes.Serialization;

internal static class CreatureSizeDefinitionLoader
{
    public static IReadOnlyList<CreatureSizeDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<CreatureSizeDefinition> LoadFromJson(
        string json)
    {
        CreatureSizeDefinitionData[] data =
            StrictJson.DeserializeArray<CreatureSizeDefinitionData>(
                json,
                "Creature size");

        var definitions =
            new List<CreatureSizeDefinition>(data.Length);
        var ids = new HashSet<CreatureSizeId>();

        for (int index = 0; index < data.Length; index++)
        {
            CreatureSizeDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid creature-size definition " +
                    $"at index {index}.");
            }

            CreatureSizeDefinition definition;

            try
            {
                definition = Map(itemData);
                CreatureSizeDefinitionValidator.EnsureValid(
                    definition);
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
                    "Invalid creature-size definition " +
                    $"at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate creature-size ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static CreatureSizeDefinition Map(
        CreatureSizeDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new CreatureSizeId(
            data.Id
            ?? throw new ArgumentException(
                "Creature-size ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Creature-size name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Creature-size sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new CreatureSizeDefinition(
            id,
            name,
            sources);
    }
}
