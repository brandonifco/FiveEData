namespace FiveEData.Rules.Classes.Indomitable;

public sealed record IndomitableProgressionDetail
{
    public IndomitableProgressionDetail(
        IEnumerable<IndomitableUseGrant> usesByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(usesByLevel);

        UsesByLevel = Array.AsReadOnly(usesByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<IndomitableUseGrant> UsesByLevel { get; }

    public bool RecoversOnShortRest { get; }
}
