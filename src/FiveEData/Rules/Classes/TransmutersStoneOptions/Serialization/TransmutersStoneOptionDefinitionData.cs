using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.TransmutersStoneOptions.Serialization;

internal sealed class TransmutersStoneOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? DarkvisionRangeFeet { get; init; }

    [JsonRequired]
    public int? SpeedBonusFeet { get; init; }

    [JsonRequired]
    public bool RequiresUnencumbered { get; init; }

    [JsonRequired]
    public string? SavingThrowProficiencyAbilityId { get; init; }

    [JsonRequired]
    public string[]? ChoosableResistedDamageTypeIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
