using System.Text.Json.Serialization;

namespace FiveEData.Rules.Characters.Encumbrance.Serialization;

internal sealed class CarryingCapacitySizeMultiplierGrantData
{
    [JsonRequired]
    public string? SizeId { get; init; }

    [JsonRequired]
    public double Multiplier { get; init; }
}
