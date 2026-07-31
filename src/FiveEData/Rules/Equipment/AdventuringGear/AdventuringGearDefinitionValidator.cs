using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.AdventuringGear;

internal static class AdventuringGearDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        AdventuringGearDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Adventuring gear ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Adventuring gear name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add("Adventuring gear cost must be greater than zero.");
        }

        if (definition.ListedWeight is { Weight.Pounds: <= 0 })
        {
            errors.Add(
                "Adventuring gear listed weight must be greater than zero when specified.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Adventuring gear special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Adventuring gear special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Adventuring gear must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(AdventuringGearDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Adventuring gear definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
