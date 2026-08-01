using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Catalog;

internal sealed class RulesetDefinitionSet
{
    public RulesetDefinitionSet(
        IReadOnlyList<WeaponDefinition> weapons,
        IReadOnlyList<SourceDocument> sourceDocuments,
        IReadOnlyList<AmmunitionDefinition> ammunition,
        IReadOnlyList<RuleDefinition> rules,
        IReadOnlyList<ArmorDefinition> armor,
        IReadOnlyList<ShieldDefinition> shields,
        IReadOnlyList<AdventuringGearDefinition> adventuringGear,
        IReadOnlyList<ContainerCapacityDefinition> containerCapacities,
        IReadOnlyList<ToolFamilyDefinition> toolFamilies,
        IReadOnlyList<ToolDefinition> tools,
        IReadOnlyList<MountDefinition> mounts,
        ArmorUsageRules? armorUsage = null)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(armor);
        ArgumentNullException.ThrowIfNull(shields);
        ArgumentNullException.ThrowIfNull(adventuringGear);
        ArgumentNullException.ThrowIfNull(containerCapacities);
        ArgumentNullException.ThrowIfNull(toolFamilies);
        ArgumentNullException.ThrowIfNull(tools);
        ArgumentNullException.ThrowIfNull(mounts);

        Weapons = weapons;
        SourceDocuments = sourceDocuments;
        Ammunition = ammunition;
        Rules = rules;
        Armor = armor;
        Shields = shields;
        AdventuringGear = adventuringGear;
        ContainerCapacities = containerCapacities;
        ToolFamilies = toolFamilies;
        Tools = tools;
        Mounts = mounts;
        ArmorUsage = armorUsage;
    }

    public IReadOnlyList<WeaponDefinition> Weapons { get; }
    public IReadOnlyList<SourceDocument> SourceDocuments { get; }
    public IReadOnlyList<AmmunitionDefinition> Ammunition { get; }
    public IReadOnlyList<RuleDefinition> Rules { get; }
    public IReadOnlyList<ArmorDefinition> Armor { get; }
    public IReadOnlyList<ShieldDefinition> Shields { get; }
    public IReadOnlyList<AdventuringGearDefinition> AdventuringGear { get; }
    public IReadOnlyList<ContainerCapacityDefinition> ContainerCapacities { get; }
    public IReadOnlyList<ToolFamilyDefinition> ToolFamilies { get; }
    public IReadOnlyList<ToolDefinition> Tools { get; }
    public IReadOnlyList<MountDefinition> Mounts { get; }
    public ArmorUsageRules? ArmorUsage { get; }
}
