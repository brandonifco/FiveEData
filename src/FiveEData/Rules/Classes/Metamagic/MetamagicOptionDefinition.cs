using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.Metamagic;

public sealed class MetamagicOptionDefinition
{
    internal MetamagicOptionDefinition(
        MetamagicOptionId id,
        string name,
        int? fixedSorceryPointCost,
        bool costEqualsSpellLevelWithCantripMinimum,
        bool protectsCreatureCountUpToSpellcastingModifier,
        bool doublesRange,
        int? touchRangeBecomesFeet,
        bool rerollsDiceCountUpToSpellcastingModifier,
        int? doublesDurationMaxHours,
        bool grantsDisadvantageOnFirstSavingThrow,
        bool changesCastingTimeToBonusAction,
        bool removesVerbalAndSomaticComponents,
        bool targetsSecondCreatureInRange,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        FixedSorceryPointCost = fixedSorceryPointCost;
        CostEqualsSpellLevelWithCantripMinimum =
            costEqualsSpellLevelWithCantripMinimum;
        ProtectsCreatureCountUpToSpellcastingModifier =
            protectsCreatureCountUpToSpellcastingModifier;
        DoublesRange = doublesRange;
        TouchRangeBecomesFeet = touchRangeBecomesFeet;
        RerollsDiceCountUpToSpellcastingModifier =
            rerollsDiceCountUpToSpellcastingModifier;
        DoublesDurationMaxHours = doublesDurationMaxHours;
        GrantsDisadvantageOnFirstSavingThrow =
            grantsDisadvantageOnFirstSavingThrow;
        ChangesCastingTimeToBonusAction = changesCastingTimeToBonusAction;
        RemovesVerbalAndSomaticComponents =
            removesVerbalAndSomaticComponents;
        TargetsSecondCreatureInRange = targetsSecondCreatureInRange;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public MetamagicOptionId Id { get; }
    public string Name { get; }
    public int? FixedSorceryPointCost { get; }
    public bool CostEqualsSpellLevelWithCantripMinimum { get; }
    public bool ProtectsCreatureCountUpToSpellcastingModifier { get; }
    public bool DoublesRange { get; }
    public int? TouchRangeBecomesFeet { get; }
    public bool RerollsDiceCountUpToSpellcastingModifier { get; }
    public int? DoublesDurationMaxHours { get; }
    public bool GrantsDisadvantageOnFirstSavingThrow { get; }
    public bool ChangesCastingTimeToBonusAction { get; }
    public bool RemovesVerbalAndSomaticComponents { get; }
    public bool TargetsSecondCreatureInRange { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
