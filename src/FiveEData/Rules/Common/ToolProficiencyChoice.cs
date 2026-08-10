using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Rules.Common;

/// <summary>
/// A "choose N tool proficiencies" grant. The PHB states these two ways,
/// and they are mutually exclusive: by <em>family</em> ("three musical
/// instruments of your choice", "one type of artisan's tools or one
/// musical instrument") or by an explicit <em>option list</em> ("the
/// artisan's tools of your choice: smith's tools, brewer's supplies, or
/// mason's tools" — the Dwarf's Tool Proficiency).
///
/// <see cref="ToolFamilyIds"/> holds more than one family only where the
/// book genuinely offers a cross-family choice (Monk). A single family is
/// the common case; an option list of one would not be a choice at all,
/// so a populated <see cref="ToolOptionIds"/> must hold at least two.
///
/// Fixed grants ("Thieves' tools", "the disguise kit and the poisoner's
/// kit") are not choices and are stored as a plain tool ID list on the
/// owning definition instead.
/// </summary>
public sealed record ToolProficiencyChoice
{
    public ToolProficiencyChoice(
        int count,
        IEnumerable<ToolFamilyId>? toolFamilyIds = null,
        IEnumerable<ToolId>? toolOptionIds = null)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "A tool proficiency choice must grant at least one " +
                "proficiency.");
        }

        ToolFamilyId[] families = toolFamilyIds?.ToArray() ?? [];
        ToolId[] options = toolOptionIds?.ToArray() ?? [];

        if (families.Length == 0 && options.Length == 0)
        {
            throw new ArgumentException(
                "A tool proficiency choice must name either tool families " +
                "or explicit tool options.",
                nameof(toolFamilyIds));
        }

        if (families.Length > 0 && options.Length > 0)
        {
            throw new ArgumentException(
                "A tool proficiency choice must name tool families or " +
                "explicit tool options, not both.",
                nameof(toolOptionIds));
        }

        if (families.Distinct().Count() != families.Length)
        {
            throw new ArgumentException(
                "A tool proficiency choice must not repeat a tool family.",
                nameof(toolFamilyIds));
        }

        if (options.Distinct().Count() != options.Length)
        {
            throw new ArgumentException(
                "A tool proficiency choice must not repeat a tool option.",
                nameof(toolOptionIds));
        }

        if (options.Length is 1)
        {
            throw new ArgumentException(
                "A tool proficiency choice offering explicit options must " +
                "offer at least two — a choice of one is not a choice.",
                nameof(toolOptionIds));
        }

        if (options.Length > 0 && count >= options.Length)
        {
            throw new ArgumentException(
                "A tool proficiency choice must grant fewer proficiencies " +
                "than the number of options it offers.",
                nameof(count));
        }

        Count = count;
        ToolFamilyIds = Array.AsReadOnly(families);
        ToolOptionIds = Array.AsReadOnly(options);
    }

    public int Count { get; }

    public IReadOnlyList<ToolFamilyId> ToolFamilyIds { get; }

    public IReadOnlyList<ToolId> ToolOptionIds { get; }
}
