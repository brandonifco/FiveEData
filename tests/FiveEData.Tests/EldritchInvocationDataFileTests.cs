using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.EldritchInvocations.Serialization;

namespace FiveEData.Tests;

public sealed class EldritchInvocationDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactInvocationClosure()
    {
        IReadOnlyList<EldritchInvocationDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.eldritch-invocation.agonizing-blast",
                "dnd5e2014.eldritch-invocation.armor-of-shadows",
                "dnd5e2014.eldritch-invocation.ascendant-step",
                "dnd5e2014.eldritch-invocation.beast-speech",
                "dnd5e2014.eldritch-invocation.beguiling-influence",
                "dnd5e2014.eldritch-invocation.bewitching-whispers",
                "dnd5e2014.eldritch-invocation.book-of-ancient-secrets",
                "dnd5e2014.eldritch-invocation.chains-of-carceri",
                "dnd5e2014.eldritch-invocation.devils-sight",
                "dnd5e2014.eldritch-invocation.dreadful-word",
                "dnd5e2014.eldritch-invocation.eldritch-sight",
                "dnd5e2014.eldritch-invocation.eldritch-spear",
                "dnd5e2014.eldritch-invocation.eyes-of-the-rune-keeper",
                "dnd5e2014.eldritch-invocation.fiendish-vigor",
                "dnd5e2014.eldritch-invocation.gaze-of-two-minds",
                "dnd5e2014.eldritch-invocation.lifedrinker",
                "dnd5e2014.eldritch-invocation.mask-of-many-faces",
                "dnd5e2014.eldritch-invocation.master-of-myriad-forms",
                "dnd5e2014.eldritch-invocation.minions-of-chaos",
                "dnd5e2014.eldritch-invocation.mire-the-mind",
                "dnd5e2014.eldritch-invocation.misty-visions",
                "dnd5e2014.eldritch-invocation.one-with-shadows",
                "dnd5e2014.eldritch-invocation.otherworldly-leap",
                "dnd5e2014.eldritch-invocation.repelling-blast",
                "dnd5e2014.eldritch-invocation.sculptor-of-flesh",
                "dnd5e2014.eldritch-invocation.sign-of-ill-omen",
                "dnd5e2014.eldritch-invocation.thief-of-five-fates",
                "dnd5e2014.eldritch-invocation.thirsting-blade",
                "dnd5e2014.eldritch-invocation.visions-of-distant-realms",
                "dnd5e2014.eldritch-invocation.voice-of-the-chain-master",
                "dnd5e2014.eldritch-invocation.whispers-of-the-grave",
                "dnd5e2014.eldritch-invocation.witch-sight"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(
        "dnd5e2014.eldritch-invocation.agonizing-blast",
        true,
        null,
        null)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.eldritch-spear",
        true,
        null,
        null)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.repelling-blast",
        true,
        null,
        null)]
    public void CantripGatedInvocation_HasExpectedPrerequisites(
        string id,
        bool expectedRequiresCantrip,
        int? expectedLevel,
        WarlockPactBoon? expectedPactBoon)
    {
        EldritchInvocationDefinition definition = Get(id);

        Assert.Equal(
            expectedRequiresCantrip,
            definition.RequiresEldritchBlastCantrip);
        Assert.Equal(expectedLevel, definition.RequiredMinimumLevel);
        Assert.Equal(expectedPactBoon, definition.RequiresPactBoon);
    }

    [Theory]
    [InlineData("dnd5e2014.eldritch-invocation.ascendant-step", 9)]
    [InlineData("dnd5e2014.eldritch-invocation.bewitching-whispers", 7)]
    [InlineData("dnd5e2014.eldritch-invocation.dreadful-word", 7)]
    [InlineData("dnd5e2014.eldritch-invocation.master-of-myriad-forms", 15)]
    [InlineData("dnd5e2014.eldritch-invocation.minions-of-chaos", 9)]
    [InlineData("dnd5e2014.eldritch-invocation.mire-the-mind", 5)]
    [InlineData("dnd5e2014.eldritch-invocation.one-with-shadows", 5)]
    [InlineData("dnd5e2014.eldritch-invocation.otherworldly-leap", 9)]
    [InlineData("dnd5e2014.eldritch-invocation.sculptor-of-flesh", 7)]
    [InlineData("dnd5e2014.eldritch-invocation.sign-of-ill-omen", 5)]
    [InlineData("dnd5e2014.eldritch-invocation.visions-of-distant-realms", 15)]
    [InlineData("dnd5e2014.eldritch-invocation.whispers-of-the-grave", 9)]
    [InlineData("dnd5e2014.eldritch-invocation.witch-sight", 15)]
    public void LevelGatedInvocation_HasExpectedMinimumLevelOnly(
        string id,
        int expectedLevel)
    {
        EldritchInvocationDefinition definition = Get(id);

        Assert.False(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(expectedLevel, definition.RequiredMinimumLevel);
        Assert.Null(definition.RequiresPactBoon);
    }

    [Theory]
    [InlineData(
        "dnd5e2014.eldritch-invocation.book-of-ancient-secrets",
        null,
        WarlockPactBoon.Tome)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.voice-of-the-chain-master",
        null,
        WarlockPactBoon.Chain)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.chains-of-carceri",
        15,
        WarlockPactBoon.Chain)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.lifedrinker",
        12,
        WarlockPactBoon.Blade)]
    [InlineData(
        "dnd5e2014.eldritch-invocation.thirsting-blade",
        5,
        WarlockPactBoon.Blade)]
    public void PactBoonGatedInvocation_HasExpectedPrerequisites(
        string id,
        int? expectedLevel,
        WarlockPactBoon expectedPactBoon)
    {
        EldritchInvocationDefinition definition = Get(id);

        Assert.False(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(expectedLevel, definition.RequiredMinimumLevel);
        Assert.Equal(expectedPactBoon, definition.RequiresPactBoon);
    }

    [Fact]
    public void ElevenInvocations_HaveNoPrerequisitesAtAll()
    {
        IReadOnlyList<EldritchInvocationDefinition> unrestricted =
            LoadCanonical()
                .Where(
                    definition =>
                        !definition.RequiresEldritchBlastCantrip &&
                        definition.RequiredMinimumLevel is null &&
                        definition.RequiresPactBoon is null)
                .ToArray();

        Assert.Equal(11, unrestricted.Count);
    }

    [Fact]
    public void AllInvocations_CitePhbFirstPrintingPage110Or111()
    {
        foreach (EldritchInvocationDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.True(
                source.Page is 110 or 111,
                $"{definition.Id} cited page {source.Page}, expected 110 " +
                "or 111.");
        }
    }

    private static EldritchInvocationDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<EldritchInvocationDefinition>
        LoadCanonical()
    {
        return EldritchInvocationDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "eldritch-invocations.json"));
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
