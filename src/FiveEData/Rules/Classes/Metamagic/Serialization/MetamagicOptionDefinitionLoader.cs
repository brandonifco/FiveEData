using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.Metamagic.Serialization;

internal static class MetamagicOptionDefinitionLoader
{
    public static IReadOnlyList<MetamagicOptionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<MetamagicOptionDefinition> LoadFromJson(
        string json)
    {
        MetamagicOptionDefinitionData[] data =
            StrictJson.DeserializeArray<MetamagicOptionDefinitionData>(
                json,
                "Metamagic option");

        var definitions =
            new List<MetamagicOptionDefinition>(data.Length);
        var ids = new HashSet<MetamagicOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            MetamagicOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid metamagic option definition at index {index}.");
            }

            MetamagicOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                MetamagicOptionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid metamagic option definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate metamagic option ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static MetamagicOptionDefinition Map(
        MetamagicOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new MetamagicOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Metamagic option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Metamagic option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Metamagic option sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new MetamagicOptionDefinition(
            id,
            name,
            data.FixedSorceryPointCost,
            data.CostEqualsSpellLevelWithCantripMinimum,
            sources);
    }
}
