using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Tools.Serialization;

internal static class ToolDefinitionLoader
{
    public static IReadOnlyList<ToolDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ToolDefinition> LoadFromJson(string json)
    {
        ToolDefinitionData[] data =
            StrictJson.DeserializeArray<ToolDefinitionData>(json, "Tool");

        var definitions = new List<ToolDefinition>(data.Length);
        var ids = new HashSet<ToolId>();

        for (int index = 0; index < data.Length; index++)
        {
            ToolDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid tool definition at index {index}.");
            }

            ToolDefinition definition;

            try
            {
                definition = Map(itemData);
                ToolDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid tool definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate tool ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ToolDefinition Map(ToolDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ToolId(
            data.Id
            ?? throw new ArgumentException("Tool ID is required.", nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException("Tool name is required.", nameof(data));

        MoneyData cost = data.Cost
            ?? throw new ArgumentException("Tool cost is required.", nameof(data));

        ToolFamilyId? familyId = data.FamilyId is null
            ? null
            : new ToolFamilyId(data.FamilyId);

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Tool special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException("Tool sources are required.", nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        Weight? weight = data.Weight is null
            ? null
            : new Weight(data.Weight.Pounds);

        return new ToolDefinition(
            id,
            name,
            new Money(cost.CopperPieces),
            weight,
            familyId,
            specialRuleIds,
            sources);
    }
}
