using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.ExtraAttack.Serialization;

namespace FiveEData.Tests;

public sealed class ExtraAttackProgressionDataFileTests
{
    private const string Standard =
        "dnd5e2014.extra-attack-progression.standard";
    private const string Fighter =
        "dnd5e2014.extra-attack-progression.fighter";

    [Fact]
    public void CanonicalFile_ContainsExactProgressionClosure()
    {
        IReadOnlyList<ExtraAttackProgressionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [Fighter, Standard],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void Standard_GrantsTwoAttacksAtFifthLevelOnly()
    {
        ExtraAttackProgressionDefinition definition = Get(Standard);

        ExtraAttackGrant grant = Assert.Single(definition.Grants);
        Assert.Equal(5, grant.CharacterLevel);
        Assert.Equal(2, grant.AttackCount);

        Assert.Equal(
            [49, 79, 85, 92],
            definition.Sources
                .Select(source => source.Page)
                .OrderBy(page => page));
    }

    [Fact]
    public void Fighter_ScalesToFourAttacksByTwentiethLevel()
    {
        ExtraAttackProgressionDefinition definition = Get(Fighter);

        Assert.Equal(
            [(5, 2), (11, 3), (20, 4)],
            definition.Grants
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.AttackCount)));

        var source = Assert.Single(definition.Sources);
        Assert.Equal(73, source.Page);
    }

    [Fact]
    public void EveryProgression_CitesPhbFirstPrinting()
    {
        foreach (
            ExtraAttackProgressionDefinition definition
            in LoadCanonical())
        {
            Assert.All(
                definition.Sources,
                source =>
                    Assert.Equal(
                        "dnd5e2014.source.phb-first-printing",
                        source.DocumentId.Value));
        }
    }

    private static ExtraAttackProgressionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<ExtraAttackProgressionDefinition>
        LoadCanonical()
    {
        return ExtraAttackProgressionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "extra-attack-progressions.json"));
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
}
