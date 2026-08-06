namespace FiveEData.Rules.Classes.MysticArcanum;

public sealed record MysticArcanumProgressionDetail
{
    public MysticArcanumProgressionDetail(
        IEnumerable<MysticArcanumGrant> arcanumByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(arcanumByLevel);

        ArcanumByLevel = Array.AsReadOnly(arcanumByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<MysticArcanumGrant> ArcanumByLevel { get; }
    public bool RecoversOnShortRest { get; }
}
