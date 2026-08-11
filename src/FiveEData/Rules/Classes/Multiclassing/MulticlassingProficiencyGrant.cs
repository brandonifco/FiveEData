using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Multiclassing;

/// <summary>
/// One row of the Multiclassing Proficiencies table (p.164) — the strict
/// subset of a class's starting proficiencies gained when multiclassing
/// into it, which is never the same as its full starting set.
///
/// Sorcerer and Wizard print an em-dash and carry <c>null</c> rather than
/// an empty grant, so a populated grant always grants something; the
/// constructor enforces it.
///
/// Druid's parenthetical "(druids will not wear armor or use shields made
/// of metal)" stays in the citation — the same deliberate unmodelled gap
/// the Druid's own starting proficiencies already leave.
/// </summary>
public sealed record MulticlassingProficiencyGrant
{
    public MulticlassingProficiencyGrant(
        IEnumerable<ArmorCategory>? armorProficiencyCategories = null,
        bool proficientWithShields = false,
        IEnumerable<WeaponProficiencyCategory>? weaponProficiencyCategories =
            null,
        IEnumerable<WeaponId>? weaponProficiencyIds = null,
        int skillChoiceCount = 0,
        bool skillChoiceFromClassSkillList = false,
        IEnumerable<ToolId>? toolProficiencyIds = null,
        ToolProficiencyChoice? toolProficiencyChoice = null)
    {
        ArmorCategory[] armor = armorProficiencyCategories?.ToArray() ?? [];
        WeaponProficiencyCategory[] weaponCategories =
            weaponProficiencyCategories?.ToArray() ?? [];
        WeaponId[] weapons = weaponProficiencyIds?.ToArray() ?? [];
        ToolId[] tools = toolProficiencyIds?.ToArray() ?? [];

        if (skillChoiceCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(skillChoiceCount),
                skillChoiceCount,
                "A multiclassing proficiency grant skill choice count must " +
                "not be negative.");
        }

        if (skillChoiceCount is 0 && skillChoiceFromClassSkillList)
        {
            throw new ArgumentException(
                "A multiclassing proficiency grant cannot restrict a skill " +
                "choice to the class's skill list without offering one.",
                nameof(skillChoiceFromClassSkillList));
        }

        if (armor.Length is 0 &&
            !proficientWithShields &&
            weaponCategories.Length is 0 &&
            weapons.Length is 0 &&
            skillChoiceCount is 0 &&
            tools.Length is 0 &&
            toolProficiencyChoice is null)
        {
            throw new ArgumentException(
                "A multiclassing proficiency grant must grant something — " +
                "a class that grants nothing carries no grant at all.",
                nameof(armorProficiencyCategories));
        }

        foreach ((string label, int distinct, int total) in new[]
        {
            ("armor category", armor.Distinct().Count(), armor.Length),
            ("weapon category",
                weaponCategories.Distinct().Count(), weaponCategories.Length),
            ("weapon", weapons.Distinct().Count(), weapons.Length),
            ("tool", tools.Distinct().Count(), tools.Length)
        })
        {
            if (distinct != total)
            {
                throw new ArgumentException(
                    $"A multiclassing proficiency grant must not repeat a " +
                    $"{label}.",
                    nameof(armorProficiencyCategories));
            }
        }

        ArmorProficiencyCategories = Array.AsReadOnly(armor);
        ProficientWithShields = proficientWithShields;
        WeaponProficiencyCategories = Array.AsReadOnly(weaponCategories);
        WeaponProficiencyIds = Array.AsReadOnly(weapons);
        SkillChoiceCount = skillChoiceCount;
        SkillChoiceFromClassSkillList = skillChoiceFromClassSkillList;
        ToolProficiencyIds = Array.AsReadOnly(tools);
        ToolProficiencyChoice = toolProficiencyChoice;
    }

    public IReadOnlyList<ArmorCategory> ArmorProficiencyCategories { get; }

    public bool ProficientWithShields { get; }

    public IReadOnlyList<WeaponProficiencyCategory>
        WeaponProficiencyCategories { get; }

    public IReadOnlyList<WeaponId> WeaponProficiencyIds { get; }

    public int SkillChoiceCount { get; }

    /// <summary>
    /// The PHB states this two ways and they are not interchangeable:
    /// Bard grants "one skill of your choice" (unrestricted), while Ranger
    /// and Rogue grant "one skill from the class's skill list".
    /// </summary>
    public bool SkillChoiceFromClassSkillList { get; }

    public IReadOnlyList<ToolId> ToolProficiencyIds { get; }

    public ToolProficiencyChoice? ToolProficiencyChoice { get; }
}
