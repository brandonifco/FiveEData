using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Adventuring.Resting;

public sealed class RestTypeDefinition
{
    internal RestTypeDefinition(
        RestTypeId id,
        string name,
        int minimumDurationHours,
        int? cooldownHours,
        int? minimumHitPointsToBenefit,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        MinimumDurationHours = minimumDurationHours;
        CooldownHours = cooldownHours;
        MinimumHitPointsToBenefit = minimumHitPointsToBenefit;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public RestTypeId Id { get; }
    public string Name { get; }
    public int MinimumDurationHours { get; }
    public int? CooldownHours { get; }
    public int? MinimumHitPointsToBenefit { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
