using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.MountSupport;

internal static class MountSupportDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        MountSupportDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Mount support ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Mount support name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add("Mount support cost must be greater than zero.");
        }

        if (definition.ListedWeight is { } listedWeight &&
            listedWeight.Pounds <= 0)
        {
            errors.Add(
                "Mount support listed weight must be greater than zero when specified.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Mount support special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Mount support special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Mount support must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(MountSupportDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Mount support definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
