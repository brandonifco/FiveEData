namespace FiveEData.Rules.Classes.MartialArts;

public sealed record MartialArtsProgressionDetail
{
    public MartialArtsProgressionDetail(
        IEnumerable<MartialArtsDieGrant> dieByLevel,
        bool canUseDexterityForAttackAndDamage,
        bool grantsBonusActionUnarmedStrike,
        bool requiresNotWearingArmor,
        bool requiresNotWieldingShield)
    {
        ArgumentNullException.ThrowIfNull(dieByLevel);

        DieByLevel = Array.AsReadOnly(dieByLevel.ToArray());
        CanUseDexterityForAttackAndDamage = canUseDexterityForAttackAndDamage;
        GrantsBonusActionUnarmedStrike = grantsBonusActionUnarmedStrike;
        RequiresNotWearingArmor = requiresNotWearingArmor;
        RequiresNotWieldingShield = requiresNotWieldingShield;
    }

    public IReadOnlyList<MartialArtsDieGrant> DieByLevel { get; }

    public bool CanUseDexterityForAttackAndDamage { get; }

    public bool GrantsBonusActionUnarmedStrike { get; }

    public bool RequiresNotWearingArmor { get; }

    public bool RequiresNotWieldingShield { get; }
}
