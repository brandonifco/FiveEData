using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.MountSupport.Serialization;

internal static class MountSupportDefinitionLoader
{
    public static IReadOnlyList<MountSupportDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<MountSupportDefinition> LoadFromJson(
        string json)
    {
        MountSupportDefinitionData[] data =
            StrictJson.DeserializeArray<MountSupportDefinitionData>(
                json,
                "Mount support");

        var definitions =
            new List<MountSupportDefinition>(data.Length);
        var ids = new HashSet<MountSupportId>();

        for (int index = 0; index < data.Length; index++)
        {
            MountSupportDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid mount support definition at index {index}.");
            }

            MountSupportDefinition definition;

            try
            {
                definition = Map(itemData);
                MountSupportDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid mount support definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate mount support ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static MountSupportDefinition Map(
        MountSupportDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new MountSupportId(
            data.Id
            ?? throw new ArgumentException(
                "Mount support ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Mount support name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Mount support cost is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Mount support special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Mount support sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        Weight? listedWeight = data.ListedWeight is null
            ? null
            : new Weight(data.ListedWeight.Pounds);

        return new MountSupportDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            listedWeight,
            specialRuleIds,
            sources);
    }
}
