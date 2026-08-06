namespace FiveEData.Rules.Classes.SneakAttack;

public sealed record SneakAttackProgressionDetail
{
    public SneakAttackProgressionDetail(
        IEnumerable<SneakAttackDiceGrant> diceByLevel,
        bool oncePerTurn,
        bool requiresFinesseOrRangedWeapon)
    {
        ArgumentNullException.ThrowIfNull(diceByLevel);

        DiceByLevel = Array.AsReadOnly(diceByLevel.ToArray());
        OncePerTurn = oncePerTurn;
        RequiresFinesseOrRangedWeapon = requiresFinesseOrRangedWeapon;
    }

    public IReadOnlyList<SneakAttackDiceGrant> DiceByLevel { get; }
    public bool OncePerTurn { get; }
    public bool RequiresFinesseOrRangedWeapon { get; }
}
