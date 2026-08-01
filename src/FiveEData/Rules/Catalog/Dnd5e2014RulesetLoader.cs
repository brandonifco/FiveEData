using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Mounts.Serialization;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountSupport.Serialization;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Vehicles.Serialization;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Rules.Catalog;

internal static class Dnd5e2014RulesetLoader
{
    private const string SourcesResource =
        "FiveEData.Data.dnd5e2014.sources.json";

    private const string AmmunitionResource =
        "FiveEData.Data.dnd5e2014.ammunition.json";

    private const string WeaponsResource =
        "FiveEData.Data.dnd5e2014.weapons.json";

    private const string ArmorResource =
        "FiveEData.Data.dnd5e2014.armor.json";

    private const string ArmorUsageResource =
        "FiveEData.Data.dnd5e2014.armor-usage.json";

    private const string ShieldsResource =
        "FiveEData.Data.dnd5e2014.shields.json";

    private const string AdventuringGearResource =
        "FiveEData.Data.dnd5e2014.adventuring-gear.json";

    private const string ContainerCapacitiesResource =
        "FiveEData.Data.dnd5e2014.container-capacities.json";

    private const string ToolFamiliesResource =
        "FiveEData.Data.dnd5e2014.tool-families.json";

    private const string ToolsResource =
        "FiveEData.Data.dnd5e2014.tools.json";

    private const string MountsResource =
        "FiveEData.Data.dnd5e2014.mounts.json";

    private const string VehiclesResource =
        "FiveEData.Data.dnd5e2014.vehicles.json";

    private const string MountSupportResource =
        "FiveEData.Data.dnd5e2014.mount-support.json";

    private const string RulesResource =
        "FiveEData.Data.dnd5e2014.rules.json";

    public static Dnd5e2014Ruleset Load()
    {
        IReadOnlyList<SourceDocument> sources =
            SourceDocumentLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(SourcesResource));

        IReadOnlyList<AmmunitionDefinition> ammunition =
            AmmunitionDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(AmmunitionResource));

        IReadOnlyList<WeaponDefinition> weapons =
            WeaponDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(WeaponsResource));

        IReadOnlyList<ArmorDefinition> armor =
            ArmorDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ArmorResource));

        ArmorUsageRules armorUsage =
            ArmorUsageRulesLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ArmorUsageResource));

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ShieldsResource));

        IReadOnlyList<AdventuringGearDefinition> adventuringGear =
            AdventuringGearDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(AdventuringGearResource));

        IReadOnlyList<ContainerCapacityDefinition> containerCapacities =
            ContainerCapacityDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ContainerCapacitiesResource));

        IReadOnlyList<ToolFamilyDefinition> toolFamilies =
            ToolFamilyDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ToolFamiliesResource));

        IReadOnlyList<ToolDefinition> tools =
            ToolDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ToolsResource));

        IReadOnlyList<MountDefinition> mounts =
            MountDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(MountsResource));

        IReadOnlyList<VehicleDefinition> vehicles =
            VehicleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(VehiclesResource));

        IReadOnlyList<MountSupportDefinition> mountSupport =
            MountSupportDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(MountSupportResource));

        IReadOnlyList<RuleDefinition> rules =
            RuleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(RulesResource));

        var definitions = new RulesetDefinitionSet(
            weapons: weapons,
            sourceDocuments: sources,
            ammunition: ammunition,
            rules: rules,
            armor: armor,
            shields: shields,
            adventuringGear: adventuringGear,
            containerCapacities: containerCapacities,
            toolFamilies: toolFamilies,
            tools: tools,
            mounts: mounts,
            vehicles: vehicles,
            mountSupport: mountSupport,
            armorUsage: armorUsage);

        CatalogIntegrityValidator.EnsureValid(definitions);

        return new Dnd5e2014Ruleset(
            weapons: new WeaponCatalog(weapons),
            ammunition: new AmmunitionCatalog(ammunition),
            armor: new ArmorCatalog(armor),
            shields: new ShieldCatalog(shields),
            adventuringGear: new AdventuringGearCatalog(adventuringGear),
            containerCapacities: new ContainerCapacityCatalog(containerCapacities),
            toolFamilies: new ToolFamilyCatalog(toolFamilies),
            tools: new ToolCatalog(tools),
            mounts: new MountCatalog(mounts),
            vehicles: new VehicleCatalog(vehicles),
            mountSupport: new MountSupportCatalog(mountSupport),
            armorUsage: armorUsage,
            sources: new SourceDocumentCatalog(sources),
            rules: new RuleCatalog(rules));
    }
}
