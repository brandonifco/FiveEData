using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.TradeGoods.Serialization;

namespace FiveEData.Tests;

public sealed class TradeGoodDataFileTests
{
    private static readonly RuleId FullValueAndCurrencyRuleId =
        new("dnd5e2014.trade-good-rule.full-value-and-currency");

    [Fact]
    public void CanonicalFile_ContainsExactlyTwentyThreeIndividualGoods()
    {
        IReadOnlyList<TradeGoodDefinition> definitions = LoadCanonical();

        Assert.Equal(23, definitions.Count);
        Assert.Equal(
            23,
            definitions.Select(definition => definition.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingTradeGoodsTable()
    {
        IReadOnlyDictionary<TradeGoodId, TradeGoodDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedTradeGoodRow expected in Expected)
        {
            TradeGoodDefinition definition =
                actual[new TradeGoodId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(
                expected.CopperPieces,
                definition.MarketValue.CopperPieces);
            Assert.Equal(1m, definition.PricingBasis.Quantity);
            Assert.Equal(expected.Unit, definition.PricingBasis.Unit);
            Assert.Equal(
                new[] { FullValueAndCurrencyRuleId },
                definition.SpecialRuleIds);

            SourceReference source = Assert.Single(definition.Sources);
            Assert.Equal(157, source.Page);
            Assert.Equal("Trade Goods", source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesPricingUnitDistribution()
    {
        IReadOnlyList<TradeGoodDefinition> definitions = LoadCanonical();

        Assert.Equal(
            13,
            definitions.Count(
                definition =>
                    definition.PricingBasis.Unit == TradeGoodUnit.Pound));
        Assert.Equal(
            4,
            definitions.Count(
                definition =>
                    definition.PricingBasis.Unit == TradeGoodUnit.SquareYard));
        Assert.Equal(
            6,
            definitions.Count(
                definition =>
                    definition.PricingBasis.Unit == TradeGoodUnit.Each));
    }

    private static IReadOnlyList<TradeGoodDefinition> LoadCanonical()
    {
        return TradeGoodDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "trade-goods.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedTradeGoodRow(
        string Id,
        string Name,
        long CopperPieces,
        TradeGoodUnit Unit);

    private static readonly ExpectedTradeGoodRow[] Expected =
    [
        new(
            "dnd5e2014.trade-good.wheat",
            "Wheat",
            1L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.flour",
            "Flour",
            2L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.chicken",
            "Chicken",
            2L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.salt",
            "Salt",
            5L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.iron",
            "Iron",
            10L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.canvas",
            "Canvas",
            10L,
            TradeGoodUnit.SquareYard),
        new(
            "dnd5e2014.trade-good.copper",
            "Copper",
            50L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.cotton-cloth",
            "Cotton cloth",
            50L,
            TradeGoodUnit.SquareYard),
        new(
            "dnd5e2014.trade-good.ginger",
            "Ginger",
            100L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.goat",
            "Goat",
            100L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.cinnamon",
            "Cinnamon",
            200L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.pepper",
            "Pepper",
            200L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.sheep",
            "Sheep",
            200L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.cloves",
            "Cloves",
            300L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.pig",
            "Pig",
            300L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.silver",
            "Silver",
            500L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.linen",
            "Linen",
            500L,
            TradeGoodUnit.SquareYard),
        new(
            "dnd5e2014.trade-good.silk",
            "Silk",
            1000L,
            TradeGoodUnit.SquareYard),
        new(
            "dnd5e2014.trade-good.cow",
            "Cow",
            1000L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.saffron",
            "Saffron",
            1500L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.ox",
            "Ox",
            1500L,
            TradeGoodUnit.Each),
        new(
            "dnd5e2014.trade-good.gold",
            "Gold",
            5000L,
            TradeGoodUnit.Pound),
        new(
            "dnd5e2014.trade-good.platinum",
            "Platinum",
            50000L,
            TradeGoodUnit.Pound)
    ];
}
