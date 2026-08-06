namespace FiveEData.Rules.Classes.CombatSuperiority;

public sealed record CombatSuperiorityProgressionDetail
{
    public CombatSuperiorityProgressionDetail(
        IEnumerable<CombatSuperiorityManeuversKnownGrant> maneuversKnownByLevel,
        IEnumerable<CombatSuperiorityDiceCountGrant> diceCountByLevel,
        IEnumerable<CombatSuperiorityDieSizeGrant> dieSizeByLevel)
    {
        ArgumentNullException.ThrowIfNull(maneuversKnownByLevel);
        ArgumentNullException.ThrowIfNull(diceCountByLevel);
        ArgumentNullException.ThrowIfNull(dieSizeByLevel);

        ManeuversKnownByLevel =
            Array.AsReadOnly(maneuversKnownByLevel.ToArray());
        DiceCountByLevel = Array.AsReadOnly(diceCountByLevel.ToArray());
        DieSizeByLevel = Array.AsReadOnly(dieSizeByLevel.ToArray());
    }

    public IReadOnlyList<CombatSuperiorityManeuversKnownGrant>
        ManeuversKnownByLevel
    { get; }

    public IReadOnlyList<CombatSuperiorityDiceCountGrant> DiceCountByLevel
    { get; }

    public IReadOnlyList<CombatSuperiorityDieSizeGrant> DieSizeByLevel
    { get; }
}
