# FiveEData

A .NET 8 library that digitally recreates every pertinent rules element of the
**2014 D&D 5th Edition Player's Handbook** as strongly-typed C# catalogs,
loaded from embedded JSON. Scope is the 2014 PHB specifically — not the
Monster Manual, DMG, or later sourcebooks. Each catalog entry that represents
official book content carries a citation (source document, page, section)
back to that PHB printing.

## Architecture

Every rules domain (weapons, armor, languages, conditions, ...) follows the
same five-piece shape:

- `<Domain>Id` — a `readonly record struct` wrapping a non-empty string ID
  (e.g. `dnd5e2014.condition.blinded`). String IDs, not enums, so consumers
  can add homebrew/extension content outside the `dnd5e2014.*` namespace
  without touching this library.
- `<Domain>Definition` — public, immutable (internal constructor, defensive
  array copies on collection properties, no public setters).
- `<Domain>DefinitionValidator` — internal static `Validate`/`EnsureValid`;
  structural checks only (non-empty ID/name, at least one source reference,
  defined enum values).
- `Serialization/<Domain>DefinitionData` + `Serialization/<Domain>DefinitionLoader`
  — internal. `*Data` is the strict JSON DTO (`[JsonRequired]` on every
  member); `*Loader` deserializes via `StrictJson.DeserializeArray` (rejects
  unknown/duplicate JSON properties), maps to the real definition type,
  validates, and rejects duplicate IDs. Loaders are never public — see
  `PublicApiBoundaryTests`.
- `Rules/Catalog/<Domain>Catalog` — public, internal constructor. Orders
  definitions by ID, rejects duplicates, wraps in a `FrozenDictionary`,
  exposes `All`, `Count`, `Get`, `TryGet`.

Data files live in `Data/dnd5e2014/*.json` and are embedded via explicit
`<EmbeddedResource>` entries in `FiveEData.csproj` (logical name
`FiveEData.Data.dnd5e2014.<file>.json`) — add new files to both the data
directory and the csproj.

`Dnd5e2014RulesetLoader.Load()` reads every embedded resource, builds a
`RulesetDefinitionSet` (all raw definitions), runs
`CatalogIntegrityValidator.EnsureValid` against it, then constructs the
public `Dnd5e2014Ruleset` (all catalogs). `Dnd5e2014Ruleset.Instance` is a
lazy singleton over that pipeline.

### Provenance discipline

`CatalogIntegrityValidator` (and domain-specific integrity validators like
`CreatureVocabularyCatalogIntegrityValidator`) check every definition's
`SourceReference.DocumentId` resolves to a loaded `SourceDocument`, and that
cross-domain ID references (e.g. a skill's associated ability) resolve too.

Separately, `OfficialCreatureVocabularySemanticValidator` (and its siblings
for expenses, weapons, etc.) is a **closed-content guardrail**: for each
domain considered "official" (currently: abilities, skills, languages,
creature sizes, conditions, damage types, senses, alignments), it hardcodes
the exact expected set of IDs, names, and one `(page, section)` citation per
domain, and fails if the canonical data file doesn't match exactly —
including rejecting *extra* entries. This is what keeps the canonical
`Data/dnd5e2014/*.json` files honest against the real book. Non-canonical
extension IDs (outside `dnd5e2014.*`) are exempt and covered by
`NoncanonicalVocabulary_RemainsExtensionFriendly`-style tests.

Before adding a new "official" domain, verify page/section citations against
a real table of contents or errata document — don't rely on memory. Getting
this wrong is exactly what these guardrail tests exist to catch, but it's
cheaper to get it right the first time.

## Races

`Rules/Creatures/Races/` holds the first non-vocabulary domain that
cross-references creature vocabulary rather than being part of it — a race
references `AbilityId`/`CreatureSizeId`/`LanguageId` from
`Rules/Creatures/*`, so it sits as its own top-level `RaceDefinitionSet` on
`RulesetDefinitionSet` (alongside `Equipment`/`Expenses`/
`CreatureVocabulary`), not folded into the vocabulary set itself.

`RaceDefinition` and `SubraceDefinition` are siblings, not nested — a
subrace carries a `RaceId` back-reference and is validated/cataloged
independently, the same shape `Tool`/`ToolFamilyId` already established.
Only what's cleanly reducible to typed fields lives on the definitions
themselves (size, speed, ability score increases as `AbilityId` + bonus
pairs, known languages, an `AdditionalLanguageChoiceCount` /
`ChoosableAbilityScoreIncreaseCount` for "N more of your choice" mechanics
introduced here for the first time). Named racial traits with real
narrative substance (Darkvision, Fey Ancestry, Draconic Ancestry, ...) are
`RuleId` references into the shared `rules.json` catalog
(`dnd5e2014.race-rule.*`), mirroring the existing `SpecialRuleIds` pattern
— consistent with this project's standing discipline of never storing
rules prose, only a citation index. Where the mechanic and the trait name
are both identical across races (Darkvision is worded identically in all
six races that have it), one `RuleId` is shared with multiple
`SourceReference` entries, the same sharing precedent
`food-drink-lodging-included-in-lifestyle` already set; where the name
differs even if the mechanic doesn't (Dwarven Resilience vs. Stout
Resilience are mechanically identical poison resistance but different
proper-noun trait names), they're kept as separate `RuleId`s.

Citations were verified against the user-supplied PHB PDF directly (Table
of Contents page numbers, cross-checked against the official WotC PHB
errata for three of them — Dwarven Combat Training p.20, Drow Magic p.24,
Infernal Legacy p.43, all landing exactly where the ToC predicted) rather
than from memory. One real errata catch: the printed Dwarf trait text says
"throwing hammer," corrected by the official errata to "light hammer" —
the corrected wording is what's stored. Variant Human is out of scope, the
same way Feats are — its "bonus feat" trait can't be represented without
Feats existing.

## Test conventions

Per vocabulary domain (see `tests/FiveEData.Tests/Condition*Tests.cs` for the
current template):

- `<Domain>FoundationTests` — ID validation, definition immutability,
  validator rejections, catalog CRUD/ordering/duplicate-rejection/trust
  boundary.
- `<Domain>DefinitionLoaderTests` — strict JSON loading: valid case, null
  root/element, unknown property, duplicate JSON property, null/missing
  required members, duplicate IDs.
- `<Domain>DataFileTests` — loads the real `Data/dnd5e2014/*.json` file and
  asserts it's the exact expected closure (count + IDs) and that every entry
  matches expected name/citation.

Equipment/expense domains have a heavier version of this same pattern plus
separate `*ImmutabilityTests`, `*CatalogIntegrityTests`, and
`Official*SemanticIntegrityTests` files — follow whichever sibling domain is
closest in shape when adding a new one.

## Known architectural note

Resolved: `Rules/Common/DamageType.cs` (a legacy bare `enum`, no citations,
predating the vocabulary-catalog pattern) has been deleted.
`WeaponDamage.DamageTypeId` now references `Rules/Creatures/DamageTypes`
(the Phase 11 citation-backed catalog) directly, the same cross-domain
`<Domain>Id`-reference shape `AmmunitionTypeId`/`RuleId` already use on
`WeaponDefinition` — `CatalogIntegrityValidator` checks it resolves against
the loaded `DamageTypeCatalog` the same way. `Data/dnd5e2014/weapons.json`'s
`damage.type` field changed from a bare enum name (`"Bludgeoning"`) to the
full dotted ID (`"dnd5e2014.damage-type.bludgeoning"`) to match.

## Status

Phases are tracked only in commit history (`git log --oneline`), not in a
separate planning doc. As of Phase 12: equipment (weapons, armor, shields,
adventuring gear, tools, mounts, vehicles, trade goods), expenses
(lifestyles, food & drink, hospitality, mundane services), creature
vocabulary (abilities, skills, languages, sizes, conditions, damage types,
senses, alignments), and races (all 9 PHB races plus all 9 subraces — see
"Races" above) are complete. Not yet started: classes, backgrounds, spells,
magic items, and combat/adventuring rule prose beyond the existing
`rules.json` citation index. Feats (and, by extension, Variant Human) are
out of scope — they aren't part of the free 2014 SRD this project's
provenance model is built around.

## Build

```bash
dotnet build
dotnet test
```

`global.json` pins SDK `8.0.129` with `"rollForward": "latestMajor"` so the
build still works on environments that only have newer major SDKs (9.x,
10.x) installed — both target frameworks stay `net8.0`.
