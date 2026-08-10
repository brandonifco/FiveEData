using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Rules.Common.Serialization;

internal static class ToolProficiencyChoiceDataMapper
{
    public static ToolProficiencyChoice Map(ToolProficiencyChoiceData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        IReadOnlyList<string> familyIdValues = data.ToolFamilyIds
            ?? throw new ArgumentException(
                "Tool proficiency choice tool family IDs are required.",
                nameof(data));

        IReadOnlyList<string> optionIdValues = data.ToolOptionIds
            ?? throw new ArgumentException(
                "Tool proficiency choice tool option IDs are required.",
                nameof(data));

        return new ToolProficiencyChoice(
            data.Count,
            familyIdValues.Select(value => new ToolFamilyId(value)),
            optionIdValues.Select(value => new ToolId(value)));
    }
}
