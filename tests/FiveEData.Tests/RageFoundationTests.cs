using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Tests;

public sealed class RageFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void DamageBonusGrant_RejectsOutOfRangeCharacterLevel(
        int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RageDamageBonusGrant(characterLevel, 2));
    }

    [Fact]
    public void DamageBonusGrant_RejectsNonPositiveBonus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RageDamageBonusGrant(1, 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void UseGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RageUseGrant(characterLevel, 2));
    }

    [Fact]
    public void UseGrant_RejectsNonPositiveUses()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RageUseGrant(1, 0));
    }

    [Fact]
    public void UseGrant_AllowsNullForUnlimitedUses()
    {
        var grant = new RageUseGrant(20, null);

        Assert.Null(grant.UsesPerLongRest);
    }

    [Fact]
    public void Detail_RejectsNonPositiveDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RageProgressionDetail(
                [new RageUseGrant(1, 2)],
                [new RageDamageBonusGrant(1, 2)],
                durationMinutes: 0,
                [new DamageTypeId("dnd5e2014.damage-type.bludgeoning")],
                requiresNotWearingHeavyArmor: true));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsCollections()
    {
        var uses = new List<RageUseGrant> { new(1, 2) };
        var damageBonuses = new List<RageDamageBonusGrant> { new(1, 2) };
        var resistedTypes = new List<DamageTypeId>
        {
            new("dnd5e2014.damage-type.bludgeoning")
        };

        var detail = new RageProgressionDetail(
            uses,
            damageBonuses,
            1,
            resistedTypes,
            true);

        uses.Clear();
        damageBonuses.Clear();
        resistedTypes.Clear();

        Assert.Single(detail.UsesByLevel);
        Assert.Single(detail.DamageBonusByLevel);
        Assert.Single(detail.ResistedDamageTypeIds);
    }
}
