using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Senses;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Creatures;

internal static class OfficialCreatureVocabularySemanticValidator
{
    private const string AbilitySection =
        "Chapter 7: Using Ability Scores — " +
        "Ability Scores and Modifiers";

    private const string SkillSection =
        "Chapter 7: Using Ability Scores — " +
        "Ability Checks — Skills";

    private const string LanguageSection =
        "Chapter 4: Personality and Background — Languages";

    private const string CreatureSizeSection =
        "Chapter 9: Combat — Movement and Position — " +
        "Creature Size";

    private const string ConditionSection =
        "Appendix A: Conditions";

    private const string DamageTypeSection =
        "Chapter 9: Combat — Damage and Healing — " +
        "Damage Types";

    private const string SenseSection =
        "Chapter 8: Adventuring — The Environment — " +
        "Vision and Light";

    private const string AlignmentSection =
        "Chapter 4: Personality and Background — Alignment";

    private static readonly SourceDocumentId SourceDocumentId =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly OfficialSourceExpectation AbilitySource =
        new(
            SourceDocumentId,
            173,
            AbilitySection);

    private static readonly OfficialSourceExpectation SkillSource =
        new(
            SourceDocumentId,
            174,
            SkillSection);

    private static readonly OfficialSourceExpectation LanguageSource =
        new(
            SourceDocumentId,
            123,
            LanguageSection);

    private static readonly OfficialSourceExpectation CreatureSizeSource =
        new(
            SourceDocumentId,
            191,
            CreatureSizeSection);

    private static readonly OfficialSourceExpectation ConditionSource =
        new(
            SourceDocumentId,
            290,
            ConditionSection);

    private static readonly OfficialSourceExpectation DamageTypeSource =
        new(
            SourceDocumentId,
            196,
            DamageTypeSection);

    private static readonly OfficialSourceExpectation SenseSource =
        new(
            SourceDocumentId,
            183,
            SenseSection);

    private static readonly OfficialSourceExpectation AlignmentSource =
        new(
            SourceDocumentId,
            122,
            AlignmentSection);

    private static readonly AbilityExpectation[] AbilityExpectations =
    [
        Ability("strength", "Strength"),
        Ability("dexterity", "Dexterity"),
        Ability("constitution", "Constitution"),
        Ability("intelligence", "Intelligence"),
        Ability("wisdom", "Wisdom"),
        Ability("charisma", "Charisma")
    ];

    private static readonly SkillExpectation[] SkillExpectations =
    [
        Skill("acrobatics", "Acrobatics", "dexterity"),
        Skill(
            "animal-handling",
            "Animal Handling",
            "wisdom"),
        Skill("arcana", "Arcana", "intelligence"),
        Skill("athletics", "Athletics", "strength"),
        Skill("deception", "Deception", "charisma"),
        Skill("history", "History", "intelligence"),
        Skill("insight", "Insight", "wisdom"),
        Skill("intimidation", "Intimidation", "charisma"),
        Skill(
            "investigation",
            "Investigation",
            "intelligence"),
        Skill("medicine", "Medicine", "wisdom"),
        Skill("nature", "Nature", "intelligence"),
        Skill("perception", "Perception", "wisdom"),
        Skill("performance", "Performance", "charisma"),
        Skill("persuasion", "Persuasion", "charisma"),
        Skill("religion", "Religion", "intelligence"),
        Skill(
            "sleight-of-hand",
            "Sleight of Hand",
            "dexterity"),
        Skill("stealth", "Stealth", "dexterity"),
        Skill("survival", "Survival", "wisdom")
    ];

    private static readonly LanguageExpectation[] LanguageExpectations =
    [
        Language(
            "common",
            "Common",
            LanguageCategory.Standard),
        Language(
            "dwarvish",
            "Dwarvish",
            LanguageCategory.Standard),
        Language(
            "elvish",
            "Elvish",
            LanguageCategory.Standard),
        Language(
            "giant",
            "Giant",
            LanguageCategory.Standard),
        Language(
            "gnomish",
            "Gnomish",
            LanguageCategory.Standard),
        Language(
            "goblin",
            "Goblin",
            LanguageCategory.Standard),
        Language(
            "halfling",
            "Halfling",
            LanguageCategory.Standard),
        Language(
            "orc",
            "Orc",
            LanguageCategory.Standard),
        Language(
            "abyssal",
            "Abyssal",
            LanguageCategory.Exotic),
        Language(
            "celestial",
            "Celestial",
            LanguageCategory.Exotic),
        Language(
            "draconic",
            "Draconic",
            LanguageCategory.Exotic),
        Language(
            "deep-speech",
            "Deep Speech",
            LanguageCategory.Exotic),
        Language(
            "infernal",
            "Infernal",
            LanguageCategory.Exotic),
        Language(
            "primordial",
            "Primordial",
            LanguageCategory.Exotic),
        Language(
            "sylvan",
            "Sylvan",
            LanguageCategory.Exotic),
        Language(
            "undercommon",
            "Undercommon",
            LanguageCategory.Exotic)
    ];

    private static readonly CreatureSizeExpectation[]
        CreatureSizeExpectations =
        [
            CreatureSize("tiny", "Tiny"),
            CreatureSize("small", "Small"),
            CreatureSize("medium", "Medium"),
            CreatureSize("large", "Large"),
            CreatureSize("huge", "Huge"),
            CreatureSize("gargantuan", "Gargantuan")
        ];

    private static readonly ConditionExpectation[]
        ConditionExpectations =
        [
            Condition("blinded", "Blinded"),
            Condition("charmed", "Charmed"),
            Condition("deafened", "Deafened"),
            Condition("exhaustion", "Exhaustion"),
            Condition("frightened", "Frightened"),
            Condition("grappled", "Grappled"),
            Condition("incapacitated", "Incapacitated"),
            Condition("invisible", "Invisible"),
            Condition("paralyzed", "Paralyzed"),
            Condition("petrified", "Petrified"),
            Condition("poisoned", "Poisoned"),
            Condition("prone", "Prone"),
            Condition("restrained", "Restrained"),
            Condition("stunned", "Stunned"),
            Condition("unconscious", "Unconscious")
        ];

    private static readonly DamageTypeExpectation[]
        DamageTypeExpectations =
        [
            DamageType("acid", "Acid"),
            DamageType("bludgeoning", "Bludgeoning"),
            DamageType("cold", "Cold"),
            DamageType("fire", "Fire"),
            DamageType("force", "Force"),
            DamageType("lightning", "Lightning"),
            DamageType("necrotic", "Necrotic"),
            DamageType("piercing", "Piercing"),
            DamageType("poison", "Poison"),
            DamageType("psychic", "Psychic"),
            DamageType("radiant", "Radiant"),
            DamageType("slashing", "Slashing"),
            DamageType("thunder", "Thunder")
        ];

    private static readonly SenseExpectation[] SenseExpectations =
    [
        Sense("darkvision", "Darkvision")
    ];

    private static readonly AlignmentExpectation[]
        AlignmentExpectations =
        [
            Alignment(
                "lawful-good",
                "Lawful Good",
                AlignmentEthic.Lawful,
                AlignmentMorality.Good),
            Alignment(
                "neutral-good",
                "Neutral Good",
                AlignmentEthic.Neutral,
                AlignmentMorality.Good),
            Alignment(
                "chaotic-good",
                "Chaotic Good",
                AlignmentEthic.Chaotic,
                AlignmentMorality.Good),
            Alignment(
                "lawful-neutral",
                "Lawful Neutral",
                AlignmentEthic.Lawful,
                AlignmentMorality.Neutral),
            Alignment(
                "neutral",
                "Neutral",
                AlignmentEthic.Neutral,
                AlignmentMorality.Neutral),
            Alignment(
                "chaotic-neutral",
                "Chaotic Neutral",
                AlignmentEthic.Chaotic,
                AlignmentMorality.Neutral),
            Alignment(
                "lawful-evil",
                "Lawful Evil",
                AlignmentEthic.Lawful,
                AlignmentMorality.Evil),
            Alignment(
                "neutral-evil",
                "Neutral Evil",
                AlignmentEthic.Neutral,
                AlignmentMorality.Evil),
            Alignment(
                "chaotic-evil",
                "Chaotic Evil",
                AlignmentEthic.Chaotic,
                AlignmentMorality.Evil)
        ];

    public static IReadOnlyList<string> Validate(
        CreatureVocabularyDefinitionSet definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        ValidateAbilities(definitions.Abilities, errors);
        ValidateSkills(definitions.Skills, errors);
        ValidateLanguages(definitions.Languages, errors);
        ValidateCreatureSizes(definitions.Sizes, errors);
        ValidateConditions(definitions.Conditions, errors);
        ValidateDamageTypes(definitions.DamageTypes, errors);
        ValidateSenses(definitions.Senses, errors);
        ValidateAlignments(definitions.Alignments, errors);

        return errors;
    }

    private static void ValidateAbilities(
        IReadOnlyList<AbilityDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != AbilityExpectations.Length)
        {
            errors.Add(
                "Official ability catalog must contain exactly " +
                $"{AbilityExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<AbilityId, AbilityDefinition>();

        foreach (AbilityDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official ability catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<AbilityId> expectedIds =
            AbilityExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (AbilityExpectation expectation in AbilityExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out AbilityDefinition? definition))
            {
                errors.Add(
                    "Official ability catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official ability '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official ability '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            AbilityId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official ability catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateSkills(
        IReadOnlyList<SkillDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != SkillExpectations.Length)
        {
            errors.Add(
                "Official skill catalog must contain exactly " +
                $"{SkillExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<SkillId, SkillDefinition>();

        foreach (SkillDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official skill catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<SkillId> expectedIds =
            SkillExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (SkillExpectation expectation in SkillExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out SkillDefinition? definition))
            {
                errors.Add(
                    "Official skill catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official skill '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            if (definition.NormallyAssociatedAbilityId !=
                expectation.NormallyAssociatedAbilityId)
            {
                errors.Add(
                    $"Official skill '{expectation.Id}' must " +
                    "normally associate with ability " +
                    $"'{expectation.NormallyAssociatedAbilityId}'; " +
                    $"found " +
                    $"'{definition.NormallyAssociatedAbilityId}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official skill '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            SkillId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official skill catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateLanguages(
        IReadOnlyList<LanguageDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != LanguageExpectations.Length)
        {
            errors.Add(
                "Official language catalog must contain exactly " +
                $"{LanguageExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<LanguageId, LanguageDefinition>();

        foreach (LanguageDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official language catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<LanguageId> expectedIds =
            LanguageExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            LanguageExpectation expectation
            in LanguageExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out LanguageDefinition? definition))
            {
                errors.Add(
                    "Official language catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official language '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            if (definition.Category != expectation.Category)
            {
                errors.Add(
                    $"Official language '{expectation.Id}' must " +
                    $"have category '{expectation.Category}'; " +
                    $"found '{definition.Category}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official language '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            LanguageId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official language catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateCreatureSizes(
        IReadOnlyList<CreatureSizeDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != CreatureSizeExpectations.Length)
        {
            errors.Add(
                "Official creature-size catalog must contain exactly " +
                $"{CreatureSizeExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<
                CreatureSizeId,
                CreatureSizeDefinition>();

        foreach (CreatureSizeDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official creature-size catalog contains " +
                    $"duplicate ID '{definition.Id}'.");
            }
        }

        HashSet<CreatureSizeId> expectedIds =
            CreatureSizeExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            CreatureSizeExpectation expectation
            in CreatureSizeExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out CreatureSizeDefinition? definition))
            {
                errors.Add(
                    "Official creature-size catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official creature size '{expectation.Id}' " +
                    $"must be named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official creature size '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            CreatureSizeId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official creature-size catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateConditions(
        IReadOnlyList<ConditionDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != ConditionExpectations.Length)
        {
            errors.Add(
                "Official condition catalog must contain exactly " +
                $"{ConditionExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<ConditionId, ConditionDefinition>();

        foreach (ConditionDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official condition catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<ConditionId> expectedIds =
            ConditionExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            ConditionExpectation expectation
            in ConditionExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out ConditionDefinition? definition))
            {
                errors.Add(
                    "Official condition catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official condition '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official condition '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            ConditionId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official condition catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateDamageTypes(
        IReadOnlyList<DamageTypeDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != DamageTypeExpectations.Length)
        {
            errors.Add(
                "Official damage-type catalog must contain exactly " +
                $"{DamageTypeExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<DamageTypeId, DamageTypeDefinition>();

        foreach (DamageTypeDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official damage-type catalog contains " +
                    $"duplicate ID '{definition.Id}'.");
            }
        }

        HashSet<DamageTypeId> expectedIds =
            DamageTypeExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            DamageTypeExpectation expectation
            in DamageTypeExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out DamageTypeDefinition? definition))
            {
                errors.Add(
                    "Official damage-type catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official damage type '{expectation.Id}' must " +
                    $"be named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official damage type '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            DamageTypeId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official damage-type catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateSenses(
        IReadOnlyList<SenseDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != SenseExpectations.Length)
        {
            errors.Add(
                "Official sense catalog must contain exactly " +
                $"{SenseExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<SenseId, SenseDefinition>();

        foreach (SenseDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official sense catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<SenseId> expectedIds =
            SenseExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            SenseExpectation expectation
            in SenseExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out SenseDefinition? definition))
            {
                errors.Add(
                    "Official sense catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official sense '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official sense '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            SenseId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official sense catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static void ValidateAlignments(
        IReadOnlyList<AlignmentDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count != AlignmentExpectations.Length)
        {
            errors.Add(
                "Official alignment catalog must contain exactly " +
                $"{AlignmentExpectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<AlignmentId, AlignmentDefinition>();

        foreach (AlignmentDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official alignment catalog contains duplicate " +
                    $"ID '{definition.Id}'.");
            }
        }

        HashSet<AlignmentId> expectedIds =
            AlignmentExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            AlignmentExpectation expectation
            in AlignmentExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out AlignmentDefinition? definition))
            {
                errors.Add(
                    "Official alignment catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official alignment '{expectation.Id}' must be " +
                    $"named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            if (definition.Ethic != expectation.Ethic)
            {
                errors.Add(
                    $"Official alignment '{expectation.Id}' must " +
                    $"have ethic '{expectation.Ethic}'; found " +
                    $"'{definition.Ethic}'.");
            }

            if (definition.Morality != expectation.Morality)
            {
                errors.Add(
                    $"Official alignment '{expectation.Id}' must " +
                    $"have morality '{expectation.Morality}'; found " +
                    $"'{definition.Morality}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official alignment '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            AlignmentId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official alignment catalog contains unexpected " +
                $"definition '{unexpectedId}'.");
        }
    }

    private static AbilityExpectation Ability(
        string idSuffix,
        string name)
    {
        return new AbilityExpectation(
            new AbilityId(
                "dnd5e2014.ability." + idSuffix),
            name,
            AbilitySource);
    }

    private static SkillExpectation Skill(
        string idSuffix,
        string name,
        string abilityIdSuffix)
    {
        return new SkillExpectation(
            new SkillId(
                "dnd5e2014.skill." + idSuffix),
            name,
            new AbilityId(
                "dnd5e2014.ability." +
                abilityIdSuffix),
            SkillSource);
    }

    private static LanguageExpectation Language(
        string idSuffix,
        string name,
        LanguageCategory category)
    {
        return new LanguageExpectation(
            new LanguageId(
                "dnd5e2014.language." + idSuffix),
            name,
            category,
            LanguageSource);
    }

    private static CreatureSizeExpectation CreatureSize(
        string idSuffix,
        string name)
    {
        return new CreatureSizeExpectation(
            new CreatureSizeId(
                "dnd5e2014.creature-size." + idSuffix),
            name,
            CreatureSizeSource);
    }

    private static ConditionExpectation Condition(
        string idSuffix,
        string name)
    {
        return new ConditionExpectation(
            new ConditionId(
                "dnd5e2014.condition." + idSuffix),
            name,
            ConditionSource);
    }

    private static DamageTypeExpectation DamageType(
        string idSuffix,
        string name)
    {
        return new DamageTypeExpectation(
            new DamageTypeId(
                "dnd5e2014.damage-type." + idSuffix),
            name,
            DamageTypeSource);
    }

    private static SenseExpectation Sense(
        string idSuffix,
        string name)
    {
        return new SenseExpectation(
            new SenseId(
                "dnd5e2014.sense." + idSuffix),
            name,
            SenseSource);
    }

    private static AlignmentExpectation Alignment(
        string idSuffix,
        string name,
        AlignmentEthic ethic,
        AlignmentMorality morality)
    {
        return new AlignmentExpectation(
            new AlignmentId(
                "dnd5e2014.alignment." + idSuffix),
            name,
            ethic,
            morality,
            AlignmentSource);
    }

    private readonly record struct AbilityExpectation(
        AbilityId Id,
        string Name,
        OfficialSourceExpectation Source);

    private readonly record struct SkillExpectation(
        SkillId Id,
        string Name,
        AbilityId NormallyAssociatedAbilityId,
        OfficialSourceExpectation Source);

    private readonly record struct LanguageExpectation(
        LanguageId Id,
        string Name,
        LanguageCategory Category,
        OfficialSourceExpectation Source);

    private readonly record struct CreatureSizeExpectation(
        CreatureSizeId Id,
        string Name,
        OfficialSourceExpectation Source);

    private readonly record struct ConditionExpectation(
        ConditionId Id,
        string Name,
        OfficialSourceExpectation Source);

    private readonly record struct DamageTypeExpectation(
        DamageTypeId Id,
        string Name,
        OfficialSourceExpectation Source);

    private readonly record struct SenseExpectation(
        SenseId Id,
        string Name,
        OfficialSourceExpectation Source);

    private readonly record struct AlignmentExpectation(
        AlignmentId Id,
        string Name,
        AlignmentEthic Ethic,
        AlignmentMorality Morality,
        OfficialSourceExpectation Source);
}
