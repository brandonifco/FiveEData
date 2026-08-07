using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.BendLuck;

public sealed record BendLuckDetail
{
    public BendLuckDetail(int sorceryPointCost, DiceExpression die)
    {
        if (sorceryPointCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sorceryPointCost),
                sorceryPointCost,
                "Bend Luck sorcery point cost must be greater than zero.");
        }

        SorceryPointCost = sorceryPointCost;
        Die = die;
    }

    public int SorceryPointCost { get; }

    public DiceExpression Die { get; }
}
