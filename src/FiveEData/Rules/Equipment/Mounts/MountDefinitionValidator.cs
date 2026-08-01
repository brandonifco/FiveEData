using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Mounts;

internal static class MountDefinitionValidator
{
    public static IReadOnlyList<string> Validate(MountDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Mount ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Mount name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add("Mount cost must be greater than zero.");
        }

        if (definition.Speed.Feet <= 0)
        {
            errors.Add("Mount speed must be greater than zero.");
        }

        if (definition.BaseCarryingCapacity.Pounds <= 0)
        {
            errors.Add("Mount base carrying capacity must be greater than zero.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Mount special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add($"Mount special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Mount must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(MountDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Mount definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
