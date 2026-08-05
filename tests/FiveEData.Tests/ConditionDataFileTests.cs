using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Conditions.Serialization;

namespace FiveEData.Tests;

public sealed class ConditionDataFileTests
{
    private const string ExpectedSection =
        "Appendix A: Conditions";

    private static readonly ExpectedCondition[] Expected =
    [
        Condition("blinded", "Blinded"),
        Condition("charmed", "Charmed"),
        Condition("deafened", "Deafened"),
        Condition("exhaustion", "Exhaustion"),
        Condition("frightened", "Frightened"),
        Condition("grappled", "Grappled"),
        Condition("incapacitated", "Incapacitated"),
        Condition("invisible", "Invisible"),
        Condition("paralyzed", "Paralyzed"),
        Condition("petrified", "Petrified"),
        Condition("poisoned", "Poisoned"),
        Condition("prone", "Prone"),
        Condition("restrained", "Restrained"),
        Condition("stunned", "Stunned"),
        Condition("unconscious", "Unconscious")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactConditionClosure()
    {
        IReadOnlyList<ConditionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(15, definitions.Count);
        Assert.Equal(
            15,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());

        Assert.Equal(
            Expected
                .Select(expected => expected.Id)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal),
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingConditions()
    {
        IReadOnlyDictionary<
            ConditionId,
            ConditionDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedCondition expected
            in Expected)
        {
            ConditionDefinition definition =
                actual[new ConditionId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(290, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<ConditionDefinition>
        LoadCanonical()
    {
        return ConditionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "conditions.json"));
    }

    private static ExpectedCondition Condition(
        string suffix,
        string name)
    {
        return new ExpectedCondition(
            "dnd5e2014.condition." + suffix,
            name);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedCondition(
        string Id,
        string Name);
}
