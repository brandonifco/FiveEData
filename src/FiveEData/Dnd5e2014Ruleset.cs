using System.Threading;
using FiveEData.Rules.Catalog;

namespace FiveEData;

public sealed class Dnd5e2014Ruleset
{
    private static readonly Lazy<Dnd5e2014Ruleset> LazyInstance =
        new(
            Dnd5e2014RulesetLoader.Load,
            LazyThreadSafetyMode.ExecutionAndPublication);

    internal Dnd5e2014Ruleset(
        WeaponCatalog weapons,
        AmmunitionCatalog ammunition,
        SourceDocumentCatalog sources,
        RuleCatalog rules)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(rules);

        Weapons = weapons;
        Ammunition = ammunition;
        Sources = sources;
        Rules = rules;
    }

    public static Dnd5e2014Ruleset Instance => LazyInstance.Value;

    public WeaponCatalog Weapons { get; }
    public AmmunitionCatalog Ammunition { get; }
    public SourceDocumentCatalog Sources { get; }
    public RuleCatalog Rules { get; }
}
