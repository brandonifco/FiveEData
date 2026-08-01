using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
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

        HashSet<VehicleId> vehicleIds =
            definitions.Vehicles
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

        foreach (TradeGoodDefinition definition in definitions.TradeGoods)
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

        if (definitions.MountVehicleRules is not null)
        {
            ValidateSources(
                "Mount and vehicle rules",
                definitions.MountVehicleRules.Sources,
                sourceIds,
                errors);

            foreach (RuleId ruleId in definitions.MountVehicleRules.ReferencedRuleIds)
            {
                if (!ruleIds.Contains(ruleId))
                {
                    errors.Add(
                        $"Mount and vehicle rules reference missing rule '{ruleId}'.");
                }
            }

            if (!vehicleIds.Contains(
                    definitions.MountVehicleRules.RowboatVehicleId))
            {
                errors.Add(
                    $"Mount and vehicle rules reference missing rowboat vehicle '{definitions.MountVehicleRules.RowboatVehicleId}'.");
            }

            ValidateMountVehicleSemanticAssociations(
                definitions,
                definitions.MountVehicleRules,
                errors);
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

    private static void ValidateMountVehicleSemanticAssociations(
        RulesetDefinitionSet definitions,
        MountVehicleRules rules,
        ICollection<string> errors)
    {
        foreach (MountDefinition definition in definitions.Mounts)
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

        foreach (VehicleDefinition definition in definitions.Vehicles)
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

        foreach (MountSupportDefinition definition in definitions.MountSupport)
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
                $"Mount and vehicle rules rowboat vehicle ID must be '{RowboatVehicleId}', but was '{rules.RowboatVehicleId}'.");
        }

        VehicleDefinition? rowboat = definitions.Vehicles.FirstOrDefault(
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

        MountVehicleRules mountVehicleRules =
            definitions.MountVehicleRules ??
            throw new ArgumentException(
                "Mount and vehicle rules are required for the official ruleset definition set.",
                nameof(definitions));

        MountVehicleRulesValidator.EnsureValid(mountVehicleRules);

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
