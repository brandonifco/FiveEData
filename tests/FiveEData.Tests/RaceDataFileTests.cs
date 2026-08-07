using FiveEData.Rules.Catalog;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.BreathWeapon;
using FiveEData.Rules.Creatures.Races.Lucky;
using FiveEData.Rules.Creatures.Races.RelentlessEndurance;
using FiveEData.Rules.Creatures.Races.SavageAttacks;
using FiveEData.Rules.Creatures.Races.Serialization;

namespace FiveEData.Tests;

public sealed class RaceDataFileTests
{
    private static readonly string[] ExpectedRaceIds =
    [
        "dnd5e2014.race.dragonborn",
        "dnd5e2014.race.dwarf",
        "dnd5e2014.race.elf",
        "dnd5e2014.race.gnome",
        "dnd5e2014.race.half-elf",
        "dnd5e2014.race.half-orc",
        "dnd5e2014.race.halfling",
        "dnd5e2014.race.human",
        "dnd5e2014.race.tiefling"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactRaceClosure()
    {
        IReadOnlyList<RaceDefinition> races = LoadRaces();

        Assert.Equal(9, races.Count);
        Assert.Equal(
            ExpectedRaceIds.OrderBy(id => id, StringComparer.Ordinal),
            races
                .Select(race => race.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_PreservesDwarfMechanics()
    {
        RaceDefinition dwarf =
            GetRace(LoadRaces(), "dnd5e2014.race.dwarf");

        Assert.Equal("Dwarf", dwarf.Name);
        Assert.Equal("dnd5e2014.creature-size.medium", dwarf.Size.Value);
        Assert.Equal(25, dwarf.Speed.Feet);

        RaceAbilityScoreIncrease increase =
            Assert.Single(dwarf.AbilityScoreIncreases);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            increase.AbilityId.Value);
        Assert.Equal(2, increase.Bonus);

        Assert.Equal(0, dwarf.ChoosableAbilityScoreIncreaseCount);
        Assert.Equal(
            [
                "dnd5e2014.language.common",
                "dnd5e2014.language.dwarvish"
            ],
            dwarf.LanguageIds.Select(id => id.Value).ToArray());
        Assert.Equal(0, dwarf.AdditionalLanguageChoiceCount);

        Assert.Equal(
            [
                "dnd5e2014.race-rule.darkvision",
                "dnd5e2014.race-rule.dwarven-resilience",
                "dnd5e2014.race-rule.dwarven-combat-training",
                "dnd5e2014.race-rule.dwarf-tool-proficiency",
                "dnd5e2014.race-rule.stonecunning"
            ],
            dwarf.TraitRuleIds.Select(id => id.Value).ToArray());

        var source = Assert.Single(dwarf.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(20, source.Page);
        Assert.Equal("Chapter 2: Races", source.Section);

        Assert.Equal(60, dwarf.DarkvisionRangeFeet);
        Assert.Equal(
            "dnd5e2014.damage-type.poison",
            Assert.Single(dwarf.ResistedDamageTypeIds).Value);
        Assert.Null(dwarf.TranceDurationHours);
        Assert.Null(dwarf.BreathWeaponProgression);
    }

    [Fact]
    public void CanonicalFile_PreservesElfTrance()
    {
        RaceDefinition elf = GetRace(LoadRaces(), "dnd5e2014.race.elf");

        Assert.Equal(60, elf.DarkvisionRangeFeet);
        Assert.Empty(elf.ResistedDamageTypeIds);
        Assert.Equal(4, elf.TranceDurationHours);
    }

    [Fact]
    public void CanonicalFile_PreservesTieflingFireResistance()
    {
        RaceDefinition tiefling =
            GetRace(LoadRaces(), "dnd5e2014.race.tiefling");

        Assert.Equal(60, tiefling.DarkvisionRangeFeet);
        Assert.Equal(
            "dnd5e2014.damage-type.fire",
            Assert.Single(tiefling.ResistedDamageTypeIds).Value);
    }

    [Fact]
    public void CanonicalFile_PreservesDragonbornBreathWeaponProgression()
    {
        RaceDefinition dragonborn =
            GetRace(LoadRaces(), "dnd5e2014.race.dragonborn");

        Assert.Null(dragonborn.DarkvisionRangeFeet);
        Assert.Empty(dragonborn.ResistedDamageTypeIds);

        BreathWeaponProgressionDetail breathWeapon =
            dragonborn.BreathWeaponProgression
            ?? throw new InvalidOperationException(
                "Expected Dragonborn to have a Breath Weapon progression.");

        Assert.Equal(
            [(1, 2), (6, 3), (11, 4), (16, 5)],
            breathWeapon.DamageByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Damage.Count)));
        Assert.All(
            breathWeapon.DamageByLevel,
            grant => Assert.Equal(6, grant.Damage.Sides));
        Assert.True(breathWeapon.RecoversOnShortRest);
    }

    [Theory]
    [InlineData("dnd5e2014.race.halfling")]
    [InlineData("dnd5e2014.race.human")]
    public void RacesWithNoDarkvision_HaveNullDarkvisionRangeFeet(string id)
    {
        RaceDefinition race = GetRace(LoadRaces(), id);

        Assert.Null(race.DarkvisionRangeFeet);
    }

    [Fact]
    public void CanonicalFile_PreservesHumanHasNoNamedTraits()
    {
        RaceDefinition human =
            GetRace(LoadRaces(), "dnd5e2014.race.human");

        Assert.Equal(6, human.AbilityScoreIncreases.Count);
        Assert.All(
            human.AbilityScoreIncreases,
            increase => Assert.Equal(1, increase.Bonus));
        Assert.Equal(
            6,
            human.AbilityScoreIncreases
                .Select(increase => increase.AbilityId)
                .Distinct()
                .Count());
        Assert.Equal(1, human.AdditionalLanguageChoiceCount);
        Assert.Empty(human.TraitRuleIds);
    }

    [Fact]
    public void CanonicalFile_PreservesHalfElfChoosableAbilityScoreIncrease()
    {
        RaceDefinition halfElf =
            GetRace(LoadRaces(), "dnd5e2014.race.half-elf");

        RaceAbilityScoreIncrease increase =
            Assert.Single(halfElf.AbilityScoreIncreases);
        Assert.Equal("dnd5e2014.ability.charisma", increase.AbilityId.Value);
        Assert.Equal(2, increase.Bonus);
        Assert.Equal(2, halfElf.ChoosableAbilityScoreIncreaseCount);
        Assert.Equal(1, halfElf.AdditionalLanguageChoiceCount);
    }

    [Fact]
    public void CanonicalFile_PreservesHalflingHasNoDarkvision()
    {
        RaceDefinition halfling =
            GetRace(LoadRaces(), "dnd5e2014.race.halfling");

        Assert.DoesNotContain(
            halfling.TraitRuleIds,
            id => id.Value == "dnd5e2014.race-rule.darkvision");
        Assert.Equal("dnd5e2014.creature-size.small", halfling.Size.Value);
    }

    [Fact]
    public void CanonicalFile_PreservesHalfOrcQuantizedTraits()
    {
        RaceDefinition halfOrc =
            GetRace(LoadRaces(), "dnd5e2014.race.half-orc");

        SavageAttacksDetail savageAttacks =
            halfOrc.SavageAttacks
            ?? throw new InvalidOperationException(
                "Expected Half-Orc to have Savage Attacks.");
        Assert.Equal(1, savageAttacks.AdditionalCriticalDice);
        Assert.True(savageAttacks.RequiresMeleeWeapon);

        RelentlessEnduranceDetail relentlessEndurance =
            halfOrc.RelentlessEndurance
            ?? throw new InvalidOperationException(
                "Expected Half-Orc to have Relentless Endurance.");
        Assert.Equal(1, relentlessEndurance.HitPointsRetained);
        Assert.True(relentlessEndurance.RecoversOnLongRest);
    }

    [Fact]
    public void CanonicalFile_PreservesHalflingLucky()
    {
        RaceDefinition halfling =
            GetRace(LoadRaces(), "dnd5e2014.race.halfling");

        LuckyDetail lucky =
            halfling.Lucky
            ?? throw new InvalidOperationException(
                "Expected Halfling to have Lucky.");
        Assert.Equal(1, lucky.RerollOnNaturalRoll);
        Assert.True(lucky.MustUseNewRoll);
    }

    // Savage Attacks counts additional weapon damage dice on a critical hit,
    // the same shape as Barbarian's Brutal Critical, and stores a count
    // rather than a DiceExpression for the same reason: the die size comes
    // from the weapon, not the feature.
    [Fact]
    public void CanonicalFile_SavageAttacksCountsDiceWithoutNamingADieSize()
    {
        RaceDefinition halfOrc =
            GetRace(LoadRaces(), "dnd5e2014.race.half-orc");

        SavageAttacksDetail savageAttacks =
            halfOrc.SavageAttacks
            ?? throw new InvalidOperationException(
                "Expected Half-Orc to have Savage Attacks.");

        Assert.Equal(1, savageAttacks.AdditionalCriticalDice);
    }

    // Rock Gnome's Tinker carries real numbers - 1 hour and 10 gp to build,
    // AC 5, 1 hp, 24 hours, up to three active - but they describe a
    // constructed object, and objects are not a modeled domain here. Its
    // three device options are per-option effect prose besides. Declined and
    // left to the citation, the same line Pact Boon and Indomitable Might
    // sit on; revisit if a crafting or object domain ever exists.
    [Fact]
    public void CanonicalFile_RockGnomeTinkerStaysCitationOnly()
    {
        SubraceDefinition rockGnome = Dnd5e2014Ruleset.Instance.Subraces
            .Get(new SubraceId("dnd5e2014.subrace.rock-gnome"));

        Assert.Contains(
            rockGnome.TraitRuleIds,
            ruleId => ruleId.Value == "dnd5e2014.race-rule.tinker");
    }

    [Theory]
    [InlineData("dnd5e2014.race.dwarf")]
    [InlineData("dnd5e2014.race.elf")]
    [InlineData("dnd5e2014.race.halfling")]
    [InlineData("dnd5e2014.race.human")]
    [InlineData("dnd5e2014.race.dragonborn")]
    [InlineData("dnd5e2014.race.gnome")]
    [InlineData("dnd5e2014.race.half-elf")]
    [InlineData("dnd5e2014.race.tiefling")]
    public void CanonicalFile_NonHalfOrcRaceDeclaresNoHalfOrcTraits(
        string raceId)
    {
        RaceDefinition race = GetRace(LoadRaces(), raceId);

        Assert.Null(race.SavageAttacks);
        Assert.Null(race.RelentlessEndurance);
    }

    [Theory]
    [InlineData("dnd5e2014.race.dwarf")]
    [InlineData("dnd5e2014.race.elf")]
    [InlineData("dnd5e2014.race.human")]
    [InlineData("dnd5e2014.race.dragonborn")]
    [InlineData("dnd5e2014.race.gnome")]
    [InlineData("dnd5e2014.race.half-elf")]
    [InlineData("dnd5e2014.race.half-orc")]
    [InlineData("dnd5e2014.race.tiefling")]
    public void CanonicalFile_NonHalflingRaceDeclaresNoLucky(string raceId)
    {
        RaceDefinition race = GetRace(LoadRaces(), raceId);

        Assert.Null(race.Lucky);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedRaceTailScalars()
    {
        RaceCatalog catalog = Dnd5e2014Ruleset.Instance.Races;

        Assert.Equal(
            1,
            (catalog.Get(new RaceId("dnd5e2014.race.half-orc")).SavageAttacks
                ?? throw new InvalidOperationException(
                    "Expected Half-Orc to have Savage Attacks."))
                .AdditionalCriticalDice);
        Assert.Equal(
            1,
            (catalog.Get(new RaceId("dnd5e2014.race.halfling")).Lucky
                ?? throw new InvalidOperationException(
                    "Expected Halfling to have Lucky.")).RerollOnNaturalRoll);
    }

    private static RaceDefinition GetRace(
        IReadOnlyList<RaceDefinition> races,
        string id)
    {
        return races.Single(race => race.Id.Value == id);
    }

    private static IReadOnlyList<RaceDefinition> LoadRaces()
    {
        return RaceDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "races.json"));
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
