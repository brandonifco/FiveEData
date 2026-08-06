using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.EldritchInvocations;

public sealed class EldritchInvocationDefinition
{
    internal EldritchInvocationDefinition(
        EldritchInvocationId id,
        string name,
        bool requiresEldritchBlastCantrip,
        int? requiredMinimumLevel,
        WarlockPactBoon? requiresPactBoon,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RequiresEldritchBlastCantrip = requiresEldritchBlastCantrip;
        RequiredMinimumLevel = requiredMinimumLevel;
        RequiresPactBoon = requiresPactBoon;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public EldritchInvocationId Id { get; }
    public string Name { get; }
    public bool RequiresEldritchBlastCantrip { get; }
    public int? RequiredMinimumLevel { get; }
    public WarlockPactBoon? RequiresPactBoon { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
