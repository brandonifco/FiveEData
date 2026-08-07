using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Rules.Classes.DraconicResilience;

public sealed record DraconicResilienceDetail
{
    public DraconicResilienceDetail(
        int hitPointBonusPerLevel,
        ArmorClassFormula unarmoredArmorClass)
    {
        if (hitPointBonusPerLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hitPointBonusPerLevel),
                hitPointBonusPerLevel,
                "Draconic Resilience hit point bonus per level must be " +
                "greater than zero.");
        }

        HitPointBonusPerLevel = hitPointBonusPerLevel;
        UnarmoredArmorClass = unarmoredArmorClass;
    }

    public int HitPointBonusPerLevel { get; }

    public ArmorClassFormula UnarmoredArmorClass { get; }
}
