using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Catalog;

internal sealed class RulesetDefinitionSet
{
    public RulesetDefinitionSet(
        IReadOnlyList<SourceDocument> sourceDocuments,
        IReadOnlyList<RuleDefinition> rules,
        EquipmentDefinitionSet equipment,
        ExpenseDefinitionSet expenses,
        CreatureVocabularyDefinitionSet creatureVocabulary,
        RaceDefinitionSet races,
        ClassDefinitionSet classes,
        IReadOnlyList<FightingStyleDefinition> fightingStyles,
        IReadOnlyList<MetamagicOptionDefinition> metamagicOptions,
        IReadOnlyList<BattleMasterManeuverDefinition> battleMasterManeuvers,
        IReadOnlyList<EldritchInvocationDefinition> eldritchInvocations,
        IReadOnlyList<ElementalDisciplineDefinition> elementalDisciplines,
        IReadOnlyList<ChannelDivinityOptionDefinition>
            channelDivinityOptions,
        IReadOnlyList<SpellSlotProgressionDefinition> spellSlotProgressions,
        IReadOnlyList<ExtraAttackProgressionDefinition>
            extraAttackProgressions,
        IReadOnlyList<BackgroundDefinition> backgrounds,
        IReadOnlyList<MagicSchoolDefinition> magicSchools,
        IReadOnlyList<SpellDefinition> spells,
        IReadOnlyList<CombatActionDefinition> combatActions,
        IReadOnlyList<CoverDefinition> cover,
        IReadOnlyList<TravelPaceDefinition> travelPaces,
        IReadOnlyList<RestTypeDefinition> restTypes,
        IReadOnlyList<DowntimeActivityDefinition> downtimeActivities)
    {
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(equipment);
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(creatureVocabulary);
        ArgumentNullException.ThrowIfNull(races);
        ArgumentNullException.ThrowIfNull(classes);
        ArgumentNullException.ThrowIfNull(fightingStyles);
        ArgumentNullException.ThrowIfNull(metamagicOptions);
        ArgumentNullException.ThrowIfNull(battleMasterManeuvers);
        ArgumentNullException.ThrowIfNull(eldritchInvocations);
        ArgumentNullException.ThrowIfNull(elementalDisciplines);
        ArgumentNullException.ThrowIfNull(channelDivinityOptions);
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

        SourceDocuments = sourceDocuments;
        Rules = rules;
        Equipment = equipment;
        Expenses = expenses;
        CreatureVocabulary = creatureVocabulary;
        Races = races;
        Classes = classes;
        FightingStyles = fightingStyles;
        MetamagicOptions = metamagicOptions;
        BattleMasterManeuvers = battleMasterManeuvers;
        EldritchInvocations = eldritchInvocations;
        ElementalDisciplines = elementalDisciplines;
        ChannelDivinityOptions = channelDivinityOptions;
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
    }

    public IReadOnlyList<SourceDocument> SourceDocuments { get; }
    public IReadOnlyList<RuleDefinition> Rules { get; }
    public EquipmentDefinitionSet Equipment { get; }
    public ExpenseDefinitionSet Expenses { get; }

    public CreatureVocabularyDefinitionSet CreatureVocabulary
    {
        get;
    }

    public RaceDefinitionSet Races { get; }
    public ClassDefinitionSet Classes { get; }
    public IReadOnlyList<FightingStyleDefinition> FightingStyles { get; }
    public IReadOnlyList<MetamagicOptionDefinition> MetamagicOptions { get; }

    public IReadOnlyList<BattleMasterManeuverDefinition>
        BattleMasterManeuvers
    { get; }

    public IReadOnlyList<EldritchInvocationDefinition> EldritchInvocations
    { get; }

    public IReadOnlyList<ElementalDisciplineDefinition> ElementalDisciplines
    { get; }

    public IReadOnlyList<ChannelDivinityOptionDefinition>
        ChannelDivinityOptions
    { get; }

    public IReadOnlyList<SpellSlotProgressionDefinition>
        SpellSlotProgressions
    { get; }

    public IReadOnlyList<ExtraAttackProgressionDefinition>
        ExtraAttackProgressions
    { get; }

    public IReadOnlyList<BackgroundDefinition> Backgrounds { get; }
    public IReadOnlyList<MagicSchoolDefinition> MagicSchools { get; }
    public IReadOnlyList<SpellDefinition> Spells { get; }
    public IReadOnlyList<CombatActionDefinition> CombatActions { get; }
    public IReadOnlyList<CoverDefinition> Cover { get; }
    public IReadOnlyList<TravelPaceDefinition> TravelPaces { get; }
    public IReadOnlyList<RestTypeDefinition> RestTypes { get; }

    public IReadOnlyList<DowntimeActivityDefinition> DowntimeActivities
    { get; }
}
