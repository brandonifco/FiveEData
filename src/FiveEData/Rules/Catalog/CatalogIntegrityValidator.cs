using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Catalog;

internal static class CatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<WeaponDefinition> weapons,
        IReadOnlyList<SourceDocument> sourceDocuments,
        IReadOnlyList<AmmunitionDefinition> ammunition,
        IReadOnlyList<RuleDefinition> rules)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(rules);

        var errors = new List<string>();

        HashSet<SourceDocumentId> sourceIds =
            sourceDocuments.Select(source => source.Id).ToHashSet();

        HashSet<AmmunitionTypeId> ammunitionIds =
            ammunition.Select(definition => definition.Id).ToHashSet();

        HashSet<RuleId> ruleIds =
            rules.Select(rule => rule.Id).ToHashSet();

        foreach (WeaponDefinition weapon in weapons)
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

        foreach (AmmunitionDefinition definition in ammunition)
        {
            ValidateSources(
                $"Ammunition '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (RuleDefinition rule in rules)
        {
            ValidateSources(
                $"Rule '{rule.Id}'",
                rule.Sources,
                sourceIds,
                errors);
        }

        return errors;
    }

    public static void EnsureValid(
        IReadOnlyList<WeaponDefinition> weapons,
        IReadOnlyList<SourceDocument> sourceDocuments,
        IReadOnlyList<AmmunitionDefinition> ammunition,
        IReadOnlyList<RuleDefinition> rules)
    {
        IReadOnlyList<string> errors =
            Validate(
                weapons,
                sourceDocuments,
                ammunition,
                rules);

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
