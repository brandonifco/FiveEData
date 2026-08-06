using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.EldritchInvocations.Serialization;

internal sealed class EldritchInvocationDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public bool RequiresEldritchBlastCantrip { get; init; }

    [JsonRequired]
    public int? RequiredMinimumLevel { get; init; }

    [JsonRequired]
    public WarlockPactBoon? RequiresPactBoon { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
