using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Spells.Concentration;
using FiveEData.Rules.Spells.Concentration.Serialization;

namespace FiveEData.Tests;

public sealed class ConcentrationRulesTests
{
    [Fact]
    public void CanonicalFile_MatchesThePrintedRule()
    {
        ConcentrationRules rules = LoadCanonical();

        Assert.Equal(
            "dnd5e2014.ability.constitution",
            rules.SavingThrowAbilityId.Value);
        Assert.Equal(10, rules.MinimumSavingThrowDC);
        Assert.Equal(2, rules.DamageDivisorForSavingThrowDC);
        Assert.True(rules.SeparateSavingThrowPerDamageSource);
        Assert.True(rules.EndsWhenCastingAnotherConcentrationSpell);
        Assert.True(rules.EndsOnDeath);
        Assert.True(rules.CanBeEndedVoluntarilyWithoutAnAction);
        Assert.Equal(
            ["dnd5e2014.condition.incapacitated"],
            rules.EndedByConditionIds.Select(id => id.Value));
    }

    [Fact]
    public void CanonicalFile_IsCitedWhereTheSectionStarts()
    {
        // The section's heading and body begin on p.203; only the DM's
        // environmental-phenomena paragraph spills onto p.204, so the
        // citation is p.203 per the standing "cite where the body text
        // starts" rule.
        SourceReference source = Assert.Single(LoadCanonical().Sources);

        Assert.Equal(203, source.Page);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
    }

    [Fact]
    public void CanonicalFile_StoresTheDamageDivisorNotAPrecomputedDC()
    {
        // "10 or half the damage you take, whichever number is higher" is
        // the only genuinely non-derivable constant in this section, and
        // it is stored as the printed floor plus the printed divisor.
        ConcentrationRules rules = LoadCanonical();

        Assert.Equal(
            10,
            Math.Max(
                rules.MinimumSavingThrowDC,
                14 / rules.DamageDivisorForSavingThrowDC));
        Assert.Equal(
            15,
            Math.Max(
                rules.MinimumSavingThrowDC,
                30 / rules.DamageDivisorForSavingThrowDC));
    }

    [Fact]
    public void PublishedRuleset_ExposesTheSameRulesAsTheFile()
    {
        ConcentrationRules published =
            Dnd5e2014Ruleset.Instance.Concentration;
        ConcentrationRules file = LoadCanonical();

        Assert.Equal(
            file.SavingThrowAbilityId.Value,
            published.SavingThrowAbilityId.Value);
        Assert.Equal(file.MinimumSavingThrowDC, published.MinimumSavingThrowDC);
        Assert.Equal(
            file.EndedByConditionIds.Select(id => id.Value),
            published.EndedByConditionIds.Select(id => id.Value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsANonPositiveMinimumDC(int dc)
    {
        Assert.Contains(
            ConcentrationRulesValidator.Validate(Create(minimumSavingThrowDC: dc)),
            error => error.Contains("minimum saving throw DC"));
    }

    [Fact]
    public void Validator_RejectsANonPositiveDamageDivisor()
    {
        Assert.Contains(
            ConcentrationRulesValidator.Validate(
                Create(damageDivisorForSavingThrowDC: 0)),
            error => error.Contains("damage divisor"));
    }

    [Fact]
    public void Validator_RejectsNoEndingCondition()
    {
        Assert.Contains(
            ConcentrationRulesValidator.Validate(Create(conditionIds: [])),
            error => error.Contains("at least one condition"));
    }

    [Fact]
    public void Validator_RejectsARepeatedEndingCondition()
    {
        Assert.Contains(
            ConcentrationRulesValidator.Validate(
                Create(conditionIds:
                [
                    "dnd5e2014.condition.incapacitated",
                    "dnd5e2014.condition.incapacitated"
                ])),
            error => error.Contains("must not repeat"));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        Assert.Contains(
            ConcentrationRulesValidator.Validate(Create(withSource: false)),
            error => error.Contains("at least one source"));
    }

    [Fact]
    public void Loader_RejectsAnUnknownProperty()
    {
        Assert.Throws<InvalidDataException>(() =>
            ConcentrationRulesLoader.LoadFromJson(
                """
                {
                  "savingThrowAbilityId": "dnd5e2014.ability.constitution",
                  "minimumSavingThrowDC": 10,
                  "damageDivisorForSavingThrowDC": 2,
                  "separateSavingThrowPerDamageSource": true,
                  "endsWhenCastingAnotherConcentrationSpell": true,
                  "endsOnDeath": true,
                  "canBeEndedVoluntarilyWithoutAnAction": true,
                  "endedByConditionIds": [],
                  "sources": [],
                  "unexpected": true
                }
                """));
    }

    [Fact]
    public void Loader_RejectsAMissingRequiredMember()
    {
        Assert.Throws<InvalidDataException>(() =>
            ConcentrationRulesLoader.LoadFromJson(
                """
                {
                  "savingThrowAbilityId": "dnd5e2014.ability.constitution"
                }
                """));
    }

    private static ConcentrationRules Create(
        int minimumSavingThrowDC = 10,
        int damageDivisorForSavingThrowDC = 2,
        string[]? conditionIds = null,
        bool withSource = true)
    {
        return new ConcentrationRules(
            new AbilityId("dnd5e2014.ability.constitution"),
            minimumSavingThrowDC,
            damageDivisorForSavingThrowDC,
            separateSavingThrowPerDamageSource: true,
            endsWhenCastingAnotherConcentrationSpell: true,
            endsOnDeath: true,
            canBeEndedVoluntarilyWithoutAnAction: true,
            (conditionIds ?? ["dnd5e2014.condition.incapacitated"])
                .Select(value => new ConditionId(value)),
            withSource
                ?
                [
                    new SourceReference(
                        new SourceDocumentId(
                            "dnd5e2014.source.phb-first-printing"),
                        203,
                        "Chapter 10: Spellcasting — Concentration")
                ]
                : []);
    }

    private static ConcentrationRules LoadCanonical() =>
        ConcentrationRulesLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "concentration.json"));

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
