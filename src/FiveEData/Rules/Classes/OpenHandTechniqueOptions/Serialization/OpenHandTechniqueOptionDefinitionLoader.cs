using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.OpenHandTechniqueOptions.Serialization;

internal static class OpenHandTechniqueOptionDefinitionLoader
{
    public static IReadOnlyList<OpenHandTechniqueOptionDefinition>
        LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<OpenHandTechniqueOptionDefinition>
        LoadFromJson(string json)
    {
        OpenHandTechniqueOptionDefinitionData[] data =
            StrictJson.DeserializeArray<
                OpenHandTechniqueOptionDefinitionData>(
                json,
                "Open hand technique option");

        var definitions =
            new List<OpenHandTechniqueOptionDefinition>(data.Length);
        var ids = new HashSet<OpenHandTechniqueOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            OpenHandTechniqueOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid open hand technique option definition at " +
                    $"index {index}.");
            }

            OpenHandTechniqueOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                OpenHandTechniqueOptionDefinitionValidator.EnsureValid(
                    definition);
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
                    $"Invalid open hand technique option definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate open hand technique option ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static OpenHandTechniqueOptionDefinition Map(
        OpenHandTechniqueOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new OpenHandTechniqueOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Open hand technique option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Open hand technique option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Open hand technique option sources are required.",
                nameof(data));

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        ConditionId? imposedConditionId =
            data.ImposedConditionId is { } imposedConditionIdValue
                ? new ConditionId(imposedConditionIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new OpenHandTechniqueOptionDefinition(
            id: id,
            name: name,
            savingThrowAbilityId: savingThrowAbilityId,
            imposedConditionId: imposedConditionId,
            pushDistanceFeet: data.PushDistanceFeet,
            preventsReactions: data.PreventsReactions,
            preventsReactionsUntil: data.PreventsReactionsUntil,
            sources: sources);
    }
}
