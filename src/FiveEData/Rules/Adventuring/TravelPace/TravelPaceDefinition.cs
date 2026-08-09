using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Adventuring.TravelPace;

public sealed class TravelPaceDefinition
{
    internal TravelPaceDefinition(
        TravelPaceId id,
        string name,
        int feetPerMinute,
        int milesPerHour,
        int milesPerDay,
        int? passiveWisdomPerceptionPenalty,
        bool allowsStealth,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        FeetPerMinute = feetPerMinute;
        MilesPerHour = milesPerHour;
        MilesPerDay = milesPerDay;
        PassiveWisdomPerceptionPenalty = passiveWisdomPerceptionPenalty;
        AllowsStealth = allowsStealth;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public TravelPaceId Id { get; }
    public string Name { get; }
    public int FeetPerMinute { get; }
    public int MilesPerHour { get; }
    public int MilesPerDay { get; }
    public int? PassiveWisdomPerceptionPenalty { get; }
    public bool AllowsStealth { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
