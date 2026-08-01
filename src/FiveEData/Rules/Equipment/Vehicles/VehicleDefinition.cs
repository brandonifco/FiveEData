using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Vehicles;

public sealed class VehicleDefinition
{
    internal VehicleDefinition(
        VehicleId id,
        string name,
        VehicleKind kind,
        Money cost,
        Weight? listedWeight,
        VehicleSpeed? listedSpeed,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Kind = kind;
        Cost = cost;
        ListedWeight = listedWeight;
        ListedSpeed = listedSpeed;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public VehicleId Id { get; }
    public string Name { get; }
    public VehicleKind Kind { get; }
    public Money Cost { get; }
    public Weight? ListedWeight { get; }
    public VehicleSpeed? ListedSpeed { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
