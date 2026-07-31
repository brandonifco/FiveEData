using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Weapons;

public sealed record WeaponRange
{
    public WeaponRange(
        Distance normal,
        Distance longRange)
    {
        if (normal.Feet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(normal),
                normal,
                "Normal weapon range must be greater than zero.");
        }

        if (longRange.Feet < normal.Feet)
        {
            throw new ArgumentOutOfRangeException(
                nameof(longRange),
                longRange,
                "Long weapon range cannot be less than normal range.");
        }

        Normal = normal;
        Long = longRange;
    }

    public Distance Normal { get; }

    public Distance Long { get; }
}
