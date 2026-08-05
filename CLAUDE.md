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
directory and the csproj. `RuleDefinition` content is the one exception to
"one file per domain": see below.

`Dnd5e2014RulesetLoader.Load()` reads every embedded resource, builds a
`RulesetDefinitionSet` (all raw definitions), runs
`CatalogIntegrityValidator.EnsureValid` against it, then constructs the
public `Dnd5e2014Ruleset` (all catalogs). `Dnd5e2014Ruleset.Instance` is a
lazy singleton over that pipeline.

### `RuleDefinition` is split by domain, not one file

`RuleId` is referenced *across* every other domain (a weapon's special
rule, a racial trait, a class feature, ...), so unlike every other domain
it never had a natural single owner — and by the time Races and Classes
had each added their own citation-index entries, the single
`Data/dnd5e2014/rules.json` had grown to 150 entries / 1700+ lines with no
internal grouping (entries land wherever the PR that added them happened
to append them). It's now split by `dnd5e2014.<prefix>-rule.*` namespace
into `Data/dnd5e2014/rules/<prefix>-rule.json` — one file per prefix
(`weapon-rule`, `armor-rule`, `adventuring-gear-rule`, `tool-rule`,
`mount-vehicle-rule`, `trade-good-rule`, `expense-rule`, `lifestyle-rule`,
`race-rule`, `class-rule`), each embedded and loaded independently, then
merged by `RuleDefinitionLoader.LoadAndMergeFromJson` (production, from
already-read embedded-resource text) / `LoadAndMergeFromFiles` (tests and
tooling, given real file paths) into the one `IReadOnlyList<RuleDefinition>`
every catalog/validator still consumes as a flat list — nothing downstream
of the merge changed. Each per-file `RuleDefinitionLoader.LoadFromJson`
call still catches a duplicate ID *within* its own file the way every
domain loader already does; the merge step adds the one check that's new
here, a duplicate ID *across* files, since two different prefixed files
could otherwise both claim the same ID undetected.

If a single prefix's file grows large enough to become its own monolith —
`class-rule.json` is the likely first candidate, since Fighter alone (1 of
12 classes) already contributed 23 of its 23 entries — split that one file
further (e.g. `rules/class-rule/<class-slug>.json`) and fold it into the
same merge list; the merge step doesn't care how many files it's fed.

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
`RuleId` references into the shared rules catalog — `race-rule.json`
specifically, since the split described above — (`dnd5e2014.race-rule.*`),
mirroring the existing `SpecialRuleIds` pattern
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

## Classes

`Rules/Classes/` is a new top-level domain (sibling to `Rules/Equipment`,
`Rules/Expenses`, `Rules/Creatures`), not folded under `Rules/Creatures/`
like Races — a class isn't a creature-vocabulary consumer in the same way,
it's a player-build concept in its own right, so it gets its own namespace.

`ClassDefinition`/`SubclassDefinition` are siblings with a `ClassId`
back-reference, the same shape Race/Subrace and Tool/ToolFamily already
established. What's structured vs. what's a `RuleId` citation follows the
same line Races drew: hit die (`DiceExpression`, reusing the existing
weapon-damage value type — a hit die is always "1 die of this size"),
primary/saving-throw abilities, armor/weapon proficiency *categories*
(reusing the existing `ArmorCategory`/`WeaponProficiencyCategory` enums
from the Equipment domain rather than inventing new ones), a skill-choice
count/option-list (the same "N of your choice" shape Half-Elf/Human
introduced), and a flat `LevelFeatures: (Level, RuleId)` list are all
structured. Everything else — what a feature actually *does*, including
genuinely complex mechanics like Battle Master's maneuvers-and-superiority-
dice or Eldritch Knight's full spell-slot progression table — is a single
named `RuleId` citation with no sub-structure, exactly like Draconic
Ancestry's damage table wasn't modeled as data. This was a deliberate,
verified scope call, not a gap: Eldritch Knight's entire spellcasting
section (cantrips, spell slots per level, spells known) was read in full
from the PHB specifically to confirm it reduces cleanly to one citation
the same way everything else does — it does, and building a spell-slot
table structure was not needed to represent it faithfully.

A within-class choice point (Fighting Style's 6 named options, a Battle
Master maneuver list) is *not* modeled as its own structured
choose-N-of-M shape — it's left inside the single feature `RuleId` that
names the choice (`Fighting Style`, `Combat Superiority`), matching the
same restraint already applied to Draconic Ancestry's own sub-table.
Recurring milestone features (Ability Score Improvement at multiple
levels, Extra Attack scaling at 5th/11th/20th) reuse the *same* `RuleId`
at each level in `LevelFeatures` rather than minting a new one per
occurrence, since it's the same named feature recurring, not a new one.

**Cross-class `RuleId` sharing is deliberately not designed yet.** Races
could compare all 9 races' text side by side before deciding what to
share (`Darkvision`) vs. keep separate (`Dwarven Resilience` vs. `Stout
Resilience`). With only Fighter built so far there's nothing to compare
against, so every Fighter/subclass `RuleId` in this pass is Fighter-
specific (`fighter-ability-score-improvement`, `fighter-extra-attack`)
where the name is generic enough to plausibly collide with a future
class's own differently-worded version of the same-named feature, and
left bare (`fighting-style`, `indomitable`) where the name is already
distinctive. Revisit the sharing question — the same way it was resolved
for Races — once a second class's full text is in hand to compare against.

Fighter was chosen as the template class specifically because it has no
exceptions to model: full armor/weapon proficiency by category (unlike
Druid's nonmetal-only restriction), and all three of its PHB subclasses
(Champion, Battle Master, Eldritch Knight) were built in this same pass —
deliberately not just the simplest one — to prove the shape holds across
the real complexity range a class can contain, not just the easy case.

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
separate planning doc. As of Phase 13: equipment (weapons, armor, shields,
adventuring gear, tools, mounts, vehicles, trade goods), expenses
(lifestyles, food & drink, hospitality, mundane services), creature
vocabulary (abilities, skills, languages, sizes, conditions, damage types,
senses, alignments), and races (all 9 PHB races plus all 9 subraces — see
"Races" above) are complete. Classes is started but far from complete:
only Fighter (all 3 subclasses — Champion, Battle Master, Eldritch Knight)
is built, as the validated template for the domain shape — see "Classes"
above. The other 11 PHB classes (Barbarian, Bard, Cleric, Druid, Monk,
Paladin, Ranger, Rogue, Sorcerer, Warlock, Wizard) are not yet built; when
picking one up, re-derive its own RuleId cross-class-sharing decisions
against Fighter's rather than assuming Fighter's slugs are final. Not yet
started: backgrounds, spells, magic items, and combat/adventuring rule
prose beyond the existing rules citation index (`Data/dnd5e2014/rules/`,
split per-domain — see "Architecture" above). Feats (and, by extension,
Variant Human) are out of scope — they aren't part of the free 2014 SRD
this project's provenance model is built around.

## Build

```bash
dotnet build
dotnet test
```

`global.json` pins SDK `8.0.129` with `"rollForward": "latestMajor"` so the
build still works on environments that only have newer major SDKs (9.x,
10.x) installed — both target frameworks stay `net8.0`.

CI (`.github/workflows/dotnet.yml`) runs the same Debug+Release matrix —
`dotnet build`/`dotnet test` per configuration — on every push to `main`
and every pull request against it. No vulnerability scan or separate lint
gate yet; add one if that becomes a real need, matching however the
sibling 5eGoldBox project's CI evolved rather than guessing ahead of it.

## Workflow authorization

**Standing authorization.** For a narrowly-scoped, already-decided piece of
work in this repo — a new domain following the established five-piece
pattern, a bug fix, a reconciliation of known debt — proceed autonomously
through the full loop without stopping to ask permission at each step,
including the push/PR/merge:

1. Confirm `git status` clean and `main` in sync with `origin/main`.
2. One narrowly-scoped branch per concern.
3. Implement.
4. Gate: `dotnet build` (Debug) → `dotnet build -c Release` → `dotnet test`
   (all three 0 warnings / 0 failures) → `git diff --check`.
5. Self-review the diff; report gaps honestly rather than glossing over
   them.
6. **If the change is worth recording — a new domain, a status change, a
   resolved architectural note, a corrected citation — update CLAUDE.md in
   the same commit, not a separate follow-up.** This includes the
   "Status" section's phase list and, where relevant, a short addition
   under "Architecture" or "Known architectural note" recording a real
   design decision (a new cross-domain reference shape, a new sharing
   precedent for `RuleId`s, a new "choice count" mechanic) so the next
   session doesn't have to re-derive it from the diff.
7. `git add` specific paths — never `-A` or `.`.
8. Commit — one commit, message explains what and why.
9. Push, open a PR (`gh pr create`), wait for CI (`gh pr checks --watch`),
   and merge (`gh pr merge --merge --delete-branch`) once green.
10. `git fetch --prune`, confirm `main` synced, move to the next branch.

Still pause and flag rather than pushing through: gate failures, merge
conflicts, anything that looks destructive/irreversible outside the normal
branch→PR→merge flow, or content-authoring work where the source citations
can't be verified (see "Provenance discipline" above) — that's a real
blocker, not a step to skip. Still stop and ask first for anything that
isn't a narrowly-scoped, already-decided task — genuine design/product
decisions, force-pushes, history rewrites, or deleting/closing things
outside the normal merge flow.
