using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Expenses.Services.Serialization;

internal static class MundaneServiceDefinitionLoader
{
    public static IReadOnlyList<MundaneServiceDefinition>
        LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<MundaneServiceDefinition>
        LoadFromJson(string json)
    {
        MundaneServiceDefinitionData[] data =
            StrictJson.DeserializeArray<
                MundaneServiceDefinitionData>(
                    json,
                    "Mundane service");

        var definitions =
            new List<MundaneServiceDefinition>(data.Length);
        var ids = new HashSet<MundaneServiceId>();

        for (int index = 0; index < data.Length; index++)
        {
            MundaneServiceDefinitionData? itemData =
                data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid mundane-service definition " +
                    $"at index {index}.");
            }

            MundaneServiceDefinition definition;

            try
            {
                definition = Map(itemData);
                MundaneServiceDefinitionValidator
                    .EnsureValid(definition);
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
                    "Invalid mundane-service definition " +
                    $"at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate mundane-service ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static MundaneServiceDefinition Map(
        MundaneServiceDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new MundaneServiceId(
            data.Id
            ?? throw new ArgumentException(
                "Mundane-service ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Mundane-service name is required.",
                nameof(data));

        ListedCostData costData = data.Cost
            ?? throw new ArgumentException(
                "Mundane-service cost is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Mundane-service special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Mundane-service sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new MundaneServiceDefinition(
            id,
            name,
            ListedCostDataMapper.Map(costData),
            data.PricingUnit,
            specialRuleIds,
            sources);
    }
}
