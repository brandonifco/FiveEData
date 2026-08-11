using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Spells.Concentration;

namespace FiveEData.Tests;

/// <summary>
/// A minimal valid <see cref="ConcentrationRules"/> for the integrity
/// tests, the same role <see cref="TestCharacterAdvancement"/> already
/// plays: they construct a <c>RulesetDefinitionSet</c> directly and need a
/// non-null value they aren't otherwise exercising.
/// </summary>
internal static class TestConcentration
{
    public static ConcentrationRules Create(
        string sourceDocumentId = "dnd5e2014.source.phb-first-printing",
        string savingThrowAbilityId = "dnd5e2014.ability.constitution",
        params string[] endedByConditionIds)
    {
        string[] conditionIds = endedByConditionIds.Length > 0
            ? endedByConditionIds
            : ["dnd5e2014.condition.incapacitated"];

        return new ConcentrationRules(
            new AbilityId(savingThrowAbilityId),
            minimumSavingThrowDC: 10,
            damageDivisorForSavingThrowDC: 2,
            separateSavingThrowPerDamageSource: true,
            endsWhenCastingAnotherConcentrationSpell: true,
            endsOnDeath: true,
            canBeEndedVoluntarilyWithoutAnAction: true,
            conditionIds.Select(value => new ConditionId(value)),
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    203,
                    "Chapter 10: Spellcasting — Concentration")
            ]);
    }

    /// <summary>
    /// The ability the canonical concentration rules reference. A minimal
    /// definition set must contain it, since concentration is a required
    /// singleton and therefore always contributes cross-domain references
    /// — unlike a list-shaped domain, which contributes none when empty.
    /// </summary>
    public static AbilityDefinition RequiredAbility() =>
        new(
            new AbilityId("dnd5e2014.ability.constitution"),
            "Constitution",
            [CreateSource()]);

    /// <summary>The condition the canonical concentration rules reference.</summary>
    public static ConditionDefinition RequiredCondition() =>
        new(
            id: new ConditionId("dnd5e2014.condition.incapacitated"),
            name: "Incapacitated",
            preventsActionsAndReactions: true,
            preventsMovement: false,
            onlyMovementOptionIsToCrawl: false,
            speedBecomesZero: false,
            ignoresBonusesToSpeed: false,
            speechRestriction: default,
            unawareOfSurroundings: false,
            automaticallyFailsStrengthAndDexteritySavingThrows: false,
            dexteritySavingThrowsHaveDisadvantage: false,
            automaticallyFailsAbilityChecksRequiringSight: false,
            automaticallyFailsAbilityChecksRequiringHearing: false,
            ownAbilityChecksHaveDisadvantage: false,
            attackRollsAgainstTheCreature: default,
            theCreaturesOwnAttackRolls: default,
            anyHitIsACriticalHitIfAttackerIsWithinFiveFeet: false,
            requiresSourceInLineOfSightForRollEffects: false,
            cannotWillinglyMoveCloserToSource: false,
            cannotAttackOrTargetSourceWithHarmfulEffects: false,
            sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature: false,
            endsIfSourceCreatureIsIncapacitated: false,
            resistantToAllDamage: false,
            immuneToPoisonAndDisease: false,
            weightMultiplier: null,
            dropsHeldItemsAndFallsProne: false,
            heavilyObscuredForHidingPurposes: false,
            exhaustionEffect: null,
            sources: [CreateSource()]);

    private static SourceReference CreateSource() =>
        new(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            203,
            "Chapter 10: Spellcasting — Concentration");
}
