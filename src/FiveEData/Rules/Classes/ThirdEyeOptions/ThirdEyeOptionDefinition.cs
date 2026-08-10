using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.ThirdEyeOptions;

public sealed class ThirdEyeOptionDefinition
{
    internal ThirdEyeOptionDefinition(
        ThirdEyeOptionId id,
        string name,
        int? darkvisionRangeFeet,
        int? etherealSightRangeFeet,
        int? seeInvisibilityRangeFeet,
        bool canReadAllLanguages,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        DarkvisionRangeFeet = darkvisionRangeFeet;
        EtherealSightRangeFeet = etherealSightRangeFeet;
        SeeInvisibilityRangeFeet = seeInvisibilityRangeFeet;
        CanReadAllLanguages = canReadAllLanguages;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ThirdEyeOptionId Id { get; }
    public string Name { get; }
    public int? DarkvisionRangeFeet { get; }
    public int? EtherealSightRangeFeet { get; }
    public int? SeeInvisibilityRangeFeet { get; }
    public bool CanReadAllLanguages { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
