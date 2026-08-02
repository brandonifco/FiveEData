using System.Globalization;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses;

internal readonly record struct OfficialSourceExpectation(
    SourceDocumentId DocumentId,
    int Page,
    string Section);

internal static class OfficialSourceReferenceSemanticValidator
{
    public static void Validate(
        string owner,
        IReadOnlyList<SourceReference> sources,
        OfficialSourceExpectation expectation,
        ICollection<string> errors)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(errors);

        if (sources.Count != 1)
        {
            errors.Add(
                $"{owner} must have exactly one source reference; " +
                $"found {sources.Count}.");
            return;
        }

        SourceReference source = sources[0];

        if (source.DocumentId == expectation.DocumentId &&
            source.Page == expectation.Page &&
            string.Equals(
                source.Section,
                expectation.Section,
                StringComparison.Ordinal))
        {
            return;
        }

        string actualPage =
            source.Page is { } page
                ? page.ToString(CultureInfo.InvariantCulture)
                : "<none>";

        string actualSection =
            source.Section ?? "<none>";

        errors.Add(
            $"{owner} must use source " +
            $"'{expectation.DocumentId}', page " +
            $"{expectation.Page}, section " +
            $"'{expectation.Section}'; found source " +
            $"'{source.DocumentId}', page {actualPage}, section " +
            $"'{actualSection}'.");
    }
}
