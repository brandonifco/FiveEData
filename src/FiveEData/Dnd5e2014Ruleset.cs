using System.Threading;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Expenses;

namespace FiveEData;

public sealed class Dnd5e2014Ruleset
{
    private static readonly Lazy<Dnd5e2014Ruleset> LazyInstance =
        new(
            Dnd5e2014RulesetLoader.Load,
            LazyThreadSafetyMode.ExecutionAndPublication);

    internal Dnd5e2014Ruleset(
        WeaponCatalog weapons,
        AmmunitionCatalog ammunition,
        ArmorCatalog armor,
        ShieldCatalog shields,
        AdventuringGearCatalog adventuringGear,
        ContainerCapacityCatalog containerCapacities,
        ToolFamilyCatalog toolFamilies,
        ToolCatalog tools,
        MountCatalog mounts,
        VehicleCatalog vehicles,
        MountSupportCatalog mountSupport,
        MountVehicleRules mountVehicleRules,
        TradeGoodCatalog tradeGoods,
        ArmorUsageRules armorUsage,
        ExpenseCatalogs expenses,
        CreatureVocabularyCatalogs creatureVocabulary,
        RaceCatalog races,
        SubraceCatalog subraces,
        ClassCatalog classes,
        SubclassCatalog subclasses,
        FightingStyleCatalog fightingStyles,
        MetamagicOptionCatalog metamagicOptions,
        BattleMasterManeuverCatalog battleMasterManeuvers,
        EldritchInvocationCatalog eldritchInvocations,
        ElementalDisciplineCatalog elementalDisciplines,
        ChannelDivinityOptionCatalog channelDivinityOptions,
        TotemWarriorOptionCatalog totemWarriorOptions,
        HunterOptionCatalog hunterOptions,
        SpellSlotProgressionCatalog spellSlotProgressions,
        ExtraAttackProgressionCatalog extraAttackProgressions,
        BackgroundCatalog backgrounds,
        MagicSchoolCatalog magicSchools,
        SpellCatalog spells,
        CombatActionCatalog combatActions,
        CoverCatalog cover,
        TravelPaceCatalog travelPaces,
        RestTypeCatalog restTypes,
        DowntimeActivityCatalog downtimeActivities,
        SourceDocumentCatalog sources,
        RuleCatalog rules)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(armor);
        ArgumentNullException.ThrowIfNull(shields);
        ArgumentNullException.ThrowIfNull(adventuringGear);
        ArgumentNullException.ThrowIfNull(containerCapacities);
        ArgumentNullException.ThrowIfNull(toolFamilies);
        ArgumentNullException.ThrowIfNull(tools);
        ArgumentNullException.ThrowIfNull(mounts);
        ArgumentNullException.ThrowIfNull(vehicles);
        ArgumentNullException.ThrowIfNull(mountSupport);
        ArgumentNullException.ThrowIfNull(mountVehicleRules);
        ArgumentNullException.ThrowIfNull(tradeGoods);
        ArgumentNullException.ThrowIfNull(armorUsage);
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(creatureVocabulary);
        ArgumentNullException.ThrowIfNull(races);
        ArgumentNullException.ThrowIfNull(subraces);
        ArgumentNullException.ThrowIfNull(classes);
        ArgumentNullException.ThrowIfNull(subclasses);
        ArgumentNullException.ThrowIfNull(fightingStyles);
        ArgumentNullException.ThrowIfNull(metamagicOptions);
        ArgumentNullException.ThrowIfNull(battleMasterManeuvers);
        ArgumentNullException.ThrowIfNull(eldritchInvocations);
        ArgumentNullException.ThrowIfNull(elementalDisciplines);
        ArgumentNullException.ThrowIfNull(channelDivinityOptions);
        ArgumentNullException.ThrowIfNull(totemWarriorOptions);
        ArgumentNullException.ThrowIfNull(hunterOptions);
        ArgumentNullException.ThrowIfNull(spellSlotProgressions);
        ArgumentNullException.ThrowIfNull(extraAttackProgressions);
        ArgumentNullException.ThrowIfNull(backgrounds);
        ArgumentNullException.ThrowIfNull(magicSchools);
        ArgumentNullException.ThrowIfNull(spells);
        ArgumentNullException.ThrowIfNull(combatActions);
        ArgumentNullException.ThrowIfNull(cover);
        ArgumentNullException.ThrowIfNull(travelPaces);
        ArgumentNullException.ThrowIfNull(restTypes);
        ArgumentNullException.ThrowIfNull(downtimeActivities);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(rules);

        Weapons = weapons;
        Ammunition = ammunition;
        Armor = armor;
        Shields = shields;
        AdventuringGear = adventuringGear;
        ContainerCapacities = containerCapacities;
        ToolFamilies = toolFamilies;
        Tools = tools;
        Mounts = mounts;
        Vehicles = vehicles;
        MountSupport = mountSupport;
        MountVehicleRules = mountVehicleRules;
        TradeGoods = tradeGoods;
        ArmorUsage = armorUsage;
        Expenses = expenses;
        CreatureVocabulary = creatureVocabulary;
        Races = races;
        Subraces = subraces;
        Classes = classes;
        Subclasses = subclasses;
        FightingStyles = fightingStyles;
        MetamagicOptions = metamagicOptions;
        BattleMasterManeuvers = battleMasterManeuvers;
        EldritchInvocations = eldritchInvocations;
        ElementalDisciplines = elementalDisciplines;
        ChannelDivinityOptions = channelDivinityOptions;
        TotemWarriorOptions = totemWarriorOptions;
        HunterOptions = hunterOptions;
        SpellSlotProgressions = spellSlotProgressions;
        ExtraAttackProgressions = extraAttackProgressions;
        Backgrounds = backgrounds;
        MagicSchools = magicSchools;
        Spells = spells;
        CombatActions = combatActions;
        Cover = cover;
        TravelPaces = travelPaces;
        RestTypes = restTypes;
        DowntimeActivities = downtimeActivities;
        Sources = sources;
        Rules = rules;
    }

    public static Dnd5e2014Ruleset Instance => LazyInstance.Value;

    public WeaponCatalog Weapons { get; }
    public AmmunitionCatalog Ammunition { get; }
    public ArmorCatalog Armor { get; }
    public ShieldCatalog Shields { get; }
    public AdventuringGearCatalog AdventuringGear { get; }
    public ContainerCapacityCatalog ContainerCapacities { get; }
    public ToolFamilyCatalog ToolFamilies { get; }
    public ToolCatalog Tools { get; }
    public MountCatalog Mounts { get; }
    public VehicleCatalog Vehicles { get; }
    public MountSupportCatalog MountSupport { get; }
    public MountVehicleRules MountVehicleRules { get; }
    public TradeGoodCatalog TradeGoods { get; }
    public ArmorUsageRules ArmorUsage { get; }
    public ExpenseCatalogs Expenses { get; }

    public CreatureVocabularyCatalogs CreatureVocabulary
    {
        get;
    }

    public RaceCatalog Races { get; }
    public SubraceCatalog Subraces { get; }
    public ClassCatalog Classes { get; }
    public SubclassCatalog Subclasses { get; }
    public FightingStyleCatalog FightingStyles { get; }
    public MetamagicOptionCatalog MetamagicOptions { get; }
    public BattleMasterManeuverCatalog BattleMasterManeuvers { get; }
    public EldritchInvocationCatalog EldritchInvocations { get; }
    public ElementalDisciplineCatalog ElementalDisciplines { get; }
    public ChannelDivinityOptionCatalog ChannelDivinityOptions { get; }
    public TotemWarriorOptionCatalog TotemWarriorOptions { get; }
    public HunterOptionCatalog HunterOptions { get; }
    public SpellSlotProgressionCatalog SpellSlotProgressions { get; }
    public ExtraAttackProgressionCatalog ExtraAttackProgressions { get; }
    public BackgroundCatalog Backgrounds { get; }
    public MagicSchoolCatalog MagicSchools { get; }
    public SpellCatalog Spells { get; }
    public CombatActionCatalog CombatActions { get; }
    public CoverCatalog Cover { get; }
    public TravelPaceCatalog TravelPaces { get; }
    public RestTypeCatalog RestTypes { get; }
    public DowntimeActivityCatalog DowntimeActivities { get; }
    public SourceDocumentCatalog Sources { get; }
    public RuleCatalog Rules { get; }
}
