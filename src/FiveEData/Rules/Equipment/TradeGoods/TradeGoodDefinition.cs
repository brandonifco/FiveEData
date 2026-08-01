using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.TradeGoods;

public sealed class TradeGoodDefinition
{
    internal TradeGoodDefinition(
        TradeGoodId id,
        string name,
        Money marketValue,
        TradeGoodPricingBasis pricingBasis,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        MarketValue = marketValue;
        PricingBasis = pricingBasis;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public TradeGoodId Id { get; }
    public string Name { get; }
    public Money MarketValue { get; }
    public TradeGoodPricingBasis PricingBasis { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
