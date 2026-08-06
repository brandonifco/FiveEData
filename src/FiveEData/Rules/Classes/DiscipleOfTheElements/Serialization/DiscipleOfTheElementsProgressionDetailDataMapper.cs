namespace FiveEData.Rules.Classes.DiscipleOfTheElements.Serialization;

internal static class DiscipleOfTheElementsProgressionDetailDataMapper
{
    public static DiscipleOfTheElementsProgressionDetail Map(
        DiscipleOfTheElementsProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiscipleOfTheElementsDisciplinesKnownGrantData[]
            disciplinesKnownData =
                data.DisciplinesKnownByLevel
                ?? throw new ArgumentException(
                    "Disciple of the Elements progression disciplines " +
                    "known by level is required.",
                    nameof(data));

        DiscipleOfTheElementsMaxKiPointsGrantData[] maxKiPointsData =
            data.MaxKiPointsPerSpellByLevel
            ?? throw new ArgumentException(
                "Disciple of the Elements progression max ki points per " +
                "spell by level is required.",
                nameof(data));

        DiscipleOfTheElementsDisciplinesKnownGrant[] disciplinesKnownByLevel =
            disciplinesKnownData
                .Select(
                    grant => new DiscipleOfTheElementsDisciplinesKnownGrant(
                        grant.CharacterLevel,
                        grant.DisciplinesKnown))
                .ToArray();

        DiscipleOfTheElementsMaxKiPointsGrant[] maxKiPointsPerSpellByLevel =
            maxKiPointsData
                .Select(
                    grant => new DiscipleOfTheElementsMaxKiPointsGrant(
                        grant.CharacterLevel,
                        grant.MaxKiPoints))
                .ToArray();

        return new DiscipleOfTheElementsProgressionDetail(
            disciplinesKnownByLevel,
            maxKiPointsPerSpellByLevel);
    }
}
