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
        RaceDefinitionSet races)
    {
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(equipment);
        ArgumentNullException.ThrowIfNull(expenses);
        ArgumentNullException.ThrowIfNull(creatureVocabulary);
        ArgumentNullException.ThrowIfNull(races);

        SourceDocuments = sourceDocuments;
        Rules = rules;
        Equipment = equipment;
        Expenses = expenses;
        CreatureVocabulary = creatureVocabulary;
        Races = races;
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
}
