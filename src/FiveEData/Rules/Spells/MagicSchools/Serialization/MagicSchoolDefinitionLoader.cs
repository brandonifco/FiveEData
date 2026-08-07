using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Spells.MagicSchools.Serialization;

internal static class MagicSchoolDefinitionLoader
{
    public static IReadOnlyList<MagicSchoolDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<MagicSchoolDefinition> LoadFromJson(
        string json)
    {
        MagicSchoolDefinitionData[] data =
            StrictJson.DeserializeArray<MagicSchoolDefinitionData>(
                json,
                "MagicSchool");

        var definitions =
            new List<MagicSchoolDefinition>(data.Length);
        var ids = new HashSet<MagicSchoolId>();

        for (int index = 0; index < data.Length; index++)
        {
            MagicSchoolDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid magic school definition at index {index}.");
            }

            MagicSchoolDefinition definition;

            try
            {
                definition = Map(itemData);
                MagicSchoolDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid magic school definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate magic school ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static MagicSchoolDefinition Map(
        MagicSchoolDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new MagicSchoolId(
            data.Id
            ?? throw new ArgumentException(
                "Magic school ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Magic school name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Magic school sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new MagicSchoolDefinition(
            id,
            name,
            sources);
    }
}
