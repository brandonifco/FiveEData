# FiveEData

A .NET 8 library that digitally recreates every pertinent rules element of the
**2014 D&D 5th Edition Player's Handbook** as strongly-typed C# catalogs,
loaded from embedded JSON. Scope is the 2014 PHB specifically — not the
Monster Manual, DMG, or later sourcebooks. Each catalog entry that represents
official book content carries a citation (source document, page, section)
back to that PHB printing.

A second, parallel purpose: a possible future game, built and compared
independently against the sibling **5eGoldBox** project's hand-authored,
execution-ready ruleset. The two projects are never wired together. This is
why the "Quantized mechanics" work below exists — a citation alone can't run
a game.

## Current state

**Nothing in progress, nothing queued. Ask the user for the next priority
before starting new work.**

Built and complete:

- Equipment (weapons, armor, shields, adventuring gear, tools, mounts,
  vehicles, trade goods)
- Expenses (lifestyles, food & drink, hospitality, mundane services)
- Creature vocabulary (abilities, skills, languages, sizes, conditions,
  damage types, senses, alignments)
- Races — all 9 PHB races, all 9 subraces
- Classes — all 12 PHB classes, all 40 subclasses
- Backgrounds — all 13 PHB backgrounds
- Quantized mechanics — the full second pass (see that section); every
  identified per-level numeric progression and choice-point catalog is
  converted or explicitly, verifiably declined

**"Complete" means citation-complete, not mechanically quantized.** Most
named features across Classes/Races/Backgrounds are still a `RuleId`
citation with no mechanical payload — the quantized pass covered leveled
numbers and choice-point options, not every feature. Check the inventory
under "Quantized mechanics" before assuming a feature exposes real numbers.

Gate as of the last merge: Debug+Release build 0 warnings, **1743 tests**.

**In progress: the Spells domain.** `Rules/Spells/` holds `MagicSchools`
(the 8 schools, a closed official set, cited to the p.203 sidebar) and
`SpellDefinition`. **All 27 cantrips are built; levels 1–9 are not.**

Per p.202 ("Casting a Spell"), a spell entry's header block is *name, level,
school, casting time, range, components, duration* — that's what
`SpellDefinition` stores. The effect prose, and the per-spell "At Higher
Levels" text, stay in the citation: heterogeneous across spells with no
shared shape, the same call already made for Metamagic's 8 effects.

Cantrips were chosen to prove the shape because they are a complete, closed
set that exercises every school and, as it turned out, six of the possible
V/S/M combinations — which is why components are three independent bools,
not an enum. Casting time is not uniform even here (Mending is 1 minute,
Shillelagh 1 bonus action), and duration needed three independent axes:
instantaneous, "up to", and concentration. **"Up to" is not a synonym for
concentration** — Prestidigitation is "Up to 1 hour" with no concentration,
while concentration is always an "up to" duration.

**Deliberately omitted until levels 1–9 land**, per "add a field only where
it actually varies": `IsRitual` (no cantrip is a ritual), material component
cost/consumed flags (no cantrip has a costed material), area-of-effect ranges
such as "Self (15-foot cone)", and the `Reaction`/`Hour` casting-time units.
Each is a purely additive change when the content that needs it arrives —
`Alarm` on p.211 is the first ritual.

Not started: magic items, and combat/adventuring rule prose beyond the
existing citation index. Still deferred and unmodeled: per-class
cantrips-known and spells-known tables (identified during the spell slot
pass as a separate authoring job; they need no spell domain to exist). Feats (and by extension Variant Human) are out
of scope — not part of the free 2014 SRD this project's provenance model is
built around.

Phases are tracked in commit history (`git log --oneline`), not a planning
doc. The full build-by-build reasoning trail for Races/Classes/Backgrounds
and the 24-item Quantized pass lived in this file through commit `e2bd672`
and is recoverable with `git show e2bd672:CLAUDE.md` — the durable rules
those builds established are all retained below.

**When CI fails on infrastructure, not code:** check
<https://www.githubstatus.com/> to confirm it's real rather than inferring it
from symptoms, then retry with `gh run rerun <run-id>` a couple of times —
during the 2026-08-06 Actions outage, runs sat queued for up to two hours
before eventually going green on their own. Only bypass the wait-for-green
default with the user's explicit go-ahead, **asked for fresh each time**; a
past bypass never carries forward. A failure that shows real `dotnet`
build/test output is a genuine regression, not infra — investigate, never
bypass.

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
`FiveEData.Data.dnd5e2014.<file>.json`) — **add new files to both the data
directory and the csproj.**

`Dnd5e2014RulesetLoader.Load()` reads every embedded resource, builds a
`RulesetDefinitionSet` (all raw definitions), runs
`CatalogIntegrityValidator.EnsureValid` against it, then constructs the
public `Dnd5e2014Ruleset` (all catalogs). `Dnd5e2014Ruleset.Instance` is a
lazy singleton over that pipeline.

Top-level domains are siblings: `Rules/Equipment`, `Rules/Expenses`,
`Rules/Creatures`, `Rules/Classes`, `Rules/Backgrounds`. Races sit under
`Rules/Creatures/Races/` (they consume creature vocabulary); Classes and
Backgrounds do not, since a class/background is a player-build concept in its
own right, not a vocabulary consumer.

**A parent/child pair is two sibling definitions, never nested.** The child
carries a back-reference ID to its parent and is validated and cataloged
independently — `Tool`/`ToolFamilyId` set the shape, `Race`/`Subrace` and
`Class`/`Subclass` follow it. Follow it again for any future parent/child
domain rather than nesting the child inside the parent's definition.

### `RuleDefinition` is split by domain, not one file

`RuleId` is referenced *across* every other domain (a weapon's special rule,
a racial trait, a class feature, ...), so unlike every other domain it never
had a natural single owner. It's split by `dnd5e2014.<prefix>-rule.*`
namespace into `Data/dnd5e2014/rules/<prefix>-rule.json` — one file per
prefix (`weapon-rule`, `armor-rule`, `adventuring-gear-rule`, `tool-rule`,
`mount-vehicle-rule`, `trade-good-rule`, `expense-rule`, `lifestyle-rule`,
`race-rule`, `class-rule`, `background-rule`), each embedded and loaded
independently, then merged by `RuleDefinitionLoader.LoadAndMergeFromJson`
(production, from already-read embedded-resource text) /
`LoadAndMergeFromFiles` (tests and tooling, given real file paths) into the
one `IReadOnlyList<RuleDefinition>` every catalog/validator still consumes as
a flat list. Each per-file `LoadFromJson` catches a duplicate ID *within* its
own file; the merge step adds the check that's new here, a duplicate ID
*across* files.

If a single prefix's file grows into its own monolith — `class-rule.json` is
the likely first candidate — split that one file further (e.g.
`rules/class-rule/<class-slug>.json`) and fold it into the same merge list;
the merge step doesn't care how many files it's fed.

### Provenance discipline

`CatalogIntegrityValidator` (and domain-specific integrity validators like
`CreatureVocabularyCatalogIntegrityValidator`, `RaceCatalogIntegrityValidator`,
`BackgroundCatalogIntegrityValidator`) check every definition's
`SourceReference.DocumentId` resolves to a loaded `SourceDocument`, and that
cross-domain ID references (a skill's associated ability, a race's resisted
damage types, a background's sustained lifestyle, a maneuver's saving-throw
ability) resolve too. Any new cross-domain reference field gets the same
check wired in.

**A new closed official domain does not need its own `Official*SemanticValidator`.**
That runtime-validator pattern exists only for the older creature-vocabulary
and expense domains; every catalog added since Fighting Style asserts its
exact closure in `<Domain>DataFileTests` instead. Follow the newer pattern.

`OfficialCreatureVocabularySemanticValidator` (and its siblings for expenses,
weapons, etc.) is a **closed-content guardrail**: for each domain considered
"official" (abilities, skills, languages, creature sizes, conditions, damage
types, senses, alignments), it hardcodes the exact expected set of IDs,
names, and one `(page, section)` citation per domain, and fails if the
canonical data file doesn't match exactly — including rejecting *extra*
entries. Non-canonical extension IDs (outside `dnd5e2014.*`) are exempt.

**Citation rules, learned the hard way:**

- Use the cleanly-scanned PHB PDF (`~/Downloads/Player's Handbook.pdf`,
  reliable per-page footers) — **not** the archive.org OCR export, whose
  page-footer digits are missing or corrupted. An early Bard pass built from
  the OCR export had to be re-verified and partly corrected.
- **Read values off the page images, never off `pdftotext`.** That PDF's
  own text layer is OCR and is badly noisy — "Id6" for 1d6, "leveI" for
  level, "I action" for 1 action, "calltrip" for cantrip. It is fine for
  *locating* a spell or heading (grep it to find the page, which is much
  cheaper than paging through images), but every value that lands in a data
  file must come from the rendered page. The cantrip pass used exactly this
  split, and the locating step still guessed two pages wrong (Produce Flame,
  Thaumaturgy) that the images corrected.
- Cite the page where a feature's own substantial body text **starts**, not
  where it ends and not wherever a stray heading or stat line surfaces first.
  The PHB's two-column layout regularly puts a proficiency block a page ahead
  of the feature it belongs to.
- **Verify every page number against the real text, every time.** Separate
  passes found genuine pre-existing off-by-one citation errors (four during
  the Paladin auras work, six during Battle Master maneuvers). Fix them where
  you find them, in the same commit.
- Errata and body prose beat printing artifacts. The Dwarf trait's printed
  "throwing hammer" is corrected to "light hammer" by official errata, and
  that's what's stored. The Warlock table omits a 19th-level Ability Score
  Improvement row that the feature's own text explicitly names — the prose
  wins, pinned by
  `PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission`.

## Content modeling rules

### What becomes structured data vs. a citation

`RuleDefinition` is `Id`/`Name`/`Sources` — nothing else. A feature's prose
never lives in this repo; only a citation index does.

- **A leveled numeric fact belongs in structured data.**
- **A compound formula, a DM-adjudicated range, or content this project
  doesn't model as its own domain belongs in the citation.** Examples held to
  this line: Sneak Attack's alternative-to-advantage trigger (compound combat
  state), Rage's Persistent Rage (changes end conditions, not a number), Pact
  of the Blade's dismissal condition, Preserve Life's `5 × cleric level`
  healing and Radiance of the Dawn's `2d10 + level` damage (linear-in-level
  formulas), By Popular Demand's modest-*or*-comfortable lodging (a range,
  not a grant).
- **Spells are not a modeled domain.** Domain Spells, Circle Spells, Oath
  Spells, every caster's spell list, and an Elemental Discipline's cast spell
  all stay inside the surrounding citation. There is no `SpellId` type.

### Unmodeled gaps (deliberate, not oversights)

Tool proficiencies and tool-proficiency *choices*, starting equipment, and
Druid's nonmetal-armor restriction have no fields. Every one was a considered
call: no field exists, and adding one would be generality ahead of a real
consumer. Revisit only when a downstream domain (multiclassing proficiency
stacking, a character builder, equipment validation) actually needs them.

Also out of scope: variant backgrounds (Spy, Gladiator, Guild Merchant,
Knight, Retainers, Pirate) and Variant Human — an optional reskin isn't a new
named entity, it's prose the citation already covers. Suggested
Characteristics tables and per-background flavor sub-tables are pure roleplay
with no mechanical weight.

### `RuleId` sharing — the single most important discipline here

**Default to a shared, unprefixed `RuleId` for a mechanic bearing a generic
name; prefix and split only when a specific class's actual PHB text
diverges. Verify by reading the real text side by side, every time, against
*every* previously-built class — not just the most recent one.**

Supporting rules, each earned from a real case:

- **A recurring template name is not evidence either way.** A template with a
  substituted word stays split, because the substitution *is* the mechanic
  (Wizard's 8 school `Savant` features, Cleric's 5-way `Divine Strike`,
  Warlock's 3 `Expanded Spell List`). A template with zero difference shares
  (`Potent Spellcasting` across Knowledge/Light, Tempest/War's
  `martial-and-heavy-armor-proficiency`).
- **Uncertainty resolves toward keeping separate.** The failure directions
  aren't symmetric: wrongly merging two possibly-distinct texts is a
  correctness bug; keeping one redundant citation is not. A single dropped
  word ("a *particular* style of fighting") kept `paladin-fighting-style`
  split from the shared entry even though the word carries no mechanical
  weight and might have been scan noise.
- **Trigger level alone never blocks sharing** — the level lives in
  `LevelFeatures`, not the citation. But a difference in the cited sentence
  does (`college-of-valor-extra-attack`'s "Starting at 6th" vs. the shared
  entry's "Beginning at 5th").
- **A shared choice-point `RuleId` doesn't promise every class offers the
  same options.** Fighter and Ranger share `fighting-style` while offering 6
  and 4 options respectively; the citation never enumerated them.
- Collisions get caught late as well as early — `Timeless Body` (Druid vs.
  Monk) and `Land's Stride` (Ranger vs. a Druid subclass) were both found
  against classes several merged PRs back. When a collision surfaces,
  retroactively rename the incumbent too rather than leaving one mechanic
  squatting on the generic ID (`expertise` → `rogue-expertise` when Bard's
  arrived).

**Current shared, unprefixed entries** (add a `SourceReference`, don't mint a
new ID): `ability-score-improvement` (10 — every class but the two prefixed
below), `extra-attack` (Barbarian/Monk/Ranger/Paladin), `fighting-style`
(Fighter/Ranger), `evasion` (Monk/Rogue), `lands-stride` (Ranger/Circle of
the Land),
`potent-spellcasting` (Knowledge/Light), `martial-and-heavy-armor-proficiency`
(Tempest/War).

**Deliberately prefixed splits** (don't "helpfully" consolidate):
`fighter-`/`rogue-ability-score-improvement`, `fighter-extra-attack`,
`college-of-valor-extra-attack`, `paladin-fighting-style`,
`barbarian-`/`monk-unarmored-defense`, `rogue-`/`bard-expertise`,
`monk-`/`druid-timeless-body`, every school's `Savant`, every domain's
`Divine Strike`, every patron's `Expanded Spell List`, every subclass's
`Bonus Proficiencies`, and every class's own core spellcasting.

### Class/subclass structure rules

- **Core spellcasting is always one citation, always prefixed per class,
  never shared** — even between two full casters. It isn't always table-named
  "Spellcasting" (Warlock's is "Pact Magic"). Check for a Ritual Casting
  subsection each time rather than assuming: Warlock and Sorcerer genuinely
  have none.
- **A choice point folds into one citation**, independent of option count (6
  to 32) or per-option length (one-liners to ~200-word mechanics like Pact of
  the Blade). The deciding factor is the shape — a single named gateway
  offering sub-choices — never size.
- **A framework heading earns its own `RuleId` only if it has its own table
  row.** Cleric's `Channel Divinity` is a 2nd-level row and kept a citation;
  Paladin's same-named framework text isn't, and folded into `sacred-oath`.
  The table likewise decides whether a recurring mechanic gets repeated
  `LevelFeatures` entries (Wild Shape, Circle Forms, the core Paladin auras'
  explicit "18th: Aura improvements" row) or stays one citation with the
  scaling left in prose (the oath-specific auras' own 18th-level increase).
- **Recurring features reuse the same `RuleId` at each level** rather than
  minting one per occurrence.
- `PrimaryAbilityIds`/`RequiresAllPrimaryAbilities` come from the **Chapter 6
  multiclassing prerequisite table (p.163)**, not the class's own Quick Build
  text — those read similarly and mean different things.
- Subclass gateway levels vary: Cleric 1st, Wizard 2nd, everyone else 3rd.
  Driven by `ExpectedChosenAtLevelByClassId` in `SubclassDataFileTests`.
- Weapon proficiency is category + named exceptions in any combination —
  `[Simple]` alone (Fighter/Barbarian), `[Simple]` plus named IDs
  (Monk/Rogue), or named IDs with **no** category at all (Wizard/Sorcerer,
  who also have no armor proficiency).

### Backgrounds

Structurally the simplest domain: a flat, one-shot grant with no ability
scores, no size/speed, no level-gated features. **Every one of the 13 grants
exactly two fixed skill proficiencies, never a choice** — verified across all
13, which is why `SkillProficiencyIds` is validated as exactly-two rather
than a choice-count/option-list pair like class skill choices. Language
grants are always "N of your choice" (0, 1, or 2), never named languages.

## Quantized mechanics

A completed second pass over already-built content, extracting the actual
numbers that a running game needs from features previously stored as
citations only. Every citation it touched was left in place — this is
additive.

**Inventory — what actually carries numbers.** Anything not listed here is
citation-only. Verified against the definition types, not this list; re-check
rather than trusting it.

- **Catalogs referenced by ID:** `SpellSlotProgressionId` (+
  `SpellcastingAbilityId`) and `ExtraAttackProgressionId`, on both
  `ClassDefinition` and `SubclassDefinition`.
- **`SpellDefinition`** carries its own header block as structured data
  (level, school, casting time, range, components, duration) plus
  `AvailableToClassIds` — cantrips only so far; see "Current state".
- **Choice-point catalogs** (standalone, not referenced from a definition):
  Fighting Style, Metamagic, Battle Master maneuvers, Eldritch Invocations,
  Elemental Disciplines, Channel Divinity options.
- **Embedded on `ClassDefinition`:** Rage, Sneak Attack, Ki, Sorcery Points,
  Wild Shape, Bardic Inspiration, Song of Rest, Channel Divinity uses,
  Mystic Arcanum, Font of Magic conversion, Aura of Protection, Aura of
  Courage, Eldritch Invocations known.
- **Embedded on `SubclassDefinition`:** Divine Strike, Circle Forms, Combat
  Superiority, Disciple of the Elements, Aura of Devotion, Aura of Warding.
- **On `RaceDefinition`/`SubraceDefinition`:** `DarkvisionRangeFeet`,
  `ResistedDamageTypeIds`, `TranceDurationHours`, `HitPointBonusPerLevel`,
  the subrace `Speed` override, and the embedded Breath Weapon progression.
- **On `BackgroundDefinition`:** `SustainedLifestyleId` (a cross-domain
  reference into the Lifestyles catalog), `AdditionalPeopleFedPerDay`,
  `GuildDuesGoldPerMonth`, `FastTravelSpeedMultiplier`.

**Shape selection:**

- **A fact shared across multiple classes → a catalog referenced by ID.**
  `SpellSlotProgressionId` (4 rows: full/half/third caster + Pact Magic),
  `ExtraAttackProgressionId` (2 rows: `standard`, `fighter`), and the
  choice-point catalogs below.
- **A single-class/single-race/single-background fact → a bare field or a
  small embedded value object**, mapped inline by a `*DataMapper`, no
  top-level catalog. Rage, Sneak Attack, Ki, Sorcery Points, Wild Shape,
  Breath Weapon, and the four background scalars are all this.
- Never build the general shape by default — build only what the real content
  needs.

**Mechanism representation — no generic effect DSL.** 5e's mechanics are too
heterogeneous for one flat `Effect` type to cover honestly. Instead a
definition carries **several typed, nullable mechanism fields side by side,
with the validator enforcing exactly one populated** — the shape
`WeaponDefinition` already used (`Damage`/`Range`/`VersatileDamage`/
`AmmunitionTypeId`) and `FightingStyleDefinition` generalized. It has since
held for a cost formula (Metamagic's fixed-vs-spell-level cost), a damage
payload (Divine Strike's fixed/choosable/matches-weapon), and non-exclusive
prerequisite sets (Eldritch Invocations stacks level + pact boon). This is a
deliberate rejection of a discriminated-union/DSL pattern the codebase
doesn't otherwise use.

**Supporting rules:**

- **Add a field only where it actually varies.** `RequiresConsciousness`
  exists on the auras because Aura of Warding genuinely lacks the clause;
  Metamagic has no `AvailableToClassIds` because it's Sorcerer-only and the
  list would always hold one entry.
- **Capture the full mechanical fact set, not just the leveled number** —
  Rage's `DurationMinutes`/`RequiresNotWearingHeavyArmor`, Sneak Attack's
  `OncePerTurn`/`RequiresFinesseOrRangedWeapon`, Bardic Inspiration's
  `RangeFeet`/`DurationMinutes`. But verify the set rather than assuming:
  Song of Rest genuinely has only a die, and Wild Shape's use count is a flat
  constant even though it sits under a leveled table.
- **Reuse existing types over new number spaces.** `DiceExpression` (already
  used for hit dice and weapon damage) carries Sneak Attack's dice; damage
  types are catalogued `DamageTypeId`s. Conversely, don't invent a type for
  two call sites — `MaxChallengeRating` stayed a plain `double`, and
  `RecoversOnShortRest` stayed a plain bool rather than relocating the
  spell-slot-specific `SpellSlotRecoveryRest` enum and breaking a shipped
  public type.
- **`int?` over sentinels** — Rage's 20th-level "Unlimited" uses `null`, not
  `int.MaxValue`. Ascending-sequence validation treats a transition into
  unlimited as valid.
- **Dense vs. sparse grant lists follow the source table.** Ki/Sorcery Points
  change at every level → one entry per level. Rage/Extra Attack change at
  milestones → breakpoints only. Spell slot progressions carry all 20 levels
  even where empty, so every progression has the same shape.
- **Expand formulas into tables.** Circle Forms' "druid level divided by 3,
  rounded down" was hand-computed and stored as the levels where the result
  changes — store the resulting table, not the formula.
- **Don't reuse a shared validator helper whose error vocabulary doesn't
  match.** `ValidatePointsProgression` hardcodes "character level" into its
  messages, so Font of Magic — whose axis is *spell slot* level — got a
  bespoke method instead. Check what a helper's messages actually say, not
  just that the shape matches.
- **`RecoversOnShortRest` is read per feature, never inferred from the class
  family.** Ki `true`, Sorcery Points `false`, Channel Divinity `true`,
  Mystic Arcanum `false` — despite Warlock's own Pact Magic recovering on a
  short rest.

**The six choice-point catalogs**, all resolved — no two share a schema:

| Catalog | Shape captured |
| --- | --- |
| Fighting Style (6) | typed mechanism fields + `AvailableToClassIds` |
| Metamagic (8) | sorcery point cost only (fixed *or* spell-level-derived) |
| Pact Boon (3) | **declined** — no option carries a quantizable fact |
| Battle Master maneuvers (16) | effect target enum + saving-throw ability |
| Eldritch Invocations (32) | 3 independent, stackable prerequisite facts |
| Elemental Disciplines (17) | ki cost + minimum level |
| Channel Divinity options (10) | 4 independent nullable scalars |

A choice point earns a catalog **only if at least one option carries a real
quantizable fact** — Pact Boon proved the pattern isn't automatic. A "choice
point" is a category of feature, not a template: read the actual page every
time to find out which facts are real. Options with nothing to quantify still
get catalog entries (Id/Name/Sources), since the catalog's job is completing
the option list.

**Per-option effect prose stays unquantized** — Metamagic's 8 spell effects,
each maneuver's secondary effect (disarm, frighten, push, ...), each
discipline's cast spell. They're individually heterogeneous with no shared
shape, so modeling them means either a bespoke type per option or the DSL
this project rejects. Shared formulas (Battle Master's save DC, every
resource's save DC) stay unquantized too — capture only what varies per
option.

**Verify every remembered count, page number, and existing field's
completeness against the actual text, repeatedly — including numbers written
in this file.** This is not a hypothetical risk. Eldritch Invocations turned
out to be 32, not the ~20 this document claimed. Elemental Disciplines are
17, not the 18 it claimed. Two passes found real pre-existing citation page
errors. The race pass found `CanonicalFile_OnlyWoodElfOverridesSpeed` had
been **silently failing since the original Races commit** — Wood Elf's 35-foot
speed override was never populated even though the field and the test both
existed. "The test exists" and "the test passes" are different claims.

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

Equipment/expense domains have a heavier version plus separate
`*ImmutabilityTests`, `*CatalogIntegrityTests`, and
`Official*SemanticIntegrityTests` — follow whichever sibling domain is
closest in shape. A domain with no catalog of its own (an embedded value
object) needs no new test files; fold its assertions into the owning domain's
existing three.

**Prefer data-driven tests over one assertion per class.** `ChosenAtLevel`
and the shared-Ability-Score-Improvement list were each generalized into a
single test keyed by class ID after accreting near-duplicates; both now
absorb a new class as a one-line change.

**Add every new loader and validator to `PublicApiBoundaryTests`.** That
convention silently lapsed for five catalogs (Metamagic through Channel
Divinity options), leaving internal types unguarded; the gap was closed when
Magic Schools was added. A new domain's `*DataFileTests` should also assert
that `Dnd5e2014Ruleset.Instance` exposes the same closure as the on-disk
file — data-file tests read from disk, so nothing else catches a data file
that was added without its `<EmbeddedResource>` csproj entry.

**Some tests exist specifically to block a plausible-looking
consolidation** — don't delete them as redundant:
`EverySchoolSavantFeatureIsItsOwnDistinctRuleId`,
`PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission`,
`DoesNotDuplicateWizardsWeaponProficiencyIdList`.

## Build

```bash
dotnet build
dotnet test
```

`global.json` pins SDK `8.0.129` with `"rollForward": "latestMajor"` so the
build still works where only newer major SDKs (9.x, 10.x) are installed —
both target frameworks stay `net8.0`.

CI (`.github/workflows/dotnet.yml`) runs the same Debug+Release matrix on
every push to `main` and every PR against it. No vulnerability scan or
separate lint gate yet; add one if it becomes a real need, matching however
the sibling 5eGoldBox project's CI evolved rather than guessing ahead of it.

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
5. Self-review the diff; report gaps honestly rather than glossing over them.
6. **If the change is worth recording — a new domain, a status change, a
   resolved architectural note, a corrected citation — update CLAUDE.md in
   the same commit, not a separate follow-up.** Record the *durable rule* a
   decision established (a new cross-domain reference shape, a new `RuleId`
   sharing precedent, a new mechanism shape), not a narrative of the build —
   this file is loaded into context every session, so it pays for brevity.
7. `git add` specific paths — never `-A` or `.`.
8. Commit — one commit, message explains what and why.
9. Push, open a PR (`gh pr create`), wait for CI (`gh pr checks --watch`),
   and merge (`gh pr merge --merge --delete-branch`) once green.
10. `git fetch --prune`, confirm `main` synced, move to the next branch.

Still pause and flag rather than pushing through: gate failures, merge
conflicts, anything destructive/irreversible outside the normal
branch→PR→merge flow, or content-authoring work where the source citations
can't be verified — that's a real blocker, not a step to skip. Still stop and
ask first for anything that isn't narrowly-scoped and already-decided —
genuine design/product decisions, force-pushes, history rewrites, or
deleting/closing things outside the normal merge flow.
