using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;

namespace FiveEData.Tests;

public sealed class ArmorDataFileTests
{
    [Fact]
    public void CanonicalArmorFile_ContainsAllTwelvePhbArmorTypes()
    {
        IReadOnlyList<ArmorDefinition> armor = LoadCanonicalArmor();

        Assert.Equal(12, armor.Count);
        Assert.Equal(3, armor.Count(item => item.Category == ArmorCategory.Light));
        Assert.Equal(5, armor.Count(item => item.Category == ArmorCategory.Medium));
        Assert.Equal(4, armor.Count(item => item.Category == ArmorCategory.Heavy));
        Assert.Equal(12, armor.Select(item => item.Id).Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(CanonicalArmorData))]
    public void CanonicalArmorFile_MatchesFirstPrintingArmorTable(
        string id,
        string name,
        ArmorCategory category,
        long copperPieces,
        decimal pounds,
        int baseArmorClass,
        bool includesDexterityModifier,
        int? maximumDexterityModifier,
        int? minimumStrengthForFullSpeed,
        bool imposesStealthDisadvantage)
    {
        ArmorDefinition definition = LoadCanonicalArmor()
            .Single(item => item.Id == new ArmorId(id));

        Assert.Equal(name, definition.Name);
        Assert.Equal(category, definition.Category);
        Assert.Equal(copperPieces, definition.Cost.CopperPieces);
        Assert.Equal(pounds, definition.Weight.Pounds);
        Assert.Equal(baseArmorClass, definition.ArmorClass.BaseArmorClass);
        Assert.Equal(
            includesDexterityModifier,
            definition.ArmorClass.IncludesDexterityModifier);
        Assert.Equal(
            maximumDexterityModifier,
            definition.ArmorClass.MaximumDexterityModifier);
        Assert.Equal(
            minimumStrengthForFullSpeed,
            definition.MinimumStrengthForFullSpeed);
        Assert.Equal(
            imposesStealthDisadvantage,
            definition.ImposesStealthDisadvantage);

        var source = Assert.Single(definition.Sources);
        Assert.Equal(145, source.Page);
        Assert.Equal(
            "Chapter 5: Equipment — Armor and Shields",
            source.Section);
    }

    public static IEnumerable<object?[]> CanonicalArmorData()
    {
        yield return
        [
            "dnd5e2014.armor.padded", "Padded", ArmorCategory.Light,
            500L, 8m, 11, true, null, null, true
        ];
        yield return
        [
            "dnd5e2014.armor.leather", "Leather", ArmorCategory.Light,
            1000L, 10m, 11, true, null, null, false
        ];
        yield return
        [
            "dnd5e2014.armor.studded-leather", "Studded leather",
            ArmorCategory.Light, 4500L, 13m, 12, true, null, null, false
        ];
        yield return
        [
            "dnd5e2014.armor.hide", "Hide", ArmorCategory.Medium,
            1000L, 12m, 12, true, 2, null, false
        ];
        yield return
        [
            "dnd5e2014.armor.chain-shirt", "Chain shirt", ArmorCategory.Medium,
            5000L, 20m, 13, true, 2, null, false
        ];
        yield return
        [
            "dnd5e2014.armor.scale-mail", "Scale mail", ArmorCategory.Medium,
            5000L, 45m, 14, true, 2, null, true
        ];
        yield return
        [
            "dnd5e2014.armor.breastplate", "Breastplate", ArmorCategory.Medium,
            40000L, 20m, 14, true, 2, null, false
        ];
        yield return
        [
            "dnd5e2014.armor.half-plate", "Half plate", ArmorCategory.Medium,
            75000L, 40m, 15, true, 2, null, true
        ];
        yield return
        [
            "dnd5e2014.armor.ring-mail", "Ring mail", ArmorCategory.Heavy,
            3000L, 40m, 14, false, null, null, true
        ];
        yield return
        [
            "dnd5e2014.armor.chain-mail", "Chain mail", ArmorCategory.Heavy,
            7500L, 55m, 16, false, null, 13, true
        ];
        yield return
        [
            "dnd5e2014.armor.splint", "Splint", ArmorCategory.Heavy,
            20000L, 60m, 17, false, null, 15, true
        ];
        yield return
        [
            "dnd5e2014.armor.plate", "Plate", ArmorCategory.Heavy,
            150000L, 65m, 18, false, null, 15, true
        ];
    }

    private static IReadOnlyList<ArmorDefinition> LoadCanonicalArmor()
    {
        string root = FindRepositoryRoot();

        return ArmorDefinitionLoader.LoadFromFile(
            Path.Combine(root, "Data", "dnd5e2014", "armor.json"));
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
