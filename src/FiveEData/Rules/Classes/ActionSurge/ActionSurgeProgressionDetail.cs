namespace FiveEData.Rules.Classes.ActionSurge;

public sealed record ActionSurgeProgressionDetail
{
    public ActionSurgeProgressionDetail(
        IEnumerable<ActionSurgeUseGrant> usesByLevel,
        bool recoversOnShortRest,
        bool oncePerTurn)
    {
        ArgumentNullException.ThrowIfNull(usesByLevel);

        UsesByLevel = Array.AsReadOnly(usesByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
        OncePerTurn = oncePerTurn;
    }

    public IReadOnlyList<ActionSurgeUseGrant> UsesByLevel { get; }

    public bool RecoversOnShortRest { get; }

    public bool OncePerTurn { get; }
}
