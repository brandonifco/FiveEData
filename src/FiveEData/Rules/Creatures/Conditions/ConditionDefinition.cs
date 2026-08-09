using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Conditions;

public sealed class ConditionDefinition
{
    internal ConditionDefinition(
        ConditionId id,
        string name,
        bool preventsActionsAndReactions,
        bool preventsMovement,
        bool onlyMovementOptionIsToCrawl,
        bool speedBecomesZero,
        bool ignoresBonusesToSpeed,
        SpeechRestriction speechRestriction,
        bool unawareOfSurroundings,
        bool automaticallyFailsStrengthAndDexteritySavingThrows,
        bool dexteritySavingThrowsHaveDisadvantage,
        bool automaticallyFailsAbilityChecksRequiringSight,
        bool automaticallyFailsAbilityChecksRequiringHearing,
        bool ownAbilityChecksHaveDisadvantage,
        RollModifier attackRollsAgainstTheCreature,
        RollModifier theCreaturesOwnAttackRolls,
        bool anyHitIsACriticalHitIfAttackerIsWithinFiveFeet,
        bool requiresSourceInLineOfSightForRollEffects,
        bool cannotWillinglyMoveCloserToSource,
        bool cannotAttackOrTargetSourceWithHarmfulEffects,
        bool sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature,
        bool endsIfSourceCreatureIsIncapacitated,
        bool resistantToAllDamage,
        bool immuneToPoisonAndDisease,
        int? weightMultiplier,
        bool dropsHeldItemsAndFallsProne,
        bool heavilyObscuredForHidingPurposes,
        ExhaustionEffectDetail? exhaustionEffect,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        PreventsActionsAndReactions = preventsActionsAndReactions;
        PreventsMovement = preventsMovement;
        OnlyMovementOptionIsToCrawl = onlyMovementOptionIsToCrawl;
        SpeedBecomesZero = speedBecomesZero;
        IgnoresBonusesToSpeed = ignoresBonusesToSpeed;
        SpeechRestriction = speechRestriction;
        UnawareOfSurroundings = unawareOfSurroundings;
        AutomaticallyFailsStrengthAndDexteritySavingThrows =
            automaticallyFailsStrengthAndDexteritySavingThrows;
        DexteritySavingThrowsHaveDisadvantage =
            dexteritySavingThrowsHaveDisadvantage;
        AutomaticallyFailsAbilityChecksRequiringSight =
            automaticallyFailsAbilityChecksRequiringSight;
        AutomaticallyFailsAbilityChecksRequiringHearing =
            automaticallyFailsAbilityChecksRequiringHearing;
        OwnAbilityChecksHaveDisadvantage = ownAbilityChecksHaveDisadvantage;
        AttackRollsAgainstTheCreature = attackRollsAgainstTheCreature;
        TheCreaturesOwnAttackRolls = theCreaturesOwnAttackRolls;
        AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet =
            anyHitIsACriticalHitIfAttackerIsWithinFiveFeet;
        RequiresSourceInLineOfSightForRollEffects =
            requiresSourceInLineOfSightForRollEffects;
        CannotWillinglyMoveCloserToSource =
            cannotWillinglyMoveCloserToSource;
        CannotAttackOrTargetSourceWithHarmfulEffects =
            cannotAttackOrTargetSourceWithHarmfulEffects;
        SourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature =
            sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature;
        EndsIfSourceCreatureIsIncapacitated =
            endsIfSourceCreatureIsIncapacitated;
        ResistantToAllDamage = resistantToAllDamage;
        ImmuneToPoisonAndDisease = immuneToPoisonAndDisease;
        WeightMultiplier = weightMultiplier;
        DropsHeldItemsAndFallsProne = dropsHeldItemsAndFallsProne;
        HeavilyObscuredForHidingPurposes = heavilyObscuredForHidingPurposes;
        ExhaustionEffect = exhaustionEffect;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ConditionId Id { get; }
    public string Name { get; }

    // Action economy / movement / speech.
    public bool PreventsActionsAndReactions { get; }
    public bool PreventsMovement { get; }
    public bool OnlyMovementOptionIsToCrawl { get; }
    public bool SpeedBecomesZero { get; }
    public bool IgnoresBonusesToSpeed { get; }
    public SpeechRestriction SpeechRestriction { get; }
    public bool UnawareOfSurroundings { get; }

    // Saving throws / ability checks.
    public bool AutomaticallyFailsStrengthAndDexteritySavingThrows { get; }
    public bool DexteritySavingThrowsHaveDisadvantage { get; }
    public bool AutomaticallyFailsAbilityChecksRequiringSight { get; }
    public bool AutomaticallyFailsAbilityChecksRequiringHearing { get; }
    public bool OwnAbilityChecksHaveDisadvantage { get; }

    // Attack rolls.
    public RollModifier AttackRollsAgainstTheCreature { get; }
    public RollModifier TheCreaturesOwnAttackRolls { get; }
    public bool AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet { get; }

    // Facts tied to whatever creature/effect imposed the condition.
    public bool RequiresSourceInLineOfSightForRollEffects { get; }
    public bool CannotWillinglyMoveCloserToSource { get; }
    public bool CannotAttackOrTargetSourceWithHarmfulEffects { get; }

    public bool SourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature
    { get; }

    public bool EndsIfSourceCreatureIsIncapacitated { get; }

    // Damage / resistances.
    public bool ResistantToAllDamage { get; }
    public bool ImmuneToPoisonAndDisease { get; }

    // Miscellaneous.
    public int? WeightMultiplier { get; }
    public bool DropsHeldItemsAndFallsProne { get; }
    public bool HeavilyObscuredForHidingPurposes { get; }
    public ExhaustionEffectDetail? ExhaustionEffect { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
