using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Expenses.Lifestyles.Serialization;

internal static class LifestyleDefinitionLoader
{
    public static IReadOnlyList<LifestyleDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<LifestyleDefinition> LoadFromJson(
        string json)
    {
        LifestyleDefinitionData[] data =
            StrictJson.DeserializeArray<LifestyleDefinitionData>(
                json,
                "Lifestyle");

        var definitions =
            new List<LifestyleDefinition>(data.Length);
        var ids = new HashSet<LifestyleId>();

        for (int index = 0; index < data.Length; index++)
        {
            LifestyleDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid lifestyle definition at index {index}.");
            }

            LifestyleDefinition definition;

            try
            {
                definition = Map(itemData);
                LifestyleDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid lifestyle definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate lifestyle ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static LifestyleDefinition Map(
        LifestyleDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new LifestyleId(
            data.Id
            ?? throw new ArgumentException(
                "Lifestyle ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Lifestyle name is required.",
                nameof(data));

        ListedCost? dailyCost = data.DailyCost is null
            ? null
            : ListedCostDataMapper.Map(data.DailyCost);

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Lifestyle special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Lifestyle sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new LifestyleDefinition(
            id,
            name,
            dailyCost,
            specialRuleIds,
            sources);
    }
}
