using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.TradeGoods.Serialization;

namespace FiveEData.Tests;

public sealed class TradeGoodDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        TradeGoodDefinition definition = Assert.Single(
            TradeGoodDefinitionLoader.LoadFromJson(
                """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Pound"},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Trade Goods"}]}]"""));

        Assert.Equal("dnd5e2014.trade-good.test", definition.Id.Value);
        Assert.Equal(100, definition.MarketValue.CopperPieces);
        Assert.Equal(
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            definition.PricingBasis);
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Pound"},"specialRuleIds":[],"sources":[],"unexpected":true}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingPricingBasisMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSpecialRuleIdsMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Pound"},"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSourcesMember_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Pound"},"specialRuleIds":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NumericEnumValue_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":1},"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UndefinedStringEnumValue_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Bushel"},"specialRuleIds":[],"sources":[]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            """{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":1,"unit":"Pound"},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Trade Goods"}]}""";
        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void ZeroPricingQuantity_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":100},"pricingBasis":{"quantity":0,"unit":"Pound"},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Trade Goods"}]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void ZeroMarketValue_IsRejected()
    {
        string json =
            """[{"id":"dnd5e2014.trade-good.test","name":"Test trade good","marketValue":{"copperPieces":0},"pricingBasis":{"quantity":1,"unit":"Pound"},"specialRuleIds":[],"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":157,"section":"Trade Goods"}]}]""";

        Assert.Throws<InvalidDataException>(
            () => TradeGoodDefinitionLoader.LoadFromJson(json));
    }
}
