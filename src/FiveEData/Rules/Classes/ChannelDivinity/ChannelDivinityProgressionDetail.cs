namespace FiveEData.Rules.Classes.ChannelDivinity;

public sealed record ChannelDivinityProgressionDetail
{
    public ChannelDivinityProgressionDetail(
        IEnumerable<ChannelDivinityUseGrant> usesByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(usesByLevel);

        UsesByLevel = Array.AsReadOnly(usesByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<ChannelDivinityUseGrant> UsesByLevel { get; }
    public bool RecoversOnShortRest { get; }
}
