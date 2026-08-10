using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes;

internal static class ClassCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        ClassDefinitionSet definitions,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<AbilityId> abilityIds,
        IReadOnlySet<SkillId> skillIds,
        IReadOnlySet<WeaponId> weaponIds,
        IReadOnlySet<RuleId> ruleIds,
        IReadOnlySet<SpellSlotProgressionId> spellSlotProgressionIds,
        IReadOnlySet<ExtraAttackProgressionId> extraAttackProgressionIds,
        IReadOnlySet<DamageTypeId> damageTypeIds,
        IReadOnlySet<CreatureSizeId> creatureSizeIds,
        IReadOnlySet<SpellId> spellIds,
        IReadOnlySet<ConditionId> conditionIds)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(creatureSizeIds);
        ArgumentNullException.ThrowIfNull(spellIds);
        ArgumentNullException.ThrowIfNull(conditionIds);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(abilityIds);
        ArgumentNullException.ThrowIfNull(skillIds);
        ArgumentNullException.ThrowIfNull(weaponIds);
        ArgumentNullException.ThrowIfNull(ruleIds);
        ArgumentNullException.ThrowIfNull(spellSlotProgressionIds);
        ArgumentNullException.ThrowIfNull(extraAttackProgressionIds);
        ArgumentNullException.ThrowIfNull(damageTypeIds);

        var errors = new List<string>();

        HashSet<ClassId> classIds =
            definitions.Classes
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (
            ClassDefinition @class
            in definitions.Classes
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Class '{@class.Id}'";

            ValidateSources(owner, @class.Sources, sourceIds, errors);

            foreach (AbilityId abilityId in @class.PrimaryAbilityIds)
            {
                if (!abilityIds.Contains(abilityId))
                {
                    errors.Add(
                        $"{owner} references missing primary ability " +
                        $"'{abilityId}'.");
                }
            }

            foreach (AbilityId abilityId in @class.SavingThrowProficiencyIds)
            {
                if (!abilityIds.Contains(abilityId))
                {
                    errors.Add(
                        $"{owner} references missing saving throw " +
                        $"ability '{abilityId}'.");
                }
            }

            foreach (WeaponId weaponId in @class.WeaponProficiencyIds)
            {
                if (!weaponIds.Contains(weaponId))
                {
                    errors.Add(
                        $"{owner} references missing weapon " +
                        $"'{weaponId}'.");
                }
            }

            foreach (SkillId skillId in @class.SkillChoiceOptionIds)
            {
                if (!skillIds.Contains(skillId))
                {
                    errors.Add(
                        $"{owner} references missing skill choice " +
                        $"option '{skillId}'.");
                }
            }

            foreach (ClassLevelFeature feature in @class.LevelFeatures)
            {
                if (!ruleIds.Contains(feature.FeatureRuleId))
                {
                    errors.Add(
                        $"{owner} references missing level feature rule " +
                        $"'{feature.FeatureRuleId}'.");
                }
            }

            ValidateSpellcasting(
                owner,
                @class.SpellSlotProgressionId,
                @class.SpellcastingAbilityId,
                spellSlotProgressionIds,
                abilityIds,
                errors);

            if (@class.ExtraAttackProgressionId is { } extraAttackId &&
                !extraAttackProgressionIds.Contains(extraAttackId))
            {
                errors.Add(
                    $"{owner} references missing Extra Attack " +
                    $"progression '{extraAttackId}'.");
            }

            if (@class.RageProgression is { } rageProgression)
            {
                foreach (
                    DamageTypeId damageTypeId
                    in rageProgression.ResistedDamageTypeIds)
                {
                    if (!damageTypeIds.Contains(damageTypeId))
                    {
                        errors.Add(
                            $"{owner} references missing damage type " +
                            $"'{damageTypeId}' in its Rage progression.");
                    }
                }
            }

            if (@class.ImprovedDivineSmite is { } improvedDivineSmite &&
                !damageTypeIds.Contains(improvedDivineSmite.DamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{improvedDivineSmite.DamageTypeId}' in its Improved " +
                    "Divine Smite.");
            }

            if (@class.PrimalChampion is { } primalChampion)
            {
                foreach (AbilityId abilityId in primalChampion.AbilityIds)
                {
                    if (!abilityIds.Contains(abilityId))
                    {
                        errors.Add(
                            $"{owner} references missing ability " +
                            $"'{abilityId}' in its Primal Champion.");
                    }
                }
            }

            if (@class.CleansingTouchUsesPerRest is
                    { } cleansingTouchUsesPerRest &&
                !abilityIds.Contains(cleansingTouchUsesPerRest.AbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{cleansingTouchUsesPerRest.AbilityId}' in its " +
                    "Cleansing Touch.");
            }

            if (@class.RelentlessRage is { } relentlessRage &&
                !abilityIds.Contains(relentlessRage.SavingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{relentlessRage.SavingThrowAbilityId}' in its " +
                    "Relentless Rage.");
            }
        }

        foreach (
            SubclassDefinition subclass
            in definitions.Subclasses
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Subclass '{subclass.Id}'";

            ValidateSources(owner, subclass.Sources, sourceIds, errors);

            if (!classIds.Contains(subclass.ClassId))
            {
                errors.Add(
                    $"{owner} references missing class '{subclass.ClassId}'.");
            }

            foreach (ClassLevelFeature feature in subclass.LevelFeatures)
            {
                if (!ruleIds.Contains(feature.FeatureRuleId))
                {
                    errors.Add(
                        $"{owner} references missing level feature rule " +
                        $"'{feature.FeatureRuleId}'.");
                }
            }

            ValidateSpellcasting(
                owner,
                subclass.SpellSlotProgressionId,
                subclass.SpellcastingAbilityId,
                spellSlotProgressionIds,
                abilityIds,
                errors);

            if (subclass.DivineStrikeProgression is { } divineStrikeProgression)
            {
                ValidateDivineStrikeProgression(
                    owner,
                    divineStrikeProgression,
                    damageTypeIds,
                    errors);
            }

            if (subclass.HurlThroughHell is { } hurlThroughHell &&
                !damageTypeIds.Contains(hurlThroughHell.DamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{hurlThroughHell.DamageTypeId}' in its Hurl Through " +
                    "Hell.");
            }

            if (subclass.WrathOfTheStorm is { } wrathOfTheStorm)
            {
                foreach (
                    DamageTypeId damageTypeId
                    in wrathOfTheStorm.ChoosableDamageTypeIds)
                {
                    if (!damageTypeIds.Contains(damageTypeId))
                    {
                        errors.Add(
                            $"{owner} references missing damage type " +
                            $"'{damageTypeId}' in its Wrath of the Storm.");
                    }
                }

                if (!abilityIds.Contains(wrathOfTheStorm.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{wrathOfTheStorm.SavingThrowAbilityId}' in its " +
                        "Wrath of the Storm.");
                }

                if (!abilityIds.Contains(
                        wrathOfTheStorm.UsesPerRest.AbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{wrathOfTheStorm.UsesPerRest.AbilityId}' in " +
                        "its Wrath of the Storm uses per rest.");
                }
            }

            if (subclass.ThunderboltStrike is { } thunderboltStrike &&
                !creatureSizeIds.Contains(
                    thunderboltStrike.MaximumTargetSizeId))
            {
                errors.Add(
                    $"{owner} references missing creature size " +
                    $"'{thunderboltStrike.MaximumTargetSizeId}' in its " +
                    "Thunderbolt Strike.");
            }

            if (subclass.WardingFlare is { } wardingFlare &&
                !abilityIds.Contains(wardingFlare.UsesPerRest.AbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{wardingFlare.UsesPerRest.AbilityId}' in its " +
                    "Warding Flare.");
            }

            if (subclass.WarPriestUsesPerRest is { } warPriestUsesPerRest &&
                !abilityIds.Contains(warPriestUsesPerRest.AbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{warPriestUsesPerRest.AbilityId}' in its War " +
                    "Priest.");
            }

            foreach (SpellGrant grant in subclass.InnateSpellGrants)
            {
                if (!spellIds.Contains(grant.GrantedSpellId))
                {
                    errors.Add(
                        $"{owner} references missing spell " +
                        $"'{grant.GrantedSpellId}'.");
                }
            }

            foreach (
                ConditionId conditionId
                in subclass.MindlessRageImmuneConditionIds)
            {
                if (!conditionIds.Contains(conditionId))
                {
                    errors.Add(
                        $"{owner} references missing condition " +
                        $"'{conditionId}' in its Mindless Rage.");
                }
            }

            if (subclass.IntimidatingPresence is { } intimidatingPresence)
            {
                if (!abilityIds.Contains(
                        intimidatingPresence.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{intimidatingPresence.SavingThrowAbilityId}' in " +
                        "its Intimidating Presence.");
                }

                if (!conditionIds.Contains(
                        intimidatingPresence.ImposedConditionId))
                {
                    errors.Add(
                        $"{owner} references missing condition " +
                        $"'{intimidatingPresence.ImposedConditionId}' in " +
                        "its Intimidating Presence.");
                }
            }

            if (subclass.DeathStrike is { } deathStrike &&
                !abilityIds.Contains(deathStrike.SavingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{deathStrike.SavingThrowAbilityId}' in its Death " +
                    "Strike.");
            }

            if (subclass.FeyPresence is { } feyPresence)
            {
                if (!abilityIds.Contains(feyPresence.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{feyPresence.SavingThrowAbilityId}' in its Fey " +
                        "Presence.");
                }

                foreach (
                    ConditionId conditionId
                    in feyPresence.ChoosableConditionIds)
                {
                    if (!conditionIds.Contains(conditionId))
                    {
                        errors.Add(
                            $"{owner} references missing condition " +
                            $"'{conditionId}' in its Fey Presence.");
                    }
                }
            }

            if (subclass.BeguilingDefenses is { } beguilingDefenses)
            {
                if (!conditionIds.Contains(
                        beguilingDefenses.ImmuneConditionId))
                {
                    errors.Add(
                        $"{owner} references missing condition " +
                        $"'{beguilingDefenses.ImmuneConditionId}' in its " +
                        "Beguiling Defenses.");
                }

                if (!abilityIds.Contains(
                        beguilingDefenses.ReflectionSavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{beguilingDefenses.ReflectionSavingThrowAbilityId}' " +
                        "in its Beguiling Defenses.");
                }
            }

            if (subclass.DarkDelirium is { } darkDelirium)
            {
                if (!abilityIds.Contains(darkDelirium.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{darkDelirium.SavingThrowAbilityId}' in its " +
                        "Dark Delirium.");
                }

                foreach (
                    ConditionId conditionId
                    in darkDelirium.ChoosableConditionIds)
                {
                    if (!conditionIds.Contains(conditionId))
                    {
                        errors.Add(
                            $"{owner} references missing condition " +
                            $"'{conditionId}' in its Dark Delirium.");
                    }
                }
            }

            if (subclass.ThoughtShield is { } thoughtShield &&
                !damageTypeIds.Contains(thoughtShield.ResistedDamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{thoughtShield.ResistedDamageTypeId}' in its " +
                    "Thought Shield.");
            }

            if (subclass.CreateThrall is { } createThrall &&
                !conditionIds.Contains(createThrall.ImposedConditionId))
            {
                errors.Add(
                    $"{owner} references missing condition " +
                    $"'{createThrall.ImposedConditionId}' in its Create " +
                    "Thrall.");
            }

            if (subclass.HypnoticGaze is { } hypnoticGaze)
            {
                if (!abilityIds.Contains(hypnoticGaze.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{hypnoticGaze.SavingThrowAbilityId}' in its " +
                        "Hypnotic Gaze.");
                }

                foreach (
                    ConditionId conditionId
                    in hypnoticGaze.ImposedConditionIds)
                {
                    if (!conditionIds.Contains(conditionId))
                    {
                        errors.Add(
                            $"{owner} references missing condition " +
                            $"'{conditionId}' in its Hypnotic Gaze.");
                    }
                }
            }

            if (subclass.InstinctiveCharm is { } instinctiveCharm &&
                !abilityIds.Contains(instinctiveCharm.SavingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{instinctiveCharm.SavingThrowAbilityId}' in its " +
                    "Instinctive Charm.");
            }

            if (subclass.AlterMemories is { } alterMemories &&
                !abilityIds.Contains(
                    alterMemories.ForgetSavingThrowAbilityId))
            {
                errors.Add(
                    $"{owner} references missing ability " +
                    $"'{alterMemories.ForgetSavingThrowAbilityId}' in its " +
                    "Alter Memories.");
            }

            if (subclass.Overchannel is { } overchannel &&
                !damageTypeIds.Contains(overchannel.SelfDamageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{overchannel.SelfDamageTypeId}' in its Overchannel.");
            }

            if (subclass.DraconicPresence is { } draconicPresence)
            {
                if (!abilityIds.Contains(
                        draconicPresence.SavingThrowAbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{draconicPresence.SavingThrowAbilityId}' in " +
                        "its Draconic Presence.");
                }

                foreach (
                    ConditionId conditionId
                    in draconicPresence.ChoosableConditionIds)
                {
                    if (!conditionIds.Contains(conditionId))
                    {
                        errors.Add(
                            $"{owner} references missing condition " +
                            $"'{conditionId}' in its Draconic Presence.");
                    }
                }
            }
        }

        return errors;
    }

    private static void ValidateDivineStrikeProgression(
        string owner,
        DivineStrikeProgressionDetail divineStrikeProgression,
        IReadOnlySet<DamageTypeId> damageTypeIds,
        ICollection<string> errors)
    {
        if (divineStrikeProgression.FixedDamageTypeId is { } fixedDamageTypeId &&
            !damageTypeIds.Contains(fixedDamageTypeId))
        {
            errors.Add(
                $"{owner} references missing damage type " +
                $"'{fixedDamageTypeId}' in its Divine Strike progression.");
        }

        foreach (
            DamageTypeId damageTypeId
            in divineStrikeProgression.ChoosableDamageTypeIds ?? [])
        {
            if (!damageTypeIds.Contains(damageTypeId))
            {
                errors.Add(
                    $"{owner} references missing damage type " +
                    $"'{damageTypeId}' in its Divine Strike progression.");
            }
        }
    }

    private static void ValidateSpellcasting(
        string owner,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        IReadOnlySet<SpellSlotProgressionId> spellSlotProgressionIds,
        IReadOnlySet<AbilityId> abilityIds,
        ICollection<string> errors)
    {
        if (spellSlotProgressionId is { } progressionId &&
            !spellSlotProgressionIds.Contains(progressionId))
        {
            errors.Add(
                $"{owner} references missing spell slot progression " +
                $"'{progressionId}'.");
        }

        if (spellcastingAbilityId is { } abilityId &&
            !abilityIds.Contains(abilityId))
        {
            errors.Add(
                $"{owner} references missing spellcasting ability " +
                $"'{abilityId}'.");
        }

        if (spellSlotProgressionId is null != spellcastingAbilityId is null)
        {
            errors.Add(
                $"{owner} must define both a spell slot progression " +
                "and a spellcasting ability, or neither.");
        }
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
                    $"{owner} references missing source document " +
                    $"'{source.DocumentId}'.");
            }
        }
    }
}
