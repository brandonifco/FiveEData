namespace FiveEData.Rules.Common;

internal static class RuleDefinitionValidator
{
    public static void EnsureValid(RuleDefinition rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (string.IsNullOrWhiteSpace(rule.Id.Value))
        {
            throw new InvalidOperationException(
                "Rule ID must not be empty.");
        }

        if (rule.Sources.Count == 0)
        {
            throw new InvalidOperationException(
                $"Rule definition '{rule.Id}' must have at least one source reference.");
        }
    }
}
