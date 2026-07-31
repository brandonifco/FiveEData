using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;

namespace FiveEData.Tests;

public sealed class ShieldDataFileTests
{
    [Fact]
    public void CanonicalShieldFile_MatchesFirstPrintingArmorTable()
    {
        string root = FindRepositoryRoot();

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "shields.json"));

        ShieldDefinition shield = Assert.Single(shields);

        Assert.Equal(
            new ShieldId("dnd5e2014.armor.shield"),
            shield.Id);
        Assert.Equal("Shield", shield.Name);
        Assert.Equal(1000, shield.Cost.CopperPieces);
        Assert.Equal(6m, shield.Weight.Pounds);
        Assert.Equal(2, shield.ArmorClassBonus);

        var source = Assert.Single(shield.Sources);
        Assert.Equal(145, source.Page);
        Assert.Equal(
            "Chapter 5: Equipment — Armor and Shields",
            source.Section);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
