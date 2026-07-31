using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;

namespace FiveEData.Tests;

public sealed class ShieldDefinitionLoaderTests
{
    [Fact]
    public void ValidShieldJson_MapsToDomainDefinition()
    {
        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromJson(ValidShieldJson);

        ShieldDefinition shield = Assert.Single(shields);

        Assert.Equal(
            new ShieldId("dnd5e2014.armor.shield"),
            shield.Id);
        Assert.Equal("Shield", shield.Name);
        Assert.Equal(1000, shield.Cost.CopperPieces);
        Assert.Equal(6m, shield.Weight.Pounds);
        Assert.Equal(2, shield.ArmorClassBonus);
        Assert.Single(shield.Sources);
    }

    [Fact]
    public void MissingArmorClassBonus_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.armor.shield",
            "name": "Shield",
            "cost": {
              "copperPieces": 1000
            },
            "weight": {
              "pounds": 6
            },
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 145,
                "section": "Chapter 5: Equipment — Armor and Shields"
              }
            ]
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => ShieldDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void InvalidArmorClassBonus_IsRejected()
    {
        string json = ValidShieldJson.Replace(
            "\"armorClassBonus\": 2",
            "\"armorClassBonus\": 0",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ShieldDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => ShieldDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = ValidShieldJson[2..^2];
        string json = $"[{item},{item}]";

        Assert.Throws<InvalidDataException>(
            () => ShieldDefinitionLoader.LoadFromJson(json));
    }

    private const string ValidShieldJson = """
    [
      {
        "id": "dnd5e2014.armor.shield",
        "name": "Shield",
        "cost": {
          "copperPieces": 1000
        },
        "weight": {
          "pounds": 6
        },
        "armorClassBonus": 2,
        "sources": [
          {
            "documentId": "dnd5e2014.source.phb-first-printing",
            "page": 145,
            "section": "Chapter 5: Equipment — Armor and Shields"
          }
        ]
      }
    ]
    """;
}
