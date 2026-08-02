using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Abilities.Serialization;

internal static class AbilityDefinitionLoader
{
    public static IReadOnlyList<AbilityDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<AbilityDefinition> LoadFromJson(
        string json)
    {
        AbilityDefinitionData[] data =
            StrictJson.DeserializeArray<AbilityDefinitionData>(
                json,
                "Ability");

        var definitions =
            new List<AbilityDefinition>(data.Length);
        var ids = new HashSet<AbilityId>();

        for (int index = 0; index < data.Length; index++)
        {
            AbilityDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid ability definition at index {index}.");
            }

            AbilityDefinition definition;

            try
            {
                definition = Map(itemData);
                AbilityDefinitionValidator.EnsureValid(definition);
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
                    $"Invalid ability definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate ability ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static AbilityDefinition Map(
        AbilityDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new AbilityId(
            data.Id
            ?? throw new ArgumentException(
                "Ability ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Ability name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Ability sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new AbilityDefinition(id, name, sources);
    }
}
