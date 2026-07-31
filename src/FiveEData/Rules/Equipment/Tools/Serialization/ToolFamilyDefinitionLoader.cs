using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Tools.Serialization;

internal static class ToolFamilyDefinitionLoader
{
    public static IReadOnlyList<ToolFamilyDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ToolFamilyDefinition> LoadFromJson(string json)
    {
        ToolFamilyDefinitionData[] data =
            StrictJson.DeserializeArray<ToolFamilyDefinitionData>(json, "Tool family");

        var definitions = new List<ToolFamilyDefinition>(data.Length);
        var ids = new HashSet<ToolFamilyId>();

        for (int index = 0; index < data.Length; index++)
        {
            ToolFamilyDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid tool family definition at index {index}.");
            }

            ToolFamilyDefinition definition;

            try
            {
                definition = Map(itemData);
                ToolFamilyDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid tool family definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate tool family ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ToolFamilyDefinition Map(ToolFamilyDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ToolFamilyId(
            data.Id
            ?? throw new ArgumentException(
                "Tool family ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Tool family name is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Tool family special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Tool family sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ToolFamilyDefinition(
            id,
            name,
            specialRuleIds,
            sources);
    }
}
