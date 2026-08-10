using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.ElementalDisciplines;

public sealed class ElementalDisciplineDefinition
{
    internal ElementalDisciplineDefinition(
        ElementalDisciplineId id,
        string name,
        int? kiPointCost,
        int? requiredMinimumLevel,
        SpellId? grantedSpellId,
        AbilityId? savingThrowAbilityId,
        DiceExpression? baseDamage,
        DamageTypeId? baseDamageTypeId,
        bool halfDamageOnSuccessfulSave,
        int? rangeFeet,
        int? pushDistanceFeet,
        ConditionId? imposedConditionId,
        int? reachIncreaseFeet,
        DamageTypeId? changesUnarmedDamageTypeId,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        KiPointCost = kiPointCost;
        RequiredMinimumLevel = requiredMinimumLevel;
        GrantedSpellId = grantedSpellId;
        SavingThrowAbilityId = savingThrowAbilityId;
        BaseDamage = baseDamage;
        BaseDamageTypeId = baseDamageTypeId;
        HalfDamageOnSuccessfulSave = halfDamageOnSuccessfulSave;
        RangeFeet = rangeFeet;
        PushDistanceFeet = pushDistanceFeet;
        ImposedConditionId = imposedConditionId;
        ReachIncreaseFeet = reachIncreaseFeet;
        ChangesUnarmedDamageTypeId = changesUnarmedDamageTypeId;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ElementalDisciplineId Id { get; }
    public string Name { get; }
    public int? KiPointCost { get; }
    public int? RequiredMinimumLevel { get; }
    public SpellId? GrantedSpellId { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public DiceExpression? BaseDamage { get; }
    public DamageTypeId? BaseDamageTypeId { get; }
    public bool HalfDamageOnSuccessfulSave { get; }
    public int? RangeFeet { get; }
    public int? PushDistanceFeet { get; }
    public ConditionId? ImposedConditionId { get; }
    public int? ReachIncreaseFeet { get; }
    public DamageTypeId? ChangesUnarmedDamageTypeId { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
