using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Adventuring.DowntimeActivities;

public sealed class DowntimeActivityDefinition
{
    internal DowntimeActivityDefinition(
        DowntimeActivityId id,
        string name,
        int? requiredDays,
        int? costPerDayGoldPieces,
        AbilityId? savingThrowAbilityId,
        int? savingThrowDC,
        int? marketValueProgressPerDayGoldPieces,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RequiredDays = requiredDays;
        CostPerDayGoldPieces = costPerDayGoldPieces;
        SavingThrowAbilityId = savingThrowAbilityId;
        SavingThrowDC = savingThrowDC;
        MarketValueProgressPerDayGoldPieces =
            marketValueProgressPerDayGoldPieces;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public DowntimeActivityId Id { get; }
    public string Name { get; }
    public int? RequiredDays { get; }
    public int? CostPerDayGoldPieces { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public int? SavingThrowDC { get; }
    public int? MarketValueProgressPerDayGoldPieces { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
