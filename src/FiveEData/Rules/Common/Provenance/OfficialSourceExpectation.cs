namespace FiveEData.Rules.Common.Provenance;

internal readonly record struct OfficialSourceExpectation(
    SourceDocumentId DocumentId,
    int Page,
    string Section);
