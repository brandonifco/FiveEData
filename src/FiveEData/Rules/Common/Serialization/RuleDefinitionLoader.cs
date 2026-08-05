using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal static class RuleDefinitionLoader
{
    public static IReadOnlyList<RuleDefinition> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<RuleDefinition> LoadAndMergeFromFiles(
        IEnumerable<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        return LoadAndMergeFromJson(paths.Select(File.ReadAllText));
    }

    public static IReadOnlyList<RuleDefinition> LoadAndMergeFromJson(
        IEnumerable<string> jsonDocuments)
    {
        ArgumentNullException.ThrowIfNull(jsonDocuments);

        var merged = new List<RuleDefinition>();
        var ids = new HashSet<RuleId>();

        foreach (string json in jsonDocuments)
        {
            foreach (RuleDefinition rule in LoadFromJson(json))
            {
                if (!ids.Add(rule.Id))
                {
                    throw new InvalidDataException(
                        $"Duplicate rule ID '{rule.Id}' across rule files.");
                }

                merged.Add(rule);
            }
        }

        return merged;
    }

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
            RuleDefinitionData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid rule definition at index {index}.");
            }

            RuleDefinition rule;

            try
            {
                rule = Map(itemData);
                RuleDefinitionValidator.EnsureValid(rule);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

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
