using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Combat.CombatActions.Serialization;

internal static class CombatActionDefinitionLoader
{
    public static IReadOnlyList<CombatActionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<CombatActionDefinition> LoadFromJson(
        string json)
    {
        CombatActionDefinitionData[] data =
            StrictJson.DeserializeArray<CombatActionDefinitionData>(
                json,
                "CombatAction");

        var definitions =
            new List<CombatActionDefinition>(data.Length);
        var ids = new HashSet<CombatActionId>();

        for (int index = 0; index < data.Length; index++)
        {
            CombatActionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid combat action definition at index {index}.");
            }

            CombatActionDefinition definition;

            try
            {
                definition = Map(itemData);
                CombatActionDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid combat action definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate combat action ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static CombatActionDefinition Map(
        CombatActionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new CombatActionId(
            data.Id
            ?? throw new ArgumentException(
                "Combat action ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Combat action name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Combat action sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new CombatActionDefinition(
            id,
            name,
            sources);
    }
}
