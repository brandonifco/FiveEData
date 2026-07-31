using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Tools;

internal static class ToolDefinitionValidator
{
    public static IReadOnlyList<string> Validate(ToolDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Tool ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Tool name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add("Tool cost must be greater than zero.");
        }

        if (definition.Weight is { Pounds: <= 0 })
        {
            errors.Add("Tool weight must be greater than zero when specified.");
        }

        if (definition.FamilyId is { } familyId &&
            string.IsNullOrWhiteSpace(familyId.Value))
        {
            errors.Add("Tool family ID must not be empty when specified.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Tool special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add($"Tool special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Tool must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(ToolDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Tool definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
