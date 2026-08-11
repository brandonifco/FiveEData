using FiveEData.Rules.Characters.Encumbrance;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

/// <summary>
/// A minimal valid <see cref="EncumbranceRules"/> for the integrity tests,
/// the same role <see cref="TestConcentration"/> and
/// <see cref="TestCharacterAdvancement"/> already play: they construct a
/// <c>RulesetDefinitionSet</c> directly and need a non-null value they
/// aren't otherwise exercising.
/// </summary>
internal static class TestEncumbrance
{
    private static readonly (string Id, double Multiplier)[] SizeMultipliers =
    [
        ("dnd5e2014.creature-size.tiny", 0.5),
        ("dnd5e2014.creature-size.small", 1),
        ("dnd5e2014.creature-size.medium", 1),
        ("dnd5e2014.creature-size.large", 2),
        ("dnd5e2014.creature-size.huge", 4),
        ("dnd5e2014.creature-size.gargantuan", 8)
    ];

    public static EncumbranceRules Create(
        string sourceDocumentId = "dnd5e2014.source.phb-first-printing")
    {
        return new EncumbranceRules(
            SizeMultipliers.Select(
                entry => new CarryingCapacitySizeMultiplierGrant(
                    new CreatureSizeId(entry.Id),
                    entry.Multiplier)),
            encumberedCarryingCapacityMultiplier: 5,
            encumberedSpeedReductionFeet: 10,
            heavilyEncumberedCarryingCapacityMultiplier: 10,
            heavilyEncumberedSpeedReductionFeet: 20,
            [
                new AbilityId("dnd5e2014.ability.strength"),
                new AbilityId("dnd5e2014.ability.dexterity"),
                new AbilityId("dnd5e2014.ability.constitution")
            ],
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    176,
                    "Chapter 7: Using Ability Scores — Strength — " +
                        "Variant: Encumbrance")
            ]);
    }

    /// <summary>
    /// The creature sizes the canonical encumbrance rules reference. A
    /// minimal definition set must contain all six, since encumbrance is a
    /// required singleton and therefore always contributes cross-domain
    /// references — unlike a list-shaped domain, which contributes none
    /// when empty.
    /// </summary>
    public static IReadOnlyList<CreatureSizeDefinition> RequiredSizes() =>
        SizeMultipliers
            .Select(
                entry =>
                    new CreatureSizeDefinition(
                        new CreatureSizeId(entry.Id),
                        entry.Id.Split('.')[^1],
                        [CreateSource()]))
            .ToArray();

    /// <summary>
    /// The abilities the canonical encumbrance rules reference. Strength
    /// and Dexterity are always needed; Constitution overlaps with
    /// <see cref="TestConcentration.RequiredAbility"/> in every call site
    /// that already needs it, which is harmless since these tests validate
    /// references rather than build a deduplicated catalog.
    /// </summary>
    public static IReadOnlyList<AbilityDefinition> RequiredAbilities() =>
    [
        new(
            new AbilityId("dnd5e2014.ability.strength"),
            "Strength",
            [CreateSource()]),
        new(
            new AbilityId("dnd5e2014.ability.dexterity"),
            "Dexterity",
            [CreateSource()]),
        new(
            new AbilityId("dnd5e2014.ability.constitution"),
            "Constitution",
            [CreateSource()])
    ];

    private static SourceReference CreateSource() =>
        new(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            176,
            "Chapter 7: Using Ability Scores — Strength — " +
                "Variant: Encumbrance");
}
