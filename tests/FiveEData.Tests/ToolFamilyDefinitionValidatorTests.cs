using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class ToolFamilyDefinitionValidatorTests
{
    [Fact]
    public void DefaultId_IsRejected()
    {
        ToolFamilyDefinition definition = new(
            default,
            "Test family",
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);

        Assert.Contains(
            ToolFamilyDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_IsRejected()
    {
        ToolFamilyDefinition definition = new(
            new ToolFamilyId("dnd5e2014.tool-family.test"),
            "Test family",
            specialRuleIds: [],
            sources: []);

        Assert.Contains(
            ToolFamilyDefinitionValidator.Validate(definition),
            error => error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }
}
