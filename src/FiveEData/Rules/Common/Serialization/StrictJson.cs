using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal static class StrictJson
{
    private static readonly JsonSerializerOptions SerializerOptions =
        CreateSerializerOptions();

    public static T DeserializeObject<T>(
        string json,
        string dataDescription)
        where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDescription);

        try
        {
            ValidateNoDuplicateProperties(json);

            return JsonSerializer.Deserialize<T>(
                json,
                SerializerOptions)
                ?? throw new InvalidDataException(
                    $"{dataDescription} JSON must contain an object.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"{dataDescription} JSON could not be parsed.",
                exception);
        }
    }

    public static T[] DeserializeArray<T>(
        string json,
        string dataDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDescription);

        try
        {
            ValidateNoDuplicateProperties(json);

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

    private static void ValidateNoDuplicateProperties(
        string json)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(json);

        var reader =
            new Utf8JsonReader(
                utf8,
                new JsonReaderOptions
                {
                    CommentHandling =
                        JsonCommentHandling.Disallow,
                    AllowTrailingCommas = false
                });

        var propertyScopes =
            new Stack<HashSet<string>>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    propertyScopes.Push(
                        new HashSet<string>(
                            StringComparer.Ordinal));
                    break;

                case JsonTokenType.PropertyName:
                    if (propertyScopes.Count == 0)
                    {
                        throw new JsonException(
                            "JSON property appeared outside " +
                            "an object.");
                    }

                    string propertyName =
                        reader.GetString()
                        ?? throw new JsonException(
                            "JSON property name cannot be null.");

                    if (!propertyScopes.Peek().Add(
                            propertyName))
                    {
                        throw new JsonException(
                            $"Duplicate JSON property " +
                            $"'{propertyName}' at byte position " +
                            $"{reader.TokenStartIndex}.");
                    }

                    break;

                case JsonTokenType.EndObject:
                    if (propertyScopes.Count == 0)
                    {
                        throw new JsonException(
                            "JSON object scope ended " +
                            "unexpectedly.");
                    }

                    propertyScopes.Pop();
                    break;
            }
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
