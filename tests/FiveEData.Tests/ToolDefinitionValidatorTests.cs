using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class ToolDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        ToolDefinition definition = Create(id: default(ToolId));

        Assert.Contains(
            ToolDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCost_IsRejected()
    {
        ToolDefinition definition = Create(cost: new Money(0));

        Assert.Contains(
            ToolDefinitionValidator.Validate(definition),
            error => error.Contains("cost", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ZeroWeight_WhenSpecified_IsRejected()
    {
        ToolDefinition definition = Create(weight: new Weight(0));

        Assert.Contains(
            ToolDefinitionValidator.Validate(definition),
            error => error.Contains("weight", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DefaultFamilyId_WhenSpecified_IsRejected()
    {
        ToolDefinition definition = Create(familyId: default(ToolFamilyId));

        Assert.Contains(
            ToolDefinitionValidator.Validate(definition),
            error => error.Contains("family ID", StringComparison.Ordinal));
    }

    private static ToolDefinition Create(
        ToolId? id = null,
        Money? cost = null,
        Weight? weight = null,
        ToolFamilyId? familyId = null)
    {
        return new ToolDefinition(
            id ?? new ToolId("dnd5e2014.tool.test"),
            "Test tool",
            cost ?? new Money(100),
            weight,
            familyId,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);
    }
}
