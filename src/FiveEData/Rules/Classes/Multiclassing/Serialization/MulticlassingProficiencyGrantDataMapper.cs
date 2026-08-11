using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Multiclassing.Serialization;

internal static class MulticlassingProficiencyGrantDataMapper
{
    public static MulticlassingProficiencyGrant Map(
        MulticlassingProficiencyGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new MulticlassingProficiencyGrant(
            data.ArmorProficiencyCategories
                ?? throw Missing(nameof(data.ArmorProficiencyCategories)),
            data.ProficientWithShields,
            data.WeaponProficiencyCategories
                ?? throw Missing(nameof(data.WeaponProficiencyCategories)),
            (data.WeaponProficiencyIds
                ?? throw Missing(nameof(data.WeaponProficiencyIds)))
                .Select(value => new WeaponId(value)),
            data.SkillChoiceCount,
            data.SkillChoiceFromClassSkillList,
            (data.ToolProficiencyIds
                ?? throw Missing(nameof(data.ToolProficiencyIds)))
                .Select(value => new ToolId(value)),
            data.ToolProficiencyChoice is { } choiceData
                ? ToolProficiencyChoiceDataMapper.Map(choiceData)
                : null);
    }

    private static ArgumentException Missing(string member) =>
        new($"Multiclassing proficiency grant {member} is required.");
}
