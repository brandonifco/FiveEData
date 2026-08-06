using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.ChannelDivinityOptions;

public sealed class ChannelDivinityOptionDefinition
{
    internal ChannelDivinityOptionDefinition(
        ChannelDivinityOptionId id,
        string name,
        int? rangeFeet,
        AbilityId? savingThrowAbilityId,
        int? durationMinutes,
        int? rollBonus,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        DurationMinutes = durationMinutes;
        RollBonus = rollBonus;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ChannelDivinityOptionId Id { get; }
    public string Name { get; }
    public int? RangeFeet { get; }
    public AbilityId? SavingThrowAbilityId { get; }
    public int? DurationMinutes { get; }
    public int? RollBonus { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
