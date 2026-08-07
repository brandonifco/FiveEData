using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.MagicalSecrets.Serialization;

internal sealed class MagicalSecretsProgressionDetailData
{
    [JsonRequired]
    public MagicalSecretsChoiceGrantData[]? SpellsKnownByLevel { get; init; }

    [JsonRequired]
    public bool CountsAgainstSpellsKnown { get; init; }
}

internal sealed class MagicalSecretsChoiceGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int SpellsKnown { get; init; }
}
