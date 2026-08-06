using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.EldritchInvocationsKnown.Serialization;

internal sealed class EldritchInvocationsKnownProgressionDetailData
{
    [JsonRequired]
    public EldritchInvocationsKnownGrantData[]? InvocationsKnownByLevel
    {
        get;
        init;
    }
}

internal sealed class EldritchInvocationsKnownGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int InvocationsKnown { get; init; }
}
