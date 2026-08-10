using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;

internal static class ChannelDivinityOptionDefinitionLoader
{
    public static IReadOnlyList<ChannelDivinityOptionDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<ChannelDivinityOptionDefinition> LoadFromJson(
        string json)
    {
        ChannelDivinityOptionDefinitionData[] data =
            StrictJson.DeserializeArray<ChannelDivinityOptionDefinitionData>(
                json,
                "Channel Divinity option");

        var definitions =
            new List<ChannelDivinityOptionDefinition>(data.Length);
        var ids = new HashSet<ChannelDivinityOptionId>();

        for (int index = 0; index < data.Length; index++)
        {
            ChannelDivinityOptionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid Channel Divinity option definition at index " +
                    $"{index}.");
            }

            ChannelDivinityOptionDefinition definition;

            try
            {
                definition = Map(itemData);
                ChannelDivinityOptionDefinitionValidator.EnsureValid(
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
                    $"Invalid Channel Divinity option definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    "Duplicate Channel Divinity option ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static ChannelDivinityOptionDefinition Map(
        ChannelDivinityOptionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new ChannelDivinityOptionId(
            data.Id
            ?? throw new ArgumentException(
                "Channel Divinity option ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Channel Divinity option name is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Channel Divinity option sources are required.",
                nameof(data));

        AbilityId? savingThrowAbilityId =
            data.SavingThrowAbilityId is { } savingThrowAbilityIdValue
                ? new AbilityId(savingThrowAbilityIdValue)
                : null;

        ConditionId? imposedConditionId =
            data.ImposedConditionId is { } imposedConditionIdValue
                ? new ConditionId(imposedConditionIdValue)
                : null;

        SpellId? grantedSpellId =
            data.GrantedSpellId is { } grantedSpellIdValue
                ? new SpellId(grantedSpellIdValue)
                : null;

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ChannelDivinityOptionDefinition(
            id: id,
            name: name,
            rangeFeet: data.RangeFeet,
            savingThrowAbilityId: savingThrowAbilityId,
            durationMinutes: data.DurationMinutes,
            rollBonus: data.RollBonus,
            imposedConditionId: imposedConditionId,
            conditionDurationTrigger: data.ConditionDurationTrigger,
            maximizesDamageRoll: data.MaximizesDamageRoll,
            grantedSpellId: grantedSpellId,
            automaticallyFailsGrantedSpellSave:
                data.AutomaticallyFailsGrantedSpellSave,
            sources: sources);
    }
}
