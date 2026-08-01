using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.TradeGoods.Serialization;

internal static class TradeGoodDefinitionLoader
{
    public static IReadOnlyList<TradeGoodDefinition> LoadFromFile(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<TradeGoodDefinition> LoadFromJson(
        string json)
    {
        TradeGoodDefinitionData[] data =
            StrictJson.DeserializeArray<TradeGoodDefinitionData>(
                json,
                "Trade good");

        var definitions =
            new List<TradeGoodDefinition>(data.Length);
        var ids = new HashSet<TradeGoodId>();

        for (int index = 0; index < data.Length; index++)
        {
            TradeGoodDefinitionData? itemData = data[index];

            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid trade-good definition at index {index}.");
            }

            TradeGoodDefinition definition;

            try
            {
                definition = Map(itemData);
                TradeGoodDefinitionValidator.EnsureValid(definition);
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid trade-good definition at {identity}.",
                    exception);
            }

            if (!ids.Add(definition.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate trade-good ID '{definition.Id}'.");
            }

            definitions.Add(definition);
        }

        return definitions;
    }

    private static TradeGoodDefinition Map(
        TradeGoodDefinitionData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new TradeGoodId(
            data.Id
            ?? throw new ArgumentException(
                "Trade-good ID is required.",
                nameof(data)));

        string name = data.Name
            ?? throw new ArgumentException(
                "Trade-good name is required.",
                nameof(data));

        MoneyData marketValue = data.MarketValue
            ?? throw new ArgumentException(
                "Trade-good market value is required.",
                nameof(data));

        TradeGoodPricingBasisData pricingBasis = data.PricingBasis
            ?? throw new ArgumentException(
                "Trade-good pricing basis is required.",
                nameof(data));

        string[] ruleIdData = data.SpecialRuleIds
            ?? throw new ArgumentException(
                "Trade-good special rule IDs are required.",
                nameof(data));

        RuleId[] specialRuleIds = ruleIdData
            .Select(value => new RuleId(value))
            .ToArray();

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Trade-good sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new TradeGoodDefinition(
            id,
            name,
            new Money(marketValue.CopperPieces),
            new TradeGoodPricingBasis(
                pricingBasis.Quantity,
                pricingBasis.Unit),
            specialRuleIds,
            sources);
    }
}
