namespace FiveEData.Rules.Classes.BrutalCritical;

public sealed record BrutalCriticalProgressionDetail
{
    public BrutalCriticalProgressionDetail(
        IEnumerable<BrutalCriticalDiceGrant> additionalDiceByLevel,
        bool requiresMeleeAttack)
    {
        ArgumentNullException.ThrowIfNull(additionalDiceByLevel);

        AdditionalDiceByLevel =
            Array.AsReadOnly(additionalDiceByLevel.ToArray());
        RequiresMeleeAttack = requiresMeleeAttack;
    }

    public IReadOnlyList<BrutalCriticalDiceGrant> AdditionalDiceByLevel { get; }

    public bool RequiresMeleeAttack { get; }
}
