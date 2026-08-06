using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.MysticArcanum.Serialization;

internal sealed class MysticArcanumProgressionDetailData
{
    [JsonRequired]
    public MysticArcanumGrantData[]? ArcanumByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class MysticArcanumGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int SpellLevel { get; init; }
}
