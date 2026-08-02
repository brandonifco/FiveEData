using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Languages.Serialization;

internal static class LanguageDefinitionLoader
{
    public static IReadOnlyList<LanguageDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<LanguageDefinition> LoadFromJson(
        string json)
    {
        LanguageDefinitionData[] data =
            StrictJson.DeserializeArray<LanguageDefinitionData>(
                json,
                "Language");

        var definitions =
            new List<LanguageDefinition>(data.Length);
        var ids = new HashSet<LanguageId>();

        for (int index = 0; index < data.Length; index++)
        {
            LanguageDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid language definition at index {index}.");
            }

            LanguageDefinition definition;

            try
            {
                definition = Map(itemData);
                LanguageDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid language definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate language ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static LanguageDefinition Map(
        LanguageDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new LanguageId(
            data.Id
            ?? throw new ArgumentException(
                "Language ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Language name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Language sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new LanguageDefinition(
            id,
            name,
            data.Category,
            sources);
    }
}
