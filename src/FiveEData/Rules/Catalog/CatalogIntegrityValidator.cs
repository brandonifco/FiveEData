using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Classes;
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
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Expenses;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Rules.Catalog;

internal static class CatalogIntegrityValidator
{
    private static readonly VehicleId KeelboatVehicleId =
        new("dnd5e2014.vehicle.keelboat");
    private static readonly VehicleId RowboatVehicleId =
        new("dnd5e2014.vehicle.rowboat");
    private static readonly MountSupportId ExoticSaddleMountSupportId =
        new("dnd5e2014.mount-support.saddle-exotic");
    private static readonly MountSupportId MilitarySaddleMountSupportId =
        new("dnd5e2014.mount-support.saddle-military");
    private static readonly RuleId TradeGoodsFullValueAndCurrencyRuleId =
        new("dnd5e2014.trade-good-rule.full-value-and-currency");
    private static readonly RuleId
        FoodDrinkLodgingIncludedInLifestyleRuleId =
            new(
                "dnd5e2014.expense-rule." +
                "food-drink-lodging-included-in-lifestyle");

    public static IReadOnlyList<string> Validate(
        RulesetDefinitionSet definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        HashSet<SourceDocumentId> sourceIds =
            definitions.SourceDocuments
                .Select(source => source.Id)
                .ToHashSet();

        errors.AddRange(
            CreatureVocabularyCatalogIntegrityValidator.Validate(
                definitions.CreatureVocabulary,
                sourceIds));

        HashSet<LifestyleId> lifestyleIds =
            definitions.Expenses.Lifestyles
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<AmmunitionTypeId> ammunitionIds =
            definitions.Equipment.Ammunition
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<DamageTypeId> damageTypeIds =
            definitions.CreatureVocabulary.DamageTypes
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<ConditionId> conditionIds =
            definitions.CreatureVocabulary.Conditions
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<RuleId> ruleIds =
            definitions.Rules
                .Select(rule => rule.Id)
                .ToHashSet();

        HashSet<AbilityId> abilityIds =
            definitions.CreatureVocabulary.Abilities
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<CreatureSizeId> sizeIds =
            definitions.CreatureVocabulary.Sizes
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<LanguageId> languageIds =
            definitions.CreatureVocabulary.Languages
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<SkillId> skillIds =
            definitions.CreatureVocabulary.Skills
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<WeaponId> weaponIds =
            definitions.Equipment.Weapons
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<SpellId> spellIds =
            definitions.Spells
                .Select(definition => definition.Id)
                .ToHashSet();

        errors.AddRange(
            RaceCatalogIntegrityValidator.Validate(
                definitions.Races,
                sourceIds,
                abilityIds,
                sizeIds,
                languageIds,
                ruleIds,
                damageTypeIds,
                weaponIds,
                skillIds,
                spellIds));

        HashSet<SpellSlotProgressionId> spellSlotProgressionIds =
            definitions.SpellSlotProgressions
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<ExtraAttackProgressionId> extraAttackProgressionIds =
            definitions.ExtraAttackProgressions
                .Select(definition => definition.Id)
                .ToHashSet();

        errors.AddRange(
            ClassCatalogIntegrityValidator.Validate(
                definitions.Classes,
                sourceIds,
                abilityIds,
                skillIds,
                weaponIds,
                ruleIds,
                spellSlotProgressionIds,
                extraAttackProgressionIds,
                damageTypeIds,
                sizeIds,
                spellIds,
                conditionIds));

        foreach (
            SpellSlotProgressionDefinition spellSlotProgression
            in definitions.SpellSlotProgressions)
        {
            ValidateSources(
                $"Spell slot progression '{spellSlotProgression.Id}'",
                spellSlotProgression.Sources,
                sourceIds,
                errors);
        }

        foreach (
            ExtraAttackProgressionDefinition extraAttackProgression
            in definitions.ExtraAttackProgressions)
        {
            ValidateSources(
                $"Extra Attack progression '{extraAttackProgression.Id}'",
                extraAttackProgression.Sources,
                sourceIds,
                errors);
        }

        HashSet<ClassId> classIds =
            definitions.Classes.Classes
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (
            FightingStyleDefinition fightingStyle
            in definitions.FightingStyles)
        {
            string owner = $"Fighting style '{fightingStyle.Id}'";

            ValidateSources(
                owner,
                fightingStyle.Sources,
                sourceIds,
                errors);

            foreach (ClassId classId in fightingStyle.AvailableToClassIds)
            {
                if (!classIds.Contains(classId))
                {
                    errors.Add(
                        $"{owner} references missing class '{classId}'.");
                }
            }
        }

        foreach (
            MagicSchoolDefinition magicSchool
            in definitions.MagicSchools)
        {
            ValidateSources(
                $"Magic school '{magicSchool.Id}'",
                magicSchool.Sources,
                sourceIds,
                errors);
        }

        var magicSchoolIds = definitions.MagicSchools
            .Select(magicSchool => magicSchool.Id)
            .ToHashSet();

        foreach (SpellDefinition spell in definitions.Spells)
        {
            string owner = $"Spell '{spell.Id}'";

            ValidateSources(owner, spell.Sources, sourceIds, errors);

            if (!magicSchoolIds.Contains(spell.SchoolId))
            {
                errors.Add(
                    $"{owner} references missing magic school " +
                    $"'{spell.SchoolId}'.");
            }

            foreach (ClassId classId in spell.AvailableToClassIds)
            {
                if (!classIds.Contains(classId))
                {
                    errors.Add(
                        $"{owner} references missing class '{classId}'.");
                }
            }

            if (spell.DamageEffect is { } damageEffect)
            {
                if (damageEffect.DamageTypeId is { } damageTypeId &&
                    !damageTypeIds.Contains(damageTypeId))
                {
                    errors.Add(
                        $"{owner} references missing damage type " +
                        $"'{damageTypeId}'.");
                }

                foreach (
                    DamageTypeId choosableDamageTypeId
                    in damageEffect.ChoosableDamageTypeIds
                        ?? Array.Empty<DamageTypeId>())
                {
                    if (!damageTypeIds.Contains(choosableDamageTypeId))
                    {
                        errors.Add(
                            $"{owner} references missing damage type " +
                            $"'{choosableDamageTypeId}'.");
                    }
                }

                if (damageEffect.SavingThrowAbilityId is
                        { } damageSavingThrowAbilityId &&
                    !abilityIds.Contains(damageSavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{damageSavingThrowAbilityId}'.");
                }
            }

            if (spell.ConditionEffect is { } conditionEffect)
            {
                foreach (ConditionId conditionId in conditionEffect.ConditionIds)
                {
                    if (!conditionIds.Contains(conditionId))
                    {
                        errors.Add(
                            $"{owner} references missing condition " +
                            $"'{conditionId}'.");
                    }
                }

                if (!abilityIds.Contains(
                        conditionEffect.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{conditionEffect.SavingThrowAbilityId}'.");
                }
            }
        }

        foreach (
            MetamagicOptionDefinition metamagicOption
            in definitions.MetamagicOptions)
        {
            ValidateSources(
                $"Metamagic option '{metamagicOption.Id}'",
                metamagicOption.Sources,
                sourceIds,
                errors);
        }

        foreach (
            BattleMasterManeuverDefinition battleMasterManeuver
            in definitions.BattleMasterManeuvers)
        {
            string owner = $"Battle Master maneuver '{battleMasterManeuver.Id}'";

            ValidateSources(
                owner,
                battleMasterManeuver.Sources,
                sourceIds,
                errors);

            if (battleMasterManeuver.SavingThrowAbilityId is
                    { } savingThrowAbilityId &&
                !abilityIds.Contains(savingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{savingThrowAbilityId}'.");
            }

            if (battleMasterManeuver.ImposedConditionId is
                    { } imposedConditionId &&
                !conditionIds.Contains(imposedConditionId))
            {
                errors.Add(
                    $"{owner} references missing condition " +
                    $"'{imposedConditionId}'.");
            }

            if (battleMasterManeuver.MaximumTargetSizeId is
                    { } maximumTargetSizeId &&
                !sizeIds.Contains(maximumTargetSizeId))
            {
                errors.Add(
                    $"{owner} references missing creature size " +
                    $"'{maximumTargetSizeId}'.");
            }
        }

        foreach (
            EldritchInvocationDefinition eldritchInvocation
            in definitions.EldritchInvocations)
        {
            string owner = $"Eldritch invocation '{eldritchInvocation.Id}'";

            ValidateSources(
                owner,
                eldritchInvocation.Sources,
                sourceIds,
                errors);

            if (eldritchInvocation.GrantedSpellId is { } grantedSpellId &&
                !spellIds.Contains(grantedSpellId))
            {
                errors.Add(
                    $"{owner} references missing spell '{grantedSpellId}'.");
            }

            if (eldritchInvocation.ExtraDamageTypeId is
                    { } extraDamageTypeId &&
                !damageTypeIds.Contains(extraDamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{extraDamageTypeId}'.");
            }

            foreach (
                SkillId skillId
                in eldritchInvocation.SkillProficiencyIds)
            {
                if (!skillIds.Contains(skillId))
                {
                    errors.Add(
                        $"{owner} references missing skill '{skillId}'.");
                }
            }
        }

        foreach (
            ElementalDisciplineDefinition elementalDiscipline
            in definitions.ElementalDisciplines)
        {
            string owner =
                $"Elemental discipline '{elementalDiscipline.Id}'";

            ValidateSources(
                owner,
                elementalDiscipline.Sources,
                sourceIds,
                errors);

            if (elementalDiscipline.GrantedSpellId is
                    { } disciplineGrantedSpellId &&
                !spellIds.Contains(disciplineGrantedSpellId))
            {
                errors.Add(
                    $"{owner} references missing spell " +
                    $"'{disciplineGrantedSpellId}'.");
            }

            if (elementalDiscipline.SavingThrowAbilityId is
                    { } disciplineSavingThrowAbilityId &&
                !abilityIds.Contains(disciplineSavingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{disciplineSavingThrowAbilityId}'.");
            }

            if (elementalDiscipline.BaseDamageTypeId is
                    { } disciplineBaseDamageTypeId &&
                !damageTypeIds.Contains(disciplineBaseDamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{disciplineBaseDamageTypeId}'.");
            }

            if (elementalDiscipline.ImposedConditionId is
                    { } disciplineImposedConditionId &&
                !conditionIds.Contains(disciplineImposedConditionId))
            {
                errors.Add(
                    $"{owner} references missing condition " +
                    $"'{disciplineImposedConditionId}'.");
            }

            if (elementalDiscipline.ChangesUnarmedDamageTypeId is
                    { } disciplineChangesUnarmedDamageTypeId &&
                !damageTypeIds.Contains(
                    disciplineChangesUnarmedDamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{disciplineChangesUnarmedDamageTypeId}'.");
            }
        }

        foreach (
            ChannelDivinityOptionDefinition channelDivinityOption
            in definitions.ChannelDivinityOptions)
        {
            string owner =
                $"Channel Divinity option '{channelDivinityOption.Id}'";

            ValidateSources(
                owner,
                channelDivinityOption.Sources,
                sourceIds,
                errors);

            if (channelDivinityOption.SavingThrowAbilityId is
                    { } savingThrowAbilityId &&
                !abilityIds.Contains(savingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{savingThrowAbilityId}'.");
            }

            if (channelDivinityOption.ImposedConditionId is
                    { } channelDivinityImposedConditionId &&
                !conditionIds.Contains(channelDivinityImposedConditionId))
            {
                errors.Add(
                    $"{owner} references missing condition " +
                    $"'{channelDivinityImposedConditionId}'.");
            }

            if (channelDivinityOption.GrantedSpellId is
                    { } channelDivinityGrantedSpellId &&
                !spellIds.Contains(channelDivinityGrantedSpellId))
            {
                errors.Add(
                    $"{owner} references missing spell " +
                    $"'{channelDivinityGrantedSpellId}'.");
            }

            foreach (
                AbilityId choosableSavingThrowAbilityId
                in channelDivinityOption.ChoosableSavingThrowAbilityIds)
            {
                if (!abilityIds.Contains(choosableSavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{choosableSavingThrowAbilityId}'.");
                }
            }
        }

        errors.AddRange(
            BackgroundCatalogIntegrityValidator.Validate(
                definitions.Backgrounds,
                sourceIds,
                skillIds,
                ruleIds,
                lifestyleIds));

        HashSet<AdventuringGearId> adventuringGearIds =
            definitions.Equipment.AdventuringGear
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<ToolFamilyId> toolFamilyIds =
            definitions.Equipment.ToolFamilies
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<VehicleId> vehicleIds =
            definitions.Equipment.Vehicles
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (WeaponDefinition weapon in definitions.Equipment.Weapons)
        {
            ValidateSources(
                $"Weapon '{weapon.Id}'",
                weapon.Sources,
                sourceIds,
                errors);

            if (weapon.AmmunitionTypeId is { } ammunitionTypeId &&
                !ammunitionIds.Contains(ammunitionTypeId))
            {
                errors.Add(
                    $"Weapon '{weapon.Id}' references missing ammunition type '{ammunitionTypeId}'.");
            }

            if (weapon.Damage is { } damage &&
                !damageTypeIds.Contains(damage.DamageTypeId))
            {
                errors.Add(
                    $"Weapon '{weapon.Id}' references missing damage type '{damage.DamageTypeId}'.");
            }

            foreach (RuleId specialRuleId in weapon.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Weapon '{weapon.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (AmmunitionDefinition definition in definitions.Equipment.Ammunition)
        {
            ValidateSources(
                $"Ammunition '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (RuleDefinition rule in definitions.Rules)
        {
            ValidateSources(
                $"Rule '{rule.Id}'",
                rule.Sources,
                sourceIds,
                errors);
        }

        foreach (ArmorDefinition definition in definitions.Equipment.Armor)
        {
            ValidateSources(
                $"Armor '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (ShieldDefinition definition in definitions.Equipment.Shields)
        {
            ValidateSources(
                $"Shield '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (AdventuringGearDefinition definition in definitions.Equipment.AdventuringGear)
        {
            ValidateSources(
                $"Adventuring gear '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Adventuring gear '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (ContainerCapacityDefinition definition in definitions.Equipment.ContainerCapacities)
        {
            ValidateSources(
                $"Container capacity for adventuring gear '{definition.AdventuringGearId}'",
                definition.Sources,
                sourceIds,
                errors);

            if (!adventuringGearIds.Contains(definition.AdventuringGearId))
            {
                errors.Add(
                    $"Container capacity references missing adventuring gear '{definition.AdventuringGearId}'.");
            }
        }

        foreach (ToolFamilyDefinition definition in definitions.Equipment.ToolFamilies)
        {
            ValidateSources(
                $"Tool family '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Tool family '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (ToolDefinition definition in definitions.Equipment.Tools)
        {
            ValidateSources(
                $"Tool '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            if (definition.FamilyId is { } familyId &&
                !toolFamilyIds.Contains(familyId))
            {
                errors.Add(
                    $"Tool '{definition.Id}' references missing tool family '{familyId}'.");
            }

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Tool '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (MountDefinition definition in definitions.Equipment.Mounts)
        {
            ValidateSources(
                $"Mount '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Mount '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (VehicleDefinition definition in definitions.Equipment.Vehicles)
        {
            ValidateSources(
                $"Vehicle '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Vehicle '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (MountSupportDefinition definition in definitions.Equipment.MountSupport)
        {
            ValidateSources(
                $"Mount support '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Mount support '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (TradeGoodDefinition definition in definitions.Equipment.TradeGoods)
        {
            ValidateSources(
                $"Trade good '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Trade good '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }

            ValidateExactRuleAssociations(
                $"Trade good '{definition.Id}'",
                definition.SpecialRuleIds,
                [TradeGoodsFullValueAndCurrencyRuleId],
                [TradeGoodsFullValueAndCurrencyRuleId],
                errors);
        }

        foreach (LifestyleDefinition definition in definitions.Expenses.Lifestyles)
        {
            ValidateSources(
                $"Lifestyle '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Lifestyle '{definition.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        errors.AddRange(
            LifestyleRuleAssociationIntegrityValidator.Validate(
                definitions.Expenses.Lifestyles));

        foreach (FoodDrinkDefinition definition in definitions.Expenses.FoodAndDrink)
        {
            ValidateSources(
                $"Food and drink '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Food and drink '{definition.Id}' references " +
                        $"missing rule '{specialRuleId}'.");
                }
            }

            ValidateExactRuleAssociations(
                $"Food and drink '{definition.Id}'",
                definition.SpecialRuleIds,
                [FoodDrinkLodgingIncludedInLifestyleRuleId],
                errors);
        }

        foreach (
            LifestyleHospitalityCostDefinition definition
            in definitions.Expenses.HospitalityCosts)
        {
            string identity =
                $"Hospitality cost for lifestyle " +
                $"'{definition.LifestyleId}'";

            ValidateSources(
                identity,
                definition.Sources,
                sourceIds,
                errors);

            if (!lifestyleIds.Contains(definition.LifestyleId))
            {
                errors.Add(
                    $"{identity} references missing lifestyle " +
                    $"'{definition.LifestyleId}'.");
            }

            foreach (RuleId specialRuleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"{identity} references missing rule " +
                        $"'{specialRuleId}'.");
                }
            }

            ValidateExactRuleAssociations(
                identity,
                definition.SpecialRuleIds,
                [FoodDrinkLodgingIncludedInLifestyleRuleId],
                errors);
        }

        errors.AddRange(
            MundaneServiceCatalogIntegrityValidator.Validate(
                definitions.Expenses.MundaneServices,
                sourceIds,
                ruleIds));

        if (definitions.Equipment.MountVehicleRules is not null)
        {
            ValidateSources(
                "Mount and vehicle rules",
                definitions.Equipment.MountVehicleRules.Sources,
                sourceIds,
                errors);

            foreach (RuleId ruleId in definitions.Equipment.MountVehicleRules.ReferencedRuleIds)
            {
                if (!ruleIds.Contains(ruleId))
                {
                    errors.Add(
                        $"Mount and vehicle rules reference missing rule '{ruleId}'.");
                }
            }

            if (!vehicleIds.Contains(
                    definitions.Equipment.MountVehicleRules.RowboatVehicleId))
            {
                errors.Add(
                    "Mount and vehicle rules reference missing rowboat " +
                    $"vehicle '{definitions.Equipment.MountVehicleRules.RowboatVehicleId}'.");
            }

            ValidateMountVehicleSemanticAssociations(
                definitions,
                definitions.Equipment.MountVehicleRules,
                errors);
        }

        if (definitions.Equipment.ArmorUsage is not null)
        {
            ValidateSources(
                "Armor usage rules",
                definitions.Equipment.ArmorUsage.Sources,
                sourceIds,
                errors);

            foreach (RuleId ruleId in definitions.Equipment.ArmorUsage.ReferencedRuleIds)
            {
                if (!ruleIds.Contains(ruleId))
                {
                    errors.Add(
                        $"Armor usage rules reference missing rule '{ruleId}'.");
                }
            }
        }

        foreach (
            CombatActionDefinition combatAction
            in definitions.CombatActions)
        {
            ValidateSources(
                $"Combat action '{combatAction.Id}'",
                combatAction.Sources,
                sourceIds,
                errors);
        }

        foreach (CoverDefinition coverEntry in definitions.Cover)
        {
            ValidateSources(
                $"Cover '{coverEntry.Id}'",
                coverEntry.Sources,
                sourceIds,
                errors);
        }

        foreach (TravelPaceDefinition travelPace in definitions.TravelPaces)
        {
            ValidateSources(
                $"Travel pace '{travelPace.Id}'",
                travelPace.Sources,
                sourceIds,
                errors);
        }

        foreach (RestTypeDefinition restType in definitions.RestTypes)
        {
            ValidateSources(
                $"Rest type '{restType.Id}'",
                restType.Sources,
                sourceIds,
                errors);
        }

        foreach (
            DowntimeActivityDefinition downtimeActivity
            in definitions.DowntimeActivities)
        {
            string owner = $"Downtime activity '{downtimeActivity.Id}'";

            ValidateSources(
                owner,
                downtimeActivity.Sources,
                sourceIds,
                errors);

            if (downtimeActivity.SavingThrowAbilityId is
                    { } savingThrowAbilityId &&
                !abilityIds.Contains(savingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{savingThrowAbilityId}'.");
            }
        }

        return errors;
    }

    private static void ValidateMountVehicleSemanticAssociations(
        RulesetDefinitionSet definitions,
        MountVehicleRules rules,
        ICollection<string> errors)
    {
        foreach (MountDefinition definition in definitions.Equipment.Mounts)
        {
            ValidateExactRuleAssociations(
                $"Mount '{definition.Id}'",
                definition.SpecialRuleIds,
                [
                    rules.DrawnVehiclePullingRuleId,
                    rules.BardingRuleId
                ],
                rules.ReferencedRuleIds,
                errors);
        }

        foreach (VehicleDefinition definition in definitions.Equipment.Vehicles)
        {
            IReadOnlyList<RuleId> expectedRuleIds =
                GetExpectedVehicleRuleIds(definition, rules);

            ValidateExactRuleAssociations(
                $"Vehicle '{definition.Id}'",
                definition.SpecialRuleIds,
                expectedRuleIds,
                rules.ReferencedRuleIds,
                errors);
        }

        foreach (MountSupportDefinition definition in definitions.Equipment.MountSupport)
        {
            IReadOnlyList<RuleId> expectedRuleIds =
                GetExpectedMountSupportRuleIds(definition, rules);

            ValidateExactRuleAssociations(
                $"Mount support '{definition.Id}'",
                definition.SpecialRuleIds,
                expectedRuleIds,
                rules.ReferencedRuleIds,
                errors);
        }

        if (rules.RowboatVehicleId != RowboatVehicleId)
        {
            errors.Add(
                "Mount and vehicle rules rowboat vehicle ID must be " +
                $"'{RowboatVehicleId}', but was " +
                $"'{rules.RowboatVehicleId}'.");
        }

        VehicleDefinition? rowboat = definitions.Equipment.Vehicles.FirstOrDefault(
            definition => definition.Id == RowboatVehicleId);

        if (rowboat is null)
        {
            errors.Add(
                $"Official rowboat vehicle '{RowboatVehicleId}' is missing.");
        }
        else if (rowboat.Kind != VehicleKind.Water)
        {
            errors.Add(
                $"Official rowboat vehicle '{RowboatVehicleId}' must be a water vehicle.");
        }
    }

    private static IReadOnlyList<RuleId> GetExpectedVehicleRuleIds(
        VehicleDefinition definition,
        MountVehicleRules rules)
    {
        if (definition.Kind == VehicleKind.Land)
        {
            return
            [
                rules.DrawnVehiclePullingRuleId,
                rules.VehicleProficiencyRuleId
            ];
        }

        if (definition.Kind == VehicleKind.Water &&
            (definition.Id == KeelboatVehicleId ||
             definition.Id == RowboatVehicleId))
        {
            return
            [
                rules.VehicleProficiencyRuleId,
                rules.RowedVesselsRuleId
            ];
        }

        if (definition.Kind == VehicleKind.Water)
        {
            return [rules.VehicleProficiencyRuleId];
        }

        return [];
    }

    private static IReadOnlyList<RuleId> GetExpectedMountSupportRuleIds(
        MountSupportDefinition definition,
        MountVehicleRules rules)
    {
        if (definition.Id == ExoticSaddleMountSupportId)
        {
            return [rules.ExoticSaddleRuleId];
        }

        if (definition.Id == MilitarySaddleMountSupportId)
        {
            return [rules.MilitarySaddleRuleId];
        }

        return [];
    }

    private static void ValidateExactRuleAssociations(
        string owner,
        IReadOnlyList<RuleId> actualRuleIds,
        IReadOnlyList<RuleId> expectedRuleIds,
        ICollection<string> errors)
    {
        RuleId[] managedRuleIds = actualRuleIds
            .Concat(expectedRuleIds)
            .Distinct()
            .ToArray();

        ValidateExactRuleAssociations(
            owner,
            actualRuleIds,
            expectedRuleIds,
            managedRuleIds,
            errors);
    }

    private static void ValidateExactRuleAssociations(
        string owner,
        IReadOnlyList<RuleId> actualRuleIds,
        IReadOnlyList<RuleId> expectedRuleIds,
        IReadOnlyList<RuleId> managedRuleIds,
        ICollection<string> errors)
    {
        HashSet<RuleId> managed = managedRuleIds.ToHashSet();
        HashSet<RuleId> actual = actualRuleIds
            .Where(managed.Contains)
            .ToHashSet();
        HashSet<RuleId> expected = expectedRuleIds.ToHashSet();

        foreach (RuleId missingRuleId in expected.Except(actual))
        {
            errors.Add(
                $"{owner} is missing required rule association '{missingRuleId}'.");
        }

        foreach (RuleId forbiddenRuleId in actual.Except(expected))
        {
            errors.Add(
                $"{owner} has forbidden rule association '{forbiddenRuleId}'.");
        }
    }

    public static void EnsureValid(RulesetDefinitionSet definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        foreach (ContainerCapacityDefinition definition in definitions.Equipment.ContainerCapacities)
        {
            ContainerCapacityDefinitionValidator.EnsureValid(definition);
        }

        ArmorUsageRules armorUsage =
            definitions.Equipment.ArmorUsage ??
            throw new ArgumentException(
                "Armor usage rules are required for the official ruleset definition set.",
                nameof(definitions));

        ArmorUsageRulesValidator.EnsureValid(armorUsage);

        MountVehicleRules mountVehicleRules =
            definitions.Equipment.MountVehicleRules ??
            throw new ArgumentException(
                "Mount and vehicle rules are required for the official ruleset definition set.",
                nameof(definitions));

        MountVehicleRulesValidator.EnsureValid(mountVehicleRules);

        var errors = new List<string>();

        errors.AddRange(Validate(definitions));
        errors.AddRange(
            OfficialExpenseSemanticValidator.Validate(
                definitions.Rules,
                definitions.Expenses.Lifestyles,
                definitions.Expenses.FoodAndDrink,
                definitions.Expenses.HospitalityCosts,
                definitions.Expenses.MundaneServices));

        errors.AddRange(
            OfficialCreatureVocabularySemanticValidator.Validate(
                definitions.CreatureVocabulary));

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Catalog integrity validation failed:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateSources(
        string owner,
        IReadOnlyList<SourceReference> sources,
        IReadOnlySet<SourceDocumentId> sourceIds,
        ICollection<string> errors)
    {
        foreach (SourceReference source in sources)
        {
            if (!sourceIds.Contains(source.DocumentId))
            {
                errors.Add(
                    $"{owner} references missing source document '{source.DocumentId}'.");
            }
        }
    }
}
