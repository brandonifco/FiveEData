using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class ToolProficiencyChoiceData
{
    [JsonRequired]
    public int Count { get; init; }

    [JsonRequired]
    public IReadOnlyList<string>? ToolFamilyIds { get; init; }

    [JsonRequired]
    public IReadOnlyList<string>? ToolOptionIds { get; init; }
}
