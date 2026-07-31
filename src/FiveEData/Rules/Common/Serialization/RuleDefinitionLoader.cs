using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal static class RuleDefinitionLoader
{
    public static IReadOnlyList<RuleDefinition> LoadFromJson(string json)
    {
        RuleDefinitionData[] data =
            StrictJson.DeserializeArray<RuleDefinitionData>(
                json,
                "Rule");

        var rules = new List<RuleDefinition>(data.Length);
        var ids = new HashSet<RuleId>();

        for (int index = 0; index < data.Length; index++)
        {
            RuleDefinition rule;

            try
            {
                rule = Map(data[index]);
                RuleDefinitionValidator.EnsureValid(rule);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(data[index].Id)
                    ? $"index {index}"
                    : $"'{data[index].Id}'";

                throw new InvalidDataException(
                    $"Invalid rule definition at {identity}.",
                    exception);
            }

            if (!ids.Add(rule.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate rule ID '{rule.Id}'.");
            }

            rules.Add(rule);
        }

        return rules;
    }

    private static RuleDefinition Map(RuleDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new RuleId(
            data.Id
            ?? throw new ArgumentException(
                "Rule ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Rule name is required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Rule sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new RuleDefinition(
            id,
            name,
            sources);
    }
}
