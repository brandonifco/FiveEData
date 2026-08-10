using System.Text.Json.Serialization;

namespace FiveEData.Rules.Characters.CharacterAdvancement.Serialization;

internal sealed class CharacterAdvancementLevelData
{
    [JsonRequired]
    public int Level { get; init; }

    [JsonRequired]
    public int ExperiencePointThreshold { get; init; }

    [JsonRequired]
    public int ProficiencyBonus { get; init; }
}
