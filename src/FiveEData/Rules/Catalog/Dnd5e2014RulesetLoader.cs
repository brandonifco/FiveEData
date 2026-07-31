using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;
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

    private const string ShieldsResource =
        "FiveEData.Data.dnd5e2014.shields.json";

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

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ShieldsResource));

        IReadOnlyList<RuleDefinition> rules =
            RuleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(RulesResource));

        CatalogIntegrityValidator.EnsureValid(
            weapons,
            sources,
            ammunition,
            rules,
            armor,
            shields);

        return new Dnd5e2014Ruleset(
            new WeaponCatalog(weapons),
            new AmmunitionCatalog(ammunition),
            new ArmorCatalog(armor),
            new ShieldCatalog(shields),
            new SourceDocumentCatalog(sources),
            new RuleCatalog(rules));
    }
}
