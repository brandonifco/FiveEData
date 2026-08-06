namespace FiveEData.Rules.Classes.Auras;

public sealed record AuraOfCourageDetail
{
    public AuraOfCourageDetail(AuraRange range, bool requiresConsciousness)
    {
        Range = range;
        RequiresConsciousness = requiresConsciousness;
    }

    public AuraRange Range { get; }
    public bool RequiresConsciousness { get; }
}
