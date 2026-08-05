using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.DamageTypes.Serialization;

internal static class DamageTypeDefinitionLoader
{
    public static IReadOnlyList<DamageTypeDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<DamageTypeDefinition> LoadFromJson(
        string json)
    {
        DamageTypeDefinitionData[] data =
            StrictJson.DeserializeArray<DamageTypeDefinitionData>(
                json,
                "DamageType");

        var definitions =
            new List<DamageTypeDefinition>(data.Length);
        var ids = new HashSet<DamageTypeId>();

        for (int index = 0; index < data.Length; index++)
        {
            DamageTypeDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid damage type definition at index {index}.");
            }

            DamageTypeDefinition definition;

            try
            {
                definition = Map(itemData);
                DamageTypeDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid damage type definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate damage type ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static DamageTypeDefinition Map(
        DamageTypeDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new DamageTypeId(
            data.Id
            ?? throw new ArgumentException(
                "Damage type ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Damage type name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Damage type sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new DamageTypeDefinition(
            id,
            name,
            sources);
    }
}
