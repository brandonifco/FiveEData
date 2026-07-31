using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.AdventuringGear;

public sealed class ContainerCapacityDefinition
{
    internal ContainerCapacityDefinition(
        AdventuringGearId adventuringGearId,
        ContainerVolume? solidVolume,
        ContainerVolume? liquidVolume,
        Weight? gearWeightCapacity,
        bool allowsExteriorItemAttachment,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);

        AdventuringGearId = adventuringGearId;
        SolidVolume = solidVolume;
        LiquidVolume = liquidVolume;
        GearWeightCapacity = gearWeightCapacity;
        AllowsExteriorItemAttachment = allowsExteriorItemAttachment;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AdventuringGearId AdventuringGearId { get; }
    public ContainerVolume? SolidVolume { get; }
    public ContainerVolume? LiquidVolume { get; }
    public Weight? GearWeightCapacity { get; }
    public bool AllowsExteriorItemAttachment { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
