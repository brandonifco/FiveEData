using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.TradeGoods.Serialization;

internal sealed class TradeGoodDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public MoneyData? MarketValue { get; init; }

    [JsonRequired]
    public TradeGoodPricingBasisData? PricingBasis { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class TradeGoodPricingBasisData
{
    [JsonRequired]
    public decimal Quantity { get; init; }

    [JsonRequired]
    public TradeGoodUnit Unit { get; init; }
}
