using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Conditions.Serialization;

internal static class ConditionDefinitionLoader
{
    public static IReadOnlyList<ConditionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ConditionDefinition> LoadFromJson(
        string json)
    {
        ConditionDefinitionData[] data =
            StrictJson.DeserializeArray<ConditionDefinitionData>(
                json,
                "Condition");

        var definitions =
            new List<ConditionDefinition>(data.Length);
        var ids = new HashSet<ConditionId>();

        for (int index = 0; index < data.Length; index++)
        {
            ConditionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid condition definition at index {index}.");
            }

            ConditionDefinition definition;

            try
            {
                definition = Map(itemData);
                ConditionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid condition definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate condition ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ConditionDefinition Map(
        ConditionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ConditionId(
            data.Id
            ?? throw new ArgumentException(
                "Condition ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Condition name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Condition sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ConditionDefinition(
            id,
            name,
            sources);
    }
}
