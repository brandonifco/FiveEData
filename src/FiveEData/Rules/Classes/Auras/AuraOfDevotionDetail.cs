namespace FiveEData.Rules.Classes.Auras;

public sealed record AuraOfDevotionDetail
{
    public AuraOfDevotionDetail(AuraRange range, bool requiresConsciousness)
    {
        Range = range;
        RequiresConsciousness = requiresConsciousness;
    }

    public AuraRange Range { get; }
    public bool RequiresConsciousness { get; }
}
