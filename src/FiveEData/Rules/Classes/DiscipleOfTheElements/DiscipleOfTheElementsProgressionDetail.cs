namespace FiveEData.Rules.Classes.DiscipleOfTheElements;

public sealed record DiscipleOfTheElementsProgressionDetail
{
    public DiscipleOfTheElementsProgressionDetail(
        IEnumerable<DiscipleOfTheElementsDisciplinesKnownGrant>
            disciplinesKnownByLevel,
        IEnumerable<DiscipleOfTheElementsMaxKiPointsGrant>
            maxKiPointsPerSpellByLevel)
    {
        ArgumentNullException.ThrowIfNull(disciplinesKnownByLevel);
        ArgumentNullException.ThrowIfNull(maxKiPointsPerSpellByLevel);

        DisciplinesKnownByLevel =
            Array.AsReadOnly(disciplinesKnownByLevel.ToArray());
        MaxKiPointsPerSpellByLevel =
            Array.AsReadOnly(maxKiPointsPerSpellByLevel.ToArray());
    }

    public IReadOnlyList<DiscipleOfTheElementsDisciplinesKnownGrant>
        DisciplinesKnownByLevel
    { get; }

    public IReadOnlyList<DiscipleOfTheElementsMaxKiPointsGrant>
        MaxKiPointsPerSpellByLevel
    { get; }
}
