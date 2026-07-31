using System.Reflection;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Tests;

public sealed class PublicApiBoundaryTests
{
    [Fact]
    public void SerializationAndIntegrityPlumbing_AreNotPublicApi()
    {
        Assert.False(typeof(WeaponDefinitionLoader).IsPublic);
        Assert.False(typeof(AmmunitionDefinitionLoader).IsPublic);
        Assert.False(typeof(SourceDocumentLoader).IsPublic);
        Assert.False(typeof(CatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(RuleDefinitionLoader).IsPublic);
    }

    [Fact]
    public void ExportedApi_DoesNotExposeFilesystemLoadingMethods()
    {
        Assembly assembly = typeof(Dnd5e2014Ruleset).Assembly;

        MethodInfo[] offending = assembly
            .GetExportedTypes()
            .SelectMany(type => type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly))
            .Where(method =>
                method.Name.Contains(
                    "LoadFromFile",
                    StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(offending);
    }

    [Fact]
    public void RulesetCreation_LoadsEmbeddedData()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(38, ruleset.Weapons.Count);
        Assert.Equal(4, ruleset.Ammunition.Count);
        Assert.Equal(1, ruleset.Sources.Count);
    }
}
