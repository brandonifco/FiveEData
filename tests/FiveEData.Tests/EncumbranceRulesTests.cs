using FiveEData.Rules.Characters.Encumbrance;
using FiveEData.Rules.Characters.Encumbrance.Serialization;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class EncumbranceRulesTests
{
    [Fact]
    public void CanonicalFile_MatchesThePrintedSizeMultipliers()
    {
        EncumbranceRules rules = LoadCanonical();

        Assert.Equal(
            new Dictionary<string, double>
            {
                ["dnd5e2014.creature-size.tiny"] = 0.5,
                ["dnd5e2014.creature-size.small"] = 1,
                ["dnd5e2014.creature-size.medium"] = 1,
                ["dnd5e2014.creature-size.large"] = 2,
                ["dnd5e2014.creature-size.huge"] = 4,
                ["dnd5e2014.creature-size.gargantuan"] = 8
            },
            rules.SizeCarryingCapacityMultipliers.ToDictionary(
                grant => grant.SizeId.Value,
                grant => grant.Multiplier));
    }

    [Fact]
    public void CanonicalFile_MatchesThePrintedThresholds()
    {
        EncumbranceRules rules = LoadCanonical();

        Assert.Equal(5, rules.EncumberedCarryingCapacityMultiplier);
        Assert.Equal(10, rules.EncumberedSpeedReductionFeet);
        Assert.Equal(10, rules.HeavilyEncumberedCarryingCapacityMultiplier);
        Assert.Equal(20, rules.HeavilyEncumberedSpeedReductionFeet);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity",
                "dnd5e2014.ability.constitution"
            ],
            rules.HeavilyEncumberedDisadvantageAbilityIds
                .Select(id => id.Value));
    }

    [Fact]
    public void CanonicalFile_IsCitedWhereTheRulesStart()
    {
        // Both facts sit on p.176: the Size and Strength rule under
        // Lifting and Carrying, and the Variant: Encumbrance sidebar.
        EncumbranceRules rules = LoadCanonical();

        Assert.All(rules.Sources, source => Assert.Equal(176, source.Page));
        Assert.Equal(2, rules.Sources.Count);
    }

    [Fact]
    public void PublishedRuleset_ExposesTheSameRulesAsTheFile()
    {
        EncumbranceRules published = Dnd5e2014Ruleset.Instance.Encumbrance;
        EncumbranceRules file = LoadCanonical();

        Assert.Equal(
            file.SizeCarryingCapacityMultipliers.Select(
                grant => (grant.SizeId.Value, grant.Multiplier)),
            published.SizeCarryingCapacityMultipliers.Select(
                grant => (grant.SizeId.Value, grant.Multiplier)));
        Assert.Equal(
            file.HeavilyEncumberedDisadvantageAbilityIds.Select(
                id => id.Value),
            published.HeavilyEncumberedDisadvantageAbilityIds.Select(
                id => id.Value));
    }

    [Fact]
    public void Validator_RejectsNoSizeMultipliers()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(Create(sizeIds: [])),
            error => error.Contains("at least one size"));
    }

    [Fact]
    public void Validator_RejectsARepeatedSize()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(
                    sizeIds:
                    [
                        "dnd5e2014.creature-size.medium",
                        "dnd5e2014.creature-size.medium"
                    ])),
            error => error.Contains("must not repeat a creature size"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsANonPositiveEncumberedMultiplier(int value)
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(encumberedCarryingCapacityMultiplier: value)),
            error =>
                error.Contains(
                    "Encumbered carrying capacity multiplier must be " +
                    "positive"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsANonPositiveEncumberedSpeedReduction(
        int value)
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(encumberedSpeedReductionFeet: value)),
            error => error.Contains("Encumbered speed reduction"));
    }

    [Fact]
    public void Validator_RejectsAHeavilyEncumberedMultiplierAtOrBelowEncumbered()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(
                    encumberedCarryingCapacityMultiplier: 5,
                    heavilyEncumberedCarryingCapacityMultiplier: 5)),
            error =>
                error.Contains(
                    "Heavily encumbered carrying capacity multiplier " +
                    "must exceed"));
    }

    [Fact]
    public void Validator_RejectsAHeavilyEncumberedSpeedReductionAtOrBelowEncumbered()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(
                    encumberedSpeedReductionFeet: 10,
                    heavilyEncumberedSpeedReductionFeet: 10)),
            error =>
                error.Contains(
                    "Heavily encumbered speed reduction must exceed"));
    }

    [Fact]
    public void Validator_RejectsNoDisadvantageAbilities()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(disadvantageAbilityIds: [])),
            error => error.Contains("at least one ability"));
    }

    [Fact]
    public void Validator_RejectsARepeatedDisadvantageAbility()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(
                Create(
                    disadvantageAbilityIds:
                    [
                        "dnd5e2014.ability.strength",
                        "dnd5e2014.ability.strength"
                    ])),
            error => error.Contains("must not repeat an ability"));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        Assert.Contains(
            EncumbranceRulesValidator.Validate(Create(withSource: false)),
            error => error.Contains("at least one source"));
    }

    [Fact]
    public void Loader_RejectsAnUnknownProperty()
    {
        Assert.Throws<InvalidDataException>(() =>
            EncumbranceRulesLoader.LoadFromJson(
                """
                {
                  "sizeCarryingCapacityMultipliers": [],
                  "encumberedCarryingCapacityMultiplier": 5,
                  "encumberedSpeedReductionFeet": 10,
                  "heavilyEncumberedCarryingCapacityMultiplier": 10,
                  "heavilyEncumberedSpeedReductionFeet": 20,
                  "heavilyEncumberedDisadvantageAbilityIds": [],
                  "sources": [],
                  "unexpected": true
                }
                """));
    }

    [Fact]
    public void Loader_RejectsAMissingRequiredMember()
    {
        Assert.Throws<InvalidDataException>(() =>
            EncumbranceRulesLoader.LoadFromJson(
                """
                {
                  "encumberedCarryingCapacityMultiplier": 5
                }
                """));
    }

    private static EncumbranceRules Create(
        string[]? sizeIds = null,
        int encumberedCarryingCapacityMultiplier = 5,
        int encumberedSpeedReductionFeet = 10,
        int heavilyEncumberedCarryingCapacityMultiplier = 10,
        int heavilyEncumberedSpeedReductionFeet = 20,
        string[]? disadvantageAbilityIds = null,
        bool withSource = true)
    {
        return new EncumbranceRules(
            (sizeIds ?? ["dnd5e2014.creature-size.medium"])
                .Select(
                    value => new CarryingCapacitySizeMultiplierGrant(
                        new CreatureSizeId(value),
                        1)),
            encumberedCarryingCapacityMultiplier,
            encumberedSpeedReductionFeet,
            heavilyEncumberedCarryingCapacityMultiplier,
            heavilyEncumberedSpeedReductionFeet,
            (disadvantageAbilityIds ?? ["dnd5e2014.ability.strength"])
                .Select(value => new AbilityId(value)),
            withSource
                ?
                [
                    new SourceReference(
                        new SourceDocumentId(
                            "dnd5e2014.source.phb-first-printing"),
                        176,
                        "Chapter 7: Using Ability Scores — Strength — " +
                            "Variant: Encumbrance")
                ]
                : []);
    }

    private static EncumbranceRules LoadCanonical() =>
        EncumbranceRulesLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "encumbrance.json"));

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

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
