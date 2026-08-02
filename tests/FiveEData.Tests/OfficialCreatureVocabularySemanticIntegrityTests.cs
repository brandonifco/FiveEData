using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Abilities.Serialization;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Languages.Serialization;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Sizes.Serialization;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Creatures.Skills.Serialization;

namespace FiveEData.Tests;

public sealed class
    OfficialCreatureVocabularySemanticIntegrityTests
{
    [Fact]
    public void CanonicalVocabulary_HasNoErrors()
    {
        Assert.Empty(
            OfficialCreatureVocabularySemanticValidator.Validate(
                LoadCanonical()));
    }

    [Fact]
    public void MissingAbility_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                abilities:
                    canonical.Abilities
                        .Where(
                            definition =>
                                definition.Id.Value !=
                                "dnd5e2014.ability.charisma")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 6 definitions; found 5",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.ability.charisma'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredAbilityName_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        AbilityDefinition strength =
            canonical.Abilities.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.ability.strength");

        var alteredStrength = new AbilityDefinition(
            strength.Id,
            "Might",
            strength.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                abilities:
                    canonical.Abilities
                        .Select(
                            definition =>
                                definition.Id == strength.Id
                                    ? alteredStrength
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Strength'; found 'Might'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedAbility_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        AbilityDefinition template =
            canonical.Abilities[0];

        var unexpected = new AbilityDefinition(
            new AbilityId(
                "dnd5e2014.ability.luck"),
            "Luck",
            template.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                abilities:
                    canonical.Abilities
                        .Append(unexpected)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.ability.luck'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSkill_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                skills:
                    canonical.Skills
                        .Where(
                            definition =>
                                definition.Id.Value !=
                                "dnd5e2014.skill.survival")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 18 definitions; found 17",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.skill.survival'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredSkillSemantics_AreRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        SkillDefinition athletics =
            canonical.Skills.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.skill.athletics");

        var alteredAthletics = new SkillDefinition(
            athletics.Id,
            "Physical Training",
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            athletics.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                skills:
                    canonical.Skills
                        .Select(
                            definition =>
                                definition.Id == athletics.Id
                                    ? alteredAthletics
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Athletics'; found " +
                "'Physical Training'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "must normally associate with ability " +
                    "'dnd5e2014.ability.strength'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "'dnd5e2014.ability.dexterity'",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedSkill_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        SkillDefinition template =
            canonical.Skills[0];

        var unexpected = new SkillDefinition(
            new SkillId(
                "dnd5e2014.skill.luck"),
            "Luck",
            template.NormallyAssociatedAbilityId,
            template.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                skills:
                    canonical.Skills
                        .Append(unexpected)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.skill.luck'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredAbilityProvenance_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        AbilityDefinition wisdom =
            canonical.Abilities.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.ability.wisdom");

        var alteredWisdom = new AbilityDefinition(
            wisdom.Id,
            wisdom.Name,
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 174,
                    section:
                        "Chapter 7: Using Ability Scores — " +
                        "Ability Scores and Modifiers")
            ]);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                abilities:
                    canonical.Abilities
                        .Select(
                            definition =>
                                definition.Id == wisdom.Id
                                    ? alteredWisdom
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "dnd5e2014.ability.wisdom",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 173",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredSkillProvenance_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        SkillDefinition perception =
            canonical.Skills.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.skill.perception");

        var alteredPerception = new SkillDefinition(
            perception.Id,
            perception.Name,
            perception.NormallyAssociatedAbilityId,
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 175,
                    section:
                        "Chapter 7: Using Ability Scores — " +
                        "Ability Checks — Skills")
            ]);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                skills:
                    canonical.Skills
                        .Select(
                            definition =>
                                definition.Id == perception.Id
                                    ? alteredPerception
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "dnd5e2014.skill.perception",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 174",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingLanguage_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                languages:
                    canonical.Languages
                        .Where(
                            definition =>
                                definition.Id.Value !=
                                "dnd5e2014.language.common")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 16 definitions; found 15",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.language.common'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredLanguageSemantics_AreRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        LanguageDefinition common =
            canonical.Languages.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.language.common");

        var alteredCommon = new LanguageDefinition(
            common.Id,
            "Trade Tongue",
            LanguageCategory.Exotic,
            common.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                languages:
                    canonical.Languages
                        .Select(
                            definition =>
                                definition.Id == common.Id
                                    ? alteredCommon
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Common'; found 'Trade Tongue'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must have category 'Standard'; found 'Exotic'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedLanguage_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        LanguageDefinition template =
            canonical.Languages[0];

        var unexpected = new LanguageDefinition(
            new LanguageId(
                "dnd5e2014.language.aquan"),
            "Aquan",
            LanguageCategory.Exotic,
            template.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                languages:
                    canonical.Languages
                        .Append(unexpected)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.language.aquan'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredLanguageProvenance_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        LanguageDefinition primordial =
            canonical.Languages.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.language.primordial");

        var alteredPrimordial = new LanguageDefinition(
            primordial.Id,
            primordial.Name,
            primordial.Category,
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 124,
                    section:
                        "Chapter 4: Personality and Background — " +
                        "Languages")
            ]);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                languages:
                    canonical.Languages
                        .Select(
                            definition =>
                                definition.Id == primordial.Id
                                    ? alteredPrimordial
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "dnd5e2014.language.primordial",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 123",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingCreatureSize_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                sizes:
                    canonical.Sizes
                        .Where(
                            definition =>
                                definition.Id.Value !=
                                "dnd5e2014.creature-size.tiny")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 6 definitions; found 5",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.creature-size.tiny'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredCreatureSizeName_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureSizeDefinition medium =
            canonical.Sizes.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.creature-size.medium");

        var alteredMedium = new CreatureSizeDefinition(
            medium.Id,
            "Average",
            medium.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                sizes:
                    canonical.Sizes
                        .Select(
                            definition =>
                                definition.Id == medium.Id
                                    ? alteredMedium
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Medium'; found 'Average'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedCreatureSize_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureSizeDefinition template =
            canonical.Sizes[0];

        var unexpected = new CreatureSizeDefinition(
            new CreatureSizeId(
                "dnd5e2014.creature-size.colossal"),
            "Colossal",
            template.Sources);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                sizes:
                    canonical.Sizes
                        .Append(unexpected)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.creature-size.colossal'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredCreatureSizeProvenance_IsRejected()
    {
        CreatureVocabularyDefinitionSet canonical =
            LoadCanonical();

        CreatureSizeDefinition gargantuan =
            canonical.Sizes.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.creature-size.gargantuan");

        var alteredGargantuan =
            new CreatureSizeDefinition(
                gargantuan.Id,
                gargantuan.Name,
                [
                    new SourceReference(
                        new SourceDocumentId(
                            "dnd5e2014.source.phb-first-printing"),
                        page: 192,
                        section:
                            "Chapter 9: Combat — " +
                            "Movement and Position — Creature Size")
                ]);

        CreatureVocabularyDefinitionSet altered =
            CreateVocabulary(
                canonical,
                sizes:
                    canonical.Sizes
                        .Select(
                            definition =>
                                definition.Id == gargantuan.Id
                                    ? alteredGargantuan
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialCreatureVocabularySemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "dnd5e2014.creature-size.gargantuan",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 191",
                    StringComparison.Ordinal));
    }

    private static CreatureVocabularyDefinitionSet
        CreateVocabulary(
            CreatureVocabularyDefinitionSet canonical,
            IReadOnlyList<AbilityDefinition>? abilities = null,
            IReadOnlyList<SkillDefinition>? skills = null,
            IReadOnlyList<LanguageDefinition>? languages = null,
            IReadOnlyList<CreatureSizeDefinition>? sizes = null)
    {
        return new CreatureVocabularyDefinitionSet(
            abilities: abilities ?? canonical.Abilities,
            skills: skills ?? canonical.Skills,
            languages: languages ?? canonical.Languages,
            sizes: sizes ?? canonical.Sizes);
    }

    private static CreatureVocabularyDefinitionSet
        LoadCanonical()
    {
        return new CreatureVocabularyDefinitionSet(
            abilities:
                AbilityDefinitionLoader.LoadFromFile(
                    DataPath("abilities.json")),
            skills:
                SkillDefinitionLoader.LoadFromFile(
                    DataPath("skills.json")),
            languages:
                LanguageDefinitionLoader.LoadFromFile(
                    DataPath("languages.json")),
            sizes:
                CreatureSizeDefinitionLoader.LoadFromFile(
                    DataPath("creature-sizes.json")));
    }

    private static string DataPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "Data",
            "dnd5e2014",
            fileName);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
