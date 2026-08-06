using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.Spellcasting.Serialization;

internal static class SpellSlotProgressionDefinitionLoader
{
    public static IReadOnlyList<SpellSlotProgressionDefinition>
        LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SpellSlotProgressionDefinition>
        LoadFromJson(string json)
    {
        SpellSlotProgressionDefinitionData[] data =
            StrictJson.DeserializeArray<SpellSlotProgressionDefinitionData>(
                json,
                "Spell slot progression");

        var definitions =
            new List<SpellSlotProgressionDefinition>(data.Length);
        var ids = new HashSet<SpellSlotProgressionId>();

        for (int index = 0; index < data.Length; index++)
        {
            SpellSlotProgressionDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    "Invalid spell slot progression definition at " +
                    $"index {index}.");
            }

            SpellSlotProgressionDefinition definition;

            try
            {
                definition = Map(itemData);
                SpellSlotProgressionDefinitionValidator.EnsureValid(
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
                    "Invalid spell slot progression definition at " +
                    $"{identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    "Duplicate spell slot progression ID " +
                    $"'{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static SpellSlotProgressionDefinition Map(
        SpellSlotProgressionDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SpellSlotProgressionId(
            data.Id
            ?? throw new ArgumentException(
                "Spell slot progression ID is required.",
                nameof(data)));

        string name =
            data.Name
            ?? throw new ArgumentException(
                "Spell slot progression name is required.",
                nameof(data));

        SpellSlotLevelEntryData[] levelEntryData =
            data.LevelEntries
            ?? throw new ArgumentException(
                "Spell slot progression level entries are required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Spell slot progression sources are required.",
                nameof(data));

        SpellSlotLevelEntry[] levelEntries = levelEntryData
            .Select(MapLevelEntry)
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new SpellSlotProgressionDefinition(
            id,
            name,
            data.RecoveryRest,
            levelEntries,
            sources);
    }

    private static SpellSlotLevelEntry MapLevelEntry(
        SpellSlotLevelEntryData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        SpellSlotCountData[] slotData =
            data.Slots
            ?? throw new ArgumentException(
                "Spell slot progression level entry slots are " +
                "required.",
                nameof(data));

        SpellSlotCount[] slots = slotData
            .Select(
                slot => new SpellSlotCount(
                    slot.SpellLevel,
                    slot.Count))
            .ToArray();

        return new SpellSlotLevelEntry(data.CharacterLevel, slots);
    }
}
