using System.Reflection;
using System.Text;

namespace FiveEData.Rules.Catalog;

internal static class EmbeddedDataReader
{
    public static string ReadRequiredText(string resourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);

        Assembly assembly = typeof(EmbeddedDataReader).Assembly;

        using Stream stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Required embedded rules data '{resourceName}' was not found.");

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        return reader.ReadToEnd();
    }
}
