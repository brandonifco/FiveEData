using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Combat.Cover.Serialization;

internal static class CoverDefinitionLoader
{
    public static IReadOnlyList<CoverDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<CoverDefinition> LoadFromJson(string json)
    {
        CoverDefinitionData[] data =
            StrictJson.DeserializeArray<CoverDefinitionData>(json, "Cover");

        var definitions = new List<CoverDefinition>(data.Length);
        var ids = new HashSet<CoverId>();

        for (int index = 0; index < data.Length; index++)
        {
            CoverDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid cover definition at index {index}.");
            }

            CoverDefinition definition;

            try
            {
                definition = Map(itemData);
                CoverDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid cover definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate cover ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static CoverDefinition Map(CoverDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new CoverId(
            data.Id
            ?? throw new ArgumentException(
                "Cover ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Cover name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Cover sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new CoverDefinition(
            id,
            name,
            data.ArmorClassBonus,
            data.DexteritySavingThrowBonus,
            data.PreventsBeingTargeted,
            sources);
    }
}
