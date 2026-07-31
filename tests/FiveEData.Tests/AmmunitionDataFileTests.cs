using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;

namespace FiveEData.Tests;

public sealed class AmmunitionDataFileTests
{
    [Fact]
    public void AmmunitionJson_LoadsCompleteReferencedCatalog()
    {
        IReadOnlyList<AmmunitionDefinition> ammunition =
            LoadAmmunition();

        Assert.Equal(4, ammunition.Count);

        Assert.Contains(
            ammunition,
            item =>
                item.Id ==
                new AmmunitionTypeId("dnd5e2014.ammunition.arrow"));

        Assert.Contains(
            ammunition,
            item =>
                item.Id ==
                new AmmunitionTypeId("dnd5e2014.ammunition.blowgun-needle"));

        Assert.Contains(
            ammunition,
            item =>
                item.Id ==
                new AmmunitionTypeId("dnd5e2014.ammunition.crossbow-bolt"));

        Assert.Contains(
            ammunition,
            item =>
                item.Id ==
                new AmmunitionTypeId("dnd5e2014.ammunition.sling-bullet"));
    }

    [Fact]
    public void AmmunitionJson_PreservesPublishedBundleValues()
    {
        IReadOnlyList<AmmunitionDefinition> ammunition =
            LoadAmmunition();

        AmmunitionDefinition arrows =
            GetAmmunition(ammunition, "dnd5e2014.ammunition.arrow");

        Assert.Equal("Arrows", arrows.Name);
        Assert.Equal(20, arrows.BundleQuantity);
        Assert.Equal(100, arrows.Cost.CopperPieces);
        Assert.Equal(1m, arrows.Weight.Pounds);

        AmmunitionDefinition needles =
            GetAmmunition(
                ammunition,
                "dnd5e2014.ammunition.blowgun-needle");

        Assert.Equal(50, needles.BundleQuantity);
        Assert.Equal(100, needles.Cost.CopperPieces);
        Assert.Equal(1m, needles.Weight.Pounds);

        AmmunitionDefinition bolts =
            GetAmmunition(
                ammunition,
                "dnd5e2014.ammunition.crossbow-bolt");

        Assert.Equal(20, bolts.BundleQuantity);
        Assert.Equal(100, bolts.Cost.CopperPieces);
        Assert.Equal(0.5m, bolts.Weight.Pounds);

        AmmunitionDefinition bullets =
            GetAmmunition(
                ammunition,
                "dnd5e2014.ammunition.sling-bullet");

        Assert.Equal(20, bullets.BundleQuantity);
        Assert.Equal(4, bullets.Cost.CopperPieces);
        Assert.Equal(0.5m, bullets.Weight.Pounds);
    }

    private static IReadOnlyList<AmmunitionDefinition> LoadAmmunition()
    {
        string path = Path.Combine(
            FindRepositoryRoot(),
            "Data",
            "dnd5e2014",
            "ammunition.json");

        return AmmunitionDefinitionLoader.LoadFromFile(path);
    }

    private static AmmunitionDefinition GetAmmunition(
        IReadOnlyList<AmmunitionDefinition> ammunition,
        string id)
    {
        return Assert.Single(
            ammunition.Where(
                item => item.Id == new AmmunitionTypeId(id)));
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
}
