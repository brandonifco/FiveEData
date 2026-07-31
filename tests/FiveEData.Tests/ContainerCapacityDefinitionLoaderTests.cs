using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;

namespace FiveEData.Tests;

public sealed class ContainerCapacityDefinitionLoaderTests
{
    [Fact]
    public void LoadFromJson_MapsStructuredCapacity()
    {
        ContainerCapacityDefinition definition =
            Assert.Single(ContainerCapacityDefinitionLoader.LoadFromJson(ValidJson));

        Assert.Equal(
            "dnd5e2014.adventuring-gear.backpack",
            definition.AdventuringGearId.Value);
        Assert.Equal(1m, definition.SolidVolume?.Amount);
        Assert.Equal(ContainerVolumeUnit.CubicFoot, definition.SolidVolume?.Unit);
        Assert.Null(definition.LiquidVolume);
        Assert.Equal(30m, definition.GearWeightCapacity?.Pounds);
        Assert.True(definition.AllowsExteriorItemAttachment);
    }

    [Fact]
    public void UnknownJsonMember_IsRejected()
    {
        string json = ValidJson.Replace(
            "    \"allowsExteriorItemAttachment\": true,",
            "    \"allowsExteriorItemAttachment\": true,\n    \"unexpected\": true,",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ContainerCapacityDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredField_IsRejected()
    {
        string json = ValidJson.Replace(
            "    \"gearWeightCapacityPounds\": 30,\n",
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ContainerCapacityDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateGearId_IsRejected()
    {
        string duplicated =
            "[" + ValidJson.Trim()[1..^1] + "," + ValidJson.Trim()[1..^1] + "]";

        Assert.Throws<InvalidDataException>(
            () => ContainerCapacityDefinitionLoader.LoadFromJson(duplicated));
    }

    [Fact]
    public void InvalidVolumeUnit_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"unit\": \"CubicFoot\"",
            "\"unit\": \"Unknown\"",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => ContainerCapacityDefinitionLoader.LoadFromJson(json));
    }

    private const string ValidJson = """
    [
      {
        "adventuringGearId": "dnd5e2014.adventuring-gear.backpack",
        "solidVolume": { "amount": 1, "unit": "CubicFoot" },
        "liquidVolume": null,
        "gearWeightCapacityPounds": 30,
        "allowsExteriorItemAttachment": true,
        "sources": [
          {
            "documentId": "dnd5e2014.source.phb-first-printing",
            "page": 153,
            "section": "Container Capacity"
          }
        ]
      }
    ]
    """;
}
