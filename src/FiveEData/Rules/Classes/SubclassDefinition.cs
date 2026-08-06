using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes;

public sealed class SubclassDefinition
{
    internal SubclassDefinition(
        SubclassId id,
        string name,
        ClassId classId,
        int chosenAtLevel,
        IEnumerable<ClassLevelFeature> levelFeatures,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        DivineStrikeProgressionDetail? divineStrikeProgression,
        CircleFormsProgressionDetail? circleFormsProgression,
        AuraOfDevotionDetail? auraOfDevotion,
        AuraOfWardingDetail? auraOfWarding,
        CombatSuperiorityProgressionDetail? combatSuperiorityProgression,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(levelFeatures);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        ClassId = classId;
        ChosenAtLevel = chosenAtLevel;
        LevelFeatures = Array.AsReadOnly(levelFeatures.ToArray());
        SpellSlotProgressionId = spellSlotProgressionId;
        SpellcastingAbilityId = spellcastingAbilityId;
        DivineStrikeProgression = divineStrikeProgression;
        CircleFormsProgression = circleFormsProgression;
        AuraOfDevotion = auraOfDevotion;
        AuraOfWarding = auraOfWarding;
        CombatSuperiorityProgression = combatSuperiorityProgression;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SubclassId Id { get; }
    public string Name { get; }
    public ClassId ClassId { get; }
    public int ChosenAtLevel { get; }
    public IReadOnlyList<ClassLevelFeature> LevelFeatures { get; }
    public SpellSlotProgressionId? SpellSlotProgressionId { get; }
    public AbilityId? SpellcastingAbilityId { get; }
    public DivineStrikeProgressionDetail? DivineStrikeProgression { get; }
    public CircleFormsProgressionDetail? CircleFormsProgression { get; }
    public AuraOfDevotionDetail? AuraOfDevotion { get; }
    public AuraOfWardingDetail? AuraOfWarding { get; }

    public CombatSuperiorityProgressionDetail? CombatSuperiorityProgression
    { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
