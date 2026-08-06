using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FightingStyles;
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
        IReadOnlyList<SpellSlotProgressionDefinition> spellSlotProgressions,
        IReadOnlyList<ExtraAttackProgressionDefinition>
            extraAttackProgressions,
        IReadOnlyList<BackgroundDefinition> backgrounds)
    {
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(equipment);
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(creatureVocabulary);
        ArgumentNullException.ThrowIfNull(races);
        ArgumentNullException.ThrowIfNull(classes);
        ArgumentNullException.ThrowIfNull(fightingStyles);
        ArgumentNullException.ThrowIfNull(spellSlotProgressions);
        ArgumentNullException.ThrowIfNull(extraAttackProgressions);
        ArgumentNullException.ThrowIfNull(backgrounds);

        SourceDocuments = sourceDocuments;
        Rules = rules;
        Equipment = equipment;
        Expenses = expenses;
        CreatureVocabulary = creatureVocabulary;
        Races = races;
        Classes = classes;
        FightingStyles = fightingStyles;
        SpellSlotProgressions = spellSlotProgressions;
        ExtraAttackProgressions = extraAttackProgressions;
        Backgrounds = backgrounds;
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

    public IReadOnlyList<SpellSlotProgressionDefinition>
        SpellSlotProgressions
    { get; }

    public IReadOnlyList<ExtraAttackProgressionDefinition>
        ExtraAttackProgressions
    { get; }

    public IReadOnlyList<BackgroundDefinition> Backgrounds { get; }
}
