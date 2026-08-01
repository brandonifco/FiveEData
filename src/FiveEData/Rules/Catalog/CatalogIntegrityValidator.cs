using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Catalog;

internal static class CatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        RulesetDefinitionSet definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        HashSet<SourceDocumentId> sourceIds =
            definitions.SourceDocuments
                .Select(source => source.Id)
                .ToHashSet();

        HashSet<AmmunitionTypeId> ammunitionIds =
            definitions.Ammunition
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<RuleId> ruleIds =
            definitions.Rules
                .Select(rule => rule.Id)
                .ToHashSet();

        HashSet<AdventuringGearId> adventuringGearIds =
            definitions.AdventuringGear
                .Select(definition => definition.Id)
                .ToHashSet();

        HashSet<ToolFamilyId> toolFamilyIds =
            definitions.ToolFamilies
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (WeaponDefinition weapon in definitions.Weapons)
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

            foreach (RuleId specialRuleId in weapon.SpecialRuleIds)
            {
                if (!ruleIds.Contains(specialRuleId))
                {
                    errors.Add(
                        $"Weapon '{weapon.Id}' references missing rule '{specialRuleId}'.");
                }
            }
        }

        foreach (AmmunitionDefinition definition in definitions.Ammunition)
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

        foreach (ArmorDefinition definition in definitions.Armor)
        {
            ValidateSources(
                $"Armor '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (ShieldDefinition definition in definitions.Shields)
        {
            ValidateSources(
                $"Shield '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (AdventuringGearDefinition definition in definitions.AdventuringGear)
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

        foreach (ContainerCapacityDefinition definition in definitions.ContainerCapacities)
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

        foreach (ToolFamilyDefinition definition in definitions.ToolFamilies)
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

        foreach (ToolDefinition definition in definitions.Tools)
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

        foreach (MountDefinition definition in definitions.Mounts)
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

        foreach (VehicleDefinition definition in definitions.Vehicles)
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

        foreach (MountSupportDefinition definition in definitions.MountSupport)
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

        if (definitions.ArmorUsage is not null)
        {
            ValidateSources(
                "Armor usage rules",
                definitions.ArmorUsage.Sources,
                sourceIds,
                errors);

            foreach (RuleId ruleId in definitions.ArmorUsage.ReferencedRuleIds)
            {
                if (!ruleIds.Contains(ruleId))
                {
                    errors.Add(
                        $"Armor usage rules reference missing rule '{ruleId}'.");
                }
            }
        }

        return errors;
    }

    public static void EnsureValid(RulesetDefinitionSet definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        foreach (ContainerCapacityDefinition definition in definitions.ContainerCapacities)
        {
            ContainerCapacityDefinitionValidator.EnsureValid(definition);
        }

        ArmorUsageRules armorUsage =
            definitions.ArmorUsage ??
            throw new ArgumentException(
                "Armor usage rules are required for the official ruleset definition set.",
                nameof(definitions));

        ArmorUsageRulesValidator.EnsureValid(armorUsage);

        IReadOnlyList<string> errors = Validate(definitions);

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
