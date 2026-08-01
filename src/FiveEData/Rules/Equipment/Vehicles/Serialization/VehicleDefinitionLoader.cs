using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Vehicles.Serialization;

internal static class VehicleDefinitionLoader
{
    public static IReadOnlyList<VehicleDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<VehicleDefinition> LoadFromJson(string json)
    {
        VehicleDefinitionData[] data =
            StrictJson.DeserializeArray<VehicleDefinitionData>(json, "Vehicle");

        var definitions = new List<VehicleDefinition>(data.Length);
        var ids = new HashSet<VehicleId>();

        for (int index = 0; index < data.Length; index++)
        {
            VehicleDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid vehicle definition at index {index}.");
            }

            VehicleDefinition definition;

            try
            {
                definition = Map(itemData);
                VehicleDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid vehicle definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate vehicle ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static VehicleDefinition Map(VehicleDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new VehicleId(
            data.Id
            ?? throw new ArgumentException(
                "Vehicle ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Vehicle name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Vehicle cost is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Vehicle special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Vehicle sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        Weight? listedWeight = data.ListedWeight is null
            ? null
            : new Weight(data.ListedWeight.Pounds);

        VehicleSpeed? listedSpeed =
            data.ListedSpeedMilesPerHour is null
                ? null
                : new VehicleSpeed(
                    data.ListedSpeedMilesPerHour.Value);

        return new VehicleDefinition(
            id,
            name,
            data.Kind,
            new Money(cost.CopperPieces),
            listedWeight,
            listedSpeed,
            specialRuleIds,
            sources);
    }
}
