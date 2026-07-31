using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Ammunition.Serialization;

internal static class AmmunitionDefinitionLoader
{
    public static IReadOnlyList<AmmunitionDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<AmmunitionDefinition> LoadFromJson(string json)
    {
        AmmunitionDefinitionData[] data =
            StrictJson.DeserializeArray<AmmunitionDefinitionData>(
                json,
                "Ammunition");

        var definitions = new List<AmmunitionDefinition>(data.Length);
        var ids = new HashSet<AmmunitionTypeId>();

        for (int index = 0; index < data.Length; index++)
        {
            AmmunitionDefinition definition;

            try
            {
                definition = Map(data[index]);
                AmmunitionDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(data[index].Id)
                    ? $"index {index}"
                    : $"'{data[index].Id}'";

                throw new InvalidDataException(
                    $"Invalid ammunition definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate ammunition ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static AmmunitionDefinition Map(AmmunitionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new AmmunitionTypeId(
            data.Id
            ?? throw new ArgumentException(
                "Ammunition ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Ammunition name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Ammunition cost is required.",
                nameof(data));

        WeightData weight = data.Weight
            ?? throw new ArgumentException(
                "Ammunition weight is required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Ammunition sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new AmmunitionDefinition(
            id,
            name,
            data.BundleQuantity,
            new Money(cost.CopperPieces),
            new Weight(weight.Pounds),
            sources);
    }
}
