using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.TradeGoods;

internal static class TradeGoodDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        TradeGoodDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Trade-good ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Trade-good name must not be empty.");
        }

        if (definition.MarketValue.CopperPieces <= 0)
        {
            errors.Add("Trade-good market value must be greater than zero.");
        }

        if (definition.PricingBasis.Quantity <= 0)
        {
            errors.Add(
                "Trade-good pricing quantity must be greater than zero.");
        }

        if (!Enum.IsDefined(definition.PricingBasis.Unit))
        {
            errors.Add("Trade-good pricing unit must be defined.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Trade-good special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Trade-good special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Trade good must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(TradeGoodDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Trade-good definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
