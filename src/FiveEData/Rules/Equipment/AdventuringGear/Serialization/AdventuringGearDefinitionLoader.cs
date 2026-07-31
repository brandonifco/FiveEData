using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.AdventuringGear.Serialization;

internal static class AdventuringGearDefinitionLoader
{
    public static IReadOnlyList<AdventuringGearDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<AdventuringGearDefinition> LoadFromJson(
        string json)
    {
        AdventuringGearDefinitionData[] data =
            StrictJson.DeserializeArray<AdventuringGearDefinitionData>(
                json,
                "Adventuring gear");

        var definitions =
            new List<AdventuringGearDefinition>(data.Length);
        var ids = new HashSet<AdventuringGearId>();

        for (int index = 0; index < data.Length; index++)
        {
            AdventuringGearDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid adventuring gear definition at index {index}.");
            }

            AdventuringGearDefinition definition;

            try
            {
                definition = Map(itemData);
                AdventuringGearDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid adventuring gear definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate adventuring gear ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static AdventuringGearDefinition Map(
        AdventuringGearDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new AdventuringGearId(
            data.Id
            ?? throw new ArgumentException(
                "Adventuring gear ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Adventuring gear name is required.",
                nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException(
                "Adventuring gear cost is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Adventuring gear special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Adventuring gear sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        ListedWeight? listedWeight = data.ListedWeight is null
            ? null
            : new ListedWeight(
                new Weight(data.ListedWeight.Pounds),
                data.ListedWeight.Qualifier);

        return new AdventuringGearDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            listedWeight,
            specialRuleIds,
            sources);
    }
}
