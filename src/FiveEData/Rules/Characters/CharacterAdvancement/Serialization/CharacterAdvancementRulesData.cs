using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Characters.CharacterAdvancement.Serialization;

internal sealed class CharacterAdvancementRulesData
{
    [JsonRequired]
    public CharacterAdvancementLevelData[]? Levels { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
