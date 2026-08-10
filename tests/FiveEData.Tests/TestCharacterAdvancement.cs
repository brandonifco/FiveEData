using FiveEData.Rules.Characters.CharacterAdvancement;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

/// <summary>
/// A minimal valid <see cref="CharacterAdvancementRules"/> for the
/// integrity tests, which construct a <c>RulesetDefinitionSet</c>
/// directly and need a non-null value they aren't otherwise exercising.
/// </summary>
internal static class TestCharacterAdvancement
{
    public static CharacterAdvancementRules Create(
        string sourceDocumentId = "dnd5e2014.source.phb-first-printing")
    {
        CharacterAdvancementLevel[] levels =
            Enumerable
                .Range(1, CharacterAdvancementRules.MaximumLevel)
                .Select(
                    level => new CharacterAdvancementLevel(
                        level: level,
                        experiencePointThreshold: (level - 1) * 100,
                        proficiencyBonus: 2 + ((level - 1) / 4)))
                .ToArray();

        return new CharacterAdvancementRules(
            levels,
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 15)
            ]);
    }
}
