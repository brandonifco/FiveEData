using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.ThirdEyeOptions.Serialization;

internal static class ThirdEyeOptionDefinitionLoader
{
    public static IReadOnlyList<ThirdEyeOptionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ThirdEyeOptionDefinition> LoadFromJson(
        string json)
    {
        ThirdEyeOptionDefinitionData[] data =
            StrictJson.DeserializeArray<ThirdEyeOptionDefinitionData>(
                json,
                "Third Eye option");

        var definitions = new List<ThirdEyeOptionDefinition>(data.Length);
        var ids = new HashSet<ThirdEyeOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            ThirdEyeOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid Third Eye option definition at index " +
                    $"{index}.");
            }

            ThirdEyeOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                ThirdEyeOptionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid Third Eye option definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate Third Eye option ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ThirdEyeOptionDefinition Map(
        ThirdEyeOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ThirdEyeOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Third Eye option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Third Eye option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Third Eye option sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ThirdEyeOptionDefinition(
            id: id,
            name: name,
            darkvisionRangeFeet: data.DarkvisionRangeFeet,
            etherealSightRangeFeet: data.EtherealSightRangeFeet,
            seeInvisibilityRangeFeet: data.SeeInvisibilityRangeFeet,
            canReadAllLanguages: data.CanReadAllLanguages,
            sources: sources);
    }
}
