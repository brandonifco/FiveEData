using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;

internal static class BattleMasterManeuverDefinitionLoader
{
    public static IReadOnlyList<BattleMasterManeuverDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<BattleMasterManeuverDefinition> LoadFromJson(
        string json)
    {
        BattleMasterManeuverDefinitionData[] data =
            StrictJson.DeserializeArray<BattleMasterManeuverDefinitionData>(
                json,
                "Battle Master maneuver");

        var definitions =
            new List<BattleMasterManeuverDefinition>(data.Length);
        var ids = new HashSet<BattleMasterManeuverId>();

        for (int index = 0; index < data.Length; index++)
        {
            BattleMasterManeuverDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid Battle Master maneuver definition at index " +
                    $"{index}.");
            }

            BattleMasterManeuverDefinition definition;

            try
            {
                definition = Map(itemData);
                BattleMasterManeuverDefinitionValidator.EnsureValid(
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
                    "Invalid Battle Master maneuver definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    "Duplicate Battle Master maneuver ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static BattleMasterManeuverDefinition Map(
        BattleMasterManeuverDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new BattleMasterManeuverId(
            data.Id
            ?? throw new ArgumentException(
                "Battle Master maneuver ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Battle Master maneuver name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Battle Master maneuver sources are required.",
                nameof(data));

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new BattleMasterManeuverDefinition(
            id,
            name,
            data.EffectTarget,
            savingThrowAbilityId,
            sources);
    }
}
