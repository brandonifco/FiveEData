using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Multiclassing.Serialization;

internal sealed class MulticlassingProficiencyGrantData
{
    [JsonRequired]
    public ArmorCategory[]? ArmorProficiencyCategories { get; init; }

    [JsonRequired]
    public bool ProficientWithShields { get; init; }

    [JsonRequired]
    public WeaponProficiencyCategory[]? WeaponProficiencyCategories { get; init; }

    [JsonRequired]
    public string[]? WeaponProficiencyIds { get; init; }

    [JsonRequired]
    public int SkillChoiceCount { get; init; }

    [JsonRequired]
    public bool SkillChoiceFromClassSkillList { get; init; }

    [JsonRequired]
    public string[]? ToolProficiencyIds { get; init; }

    [JsonRequired]
    public ToolProficiencyChoiceData? ToolProficiencyChoice { get; init; }
}
