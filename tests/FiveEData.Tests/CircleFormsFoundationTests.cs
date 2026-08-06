using FiveEData.Rules.Classes.CircleForms;

namespace FiveEData.Tests;

public sealed class CircleFormsFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void ChallengeRatingGrant_RejectsOutOfRangeCharacterLevel(
        int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CircleFormsChallengeRatingGrant(characterLevel, 1.0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChallengeRatingGrant_RejectsNonPositiveMaxChallengeRating(
        double maxChallengeRating)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CircleFormsChallengeRatingGrant(2, maxChallengeRating));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsMaxChallengeRatingByLevel()
    {
        var grants = new List<CircleFormsChallengeRatingGrant>
        {
            new(2, 1.0)
        };

        var detail = new CircleFormsProgressionDetail(grants);

        grants.Clear();

        Assert.Single(detail.MaxChallengeRatingByLevel);
    }
}
