using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Armor.Serialization;

internal sealed class ArmorDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public ArmorCategory Category { get; init; }

    [JsonRequired]
    public MoneyData? Cost { get; init; }

    [JsonRequired]
    public WeightData? Weight { get; init; }

    [JsonRequired]
    public ArmorClassFormulaData? ArmorClass { get; init; }

    [JsonRequired]
    public int? MinimumStrengthForFullSpeed { get; init; }

    [JsonRequired]
    public bool ImposesStealthDisadvantage { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ArmorClassFormulaData
{
    [JsonRequired]
    public int BaseArmorClass { get; init; }

    [JsonRequired]
    public bool IncludesDexterityModifier { get; init; }

    [JsonRequired]
    public int? MaximumDexterityModifier { get; init; }
}
