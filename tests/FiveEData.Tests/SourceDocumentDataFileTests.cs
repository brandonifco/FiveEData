using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Tests;

public sealed class SourceDocumentDataFileTests
{
    [Fact]
    public void SourcesJson_LoadsFirstPrintingPlayerHandbook()
    {
        string path = Path.Combine(
            FindRepositoryRoot(),
            "Data",
            "dnd5e2014",
            "sources.json");

        SourceDocument source =
            Assert.Single(SourceDocumentLoader.LoadFromFile(path));

        Assert.Equal(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            source.Id);
        Assert.Equal("Player's Handbook", source.Title);
        Assert.Equal("D&D 5th Edition", source.Edition);
        Assert.Equal("First Printing", source.Printing);
        Assert.Equal("2014-08", source.PublicationDate);
        Assert.Equal("978-0-7869-6560-1", source.Isbn);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
