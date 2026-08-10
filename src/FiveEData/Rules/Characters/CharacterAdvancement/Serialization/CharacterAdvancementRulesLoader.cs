using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Characters.CharacterAdvancement.Serialization;

internal static class CharacterAdvancementRulesLoader
{
    public static CharacterAdvancementRules LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static CharacterAdvancementRules LoadFromJson(string json)
    {
        CharacterAdvancementRulesData data =
            StrictJson.DeserializeObject<CharacterAdvancementRulesData>(
                json,
                "Character advancement rules");

        try
        {
            CharacterAdvancementRules rules = Map(data);
            CharacterAdvancementRulesValidator.EnsureValid(rules);
            return rules;
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            throw new InvalidDataException(
                "Invalid character advancement rules definition.",
                exception);
        }
    }

    private static CharacterAdvancementRules Map(
        CharacterAdvancementRulesData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CharacterAdvancementLevelData[] levelData =
            data.Levels
            ?? throw new ArgumentException(
                "Character advancement levels are required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Character advancement sources are required.",
                nameof(data));

        CharacterAdvancementLevel[] levels = levelData
            .Select(
                (level, index) =>
                    level is null
                        ? throw new ArgumentException(
                            $"Character advancement level at index {index} " +
                            $"is required.",
                            nameof(data))
                        : new CharacterAdvancementLevel(
                            level: level.Level,
                            experiencePointThreshold:
                                level.ExperiencePointThreshold,
                            proficiencyBonus: level.ProficiencyBonus))
            .ToArray();

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new CharacterAdvancementRules(levels, sources);
    }
}
