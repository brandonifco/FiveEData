using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class SourceReferenceTests
{
    [Fact]
    public void DefaultDocumentId_IsRejected()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => new SourceReference(default));

        Assert.Equal("documentId", exception.ParamName);
    }

    [Fact]
    public void ValidDocumentId_WithOmittedPageAndSection_IsAccepted()
    {
        var documentId =
            new SourceDocumentId("dnd5e2014.source.test");

        var source = new SourceReference(documentId);

        Assert.Equal(documentId, source.DocumentId);
        Assert.Null(source.Page);
        Assert.Null(source.Section);
    }

    [Fact]
    public void PositivePageAndNonblankSection_AreAccepted()
    {
        var source = new SourceReference(
            new SourceDocumentId("dnd5e2014.source.test"),
            page: 123,
            section: "Chapter 4: Personality and Background");

        Assert.Equal(123, source.Page);
        Assert.Equal(
            "Chapter 4: Personality and Background",
            source.Section);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonpositivePage_IsRejected(int page)
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.test"),
                    page));

        Assert.Equal("page", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void BlankSection_IsRejected(string section)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.test"),
                    section: section));

        Assert.Equal("section", exception.ParamName);
    }

    [Fact]
    public void EqualValues_RemainEqual()
    {
        var documentId =
            new SourceDocumentId("dnd5e2014.source.test");

        var first = new SourceReference(
            documentId,
            page: 42,
            section: "Test Section");

        var second = new SourceReference(
            documentId,
            page: 42,
            section: "Test Section");

        Assert.Equal(first, second);
    }
}
