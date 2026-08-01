using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Mounts.Serialization;

internal static class MountDefinitionLoader
{
    public static IReadOnlyList<MountDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<MountDefinition> LoadFromJson(string json)
    {
        MountDefinitionData[] data =
            StrictJson.DeserializeArray<MountDefinitionData>(json, "Mount");

        var definitions = new List<MountDefinition>(data.Length);
        var ids = new HashSet<MountId>();

        for (int index = 0; index < data.Length; index++)
        {
            MountDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid mount definition at index {index}.");
            }

            MountDefinition definition;

            try
            {
                definition = Map(itemData);
                MountDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid mount definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate mount ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static MountDefinition Map(MountDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new MountId(
            data.Id
            ?? throw new ArgumentException(
                "Mount ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Mount name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Mount cost is required.",
                nameof(data));

        WeightData baseCarryingCapacity = data.BaseCarryingCapacity
            ?? throw new ArgumentException(
                "Mount base carrying capacity is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Mount special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Mount sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new MountDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            new Distance(data.SpeedFeet),
            new Weight(baseCarryingCapacity.Pounds),
            specialRuleIds,
            sources);
    }
}
