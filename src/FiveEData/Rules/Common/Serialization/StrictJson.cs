using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal static class StrictJson
{
    private static readonly JsonSerializerOptions SerializerOptions =
        CreateSerializerOptions();

    public static T[] DeserializeArray<T>(
        string json,
        string dataDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDescription);

        try
        {
            return JsonSerializer.Deserialize<T[]>(
                json,
                SerializerOptions)
                ?? throw new InvalidDataException(
                    $"{dataDescription} JSON must contain an array.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"{dataDescription} JSON could not be parsed.",
                exception);
        }
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            AllowTrailingCommas = false,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        options.Converters.Add(
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false));

        return options;
    }
}
