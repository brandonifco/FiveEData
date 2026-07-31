using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Tests;

public sealed class RuleSerializationBoundaryTests
{
    [Fact]
    public void RuleJson_MissingName_IsRejected()
    {
        const string json = "[{\"id\":\"dnd5e2014.weapon-rule.test\",\"sources\":[]}]";

        Assert.Throws<InvalidDataException>(
            () => RuleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void RuleJson_MissingSources_IsRejected()
    {
        const string json = "[{\"id\":\"dnd5e2014.weapon-rule.test\",\"name\":\"Test rule\"}]";

        Assert.Throws<InvalidDataException>(
            () => RuleDefinitionLoader.LoadFromJson(json));
    }
}
