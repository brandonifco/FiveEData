using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.Spellcasting.Serialization;

internal sealed class SpellSlotProgressionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public SpellSlotRecoveryRest RecoveryRest { get; init; }

    [JsonRequired]
    public SpellSlotLevelEntryData[]? LevelEntries { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class SpellSlotLevelEntryData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public SpellSlotCountData[]? Slots { get; init; }
}

internal sealed class SpellSlotCountData
{
    [JsonRequired]
    public int SpellLevel { get; init; }

    [JsonRequired]
    public int Count { get; init; }
}
