using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DraconicResilience.Serialization;

internal sealed class DraconicResilienceDetailData
{
    [JsonRequired]
    public int HitPointBonusPerLevel { get; init; }

    [JsonRequired]
    public int UnarmoredBaseArmorClass { get; init; }

    [JsonRequired]
    public bool UnarmoredIncludesDexterityModifier { get; init; }
}
