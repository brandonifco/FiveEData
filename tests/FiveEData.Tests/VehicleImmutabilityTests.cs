using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class VehicleImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsCollectionInputs()
    {
        var rules = new List<RuleId>();
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 157)
        };

        var vehicle = new VehicleDefinition(
            new VehicleId("dnd5e2014.vehicle.test"),
            "Test vehicle",
            VehicleKind.Land,
            new Money(100),
            listedWeight: new Weight(100),
            listedSpeed: null,
            specialRuleIds: rules,
            sources: sources);

        rules.Add(new RuleId("dnd5e2014.vehicle-rule.mutated"));
        sources.Clear();

        Assert.Empty(vehicle.SpecialRuleIds);
        Assert.Single(vehicle.Sources);
    }
}
