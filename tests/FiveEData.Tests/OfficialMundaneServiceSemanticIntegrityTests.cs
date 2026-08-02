using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class
    OfficialMundaneServiceSemanticIntegrityTests
{
    [Fact]
    public void CanonicalDefinitions_HaveNoErrors()
    {
        Assert.Empty(
            OfficialMundaneServiceSemanticValidator.Validate(
                LoadCanonical()));
    }

    [Fact]
    public void MissingDefinition_IsRejected()
    {
        IReadOnlyList<MundaneServiceDefinition> altered =
            LoadCanonical()
                .Where(
                    definition =>
                        definition.Id.Value !=
                        "dnd5e2014.mundane-service.ship-passage")
                .ToArray();

        IReadOnlyList<string> errors =
            OfficialMundaneServiceSemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 7 definitions; found 6",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing " +
                "'dnd5e2014.mundane-service.ship-passage'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredSemantics_AreRejected()
    {
        IReadOnlyList<MundaneServiceDefinition> canonical =
            LoadCanonical();

        MundaneServiceDefinition skilled =
            canonical.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.mundane-service." +
                    "hireling-skilled");

        var alteredSkilled =
            new MundaneServiceDefinition(
                skilled.Id,
                "Skilled worker",
                new ListedCost(
                    new Money(201),
                    ListedCostKind.Exact),
                ServicePricingUnit.Flat,
                skilled.SpecialRuleIds,
                skilled.Sources);

        IReadOnlyList<MundaneServiceDefinition> altered =
            canonical
                .Select(
                    definition =>
                        definition.Id == skilled.Id
                            ? alteredSkilled
                            : definition)
                .ToArray();

        IReadOnlyList<string> errors =
            OfficialMundaneServiceSemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Hireling, skilled'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must cost 200 cp; found 201 cp",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must use cost kind 'Minimum'; found 'Exact'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must use pricing unit 'Day'; found 'Flat'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void PricedSpellcastingService_IsRejected()
    {
        MundaneServiceDefinition template =
            LoadCanonical()[0];

        var spellcasting =
            new MundaneServiceDefinition(
                new MundaneServiceId(
                    "dnd5e2014.mundane-service." +
                    "spellcasting-service"),
                "Spellcasting service",
                new ListedCost(
                    new Money(1000),
                    ListedCostKind.Exact),
                ServicePricingUnit.Flat,
                specialRuleIds: [],
                template.Sources);

        IReadOnlyList<string> errors =
            OfficialMundaneServiceSemanticValidator.Validate(
                LoadCanonical()
                    .Append(spellcasting)
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "must not contain a priced spellcasting service",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateIdentity_IsRejected()
    {
        IReadOnlyList<MundaneServiceDefinition> canonical =
            LoadCanonical();

        IReadOnlyList<string> errors =
            OfficialMundaneServiceSemanticValidator.Validate(
                canonical
                    .Append(canonical[0])
                    .ToArray());

        Assert.Contains(
            errors,
            error => error.Contains(
                "duplicate ID",
                StringComparison.Ordinal));
    }

    private static IReadOnlyList<MundaneServiceDefinition>
        LoadCanonical()
    {
        return MundaneServiceDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "mundane-services.json"));
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
