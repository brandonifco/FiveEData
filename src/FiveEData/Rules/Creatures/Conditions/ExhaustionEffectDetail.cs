namespace FiveEData.Rules.Creatures.Conditions;

public sealed record ExhaustionEffectDetail
{
    public ExhaustionEffectDetail(
        IEnumerable<ExhaustionLevelEffect> levelEffects,
        bool recoversOneLevelPerLongRest,
        bool recoveryRequiresFoodAndDrink)
    {
        ArgumentNullException.ThrowIfNull(levelEffects);

        ExhaustionLevelEffect[] levelEffectsArray =
            levelEffects.ToArray();

        if (levelEffectsArray.Length != 6)
        {
            throw new ArgumentException(
                "Exhaustion must specify an effect for each of its six " +
                "levels.",
                nameof(levelEffects));
        }

        LevelEffects = Array.AsReadOnly(levelEffectsArray);
        RecoversOneLevelPerLongRest = recoversOneLevelPerLongRest;
        RecoveryRequiresFoodAndDrink = recoveryRequiresFoodAndDrink;
    }

    public IReadOnlyList<ExhaustionLevelEffect> LevelEffects { get; }
    public bool RecoversOneLevelPerLongRest { get; }
    public bool RecoveryRequiresFoodAndDrink { get; }
}
