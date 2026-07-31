using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;

namespace FiveEData.Tests;

public sealed class ArmorDefinitionLoaderTests
{
    [Fact]
    public void ValidArmorJson_MapsToDomainDefinition()
    {
        IReadOnlyList<ArmorDefinition> armor =
            ArmorDefinitionLoader.LoadFromJson(ValidArmorJson);

        ArmorDefinition definition = Assert.Single(armor);

        Assert.Equal(
            new ArmorId("dnd5e2014.armor.test"),
            definition.Id);
        Assert.Equal("Test armor", definition.Name);
        Assert.Equal(ArmorCategory.Medium, definition.Category);
        Assert.Equal(5000, definition.Cost.CopperPieces);
        Assert.Equal(20m, definition.Weight.Pounds);
        Assert.Equal(14, definition.ArmorClass.BaseArmorClass);
        Assert.True(definition.ArmorClass.IncludesDexterityModifier);
        Assert.Equal(2, definition.ArmorClass.MaximumDexterityModifier);
        Assert.Null(definition.MinimumStrengthForFullSpeed);
        Assert.False(definition.ImposesStealthDisadvantage);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void UnknownJsonMember_IsRejected()
    {
        string json = ValidArmorJson.Replace(
            "\"imposesStealthDisadvantage\": false,",
            "\"imposesStealthDisadvantage\": false,\n    \"unexpected\": true,",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredArmorClass_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.armor.test",
            "name": "Test armor",
            "category": "Medium",
            "cost": {
              "copperPieces": 5000
            },
            "weight": {
              "pounds": 20
            },
            "minimumStrengthForFullSpeed": null,
            "imposesStealthDisadvantage": false,
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
            () => ArmorDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void InvalidCategory_IsRejected()
    {
        string json = ValidArmorJson.Replace(
            "\"category\": \"Medium\"",
            "\"category\": \"Unknown\"",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void InvalidCategorySemantics_AreRejected()
    {
        string json = ValidArmorJson.Replace(
            "\"maximumDexterityModifier\": 2",
            "\"maximumDexterityModifier\": null",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ArmorDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = ValidArmorJson[2..^2];
        string json = $"[{item},{item}]";

        Assert.Throws<InvalidDataException>(
            () => ArmorDefinitionLoader.LoadFromJson(json));
    }

    private const string ValidArmorJson = """
    [
      {
        "id": "dnd5e2014.armor.test",
        "name": "Test armor",
        "category": "Medium",
        "cost": {
          "copperPieces": 5000
        },
        "weight": {
          "pounds": 20
        },
        "armorClass": {
          "baseArmorClass": 14,
          "includesDexterityModifier": true,
          "maximumDexterityModifier": 2
        },
        "minimumStrengthForFullSpeed": null,
        "imposesStealthDisadvantage": false,
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
