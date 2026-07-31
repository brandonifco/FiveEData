using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Shields.Serialization;

internal static class ShieldDefinitionLoader
{
    public static IReadOnlyList<ShieldDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ShieldDefinition> LoadFromJson(string json)
    {
        ShieldDefinitionData[] data =
            StrictJson.DeserializeArray<ShieldDefinitionData>(
                json,
                "Shield");

        var definitions = new List<ShieldDefinition>(data.Length);
        var ids = new HashSet<ShieldId>();

        for (int index = 0; index < data.Length; index++)
        {
            ShieldDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid shield definition at index {index}.");
            }

            ShieldDefinition definition;

            try
            {
                definition = Map(itemData);
                ShieldDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid shield definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate shield ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ShieldDefinition Map(ShieldDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ShieldId(
            data.Id
            ?? throw new ArgumentException(
                "Shield ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Shield name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Shield cost is required.",
                nameof(data));

        WeightData weight = data.Weight
            ?? throw new ArgumentException(
                "Shield weight is required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Shield sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ShieldDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            new Weight(weight.Pounds),
            data.ArmorClassBonus,
            sources);
    }
}
