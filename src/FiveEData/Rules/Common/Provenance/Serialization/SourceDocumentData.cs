using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Provenance.Serialization;

internal sealed class SourceDocumentData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Title { get; init; }

    [JsonRequired]
    public string? Edition { get; init; }

    [JsonRequired]
    public string? Printing { get; init; }

    [JsonRequired]
    public string? PublicationDate { get; init; }

    [JsonRequired]
    public string? Isbn { get; init; }
}
