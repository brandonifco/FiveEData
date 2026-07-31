namespace FiveEData.Rules.Common;

internal static class RuleDefinitionValidator
{
    public static void EnsureValid(RuleDefinition rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (rule.Sources.Count == 0)
        {
            throw new InvalidOperationException(
                $"Rule definition '{rule.Id}' must have at least one source reference.");
        }
    }
}
