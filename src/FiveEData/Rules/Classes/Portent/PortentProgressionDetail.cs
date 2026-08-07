namespace FiveEData.Rules.Classes.Portent;

public sealed record PortentProgressionDetail
{
    public PortentProgressionDetail(
        IEnumerable<PortentRollGrant> foretellingRollsByLevel,
        bool oncePerTurn,
        bool recoversOnLongRest)
    {
        ArgumentNullException.ThrowIfNull(foretellingRollsByLevel);

        ForetellingRollsByLevel =
            Array.AsReadOnly(foretellingRollsByLevel.ToArray());
        OncePerTurn = oncePerTurn;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public IReadOnlyList<PortentRollGrant> ForetellingRollsByLevel { get; }

    public bool OncePerTurn { get; }

    public bool RecoversOnLongRest { get; }
}
