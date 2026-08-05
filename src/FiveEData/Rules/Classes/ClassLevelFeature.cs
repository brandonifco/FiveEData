using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes;

public readonly record struct ClassLevelFeature
{
    public ClassLevelFeature(int level, RuleId featureRuleId)
    {
        if (level is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(level),
                level,
                "Class level must be between 1 and 20.");
        }

        if (string.IsNullOrWhiteSpace(featureRuleId.Value))
        {
            throw new ArgumentException(
                "Class level feature rule ID must not be empty.",
                nameof(featureRuleId));
        }

        Level = level;
        FeatureRuleId = featureRuleId;
    }

    public int Level { get; }

    public RuleId FeatureRuleId { get; }
}
