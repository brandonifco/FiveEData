using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.CreateThrall.Serialization;

internal sealed class CreateThrallDetailData
{
    [JsonRequired]
    public bool RequiresIncapacitatedTarget { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public bool GrantsTelepathyWhileOnSamePlane { get; init; }
}
