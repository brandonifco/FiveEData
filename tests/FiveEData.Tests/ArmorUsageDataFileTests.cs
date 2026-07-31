using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment;
using FiveEData.Rules.Equipment.Armor.Serialization;

namespace FiveEData.Tests;

public sealed class ArmorUsageDataFileTests
{
    [Fact]
    public void CanonicalUsageData_MatchesFirstPrintingCoreValues()
    {
        var rules = LoadCanonical();

        Assert.Equal(10, rules.InsufficientStrengthSpeedReduction.Feet);
        Assert.Equal(1, rules.ShieldHandsRequired);
        Assert.Equal(1, rules.MaximumBenefitingShields);
        Assert.True(rules.RequiresFullDonDurationForArmorClassBenefit);
        Assert.Equal(2, rules.DoffingWithHelpDivisor);
    }

    [Fact]
    public void CanonicalUsageData_MatchesDonAndDoffTable()
    {
        var rules = LoadCanonical();

        AssertTiming(
            rules.LightArmorChangeTiming,
            donAmount: 1,
            donUnit: EquipmentChangeTimeUnit.Minute,
            doffAmount: 1,
            doffUnit: EquipmentChangeTimeUnit.Minute);
        AssertTiming(
            rules.MediumArmorChangeTiming,
            donAmount: 5,
            donUnit: EquipmentChangeTimeUnit.Minute,
            doffAmount: 1,
            doffUnit: EquipmentChangeTimeUnit.Minute);
        AssertTiming(
            rules.HeavyArmorChangeTiming,
            donAmount: 10,
            donUnit: EquipmentChangeTimeUnit.Minute,
            doffAmount: 5,
            doffUnit: EquipmentChangeTimeUnit.Minute);
        AssertTiming(
            rules.ShieldChangeTiming,
            donAmount: 1,
            donUnit: EquipmentChangeTimeUnit.Action,
            doffAmount: 1,
            doffUnit: EquipmentChangeTimeUnit.Action);
    }

    [Fact]
    public void CanonicalUsageData_ReferencesFiveDistinctRules()
    {
        var rules = LoadCanonical();

        Assert.Equal(5, rules.ReferencedRuleIds.Count);
        Assert.Equal(5, rules.ReferencedRuleIds.Distinct().Count());
        Assert.Contains(
            new RuleId("dnd5e2014.armor-rule.proficiency"),
            rules.ReferencedRuleIds);
        Assert.Contains(
            new RuleId("dnd5e2014.armor-rule.don-doff"),
            rules.ReferencedRuleIds);
    }

    [Fact]
    public void CanonicalUsageData_UsesPrintedBookProvenance()
    {
        var rules = LoadCanonical();

        Assert.Equal(2, rules.Sources.Count);
        Assert.Contains(rules.Sources, source => source.Page == 144);
        Assert.Contains(rules.Sources, source => source.Page == 146);
        Assert.All(
            rules.Sources,
            source => Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value));
    }

    private static FiveEData.Rules.Equipment.Armor.ArmorUsageRules LoadCanonical()
    {
        string root = FindRepositoryRoot();
        return ArmorUsageRulesLoader.LoadFromFile(
            Path.Combine(root, "Data", "dnd5e2014", "armor-usage.json"));
    }

    private static void AssertTiming(
        EquipmentChangeTiming timing,
        int donAmount,
        EquipmentChangeTimeUnit donUnit,
        int doffAmount,
        EquipmentChangeTimeUnit doffUnit)
    {
        Assert.Equal(donAmount, timing.Don.Amount);
        Assert.Equal(donUnit, timing.Don.Unit);
        Assert.Equal(doffAmount, timing.Doff.Amount);
        Assert.Equal(doffUnit, timing.Doff.Unit);
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
