using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Provenance.Serialization;

internal sealed class SourceReferenceData
{
    [JsonRequired]
    public string? DocumentId { get; init; }

    [JsonRequired]
    public int? Page { get; init; }

    [JsonRequired]
    public string? Section { get; init; }
}
