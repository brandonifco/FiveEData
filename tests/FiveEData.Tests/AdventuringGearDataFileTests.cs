using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;

namespace FiveEData.Tests;

public sealed class AdventuringGearDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsAllNinetyFiveNonAmmunitionLeafListings()
    {
        IReadOnlyList<AdventuringGearDefinition> definitions = LoadCanonical();

        Assert.Equal(95, definitions.Count);
        Assert.Equal(95, definitions.Select(item => item.Id).Distinct().Count());

        string[] ammunitionNames =
        [
            "Arrows (20)",
            "Blowgun needles (50)",
            "Crossbow bolts (20)",
            "Sling bullets (20)"
        ];

        Assert.DoesNotContain(
            definitions,
            definition => ammunitionNames.Contains(
                definition.Name,
                StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingAdventuringGearTable()
    {
        IReadOnlyDictionary<AdventuringGearId, AdventuringGearDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedGearRow expected in Expected)
        {
            AdventuringGearDefinition definition =
                actual[new AdventuringGearId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(expected.CopperPieces, definition.Cost.CopperPieces);

            if (expected.Pounds is null)
            {
                Assert.Null(definition.ListedWeight);
            }
            else
            {
                Assert.NotNull(definition.ListedWeight);
                Assert.Equal(
                    expected.Pounds.Value,
                    definition.ListedWeight.Weight.Pounds);
                Assert.Equal(
                    expected.Qualifier,
                    definition.ListedWeight.Qualifier);
            }

            string[] expectedRuleIds = ExpectedSpecialRuleIds.TryGetValue(
                expected.Id,
                out string[]? ruleIds)
                ? ruleIds
                : [];

            Assert.Equal(
                expectedRuleIds,
                definition.SpecialRuleIds
                    .Select(ruleId => ruleId.Value)
                    .ToArray());

            var source = Assert.Single(definition.Sources);
            Assert.Equal(150, source.Page);
            Assert.Equal(
                "Chapter 5: Equipment — Adventuring Gear",
                source.Section);
        }
    }

    private static IReadOnlyList<AdventuringGearDefinition> LoadCanonical()
    {
        string root = FindRepositoryRoot();

        return AdventuringGearDefinitionLoader.LoadFromFile(
            Path.Combine(
                root,
                "Data",
                "dnd5e2014",
                "adventuring-gear.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedGearRow(
        string Id,
        string Name,
        long CopperPieces,
        decimal? Pounds,
        string? Qualifier);

    private static readonly IReadOnlyDictionary<string, string[]> ExpectedSpecialRuleIds =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["dnd5e2014.adventuring-gear.acid-vial"] = ["dnd5e2014.adventuring-gear-rule.acid"],
            ["dnd5e2014.adventuring-gear.alchemists-fire-flask"] = ["dnd5e2014.adventuring-gear-rule.alchemists-fire"],
            ["dnd5e2014.adventuring-gear.antitoxin-vial"] = ["dnd5e2014.adventuring-gear-rule.antitoxin"],
            ["dnd5e2014.adventuring-gear.arcane-focus-crystal"] = ["dnd5e2014.adventuring-gear-rule.arcane-focus"],
            ["dnd5e2014.adventuring-gear.arcane-focus-orb"] = ["dnd5e2014.adventuring-gear-rule.arcane-focus"],
            ["dnd5e2014.adventuring-gear.arcane-focus-rod"] = ["dnd5e2014.adventuring-gear-rule.arcane-focus"],
            ["dnd5e2014.adventuring-gear.arcane-focus-staff"] = ["dnd5e2014.adventuring-gear-rule.arcane-focus"],
            ["dnd5e2014.adventuring-gear.arcane-focus-wand"] = ["dnd5e2014.adventuring-gear-rule.arcane-focus"],
            ["dnd5e2014.adventuring-gear.ball-bearings-bag-1000"] = ["dnd5e2014.adventuring-gear-rule.ball-bearings"],
            ["dnd5e2014.adventuring-gear.block-and-tackle"] = ["dnd5e2014.adventuring-gear-rule.block-and-tackle"],
            ["dnd5e2014.adventuring-gear.book"] = ["dnd5e2014.adventuring-gear-rule.book"],
            ["dnd5e2014.adventuring-gear.caltrops-bag-20"] = ["dnd5e2014.adventuring-gear-rule.caltrops"],
            ["dnd5e2014.adventuring-gear.candle"] = ["dnd5e2014.adventuring-gear-rule.candle"],
            ["dnd5e2014.adventuring-gear.case-crossbow-bolt"] = ["dnd5e2014.adventuring-gear-rule.case-crossbow-bolt"],
            ["dnd5e2014.adventuring-gear.case-map-or-scroll"] = ["dnd5e2014.adventuring-gear-rule.case-map-or-scroll"],
            ["dnd5e2014.adventuring-gear.chain-10-feet"] = ["dnd5e2014.adventuring-gear-rule.chain"],
            ["dnd5e2014.adventuring-gear.climbers-kit"] = ["dnd5e2014.adventuring-gear-rule.climbers-kit"],
            ["dnd5e2014.adventuring-gear.component-pouch"] = ["dnd5e2014.adventuring-gear-rule.component-pouch"],
            ["dnd5e2014.adventuring-gear.crowbar"] = ["dnd5e2014.adventuring-gear-rule.crowbar"],
            ["dnd5e2014.adventuring-gear.druidic-focus-sprig-of-mistletoe"] = ["dnd5e2014.adventuring-gear-rule.druidic-focus"],
            ["dnd5e2014.adventuring-gear.druidic-focus-totem"] = ["dnd5e2014.adventuring-gear-rule.druidic-focus"],
            ["dnd5e2014.adventuring-gear.druidic-focus-wooden-staff"] = ["dnd5e2014.adventuring-gear-rule.druidic-focus"],
            ["dnd5e2014.adventuring-gear.druidic-focus-yew-wand"] = ["dnd5e2014.adventuring-gear-rule.druidic-focus"],
            ["dnd5e2014.adventuring-gear.fishing-tackle"] = ["dnd5e2014.adventuring-gear-rule.fishing-tackle"],
            ["dnd5e2014.adventuring-gear.healers-kit"] = ["dnd5e2014.adventuring-gear-rule.healers-kit"],
            ["dnd5e2014.adventuring-gear.holy-symbol-amulet"] = ["dnd5e2014.adventuring-gear-rule.holy-symbol"],
            ["dnd5e2014.adventuring-gear.holy-symbol-emblem"] = ["dnd5e2014.adventuring-gear-rule.holy-symbol"],
            ["dnd5e2014.adventuring-gear.holy-symbol-reliquary"] = ["dnd5e2014.adventuring-gear-rule.holy-symbol"],
            ["dnd5e2014.adventuring-gear.holy-water-flask"] = ["dnd5e2014.adventuring-gear-rule.holy-water"],
            ["dnd5e2014.adventuring-gear.hunting-trap"] = ["dnd5e2014.adventuring-gear-rule.hunting-trap"],
            ["dnd5e2014.adventuring-gear.lamp"] = ["dnd5e2014.adventuring-gear-rule.lamp"],
            ["dnd5e2014.adventuring-gear.lantern-bullseye"] = ["dnd5e2014.adventuring-gear-rule.lantern-bullseye"],
            ["dnd5e2014.adventuring-gear.lantern-hooded"] = ["dnd5e2014.adventuring-gear-rule.lantern-hooded"],
            ["dnd5e2014.adventuring-gear.lock"] = ["dnd5e2014.adventuring-gear-rule.lock"],
            ["dnd5e2014.adventuring-gear.magnifying-glass"] = ["dnd5e2014.adventuring-gear-rule.magnifying-glass"],
            ["dnd5e2014.adventuring-gear.manacles"] = ["dnd5e2014.adventuring-gear-rule.manacles"],
            ["dnd5e2014.adventuring-gear.mess-kit"] = ["dnd5e2014.adventuring-gear-rule.mess-kit"],
            ["dnd5e2014.adventuring-gear.oil-flask"] = ["dnd5e2014.adventuring-gear-rule.oil"],
            ["dnd5e2014.adventuring-gear.poison-basic-vial"] = ["dnd5e2014.adventuring-gear-rule.poison-basic"],
            ["dnd5e2014.adventuring-gear.potion-of-healing"] = ["dnd5e2014.adventuring-gear-rule.potion-of-healing"],
            ["dnd5e2014.adventuring-gear.pouch"] = ["dnd5e2014.adventuring-gear-rule.pouch"],
            ["dnd5e2014.adventuring-gear.quiver"] = ["dnd5e2014.adventuring-gear-rule.quiver"],
            ["dnd5e2014.adventuring-gear.ram-portable"] = ["dnd5e2014.adventuring-gear-rule.ram-portable"],
            ["dnd5e2014.adventuring-gear.rations-1-day"] = ["dnd5e2014.adventuring-gear-rule.rations"],
            ["dnd5e2014.adventuring-gear.rope-hempen-50-feet"] = ["dnd5e2014.adventuring-gear-rule.rope"],
            ["dnd5e2014.adventuring-gear.rope-silk-50-feet"] = ["dnd5e2014.adventuring-gear-rule.rope"],
            ["dnd5e2014.adventuring-gear.scale-merchants"] = ["dnd5e2014.adventuring-gear-rule.scale-merchants"],
            ["dnd5e2014.adventuring-gear.spellbook"] = ["dnd5e2014.adventuring-gear-rule.spellbook"],
            ["dnd5e2014.adventuring-gear.spyglass"] = ["dnd5e2014.adventuring-gear-rule.spyglass"],
            ["dnd5e2014.adventuring-gear.tent-two-person"] = ["dnd5e2014.adventuring-gear-rule.tent"],
            ["dnd5e2014.adventuring-gear.tinderbox"] = ["dnd5e2014.adventuring-gear-rule.tinderbox"],
            ["dnd5e2014.adventuring-gear.torch"] = ["dnd5e2014.adventuring-gear-rule.torch"]
        };

    private static readonly ExpectedGearRow[] Expected =
    [
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.abacus",
            "Abacus",
            200L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.acid-vial",
            "Acid (vial)",
            2500L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.alchemists-fire-flask",
            "Alchemist's fire (flask)",
            5000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.antitoxin-vial",
            "Antitoxin (vial)",
            5000L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.arcane-focus-crystal",
            "Crystal",
            1000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.arcane-focus-orb",
            "Orb",
            2000L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.arcane-focus-rod",
            "Rod",
            1000L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.arcane-focus-staff",
            "Staff",
            500L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.arcane-focus-wand",
            "Wand",
            1000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.backpack",
            "Backpack",
            200L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.ball-bearings-bag-1000",
            "Ball bearings (bag of 1,000)",
            100L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.barrel",
            "Barrel",
            200L,
            70m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.basket",
            "Basket",
            40L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.bedroll",
            "Bedroll",
            100L,
            7m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.bell",
            "Bell",
            100L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.blanket",
            "Blanket",
            50L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.block-and-tackle",
            "Block and tackle",
            100L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.book",
            "Book",
            2500L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.bottle-glass",
            "Bottle, glass",
            200L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.bucket",
            "Bucket",
            5L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.caltrops-bag-20",
            "Caltrops (bag of 20)",
            100L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.candle",
            "Candle",
            1L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.case-crossbow-bolt",
            "Case, crossbow bolt",
            100L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.case-map-or-scroll",
            "Case, map or scroll",
            100L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.chain-10-feet",
            "Chain (10 feet)",
            500L,
            10m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.chalk-1-piece",
            "Chalk (1 piece)",
            1L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.chest",
            "Chest",
            500L,
            25m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.climbers-kit",
            "Climber's kit",
            2500L,
            12m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.clothes-common",
            "Clothes, common",
            50L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.clothes-costume",
            "Clothes, costume",
            500L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.clothes-fine",
            "Clothes, fine",
            1500L,
            6m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.clothes-travelers",
            "Clothes, traveler's",
            200L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.component-pouch",
            "Component pouch",
            2500L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.crowbar",
            "Crowbar",
            200L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.druidic-focus-sprig-of-mistletoe",
            "Sprig of mistletoe",
            100L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.druidic-focus-totem",
            "Totem",
            100L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.druidic-focus-wooden-staff",
            "Wooden staff",
            500L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.druidic-focus-yew-wand",
            "Yew wand",
            1000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.fishing-tackle",
            "Fishing tackle",
            100L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.flask-or-tankard",
            "Flask or tankard",
            2L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.grappling-hook",
            "Grappling hook",
            200L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.hammer",
            "Hammer",
            100L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.hammer-sledge",
            "Hammer, sledge",
            200L,
            10m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.healers-kit",
            "Healer's kit",
            500L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.holy-symbol-amulet",
            "Amulet",
            500L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.holy-symbol-emblem",
            "Emblem",
            500L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.holy-symbol-reliquary",
            "Reliquary",
            500L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.holy-water-flask",
            "Holy water (flask)",
            2500L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.hourglass",
            "Hourglass",
            2500L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.hunting-trap",
            "Hunting trap",
            500L,
            25m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.ink-1-ounce-bottle",
            "Ink (1 ounce bottle)",
            1000L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.ink-pen",
            "Ink pen",
            2L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.jug-or-pitcher",
            "Jug or pitcher",
            2L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.ladder-10-foot",
            "Ladder (10-foot)",
            10L,
            25m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.lamp",
            "Lamp",
            50L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.lantern-bullseye",
            "Lantern, bullseye",
            1000L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.lantern-hooded",
            "Lantern, hooded",
            500L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.lock",
            "Lock",
            1000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.magnifying-glass",
            "Magnifying glass",
            10000L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.manacles",
            "Manacles",
            200L,
            6m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.mess-kit",
            "Mess kit",
            20L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.mirror-steel",
            "Mirror, steel",
            500L,
            0.5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.oil-flask",
            "Oil (flask)",
            10L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.paper-one-sheet",
            "Paper (one sheet)",
            20L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.parchment-one-sheet",
            "Parchment (one sheet)",
            10L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.perfume-vial",
            "Perfume (vial)",
            500L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.pick-miners",
            "Pick, miner's",
            200L,
            10m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.piton",
            "Piton",
            5L,
            0.25m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.poison-basic-vial",
            "Poison, basic (vial)",
            10000L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.pole-10-foot",
            "Pole (10-foot)",
            5L,
            7m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.pot-iron",
            "Pot, iron",
            200L,
            10m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.potion-of-healing",
            "Potion of healing",
            5000L,
            0.5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.pouch",
            "Pouch",
            50L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.quiver",
            "Quiver",
            100L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.ram-portable",
            "Ram, portable",
            400L,
            35m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.rations-1-day",
            "Rations (1 day)",
            50L,
            2m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.robes",
            "Robes",
            100L,
            4m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.rope-hempen-50-feet",
            "Rope, hempen (50 feet)",
            100L,
            10m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.rope-silk-50-feet",
            "Rope, silk (50 feet)",
            1000L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.sack",
            "Sack",
            1L,
            0.5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.scale-merchants",
            "Scale, merchant's",
            500L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.sealing-wax",
            "Sealing wax",
            50L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.shovel",
            "Shovel",
            200L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.signal-whistle",
            "Signal whistle",
            5L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.signet-ring",
            "Signet ring",
            500L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.soap",
            "Soap",
            2L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.spellbook",
            "Spellbook",
            5000L,
            3m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.spikes-iron-10",
            "Spikes, iron (10)",
            100L,
            5m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.spyglass",
            "Spyglass",
            100000L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.tent-two-person",
            "Tent, two-person",
            200L,
            20m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.tinderbox",
            "Tinderbox",
            50L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.torch",
            "Torch",
            1L,
            1m,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.vial",
            "Vial",
            100L,
            null,
            null),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.waterskin",
            "Waterskin",
            20L,
            5m,
            "full"),
        new ExpectedGearRow(
            "dnd5e2014.adventuring-gear.whetstone",
            "Whetstone",
            1L,
            1m,
            null)
    ];
}
