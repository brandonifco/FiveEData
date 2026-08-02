using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;

namespace FiveEData.Tests;

public sealed class OfficialLifestyleSemanticIntegrityTests
{
    [Fact]
    public void CanonicalDefinitions_HaveNoErrors()
    {
        Assert.Empty(
            OfficialLifestyleSemanticValidator.Validate(
                LoadCanonical()));
    }

    [Fact]
    public void MissingDefinition_IsRejected()
    {
        IReadOnlyList<LifestyleDefinition> altered =
            LoadCanonical()
                .Where(
                    definition =>
                        definition.Id.Value !=
                        "dnd5e2014.lifestyle.wealthy")
                .ToArray();

        IReadOnlyList<string> errors =
            OfficialLifestyleSemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 7 definitions; found 6",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.lifestyle.wealthy'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredPaidLifestyle_IsRejected()
    {
        IReadOnlyList<LifestyleDefinition> canonical =
            LoadCanonical();

        LifestyleDefinition modest =
            canonical.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.lifestyle.modest");

        var alteredModest =
            new LifestyleDefinition(
                modest.Id,
                "Moderate",
                new ListedCost(
                    new Money(101),
                    ListedCostKind.Minimum),
                modest.SpecialRuleIds,
                modest.Sources);

        IReadOnlyList<string> errors =
            OfficialLifestyleSemanticValidator.Validate(
                canonical
                    .Select(
                        definition =>
                            definition.Id == modest.Id
                                ? alteredModest
                                : definition)
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Modest'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must cost 100 cp per day; found 101 cp",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must use cost kind 'Exact'; found 'Minimum'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void WretchedListedCost_IsRejected()
    {
        IReadOnlyList<LifestyleDefinition> canonical =
            LoadCanonical();

        LifestyleDefinition wretched =
            canonical.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.lifestyle.wretched");

        var alteredWretched =
            new LifestyleDefinition(
                wretched.Id,
                wretched.Name,
                new ListedCost(
                    new Money(1),
                    ListedCostKind.Exact),
                wretched.SpecialRuleIds,
                wretched.Sources);

        IReadOnlyList<string> errors =
            OfficialLifestyleSemanticValidator.Validate(
                canonical
                    .Select(
                        definition =>
                            definition.Id == wretched.Id
                                ? alteredWretched
                                : definition)
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "must have no listed daily cost",
                StringComparison.Ordinal));
    }

    [Fact]
    public void PaidLifestyleWithoutCost_IsRejected()
    {
        IReadOnlyList<LifestyleDefinition> canonical =
            LoadCanonical();

        LifestyleDefinition poor =
            canonical.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.lifestyle.poor");

        var alteredPoor =
            new LifestyleDefinition(
                poor.Id,
                poor.Name,
                dailyCost: null,
                poor.SpecialRuleIds,
                poor.Sources);

        IReadOnlyList<string> errors =
            OfficialLifestyleSemanticValidator.Validate(
                canonical
                    .Select(
                        definition =>
                            definition.Id == poor.Id
                                ? alteredPoor
                                : definition)
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "must have a listed daily cost",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedAndDuplicateIdentities_AreRejected()
    {
        IReadOnlyList<LifestyleDefinition> canonical =
            LoadCanonical();

        LifestyleDefinition template = canonical[0];

        var unexpected =
            new LifestyleDefinition(
                new LifestyleId(
                    "dnd5e2014.lifestyle.unexpected"),
                "Unexpected",
                new ListedCost(
                    new Money(1),
                    ListedCostKind.Exact),
                template.SpecialRuleIds,
                template.Sources);

        IReadOnlyList<string> errors =
            OfficialLifestyleSemanticValidator.Validate(
                canonical
                    .Append(unexpected)
                    .Append(canonical[0])
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.lifestyle.unexpected'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "duplicate ID",
                StringComparison.Ordinal));
    }

    private static IReadOnlyList<LifestyleDefinition>
        LoadCanonical()
    {
        return LifestyleDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "lifestyles.json"));
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
