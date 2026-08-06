using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Classes.FightingStyles.Serialization;

namespace FiveEData.Tests;

public sealed class FightingStyleDataFileTests
{
    private const string Fighter = "dnd5e2014.class.fighter";
    private const string Ranger = "dnd5e2014.class.ranger";
    private const string Paladin = "dnd5e2014.class.paladin";

    [Fact]
    public void CanonicalFile_ContainsExactFightingStyleClosure()
    {
        IReadOnlyList<FightingStyleDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.fighting-style.archery",
                "dnd5e2014.fighting-style.defense",
                "dnd5e2014.fighting-style.dueling",
                "dnd5e2014.fighting-style.great-weapon-fighting",
                "dnd5e2014.fighting-style.protection",
                "dnd5e2014.fighting-style.two-weapon-fighting"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void Archery_GrantsRangedAttackRollBonusToFighterAndRangerOnly()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.archery");

        Assert.Equal("Archery", definition.Name);
        AssertAvailableTo(definition, Fighter, Ranger);

        Assert.NotNull(definition.RollBonus);
        Assert.Equal(
            FightingStyleRollTarget.AttackRoll,
            definition.RollBonus!.Value.Target);
        Assert.Equal(2, definition.RollBonus.Value.Amount);
        Assert.Equal(
            FightingStyleWeaponRequirement.RangedWeapon,
            definition.RollBonus.Value.WeaponRequirement);
        Assert.Null(definition.ArmorClassBonus);
        Assert.Null(definition.DamageDieReroll);
        Assert.Null(definition.Reaction);
        Assert.False(definition.GrantsOffHandAbilityModifierDamage);
    }

    [Fact]
    public void Defense_GrantsArmorClassBonusToAllThreeClasses()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.defense");

        Assert.Equal("Defense", definition.Name);
        AssertAvailableTo(definition, Fighter, Ranger, Paladin);

        Assert.Equal(1, definition.ArmorClassBonus);
        Assert.Null(definition.RollBonus);
        Assert.Null(definition.DamageDieReroll);
        Assert.Null(definition.Reaction);
        Assert.False(definition.GrantsOffHandAbilityModifierDamage);
    }

    [Fact]
    public void Dueling_GrantsMeleeDamageRollBonusToAllThreeClasses()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.dueling");

        Assert.Equal("Dueling", definition.Name);
        AssertAvailableTo(definition, Fighter, Ranger, Paladin);

        Assert.NotNull(definition.RollBonus);
        Assert.Equal(
            FightingStyleRollTarget.DamageRoll,
            definition.RollBonus!.Value.Target);
        Assert.Equal(2, definition.RollBonus.Value.Amount);
        Assert.Equal(
            FightingStyleWeaponRequirement
                .MeleeWeaponWieldedAloneInOneHand,
            definition.RollBonus.Value.WeaponRequirement);
    }

    [Fact]
    public void GreatWeaponFighting_GrantsRerollToFighterAndPaladinOnly()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.great-weapon-fighting");

        Assert.Equal("Great Weapon Fighting", definition.Name);
        AssertAvailableTo(definition, Fighter, Paladin);

        Assert.NotNull(definition.DamageDieReroll);
        Assert.Equal(
            2,
            definition.DamageDieReroll!.Value.RerollAtOrBelowValue);
        Assert.Equal(
            FightingStyleWeaponRequirement
                .MeleeWeaponWithTwoHandedOrVersatileProperty,
            definition.DamageDieReroll.Value.WeaponRequirement);
    }

    [Fact]
    public void Protection_GrantsShieldReactionToFighterAndPaladinOnly()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.protection");

        Assert.Equal("Protection", definition.Name);
        AssertAvailableTo(definition, Fighter, Paladin);

        Assert.NotNull(definition.Reaction);
        Assert.Equal(5, definition.Reaction!.Value.Range.Feet);
        Assert.True(definition.Reaction.Value.RequiresShield);
    }

    [Fact]
    public void TwoWeaponFighting_GrantsOffHandDamageToFighterAndRangerOnly()
    {
        FightingStyleDefinition definition =
            Get("dnd5e2014.fighting-style.two-weapon-fighting");

        Assert.Equal("Two-Weapon Fighting", definition.Name);
        AssertAvailableTo(definition, Fighter, Ranger);

        Assert.True(definition.GrantsOffHandAbilityModifierDamage);
        Assert.Null(definition.RollBonus);
        Assert.Null(definition.ArmorClassBonus);
        Assert.Null(definition.DamageDieReroll);
        Assert.Null(definition.Reaction);
    }

    [Theory]
    [InlineData("dnd5e2014.fighting-style.archery", 72, 91)]
    [InlineData("dnd5e2014.fighting-style.defense", 72, 91, 85)]
    [InlineData("dnd5e2014.fighting-style.dueling", 72, 91, 85)]
    [InlineData(
        "dnd5e2014.fighting-style.great-weapon-fighting",
        72,
        85)]
    [InlineData("dnd5e2014.fighting-style.protection", 72, 85)]
    [InlineData(
        "dnd5e2014.fighting-style.two-weapon-fighting",
        72,
        91)]
    public void Sources_CitePhbFirstPrintingAtExpectedPages(
        string id,
        params int[] expectedPages)
    {
        FightingStyleDefinition definition = Get(id);

        Assert.Equal(
            expectedPages
                .Cast<int?>()
                .OrderBy(page => page),
            definition.Sources
                .Select(source => source.Page)
                .OrderBy(page => page));

        Assert.All(
            definition.Sources,
            source =>
                Assert.Equal(
                    "dnd5e2014.source.phb-first-printing",
                    source.DocumentId.Value));
    }

    private static void AssertAvailableTo(
        FightingStyleDefinition definition,
        params string[] expectedClassIds)
    {
        Assert.Equal(
            expectedClassIds.OrderBy(id => id, StringComparer.Ordinal),
            definition.AvailableToClassIds
                .Select(classId => classId.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    private static FightingStyleDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<FightingStyleDefinition>
        LoadCanonical()
    {
        return FightingStyleDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "fighting-styles.json"));
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
