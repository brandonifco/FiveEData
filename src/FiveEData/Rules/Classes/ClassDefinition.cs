using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes;

public sealed class ClassDefinition
{
    internal ClassDefinition(
        ClassId id,
        string name,
        DiceExpression hitDie,
        IEnumerable<AbilityId> primaryAbilityIds,
        bool requiresAllPrimaryAbilities,
        IEnumerable<AbilityId> savingThrowProficiencyIds,
        IEnumerable<ArmorCategory> armorProficiencyCategories,
        bool proficientWithShields,
        IEnumerable<WeaponProficiencyCategory> weaponProficiencyCategories,
        IEnumerable<WeaponId> weaponProficiencyIds,
        int skillChoiceCount,
        IEnumerable<SkillId> skillChoiceOptionIds,
        IEnumerable<ClassLevelFeature> levelFeatures,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        ExtraAttackProgressionId? extraAttackProgressionId,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(primaryAbilityIds);
        ArgumentNullException.ThrowIfNull(savingThrowProficiencyIds);
        ArgumentNullException.ThrowIfNull(armorProficiencyCategories);
        ArgumentNullException.ThrowIfNull(weaponProficiencyCategories);
        ArgumentNullException.ThrowIfNull(weaponProficiencyIds);
        ArgumentNullException.ThrowIfNull(skillChoiceOptionIds);
        ArgumentNullException.ThrowIfNull(levelFeatures);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        HitDie = hitDie;
        PrimaryAbilityIds = Array.AsReadOnly(primaryAbilityIds.ToArray());
        RequiresAllPrimaryAbilities = requiresAllPrimaryAbilities;
        SavingThrowProficiencyIds =
            Array.AsReadOnly(savingThrowProficiencyIds.ToArray());
        ArmorProficiencyCategories =
            Array.AsReadOnly(armorProficiencyCategories.ToArray());
        ProficientWithShields = proficientWithShields;
        WeaponProficiencyCategories =
            Array.AsReadOnly(weaponProficiencyCategories.ToArray());
        WeaponProficiencyIds = Array.AsReadOnly(weaponProficiencyIds.ToArray());
        SkillChoiceCount = skillChoiceCount;
        SkillChoiceOptionIds = Array.AsReadOnly(skillChoiceOptionIds.ToArray());
        LevelFeatures = Array.AsReadOnly(levelFeatures.ToArray());
        SpellSlotProgressionId = spellSlotProgressionId;
        SpellcastingAbilityId = spellcastingAbilityId;
        ExtraAttackProgressionId = extraAttackProgressionId;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ClassId Id { get; }
    public string Name { get; }
    public DiceExpression HitDie { get; }
    public IReadOnlyList<AbilityId> PrimaryAbilityIds { get; }
    public bool RequiresAllPrimaryAbilities { get; }
    public IReadOnlyList<AbilityId> SavingThrowProficiencyIds { get; }
    public IReadOnlyList<ArmorCategory> ArmorProficiencyCategories { get; }
    public bool ProficientWithShields { get; }
    public IReadOnlyList<WeaponProficiencyCategory> WeaponProficiencyCategories { get; }
    public IReadOnlyList<WeaponId> WeaponProficiencyIds { get; }
    public int SkillChoiceCount { get; }
    public IReadOnlyList<SkillId> SkillChoiceOptionIds { get; }
    public IReadOnlyList<ClassLevelFeature> LevelFeatures { get; }
    public SpellSlotProgressionId? SpellSlotProgressionId { get; }
    public AbilityId? SpellcastingAbilityId { get; }
    public ExtraAttackProgressionId? ExtraAttackProgressionId { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
