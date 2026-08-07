using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Rules.Classes.DraconicResilience.Serialization;

internal static class DraconicResilienceDetailDataMapper
{
    public static DraconicResilienceDetail Map(
        DraconicResilienceDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var unarmoredArmorClass = new ArmorClassFormula(
            data.UnarmoredBaseArmorClass,
            data.UnarmoredIncludesDexterityModifier);

        return new DraconicResilienceDetail(
            data.HitPointBonusPerLevel,
            unarmoredArmorClass);
    }
}
