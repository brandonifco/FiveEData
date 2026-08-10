namespace FiveEData.Rules.Characters.CharacterAdvancement;

/// <summary>
/// One row of the Character Advancement table (p.15) — the experience
/// point total required to reach a character level, and the proficiency
/// bonus a character of that level has.
/// </summary>
public readonly record struct CharacterAdvancementLevel
{
    public CharacterAdvancementLevel(
        int level,
        int experiencePointThreshold,
        int proficiencyBonus)
    {
        if (level is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(level),
                level,
                "Character level must be between 1 and 20.");
        }

        if (experiencePointThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(experiencePointThreshold),
                experiencePointThreshold,
                "Experience point threshold cannot be negative.");
        }

        if (proficiencyBonus <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(proficiencyBonus),
                proficiencyBonus,
                "Proficiency bonus must be greater than zero.");
        }

        Level = level;
        ExperiencePointThreshold = experiencePointThreshold;
        ProficiencyBonus = proficiencyBonus;
    }

    public int Level { get; }

    public int ExperiencePointThreshold { get; }

    public int ProficiencyBonus { get; }
}
