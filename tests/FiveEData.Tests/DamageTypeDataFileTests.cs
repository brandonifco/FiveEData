using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.DamageTypes.Serialization;

namespace FiveEData.Tests;

public sealed class DamageTypeDataFileTests
{
    private const string ExpectedSection =
        "Chapter 9: Combat — Damage and Healing — " +
        "Damage Types";

    private static readonly ExpectedDamageType[] Expected =
    [
        DamageType("acid", "Acid"),
        DamageType("bludgeoning", "Bludgeoning"),
        DamageType("cold", "Cold"),
        DamageType("fire", "Fire"),
        DamageType("force", "Force"),
        DamageType("lightning", "Lightning"),
        DamageType("necrotic", "Necrotic"),
        DamageType("piercing", "Piercing"),
        DamageType("poison", "Poison"),
        DamageType("psychic", "Psychic"),
        DamageType("radiant", "Radiant"),
        DamageType("slashing", "Slashing"),
        DamageType("thunder", "Thunder")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactDamageTypeClosure()
    {
        IReadOnlyList<DamageTypeDefinition> definitions =
            LoadCanonical();

        Assert.Equal(13, definitions.Count);
        Assert.Equal(
            13,
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
    public void CanonicalFile_MatchesFirstPrintingDamageTypes()
    {
        IReadOnlyDictionary<
            DamageTypeId,
            DamageTypeDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedDamageType expected
            in Expected)
        {
            DamageTypeDefinition definition =
                actual[new DamageTypeId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(196, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<DamageTypeDefinition>
        LoadCanonical()
    {
        return DamageTypeDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "damage-types.json"));
    }

    private static ExpectedDamageType DamageType(
        string suffix,
        string name)
    {
        return new ExpectedDamageType(
            "dnd5e2014.damage-type." + suffix,
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

    private sealed record ExpectedDamageType(
        string Id,
        string Name);
}
