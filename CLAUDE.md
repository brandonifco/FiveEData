# FiveEData

A .NET 8 library that digitally recreates every pertinent rules element of the
**2014 D&D 5th Edition Player's Handbook** as strongly-typed C# catalogs,
loaded from embedded JSON. Scope is the 2014 PHB specifically — not the
Monster Manual, DMG, or later sourcebooks. Each catalog entry that represents
official book content carries a citation (source document, page, section)
back to that PHB printing.

## Handoff, 2026-08-06 — read this first if resuming cold

Mid-stream on the "Quantized mechanics" pass (see that section below
for the full design reasoning and the running list of what's done vs.
remaining — check *there* for what's actually left, this handoff is
kept short/current on purpose, not a blow-by-blow history). **Merged
to `main`:** Fighting Style ([PR #22](https://github.com/brandonifco/FiveEData/pull/22)),
spell slot progressions ([PR #23](https://github.com/brandonifco/FiveEData/pull/23)),
Extra Attack progressions ([PR #24](https://github.com/brandonifco/FiveEData/pull/24)),
Rage ([PR #25](https://github.com/brandonifco/FiveEData/pull/25)),
Sneak Attack ([PR #26](https://github.com/brandonifco/FiveEData/pull/26)),
Divine Strike ([PR #27](https://github.com/brandonifco/FiveEData/pull/27)),
Ki and Sorcery Points ([PR #28](https://github.com/brandonifco/FiveEData/pull/28)),
Wild Shape and Circle Forms ([PR #29](https://github.com/brandonifco/FiveEData/pull/29)),
Paladin auras ([PR #30](https://github.com/brandonifco/FiveEData/pull/30)),
Bardic Inspiration die ([PR #31](https://github.com/brandonifco/FiveEData/pull/31)),
Channel Divinity uses ([PR #32](https://github.com/brandonifco/FiveEData/pull/32)),
Mystic Arcanum ([PR #33](https://github.com/brandonifco/FiveEData/pull/33)),
Font of Magic conversion ([PR #34](https://github.com/brandonifco/FiveEData/pull/34)),
Song of Rest ([PR #35](https://github.com/brandonifco/FiveEData/pull/35)),
Metamagic ([PR #36](https://github.com/brandonifco/FiveEData/pull/36)),
Battle Master maneuvers ([PR #37](https://github.com/brandonifco/FiveEData/pull/37)),
Eldritch Invocations ([PR #38](https://github.com/brandonifco/FiveEData/pull/38)),
Elemental Disciplines ([PR #39](https://github.com/brandonifco/FiveEData/pull/39)),
Channel Divinity options ([PR #40](https://github.com/brandonifco/FiveEData/pull/40)),
race trait quantization ([PR #41](https://github.com/brandonifco/FiveEData/pull/41)).
Pact Boon was evaluated and deliberately declined (no catalog needed —
see its section below), documented directly on `main` with no PR since
no code changed.

**Standing instruction, 2026-08-06: work the entire "Quantized
mechanics" remaining list to completion, one item per branch/PR, each
gated and merged before starting the next, CLAUDE.md updated in the
same commit every time.** Every per-level numeric progression is now
converted, and all six choice-point catalogs are resolved (see the
"Quantized mechanics" section below for the full closing summary of
that sub-project). Race traits (Darkvision, resistances, Trance,
Dwarven Toughness, Dragonborn's Breath Weapon) are also now converted
— see that section for the full writeup, including a genuine
pre-existing bug it turned up (Wood Elf's own speed override was
never populated despite an existing test already expecting it). **One
item left on the whole "Quantized mechanics" list: a background
numeric audit** (likely little to quantize — most background features
are narrative/social, not numeric) — do that next, and this entire
multi-week pass is done once it's merged. Check the
"Remaining, tracked but not
started" bullet at the end of "Quantized mechanics" below for the
live, shrinking version of this same list — it's kept current there
after every merge; this paragraph is the standing directive, that
bullet is the checklist.

**Nothing open right now.** `main` is synced with `origin/main`,
working tree clean.

**GitHub Actions major outage, ongoing since 2026-08-06 15:22 UTC —
check <https://www.githubstatus.com/> before assuming it's still
active, this note will go stale the moment it recovers.** PR #26's CI
failed once with `The job was not acquired by Runner of type hosted
even after multiple attempts`; a `gh run rerun` was kicked off and sat
`queued` for nearly an hour without progressing; PR #27's workflow run
never even started. Confirmed via githubstatus.com (not just inferred
from symptoms) as a major outage, still showing `"indicator":"major"`
("Partial System Outage") as of PR #41, 6+ hours in. Given the size of
the remaining work list, the user gave a **standing authorization for
the duration of this outage** (not a one-off, and not the
ask-every-time-default described below) to merge every PR directly off
the local gate without waiting for CI until GitHub Actions recovers —
PR #26 through #41 were all merged this way. **Once CI is confirmed
green again on a real PR, go back to waiting for it normally** — this
authorization is scoped to the outage, not a permanent standing
exception.

**One real, worth-remembering incident from this pass, in case it
recurs again:** PR #25's CI failed three consecutive times purely on
GitHub Actions infrastructure — never once reaching `dotnet
build`/`dotnet test` — with three different symptoms across the
attempts (`Failed to resolve action download info. Error: Service
Unavailable` twice, `The job was not acquired by Runner of type
hosted even after multiple attempts` twice, all at "Set up job").
Local gate had already passed before every push. Merged directly off
the local gate on explicit user instruction once the pattern was
clearly infra, not code. **If a future PR hits the same wall
repeatedly: check <https://www.githubstatus.com/>, retry with
`gh run rerun <run-id>` a couple of times first, and only bypass the
"wait for green CI" default with the user's explicit go-ahead — this
is not a standing exception, it needs to be asked for each time it
comes up.** A failure that ever shows real `dotnet` build/test output
is a different story entirely — that's a genuine regression, not
infra, and needs actual investigation before merging, full stop.

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

**Cross-class `RuleId` sharing was resolved once a third class's full
text was in hand.** Fighter's own "Ability Score Improvement" and "Extra
Attack" text is a genuine outlier — its sentences name extra trigger
levels (ASI at 6th/14th on top of the standard 4/8/12/16/19; Extra Attack
scaling further at 11th/20th) that no other class's version has, so both
stayed Fighter-prefixed (`fighter-ability-score-improvement`,
`fighter-extra-attack`). Barbarian's build (below) initially prefixed its
own versions too (`barbarian-ability-score-improvement`,
`barbarian-extra-attack`), reasoning there was nothing yet to compare
against. Monk's build (further below) supplied that comparison: Monk's
"Ability Score Improvement" and "Extra Attack" text is word-for-word
identical to Barbarian's (down to the exact trigger-level list), which is
exactly the Races bar for sharing (`Darkvision`-style: share only on
identical wording). Both were retroactively migrated to shared, unprefixed
`dnd5e2014.class-rule.ability-score-improvement` /
`dnd5e2014.class-rule.extra-attack`, each now carrying two
`SourceReference` entries (one per class, same multi-source shape
`food-drink-lodging-included-in-lifestyle` and `Darkvision` already use),
and the two now-orphaned Barbarian-prefixed rule entries were deleted
rather than left dead. `Unarmored Defense` is the converse case: Barbarian
and Monk share the exact *name* but not the mechanic (Dex+Con vs. Dex+Wis
AC formula) — correctly predicted and pre-emptively prefixed
(`barbarian-unarmored-defense`, `monk-unarmored-defense`) before Monk was
even built, confirming the collision was real. **The standing rule now has
real evidence behind it, not just a Races-derived guess:** default to a
shared, unprefixed `RuleId` for a mechanic bearing a generic name (Ability
Score Improvement, Extra Attack, Unarmored Defense, ...), and only
prefix/split when a specific class's actual PHB text diverges — verify by
reading the real text side by side each time a new class is built, the
same discipline this note itself is the product of. Two data points said
"don't share by default"; three said the opposite once the actual
divergent case (Fighter) was properly isolated from the two that
happened to agree (Barbarian, Monk). Don't treat either count as final —
re-check both shared IDs and both prefixed-pair precedents again once a
fourth class's text is in hand.

Fighter was chosen as the template class specifically because it has no
exceptions to model: full armor/weapon proficiency by category (unlike
Druid's nonmetal-only restriction), and all three of its PHB subclasses
(Champion, Battle Master, Eldritch Knight) were built in this same pass —
deliberately not just the simplest one — to prove the shape holds across
the real complexity range a class can contain, not just the easy case.

**Barbarian was the second class built**, chosen because its two PHB
subclasses (Path of the Berserker, Path of the Totem Warrior) exercise a
shape Fighter's own three didn't: Path of the Totem Warrior grants *two*
named features at the same chosen-at level (`Spirit Seeker` and `Totem
Spirit`, both at 3rd) rather than one, which `SubclassDefinition` already
supported structurally (`LevelFeatures` was never assumed to be
one-per-level) but had no real content exercising until now. Barbarian's
own "primary ability" (`PrimaryAbilityIds`/`RequiresAllPrimaryAbilities`)
is derived from the Chapter 6 multiclassing prerequisite table (p.163),
the same source Fighter's `[Strength, Dexterity]` / `requiresAll: false`
came from (Fighter's prereq is "Strength 13 *or* Dexterity 13") — verified
directly against that table rather than assumed from the class's own
Quick Build text, since Barbarian's Quick Build suggestion ("Strength,
followed by Constitution") reads similarly but means something different
(a stat-priority suggestion, not an either/or prerequisite). Barbarian's
prereq is a single ability ("Strength 13", no "or"), so
`RequiresAllPrimaryAbilities: true` with a one-element list — the first
real use of that flag as `true` since Fighter always had it `false`.
Recurring milestone features reuse one `RuleId` at each level, same as
Fighter's `Extra Attack`: `ability-score-improvement` (4th, 8th, 12th,
16th, 19th — see the cross-class sharing note above for why this is no
longer Barbarian-prefixed) and `brutal-critical` (9th, 13th, 17th — the
"1 die"/"2 dice"/"3 dice" scaling lives in the prose the citation points
to, not in structured data, matching how Draconic Ancestry's own
sub-table was left uncaptured).

**Monk was the third class built**, chosen specifically to supply the
comparison text the cross-class sharing note above needed, and because
its own shape stresses parts of the domain Fighter/Barbarian hadn't:
- **The first "and" multiclass prerequisite.** Fighter is "Strength 13
  *or* Dexterity 13" (`RequiresAllPrimaryAbilities: false`); Barbarian is
  a single ability, so the flag's value didn't actually matter yet. Monk
  is "Dexterity 13 *and* Wisdom 13" — the first real exercise of
  `RequiresAllPrimaryAbilities: true` with more than one ID, confirming
  the field's two-axis design (which abilities × whether all are
  required) actually covers the "and" case, not just "or" and "exactly
  one."
- **A named weapon exception alongside a category.** Monk's own
  proficiencies are "Simple weapons, shortswords" — shortsword is
  normally a *Martial* weapon, so this is `WeaponProficiencyCategories:
  [Simple]` plus `WeaponProficiencyIds: [shortsword]` together, the first
  real content in the category+explicit-ID list shape
  `WeaponProficiencyIds` was declared for but that Fighter/Barbarian
  (whose weapon proficiencies are categories only) never exercised.
- **A feature bundling several named sub-techniques with no table row of
  their own.** `Ki`'s prose grants three named ki techniques (Flurry of
  Blows, Patient Defense, Step of the Wind) as part of learning Ki
  itself — none of the three get their own `Features` column entry on
  the Monk table, so none get their own `RuleId`; they're folded inside
  `ki`'s own citation, the same restraint already applied to Fighting
  Style's 6 options.
- **A feature recurring by page, not just by level list.** `Unarmored
  Movement` is granted at 2nd level and improves again at 9th under the
  *same* heading (the 9th-level wall-walking ability is described in the
  same prose block as the 2nd-level speed bonus, not a new heading) — one
  `RuleId` reused at levels 2 and 9, the same recurring-feature shape
  Barbarian's `brutal-critical` already established, just triggered by
  "same heading covers both" rather than "same named feature growing."
- **A subclass whose 6th/11th/17th "tradition feature" slots aren't
  separately named.** Way of the Open Hand and Way of Shadow each grant a
  *distinctly titled* feature at 3rd/6th/11th/17th (matching Battle
  Master's own shape: `combat-superiority`/`know-your-enemy`/
  `improved-combat-superiority`/`relentless`, four different `RuleId`s).
  Way of the Four Elements doesn't — its whole tradition is one feature,
  `Disciple of the Elements`, that simply grants one additional elemental
  discipline choice at each of those levels with no new heading. Modeled
  as the *same* `RuleId` reused at 3rd/6th/11th/17th (four `LevelFeatures`
  entries, one `RuleId`), the recurring-feature shape rather than the
  four-distinct-features shape — a real fork in which of Fighter's two
  subclass patterns a given subclass follows, decided by reading the
  prose rather than assumed from the table's generic "Monastic Tradition
  feature" label. The 18 individual elemental disciplines themselves
  (Breath of Winter, Fangs of the Fire Snake, ...) are left inside
  `disciple-of-the-elements`'s own single citation with no
  sub-structure — the same restraint already applied to Battle Master's
  maneuver list and Eldritch Knight's spell-slot table, now proven again
  at a genuinely larger scale (18 named sub-options with individual ki
  costs, several casting real spells).
- **A real, deliberate scope gap, not an oversight:** "Tools: Choose one
  type of artisan's tools or one musical instrument" has no home in
  `ClassDefinition` — there is no tool-proficiency-choice field, the way
  there's a `SkillChoiceCount`/`SkillChoiceOptionIds` pair for skills.
  Left unmodeled, matching the precedent already set (silently) by
  Fighter's and Barbarian's own starting-equipment blocks, which were
  never captured either since no field exists for them. Revisit only if
  a later domain (multiclassing proficiency stacking, a character-builder
  consumer) actually needs structured tool-choice data — don't add the
  field speculatively ahead of that need.

**Rogue was the fourth class built**, and it supplied the first case of a
*feature*-level cross-class share, not just a class-level one, plus a
fresh divergent-ASI outlier to weigh against Fighter's:
- **`Evasion` is identical, word for word, between Monk and Rogue** —
  same 7th-level trigger, same "no damage on a success, half on a
  failure" text, confirmed by reading both side by side rather than
  assumed from the name. The existing `dnd5e2014.class-rule.evasion`
  entry (Monk's, page 79) gained a second `SourceReference` (Rogue's,
  page 96) rather than Rogue getting its own `rogue-evasion` — the same
  multi-source shared-`RuleId` shape the Ability Score Improvement /
  Extra Attack migration already established, just discovered a feature
  at a time instead of resolved once for a whole class. This is the
  proof the "verify by reading the real text every time" discipline
  actually pays for itself: `Evasion` looked like an obvious
  one-off-per-class feature name right up until it wasn't.
- **Rogue's own "Ability Score Improvement" is a *third*, independently
  divergent case, not a second confirmation of Fighter's.** Fighter gets
  it at 4/6/8/12/14/16/19 (extra levels named in its own sentence);
  Barbarian and Monk share the standard 4/8/12/16/19; Rogue's text names
  4/8/10/12/16/19 — a *different* extra level (10th, not 6th/14th) from
  Fighter's own divergence, and different from the shared standard too.
  `rogue-ability-score-improvement` is its own prefixed `RuleId`, sharing
  with neither of the other two groups — confirms the standing rule
  (default to sharing unless the actual text diverges) rather than
  either "Fighter is the only outlier" or "everyone eventually
  converges."
- **The first real content in `WeaponProficiencyIds` used alongside a
  category, not `WeaponProficiencyCategories` alone or a bare list.**
  Rogue's "Simple weapons, hand crossbows, longswords, rapiers,
  shortswords" is `WeaponProficiencyCategories: [Simple]` plus four named
  IDs — the same category+exception shape Monk's shortsword pioneered,
  now exercised with more than one named exception at once.
- **`Expertise` (choose skill/tool proficiencies to double) has no
  structured field**, the same category of gap as tool-proficiency
  choice above — folded into a single citation, reused at its own two
  grant levels (1st, 6th) the same recurring-feature shape
  `Unarmored Movement` and `disciple-of-the-elements` already
  established.
- **Arcane Trickster's own `Spellcasting` stays separately named and
  prefixed** (`arcane-trickster-spellcasting`), matching Eldritch
  Knight's precedent exactly rather than being compared for sharing —
  the two features share a heading word but nothing else (different
  spellcasting ability, different spell list restriction, a completely
  different slot progression), so this was never a real candidate for
  the generic-name default in the first place.

**Bard was the fifth class built, and the first full spellcaster**,
supplying the first real exercise of the "core `Spellcasting` feature is
a single citation" call this project made ahead of time (see "Status"
below): Bard's own Spellcasting section — Cantrips, the full Bard table,
Spell Slots, Spells Known, Spellcasting Ability, Ritual Casting,
Spellcasting Focus — reduces to one `dnd5e2014.class-rule.bard-spellcasting`
citation with no spell-slot-table structure, prefixed the same way
`eldritch-knight-spellcasting`/`arcane-trickster-spellcasting` already
are rather than compared against them for sharing (a full class's own
core spellcasting and a subclass's borrowed spellcasting share a heading
word and nothing else).
- **`Ability Score Improvement` is a third confirmation of the shared,
  unprefixed `RuleId`** — Bard's text ("When you reach 4th level, and
  again at 8th, 12th, 16th, and 19th level...") is word-for-word
  identical to Barbarian's and Monk's, verified by direct comparison
  rather than assumed from the standard levels matching. The existing
  `dnd5e2014.class-rule.ability-score-improvement` entry gained a third
  `SourceReference` rather than a `bard-ability-score-improvement`.
- **`Expertise` is a genuine mechanic-name collision with Rogue's,
  not a share** — Bard's version ("choose two skill proficiencies... at
  3rd level... two more at 10th") and Rogue's ("choose two skill
  proficiencies, or one skill proficiency and thieves' tools... at 1st
  level... two more at 6th") differ in both eligible-proficiency scope
  and grant levels when read side by side. Same shape as `Unarmored
  Defense`: Rogue's previously-unprefixed `dnd5e2014.class-rule.expertise`
  was retroactively renamed to `rogue-expertise` and Bard's own
  `bard-expertise` added alongside it, rather than leaving a name
  collision where only one of the two mechanics happened to hold the
  generic ID.
- **College of Valor's own `Extra Attack` stays separate from the shared
  `dnd5e2014.class-rule.extra-attack`** — its lead-in ("Starting at 6th
  level...") differs from the shared entry's ("Beginning at 5th
  level..."), and unlike the trigger-level-only differences that don't
  block sharing (the level itself already lives in `LevelFeatures`, not
  the `RuleId`), this is a difference in the sentence the citation
  itself points to. Prefixed `college-of-valor-extra-attack` rather than
  reusing the shared entry.
- **`Bonus Proficiencies` confirms it's inherently a subclass-specific
  name, never a sharing candidate** — College of Lore's version (three
  skills) and College of Valor's version (medium armor, shields, martial
  weapons) are mechanically unrelated aside from the name, joining
  Assassin's already-prefixed `assassin-bonus-proficiencies` as the third
  data point that this particular generic name always describes
  whatever proficiencies that specific subclass happens to grant, not a
  recurring mechanic — prefixed as `college-of-lore-bonus-proficiencies`
  / `college-of-valor-bonus-proficiencies` without first checking for a
  shared entry to reuse.
- **Tools ("three musical instruments of your choice") has no
  structured field**, the same category of gap as Monk's and Rogue's own
  tool-choice/tool-proficiency-choice text — left unmodeled, consistent
  with the standing precedent.
- **Citation precision note, resolved.** Bard's citations were originally
  built from an archive.org full-text OCR export whose embedded page-footer
  digits were largely missing or corrupted, reconstructed from a table-of-
  contents anchor plus content-flow reasoning rather than literal per-feature
  footer reads. A cleaner, actually page-scanned PHB PDF surfaced afterward
  (reliable per-page footers throughout); re-verifying against it confirmed
  most of the original reconstruction was already correct (the class's own
  page-52 citation and the `bard-spellcasting` page-53 citation both landed
  exactly right) and corrected the few that weren't (`bardic-inspiration`
  starts on 53, not 54; College of Lore's `Bonus Proficiencies`/`Cutting
  Words` and the subclass's own citation are page 54, not 55 — Cutting
  Words runs onto 55 but starts on 54, and citations point at a feature's
  starting page throughout this project). Use this same cleanly-scanned PDF
  (not the archive.org export) for all classes built from here on.

**Wizard was the sixth class built, and by far the most involved so
far** — 8 arcane traditions (the most subclasses of any PHB class) plus a
spellbook/ritual-casting/arcane-recovery mechanic no other class has:
- **Core `Spellcasting` again reduces to one citation**, following Bard's
  precedent exactly — Cantrips, the Spellbook mechanic (including the
  "Your Spellbook" sidebar on copying/replacing a lost spellbook),
  Preparing and Casting Spells, Spellcasting Ability, Ritual Casting, and
  Spellcasting Focus are all folded into `wizard-spellcasting` with no
  structured spellbook-contents field — consistent with the standing
  discipline of never storing spell lists as data.
- **`Ability Score Improvement` confirmed a fourth time**, word-for-word
  identical to the Barbarian/Monk/Bard text, gaining a fourth
  `SourceReference` rather than a `wizard-` prefixed entry.
- **Each school's `X Savant` feature stays its own prefixed `RuleId`,
  not a shared `savant` entry**, despite all eight being built from the
  identical template sentence ("the gold and time you must spend to copy
  a[n] `<school>` spell into your spellbook is halved") — the object of
  the discount is a different, mutually exclusive trigger per school (an
  Abjuration Savant gets no benefit copying a Divination spell), so this
  is the same category of decision as `Bonus Proficiencies`: a recurring
  template name that describes eight genuinely different mechanics, not
  one mechanic repeated. `EverySchoolSavantFeatureIsItsOwnDistinctRuleId`
  in `SubclassDataFileTests` pins this down explicitly so a future
  "helpful" consolidation doesn't collapse them.
- **Wizard's own weapon proficiencies are entirely named exceptions, no
  category at all** — "Daggers, darts, slings, quarterstaffs, light
  crossbows" is five `WeaponProficiencyIds` with an empty
  `WeaponProficiencyCategories`, the first class where the category list
  is empty rather than `[Simple]`-plus-exceptions (Monk/Rogue) or a bare
  category (Fighter/Barbarian). `ArmorProficiencyCategories` is likewise
  empty — Wizard is the first class with no armor proficiency of any
  kind, not even Light.
- **Arcane Tradition is chosen at 2nd level, not 3rd** — every subclass
  gateway built so far (Primal Path, Martial Archetype, Monastic
  Tradition, Roguish Archetype, Bard College) chose at 3rd level, which
  had been enough precedent that `SubclassDataFileTests` asserted it as a
  blanket rule (`ChosenAtLevel == 3` for every subclass) until Wizard's
  real table text broke that assumption. The test is now split into
  `EveryNonWizardSubclassIsChosenAtThirdLevel` /
  `EveryWizardSubclassIsChosenAtSecondLevel` rather than special-cased
  inline — a reminder that a rule confirmed by five classes in a row is
  still only as solid as the PHB text of the sixth.
- **`Spell Resistance` (Abjuration's 14th-level capstone) is a genuinely
  new, unprefixed `RuleId`** despite the generic-sounding name — nothing
  else in the catalog currently uses it, so there was no collision to
  weigh, unlike `Bonus Proficiencies`/`Expertise`/`Extra Attack`. Revisit
  if a later class's own "Spell Resistance"-named feature turns up with
  different text.
- **Page citations for this class came from the cleanly-scanned PDF from
  the start** (see the Bard citation-precision note above), not
  reconstructed after the fact — the school-by-school page assignments
  were read directly off legible per-page footers throughout, the same
  precision level as Fighter/Barbarian/Monk/Rogue.

**Cleric was the seventh class built, and the first with a genuinely
new shared-vs-prefixed split discovered from real text rather than
confirmed/denied against an existing precedent** — 7 divine domains
(Knowledge, Life, Light, Nature, Tempest, Trickery, War), each
structured like a subclass (`classId` = Cleric, its own `LevelFeatures`
and `Sources`) even though the PHB calls them domains, not subclasses:
- **`Divine Domain` is chosen at 1st level, not 2nd or 3rd** — a third
  distinct `ChosenAtLevel` value after Wizard's 2nd broke the
  3rd-level-only assumption. The blanket boolean test from before Wizard
  had already been split in two; rather than add a third near-duplicate
  test for Cleric, `SubclassDataFileTests` now carries one
  class-to-expected-level dictionary
  (`ExpectedChosenAtLevelByClassId`) and a single test that checks every
  subclass against its own class's entry — this shape should keep
  scaling as Druid (2nd), Sorcerer/Warlock (1st), and Paladin/Ranger
  (3rd) get built, rather than accreting another one-off test each time.
  The old cross-class Ability Score Improvement test
  (`SharesAbilityScoreImprovementRuleIdAcrossBarbarianMonkBardAndWizard`)
  got the same treatment here, generalized into
  `SharesAbilityScoreImprovementRuleIdAcrossClassesWithStandardWording`
  driven by a class-ID list instead of one parameter per class.
- **`Divine Strike` is the clearest `X Savant`-style case yet**: five of
  the seven domains (Life, Nature, Tempest, Trickery, War) grant an
  8th-level `Divine Strike` built from an identical template sentence
  ("you gain the ability to infuse your weapon strikes with divine
  energy... an extra 1d8 `<type>` damage... 2d8 at 14th level"), differing
  only in damage type (radiant/cold-fire-lightning-choice/thunder/poison/
  weapon-matching). Prefixed five ways
  (`life-divine-strike`/`nature-divine-strike`/etc.), same call as
  Wizard's school `Savant` features and for the same reason: the
  substituted word is the actual mechanical effect, not incidental
  flavor text.
- **`Potent Spellcasting` is the opposite finding from the same page**:
  Knowledge's and Light's own 8th-level features carry this same name
  *and* are verified word-for-word identical ("you add your Wisdom
  modifier to the damage you deal with any cleric cantrip", no
  substituted word at all) — a genuine share, gaining two
  `SourceReference`s on one `RuleId`. Reading both templates side by
  side in the same pass (`Divine Strike` right after `Potent
  Spellcasting` in the source text) is what makes this pairing a clean
  illustration of the actual dividing line: template-with-a-substitution
  stays split, template-with-zero-difference shares — not "recurring
  name" as a proxy for either outcome on its own.
- **`Bonus Proficiencies`/`Bonus Proficiency` split down the middle
  three ways among the seven domains, not uniformly**: Tempest's and
  War's own text ("At 1st level, you gain proficiency with martial
  weapons and heavy armor") is verified word-for-word identical and
  shared as `martial-and-heavy-armor-proficiency`; Life's and Nature's
  own singular `Bonus Proficiency` (heavy armor only) use different
  lead-in clauses ("When you choose this domain..." vs "Also at 1st
  level...") for the same grant and stayed separately prefixed
  (`life-bonus-proficiency`/`nature-bonus-proficiency`) on that basis —
  a real revision of the Bard-era blanket note that `Bonus
  Proficiencies` "is inherently subclass-specific, never a sharing
  candidate." That note was only three data points (Fighter/Barbarian/
  Monk/Rogue subclasses, all genuinely divergent); Tempest/War is the
  first verbatim match, proof the discipline is "verify every time," not
  "this generic name never shares."
- **Cleric's own `Channel Divinity` folds its baseline `Turn Undead`
  effect into the class-level citation rather than minting a separate
  `RuleId`** — the PHB table lists only "Channel Divinity (1/rest)" at
  2nd level, not a separate Turn Undead row, the same reasoning that
  kept Ki's Flurry of Blows/Patient Defense/Step of the Wind folded into
  `ki` rather than split out. Each *domain's* own Channel Divinity
  option (Knowledge of the Ages, Read Thoughts, Preserve Life, Radiance
  of the Dawn, Charm Animals and Plants, Destructive Wrath, Invoke
  Duplicity, Cloak of Shadows, Guided Strike, War God's Blessing) does
  get its own citation, since each is a genuinely distinct named
  sub-choice, not an automatic baseline grant.
- **`Divine Intervention` reuses one `RuleId` at both its 10th-level
  grant and 20th-level improvement**, the same recurring-feature shape
  `Unarmored Movement`/`disciple-of-the-elements`/`Expertise` already
  established — the 20th-level text ("your call for intervention
  succeeds automatically") is a modifier on the same named feature, not
  a new one.

**Warlock was the eighth class built**, chosen ahead of the remaining
five (Druid, Paladin, Ranger, Sorcerer) specifically because its shape —
only 3 patrons, but three separate named subsystems (Pact Magic, Pact
Boon, Eldritch Invocations) layered on top — stresses the domain
differently than raw subclass count does:
- **The class's own core spellcasting feature is table-named "Pact
  Magic," not "Spellcasting"** — the first class where this is true.
  `warlock-pact-magic` still folds Cantrips/Spell Slots/Spells
  Known/Spellcasting Ability/Spellcasting Focus into one citation like
  every prior class's core casting feature, but Warlock has no "Ritual
  Casting" subsection at all in its core spellcasting text (that
  capability only exists via the Pact of the Tome invocation), so
  nothing was folded in for it — a real absence, not an oversight.
- **All ~20 Eldritch Invocations fold into one `eldritch-invocations`
  citation**, the same call as Fighting Style's 6 options, Battle
  Master's maneuver list, and Way of the Four Elements' 18 elemental
  disciplines — a flat choose-N-of-a-published-list feature stays one
  citation regardless of list size, since the deciding factor per
  precedent is "is this a choice point," not "how many options does it
  have." The two PHB headings that together describe this feature (the
  Class Features section's mechanical framework, and the "Eldritch
  Invocations" reference-list preamble immediately before the
  alphabetized options) both point at the same single `RuleId`.
- **`Pact Boon`'s three named options (Chain/Blade/Tome) also fold into
  one citation** rather than getting individual `RuleId`s, the same
  choice-point treatment — even though each option here is a
  substantial, multi-paragraph mechanic (Pact of the Blade alone runs
  to roughly 200 words), well past Fighting Style's one-liners in size.
  The precedent's dividing line is structural (a single choice point
  offering named sub-options) rather than word count, so size alone
  didn't earn them separate citations.
- **`Expanded Spell List` is a `Divine Strike`-shaped case, not a
  `Potent Spellcasting`-shaped one**: all three patrons use the
  identical template sentence ("`<Patron>` lets you choose from an
  expanded list of spells..."), but each actually adds a *different*
  set of spells to the list — the substituted content is the entire
  point of the feature, so all three stay prefixed
  (`archfey-`/`fiend-`/`great-old-one-expanded-spell-list`), matching
  the school-`Savant`/`Divine Strike` reasoning rather than
  `Potent Spellcasting`'s zero-difference share.
- **`Ability Score Improvement` is shared a sixth time, but only after
  resolving a real table/prose conflict.** The printed Warlock table's
  Features column has no 19th-level row (only 4th/8th/12th/16th), but
  the feature's own body text explicitly names 19th level, word-for-word
  identical to the shared entry's canonical wording used by every other
  sharing class. Treated the prose as authoritative over the seemingly
  incomplete table — the same resolution precedent (corrected text wins
  over a printing artifact) as the Dwarf throwing-hammer/light-hammer
  errata from the Races pass, formalized in the data as levels
  4/8/12/16/19 and called out explicitly in
  `PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission`
  so a future reader checking the level list against the table doesn't
  "fix" it back to a mistaken 4/8/12/16.
- **`Mystic Arcanum` reuses one `RuleId` across all four grant levels**
  (11th/13th/15th/17th), the same recurring-citation shape as `Divine
  Intervention`/`Brutal Critical` — the per-level spell-tier scaling
  lives in the citation's own prose, not in structured data.

**Druid was the ninth class built.** Its own "Fighter was chosen as the
template class specifically because it has no exceptions to model...
unlike Druid's nonmetal-only restriction" note (see "Architecture"
above, written well before either class's own section existed) flagged
this in advance — here's how it actually resolved:
- **The nonmetal-armor/shield restriction stays unmodeled, the same
  gap category as tool-proficiency choice.** The PHB text is a
  parenthetical aside on the `Armor:` proficiency line ("druids will not
  wear armor or use shields made of metal") rather than a separate
  mechanical feature elsewhere in the rules, and `ArmorProficiencyCategories`
  has no material-restriction axis the way `WeaponProficiencyIds` has a
  category+exception shape. Modeling it would mean inventing a new field
  with no second consumer yet — the same "don't add fields speculatively
  ahead of real need" call already made for Monk's/Rogue's/Bard's own
  unmodeled tool choices. Revisit if a later domain (equipment
  validation, a character builder) actually needs to enforce it.
- **`Timeless Body` is a second `Unarmored Defense`-shaped collision**,
  this time against Monk's own already-built entry rather than caught
  within the same class's build: Druid's version ("age more slowly, 1
  year per 10") and Monk's ("suffer none of the frailty of old age...
  still die of old age, however... no longer need food or water") share
  a name but grant genuinely different benefits. Monk's previously-bare
  `dnd5e2014.class-rule.timeless-body` was retroactively renamed to
  `monk-timeless-body` alongside the new `druid-timeless-body`, the same
  resolution as Rogue's `expertise` → `rogue-expertise` migration when
  Bard's Expertise collided — the pattern holds across both "collision
  found while building the second class" (Bard/Rogue) and "collision
  found by cross-checking an unrelated earlier class while building a
  much later one" (Druid/Monk), which is exactly why the standing rule
  is "verify every time," not "verify against the immediately preceding
  class."
- **`Wild Shape` reuses one `RuleId` across its 2nd/4th/8th-level
  entries** — the level-gated CR/speed thresholds live entirely in the
  Beast Shapes table referenced from the one citation, the same
  recurring-feature shape as `Circle Forms` (Circle of the Moon's own
  2nd/6th-level CR-cap increases, folded the same way).
- **`Circle Spells` is Circle of the Land's version of the `Domain
  Spells` non-decision already made for Cleric** — 8 terrain choices
  (Arctic, Coast, Desert, Forest, Grassland, Mountain, Swamp,
  Underdark) × 4 grant levels × 2 spells each is real spell-list content
  that stays entirely inside one citation's prose rather than becoming
  56 structured spell references, consistent with the project not
  modeling spells as a domain yet.
- **`Ability Score Improvement` shared a seventh time**, word-for-word
  identical text, no complications this time (unlike Warlock's
  table/prose conflict).

**Ranger was the tenth class built**, and the first to supply *three*
new cross-class shares in one pass rather than a same-class or
same-page finding — all three discovered by reading Ranger's actual
text and checking it against classes built in entirely separate,
already-merged PRs, not just the most recently built one:
- **`Fighting Style` is shared with Fighter's existing entry.** Ranger's
  gateway sentence ("you adopt a particular style of fighting as your
  specialty. Choose one of the following options. You can't take a
  Fighting Style option more than once...") is word-for-word identical
  to Fighter's own (module the "At 2nd level" trigger clause, already
  handled by `LevelFeatures` rather than the citation text). Ranger
  offers only 4 of Fighter's 6 named options (no Great Weapon
  Fighting/Protection) — confirmed this doesn't block sharing, since the
  citation never enumerated which options belong to which class in the
  first place; a shared choice-point `RuleId` was never a promise that
  every consuming class offers the identical option set, only that the
  *gateway mechanic's own wording* matches.
- **`Extra Attack` joins the existing Barbarian/Monk share**, using
  their exact "Beginning at 5th level..." wording rather than Fighter's
  own divergent (extra-scaling) version — a third confirmation of that
  specific shared entry, not a new one.
- **`Land's Stride` turned out to already exist, word-for-word, as
  Circle of the Land's own entry** (a Druid subclass feature from two
  classes and two merged PRs earlier). Ranger's version differs only in
  trigger level (8th vs. Circle of the Land's 6th) — everything else,
  including the `entangle`-spell example, matches exactly. This is the
  first share discovered across genuinely non-adjacent classes (Druid
  wasn't the class immediately before Ranger — Warlock was), which is
  the real proof the "verify against every earlier class, not just the
  last one" framing (added during Druid's `Timeless Body` collision) is
  necessary and not just cautious phrasing: a same-page or same-build
  comparison alone would never have caught this one.
- **No new archetype-specific `RuleId`s split by a template this time**
  — Hunter's and Beast Master's own features are each uniquely named
  with no repeated-template-across-siblings shape (unlike Wizard's
  Savants or Cleric's Divine Strike), so this class contributed sharing
  *discoveries* without also contributing a new *splitting* precedent.
  `Hunter's Prey`/`Defensive Tactics`/`Multiattack`/`Superior Hunter's
  Defense` each fold their own named sub-options (Colossus Slayer/Giant
  Killer/Horde Breaker, etc.) into one citation apiece, the same
  choice-point treatment as `Pact Boon` and `Fighting Style` itself.

**Paladin was the eleventh class built**, and the first to produce a
*near*-miss on `Fighting Style` sharing rather than a clean confirm or a
clear divergence like College of Valor's:
- **`Fighting Style` stayed unshared on a single dropped word, treated
  as real rather than assumed OCR noise.** Paladin's gateway text reads
  "you adopt a style of fighting as your specialty," missing the word
  "particular" that both Fighter's and Ranger's identical phrasing
  carries ("a *particular* style of fighting"). Working from an OCR
  extraction, a one-word gap is genuinely ambiguous — it could be a
  scanning artifact or a real, if inconsequential, printing difference.
  Given that ambiguity, the choice was to keep `paladin-fighting-style`
  separate rather than share: the two failure directions aren't
  symmetric (wrongly merging two possibly-distinct texts under one
  citation is a correctness problem; keeping an extra, possibly-
  redundant citation around is not). This is the same strict standard
  College of Valor's Extra Attack was held to over "Starting" vs.
  "Beginning," applied consistently even though "particular" carries no
  mechanical weight at all — the rule is about verified-identical text,
  not about how much the difference seems to matter.
- **`Extra Attack` and `Ability Score Improvement` both shared cleanly**,
  word-for-word identical to the existing entries, no complications.
- **`Aura of Protection` and `Aura of Courage` both reuse one `RuleId`
  across their 6th/10th-level grant and shared 18th-level range
  increase** — the class table's own "18th: Aura improvements" row is a
  single generic placeholder covering both core auras at once (neither
  aura gets its own numbered table row at 18th), the same recurring-
  citation shape as `Wild Shape`. The two *oath*-specific auras (Aura of
  Devotion, Aura of Warding) also each have their own "expands to 30 feet
  at 18th level" clause in their prose, but with no table row cueing it
  (unlike the core auras' explicit "18th: Aura improvements" row), that
  detail was left inside each aura's own single citation rather than
  given a second `LevelFeatures` entry — the table is what decides
  whether a recurring mechanic gets a repeated `LevelFeatures` entry or
  stays folded into one grant-level citation, not just "does the prose
  mention scaling."
- **Each oath's own `Oath Spells` framework got its own per-oath
  `RuleId`, reused across all five grant levels (3rd/5th/9th/13th/17th)**,
  matching Circle of the Land's `Circle Spells` treatment rather than
  Cleric's `Domain Spells` (which folded into the gateway citation with
  no separate entry at all) — the deciding factor is the same one used
  throughout: `Circle Spells` and `Oath Spells` both show up as their
  own explicitly leveled grants in each subclass's own text structure,
  the same shape Cleric's domain spells lacked.
- **Both class-level "framework" headings — `Oath Spells` (the general
  explanation of how oath spells work) and `Channel Divinity` (the
  general explanation of how Channel Divinity works) — folded into
  `sacred-oath`'s own citation rather than getting their own `RuleId`s**,
  since neither is its own row in the class table (unlike Cleric, where
  `Channel Divinity` *is* its own 2nd-level table row and kept its own
  citation for exactly that reason). The per-oath Channel Divinity
  *options* (Sacred Weapon, Turn the Unholy, Nature's Wrath, Turn the
  Faithless, Abjure Enemy, Vow of Enmity) each still get their own
  citation, the same treatment Cleric's domain-specific Channel Divinity
  options already established.

**Sorcerer was the twelfth and final PHB class built**, closing out the
Classes domain. Structurally the simplest of the spellcasters (only 2
origins), but with two class-specific subsystems (Font of Magic,
Metamagic) that don't exist anywhere else:
- **`Font of Magic` folds Sorcery Points, Flexible Casting, the
  Creating Spell Slots table, and the sorcery-points-to-spell-slot
  conversion math into one citation** — the same "a feature's own
  reference table stays inside its citation" treatment already given to
  Destroy Undead's CR table and the Beast Shapes table Wild Shape
  points to.
- **All 8 Metamagic options (Careful/Distant/Empowered/Extended/
  Heightened/Quickened/Subtle/Twinned Spell) fold into one `metamagic`
  citation**, reused across all three grant levels (3rd/10th/17th) —
  the same choice-point treatment as Eldritch Invocations, Pact Boon,
  and Fighting Style, now exercised at yet another list size.
- **No Ritual Casting subsection exists in core Spellcasting**, the
  second class after Warlock where that's true (real 5e Sorcerers get
  no baseline ritual casting) — confirms this is a genuine per-class
  absence worth checking for each time, not a one-off Warlock quirk.
- **Ability Score Improvement shared cleanly again**, word-for-word
  identical, no complications — the eighth class to share this entry
  without incident, against the two (Fighter, Rogue) and one near-miss
  (Paladin's dropped "particular") that didn't.
- **Sorcerer's five named weapon exceptions are identical to Wizard's
  own list** (dagger, dart, sling, quarterstaff, light crossbow) —
  confirmed by direct comparison rather than assumed from both classes
  "feeling similar." This isn't a `RuleId`-sharing case (weapon lists
  aren't citations), just a data-fidelity check that two independently
  transcribed lists actually match; pinned down by
  `DoesNotDuplicateWizardsWeaponProficiencyIdList` so a future edit to
  either class's list doesn't silently drift the other out of sync
  without someone noticing.

With Sorcerer merged, all 12 PHB classes and all 40 of their subclasses
are built. See "Status" below for the closing summary of what the
Classes pass as a whole established.

## Backgrounds

`Rules/Backgrounds/` is a new top-level domain (sibling to
`Rules/Equipment`, `Rules/Expenses`, `Rules/Creatures`, `Rules/Classes`),
not nested under `Rules/Creatures/` — same reasoning as Classes: a
background is a player-build concept in its own right, not a
creature-vocabulary consumer.

Backgrounds turned out structurally much simpler than every other
domain built so far, once the real content was read rather than assumed:
- **`BackgroundDefinition` has no ability scores, no size/speed, no
  level-gated feature list** — a background is a flat, one-shot grant.
  The 2014 PHB's 13 backgrounds each reduce to exactly the same five
  facts: two fixed skill proficiencies (never a choice — verified across
  all 13, not assumed from the first few), a language choice count
  (0, 1, or 2 — always "N of your choice," never named languages), and
  one signature feature. `SkillProficiencyIds` is validated to be
  exactly two, since that's true of every real background and a
  background with one or three would indicate a data error, not a
  legitimate variant.
- **Tool proficiencies and starting equipment stay unmodeled**, the same
  gap already established for classes (Monk's/Bard's/Rogue's own
  unmodeled tool choices, Fighter's/Barbarian's own unmodeled starting
  equipment). Backgrounds actually lean on this gap harder than classes
  did — most backgrounds grant tool proficiencies (sometimes fixed,
  sometimes "one type of X," occasionally both in the same background),
  and every one of them lists starting equipment — but the same
  "don't add a field speculatively ahead of a real consumer" reasoning
  applies without needing a new exception.
- **Suggested Characteristics tables (Personality Trait/Ideal/Bond/Flaw)
  and the per-background flavor sub-tables (Criminal Specialty,
  Entertainer Routines, Defining Event, Guild Business, Life of
  Seclusion, Origin) are pure roleplay flavor with no mechanical
  weight** — left out entirely, the same treatment already given to
  Draconic Ancestry's damage sub-table and Battle Master's maneuver
  list: real content, deliberately not modeled, because the citation
  already covers it.
- **Variant backgrounds (Spy, Gladiator, Guild Merchant, Knight,
  Retainers, Pirate) are out of scope**, the same call already made for
  Variant Human during Races — an optional reskin of an existing
  background with swapped proficiencies isn't a new named background,
  it's optional prose the citation already covers.
- **One rule file per domain continues**: `background-rule.json`
  joins `race-rule.json`/`class-rule.json` as its own file under
  `Data/dnd5e2014/rules/`, merged the same way at load time. All 13
  features are uniquely named (no cross-domain or cross-background
  collisions), so no sharing/prefixing question came up this time —
  the first domain pass where that didn't happen.
- **Page citations were pinned down against the same cleanly-scanned
  PDF** used for every class since Bard's citation-precision fix,
  including several cases of the by-now-familiar two-column layout
  artifact (a background's fixed-proficiency block appearing a page
  before its own flavor text and named feature, because the facing
  page's right column ran ahead of the left) — resolved the same way
  as always: cite the page where the feature's own substantial body
  text lives, not wherever a stray heading or stat line surfaces first.

## Quantized mechanics — a second pass over already-built content

**Standing decision, 2026-08-06.** Every domain built so far
(Races/Classes/Backgrounds) stores a named feature's actual
game-mechanical payload — Rage's damage bonus, Extra Attack's count, a
spell slot table, a Fighting Style's own numbers — as a `RuleId`
citation only (`RuleDefinition` is `Id`/`Name`/`Sources`, nothing else).
That was the right call for the provenance-verified-breadth goal
Classes/Races/Backgrounds were built for, but this project's other
purpose — a possible future game, built and compared independently
against 5eGoldBox's own hand-authored, execution-ready ruleset content
(the two are parallel projects and are never wired together) — needs
the numbers themselves, not a page reference. This is a real, large
second pass over content already marked "complete" above, not new
domain coverage, and it isn't finished — Fighting Style is the first
domain converted, proving the shape; everything else this section
lists as remaining is still citation-only.

**Fighting Style converted first, proving the shape.** Chosen for the
same reason Fighter was chosen to prove the Classes shape: shared
across three classes (Fighter/Ranger/Paladin), exactly six named
options, no sub-choices of its own, and — once actually read side by
side against the PHB rather than assumed — a real per-class
availability asymmetry worth getting right: Ranger offers 4 of 6 (no
Great Weapon Fighting/Protection), Paladin offers a different 4 of 6
(no Archery/Two-Weapon Fighting), Fighter alone offers all 6. Verified
directly against the cleanly-scanned PHB PDF (pages 72/85/91), not
from memory.

- **New domain, additive only — no existing type touched.**
  `Rules/Classes/FightingStyles/` follows the same five-piece shape
  every domain uses (`FightingStyleId`/`FightingStyleDefinition`/
  `FightingStyleDefinitionValidator`/`Serialization/*`/
  `FightingStyleCatalog`), sibling to `Rules/Classes` the way
  `Rules/Equipment/Weapons` sits under `Rules/Equipment`.
  `ClassLevelFeature`/`classes.json`'s existing
  `dnd5e2014.class-rule.fighting-style`/`paladin-fighting-style`
  gateway citations are untouched — a fighting style option instead
  carries its own `AvailableToClassIds: IReadOnlyList<ClassId>`, since
  the per-class option subset can't be derived from which gateway
  `RuleId` a class cites (Fighter and Ranger cite the *same* shared
  gateway, yet offer different option sets).
- **The effect schema is deliberately not a generic DSL.** 5e's
  mechanics are too heterogeneous for one flat "Effect" shape to cover
  honestly (a flat roll bonus, a conditional damage bonus, a reroll
  rule, and a reaction are different in kind, not just in parameters)
  — instead `FightingStyleDefinition` carries several typed, nullable
  mechanism fields side by side (`RollBonus`/`ArmorClassBonus`/
  `DamageDieReroll`/`Reaction`/`GrantsOffHandAbilityModifierDamage`),
  the same multi-optional-field shape `WeaponDefinition` already
  established (`Damage`/`Range`/`VersatileDamage`/`AmmunitionTypeId`),
  not a new discriminated-union pattern this codebase doesn't
  otherwise use. `FightingStyleDefinitionValidator` enforces exactly
  one mechanism populated, since every real 2014 PHB option is a
  single distinct mechanic.
- **A new `FightingStyleWeaponRequirement` enum, not a reuse of
  `WeaponUsageCategory`/`WeaponProperty`.** Archery's "ranged weapons"
  condition could reuse `WeaponUsageCategory.Ranged` directly, but
  Dueling's "wielding a melee weapon in one hand and no other weapons"
  and Two-Weapon Fighting's "engaged in two-weapon fighting" are
  wielding-state conditions — about what else is in the other hand —
  not a property of a single weapon record, so they can't be expressed
  by referencing `WeaponDefinition` fields alone. Great Weapon
  Fighting's condition *does* map onto existing
  `WeaponProperty.TwoHanded`/`.Versatile`, but is still named through
  the same enum for one consistent "what does this fighting style
  require of the weapon being used" axis across all three roll/reroll
  mechanisms, rather than splitting reused-vs-new conditions across two
  different types.
- Public API: the exported surface grew by the new domain's own public
  types (`FightingStyleId`, `FightingStyleDefinition`,
  `FightingStyleRollTarget`, `FightingStyleWeaponRequirement`,
  `FightingStyleRollBonus`, `FightingStyleDamageDieReroll`,
  `FightingStyleReaction`, `FightingStyleCatalog`, plus
  `Dnd5e2014Ruleset.FightingStyles`) — no existing public type's shape
  changed. `RulesetDefinitionSet`/`Dnd5e2014RulesetLoader`/
  `CatalogIntegrityValidator` wired through the same pattern every
  other domain uses (cross-references `AvailableToClassIds` against
  the real `ClassId` set). Full gate green: Debug+Release build 0
  warnings, 1158 tests (was 1115; +43 new —
  `FightingStyleFoundationTests`/`FightingStyleDefinitionLoaderTests`/
  `FightingStyleDataFileTests`, the same three-file convention every
  domain follows).
- **Not yet decided: how the other, larger choice-point catalogs**
  (Eldritch Invocations, Battle Master maneuvers, Metamagic, Elemental
  Disciplines, Channel Divinity options, Pact Boon) **should reuse or
  diverge from this shape** — each is bigger, and some (Pact Boon,
  Battle Master maneuvers) have real sub-structure of their own (a
  maneuver has its own save DC and triggering condition, not just a
  flat bonus). Fighting Style proved the *pattern* (typed mechanism
  fields, not a DSL; a class-availability list kept separate from the
  gateway citation); it didn't prove every mechanism shape a later
  catalog will need.
**Spellcasting slot tables converted second**, closing the first item
on that "remaining" list. Every caster's `Spellcasting`/`Pact Magic`
`RuleId` citation stayed exactly as it was — this only adds the two
facts a game actually needs to run the resource economy (how many
slots, of what level, at each character level, and which ability
governs it), not the surrounding prose (cantrips-known/spells-known
counts, ritual casting, spellbook mechanics all stay citation-only,
a deliberate scope cut — see below).

- **A new domain, `SpellSlotProgression`, referenced by ID rather than
  duplicated per class.** All 5 full casters (Bard, Cleric, Druid,
  Sorcerer, Wizard) share one byte-identical slot table; Paladin and
  Ranger share a second (the "half-caster" table); Eldritch Knight and
  Arcane Trickster share a third (the "third-caster" table); Warlock's
  Pact Magic is a fourth, structurally different table (one slot
  *level* per character level rather than a spread across several
  spell levels, and — read directly off the Warlock's own page rather
  than assumed — recovers on a short *or* long rest, not long rest
  only like every other caster). Four `SpellSlotProgressionDefinition`
  rows total, cross-referenced by ID from `ClassDefinition`
  (`SpellSlotProgressionId?`/`SpellcastingAbilityId?`, both null for
  the four non-caster base classes) and from `SubclassDefinition` (null
  for every subclass except Eldritch Knight/Arcane Trickster, which
  grant casting at the subclass level, not the class level) — the same
  "reference a shared row by ID instead of repeating it" shape
  `RuleId` itself already established, chosen specifically because
  repeating a 20-row table five times in `classes.json` would have
  been real, avoidable duplication.
- **The table shape is uniform on purpose.** A `SpellSlotLevelEntry` is
  always present for character levels 1-20 even where a caster has no
  slots yet (Ranger/Paladin at 1st, Eldritch Knight/Arcane Trickster at
  1st-2nd) — an empty `Slots` list, not a missing row — so every
  progression has the same 20-entry shape regardless of when its
  caster actually starts casting, and `SpellSlotProgressionDefinitionValidator`
  enforces exactly one entry per level 1-20, not "at least the levels
  that matter."
- **Every number was read directly off the PHB, not derived from the
  well-known 5e slot-progression tables from memory** — the project's
  own standing discipline, re-applied here specifically because a
  table this size is exactly where a memory slip is easy and costly.
  Confirmed page-for-page: Wizard's own table (full caster), Ranger's
  *and* Paladin's own tables independently (half caster — not assumed
  shared from one, checked against both, the same "verify every time"
  rule the Classes pass established for `RuleId` sharing), Eldritch
  Knight's *and* Arcane Trickster's own tables independently (third
  caster), and Warlock's own table (Pact Magic).
- **`SpellcastingAbilityId` rides alongside the slot progression on
  the same two fields**, not a separate lookup — without it, a slot
  table alone can't compute an attack bonus or save DC, so it was
  added in the same pass rather than deferred: Charisma (Bard,
  Sorcerer, Paladin, Warlock), Wisdom (Cleric, Druid, Ranger),
  Intelligence (Wizard, Eldritch Knight, Arcane Trickster) — each
  verified against its own class's "Spellcasting Ability" paragraph,
  not inferred from the class's primary ability score.
- **Deliberately still citation-only, not folded into this pass:**
  cantrips-known-by-level and spells-known-by-level (both are real
  per-class tables, but *not* shared across classes the way slots are
  — each full caster's own cantrip progression differs — so
  quantizing them is a second, separate content-authoring pass, not a
  reference-by-ID problem like this one was), ritual casting,
  spellbook/preparation mechanics, and Sorcery Points/Font of Magic
  (a Sorcerer-specific resource, picked up instead under the
  per-level-numeric-progressions item below).
- Public API: four new public types
  (`SpellSlotProgressionId`/`Definition`/`Catalog`, plus
  `SpellSlotRecoveryRest`/`SpellSlotCount`/`SpellSlotLevelEntry`) and
  two new members apiece on the already-public `ClassDefinition`/
  `SubclassDefinition`. Full gate green: Debug+Release build 0
  warnings, 1214 tests (was 1158; +56 new — foundation/loader/data-file
  coverage for the new domain, plus per-class and per-subclass
  spellcasting-field assertions folded into the existing
  `ClassDataFileTests`/`SubclassDataFileTests`).
**Extra Attack converted third — the first of the "other per-level
numeric progressions" and a smaller shape than the previous two.**
Where Fighting Style needed a full mechanism-typed effect and spell
slots needed a dense 20-row table, Extra Attack is just "at which
level(s) does the attack count go up, and to what" — a sparse list of
breakpoints, not a value at every level. The same shared-vs-prefixed
question the Classes pass already answered for the underlying `RuleId`
(Barbarian/Monk/Ranger/Paladin share one "Beginning at 5th level, you
can attack twice" citation; Fighter's own diverges with extra scaling
at 11th/20th, so it stayed separately prefixed) carries over exactly
to the quantized data: two progressions, `standard` (one grant, level
5 → 2 attacks) and `fighter` (three grants: 5→2, 11→3, 20→4).

- **New domain, `Rules/Classes/ExtraAttack/`**, same five-piece shape,
  referenced from `ClassDefinition.ExtraAttackProgressionId` only —
  no subclass grants Extra Attack in this ruleset, unlike spellcasting's
  Eldritch Knight/Arcane Trickster case, so `SubclassDefinition` was
  left untouched. `ExtraAttackGrant` (`CharacterLevel`, `AttackCount`)
  self-validates in its own constructor (level 1-20, count ≥ 2 — 1
  attack is the baseline everyone already has, not something a class
  feature grants) the same way `ClassLevelFeature`/`RaceAbilityScoreIncrease`
  already do; `ExtraAttackProgressionDefinitionValidator` additionally
  checks grants are level-ordered with a strictly increasing attack
  count, since a progression that doesn't increase isn't a real grant.
- **Citations reused directly from the existing, already-verified
  `RuleId` entries** (`dnd5e2014.class-rule.extra-attack`'s four pages
  — Barbarian 49, Monk 79, Paladin 85, Ranger 92 — and
  `fighter-extra-attack`'s page 73) rather than re-fetched from the
  PDF, since those page numbers were already provenance-checked when
  the Classes pass built the citation itself; re-deriving them here
  would have been re-verifying already-verified work, not real risk
  reduction.
- Public API: three new public types
  (`ExtraAttackProgressionId`/`Definition`/`Catalog`, plus
  `ExtraAttackGrant`) and one new member on `ClassDefinition`. Full
  gate green: Debug+Release build 0 warnings, 1256 tests (was 1214;
  +42 new).
**Rage converted fourth — the first case where the shared-catalog
pattern was deliberately *not* reused.** Fighting Style/Spell Slots/
Extra Attack all needed a referenced-by-ID catalog because multiple
classes shared the same table. Rage belongs to exactly one class
(Barbarian) — nothing to share — so building a whole catalog+loader
domain for it would have been indirection with no payoff, the same
"don't build generality ahead of real need" call this project has made
before. Instead `RageProgressionDetail` is a plain embedded value
object, mapped inline by `RageProgressionDetailDataMapper` the same
way `RaceAbilityScoreIncreaseData` is mapped inline inside
`RaceDefinitionLoader` rather than getting its own top-level loader.

- **The full mechanical fact set was captured, not just the two
  leveled numbers.** Read directly off the Barbarian's own table and
  Rage's own prose (pages 47-48): `UsesByLevel` (breakpoints at 1st→2,
  3rd→3, 6th→4, 12th→5, 17th→6, 20th→unlimited) and
  `DamageBonusByLevel` (1st→+2, 9th→+3, 16th→+4) are the two leveled
  progressions, but a slot table alone still wouldn't make Rage
  actually runnable — so `DurationMinutes` (1, "Your rage lasts for 1
  minute" — not the 10-rounds figure a AD&D-era memory might reach
  for, confirmed by reading the actual sentence), `ResistedDamageTypeIds`
  (bludgeoning/piercing/slashing, a constant, not leveled), and
  `RequiresNotWearingHeavyArmor` (every rage benefit — advantage on
  Strength checks/saves, the damage bonus, the resistance — is gated
  on this, per the table's own "if you aren't wearing heavy armor"
  clause) all rode along in the same value object. Deliberately left
  out: Persistent Rage (15th level, changes rage's *end conditions*,
  not a number), Relentless Rage and Feral Instinct (separate features
  entirely, not part of Rage's own progression) — all three stay
  citation-only, since none of them reduces to a leveled number the
  way this pass is scoped to capture.
- **`UsesPerLongRest` is `int?`, not an `int` with a sentinel value,**
  specifically so 20th level's "Unlimited" has an honest, unambiguous
  representation (null) rather than something like `int.MaxValue`
  standing in for a concept the type doesn't actually name.
  `ClassDefinitionValidator`'s new ascending-sequence check treats a
  transition *into* unlimited, or staying unlimited at a later grant,
  as always valid — the "must increase" rule only applies while both
  the previous and current grant are still finite numbers.
- Public API: one new public type (`RageProgressionDetail`, plus its
  two nested grant record structs `RageUseGrant`/`RageDamageBonusGrant`)
  and one new member on `ClassDefinition`. No new catalog, no new
  embedded-resource file — Barbarian's Rage data lives directly inside
  `classes.json`. Full gate green: Debug+Release build 0 warnings,
  1282 tests (was 1256; +26 new).
**Sneak Attack converted fifth, same "embedded value object" shape as
Rage** — Rogue-exclusive, nothing to share. Read directly off the
Rogue's own table and Sneak Attack's own prose (page 96): dice count
doubles at every odd level from 1st (1d6) through 19th (10d6), always
the same die size.

- **Each grant reuses the existing `DiceExpression` type** (already
  used for `HitDie`/`WeaponDamage`) rather than a bare "dice count"
  int paired with a separate constant die-size field — a
  `SneakAttackDiceGrant` is `(CharacterLevel, DiceExpression)`, so
  1d6→10d6 is stored exactly like a weapon's damage die would be,
  instead of inventing a redundant number space next to a type that
  already exists for precisely this.
- **The full mechanical gate was captured alongside the leveled dice,
  the same call as Rage's `RequiresNotWearingHeavyArmor`:**
  `OncePerTurn` and `RequiresFinesseOrRangedWeapon` are both real,
  simple, always-true facts read straight from the feature text
  ("Once per turn... The attack must use a finesse or a ranged
  weapon"). `ClassDefinitionValidator` additionally checks every
  grant shares the same die size, since a Sneak Attack progression
  that suddenly switched from d6 to d8 mid-table would be a data
  error, not a legitimate variant.
- **Deliberately left unmodeled: the alternative-to-advantage trigger**
  ("you don't need advantage... if another enemy of the target is
  within 5 feet of it, that enemy isn't incapacitated, and you don't
  have disadvantage") — unlike `RequiresNotWearingHeavyArmor`, this
  is compound combat-state prose (position, a target's own condition,
  the attacker's own roll state), not a single static fact a data
  file can hold without turning into a small rules engine. It stays
  inside the existing `dnd5e2014.class-rule.sneak-attack` citation,
  matching the same boundary Rage drew around Persistent Rage.
- Public API: one new public type
  (`SneakAttackProgressionDetail`, plus its nested
  `SneakAttackDiceGrant`) and one new member on `ClassDefinition`.
  Full gate green: Debug+Release build 0 warnings, 1301 tests (was
  1282; +19 new).
**Divine Strike converted sixth — the predicted "one to watch" case,
confirmed exactly as predicted.** All five Cleric domains that grant it
(Life, Nature, Tempest, Trickery, War) share the identical "1d8 at 8th,
2d8 at 14th" template, but — read directly off the PHB (pages 60-63)
rather than assumed — the damage-type payload actually takes three
different shapes across the five, not one: a fixed type (Life/radiant,
Tempest/thunder, Trickery/poison), a per-attack choice among several
types (Nature/cold-fire-lightning), and a type that matches whatever
weapon dealt the hit (War). This is a new shape this pass hadn't hit
yet — Rage/Sneak Attack's own damage-type field (`ResistedDamageTypeIds`)
is always a fixed, ID-referenced set, never a choice or a
match-the-weapon rule.

- **Lives on `SubclassDefinition`, not `ClassDefinition`** — the first
  quantized progression embedded at the subclass level rather than the
  class level, since Divine Strike is granted per-domain, not by
  Cleric itself. `Rules/Classes/DivineStrike/` follows the same
  five-piece-minus-catalog shape as `Rules/Classes/Rage/` and
  `Rules/Classes/SneakAttack/` (a plain embedded value object mapped
  inline by `DivineStrikeProgressionDetailDataMapper`, no top-level
  catalog) for the same reason Rage skipped one: nothing shares this
  table across classes, only across five subclasses of the same class,
  and even that sharing is at the template level only, not the data.
- **Three typed, mutually-exclusive damage-type fields, not an enum +
  payload DSL** — `FixedDamageTypeId`/`ChoosableDamageTypeIds`/
  `MatchesWeaponDamageType` sit side by side on
  `DivineStrikeProgressionDetail`, the same "several typed nullable
  mechanism fields, exactly one populated" shape
  `FightingStyleDefinition` already established for its own five
  mechanisms, rather than reusing `RageProgressionDetail`'s plain
  `IReadOnlyList<DamageTypeId>` (which only ever means "resists all of
  these," never "pick one" or "match the weapon"). Reusing Rage's shape
  here would have silently misrepresented Nature's and War's mechanics.
  `SubclassDefinitionValidator` enforces exactly one of the three is
  set, at least two options when the choosable list is used, and the
  same ascending-count/same-die-size checks Sneak Attack's own
  `DiceByLevel` already established.
- **Every damage type reused already-catalogued `DamageTypeId`s**
  (radiant, cold, fire, lightning, thunder, poison — all pre-existing
  from the Phase 11 creature-vocabulary pass), cross-referenced by
  `ClassCatalogIntegrityValidator` the same way Rage's
  `ResistedDamageTypeIds` already are; no new damage type needed.
- Public API: one new public type (`DivineStrikeProgressionDetail`,
  plus its nested `DivineStrikeDamageGrant`) and one new member on
  `SubclassDefinition`. Full gate green: Debug+Release build 0
  warnings, 1317 tests (was 1301; +16 new).

**Ki (Monk) and Sorcery Points (Sorcerer) converted seventh and
eighth, together in one pass** — the first two "other per-level
numeric progressions" from the remaining list, picked together because
reading both PHB tables side by side (pages 77-78 and 100-101) showed
they're numerically identical (`points = character level`, granted
starting at 2nd level, every level 2-20) despite fueling completely
unrelated mechanics (Ki's martial-arts trio vs. Font of Magic's spell
slot conversion). That coincidence was explicitly treated as a
non-reason to share a type — see the cross-class `RuleId`-sharing
discipline in "Classes" above (a recurring template/number is never by
itself evidence for or against sharing; only the actual mechanic
matters) — and reading the full prose confirmed the mechanics
genuinely differ: Ki recovers on a short **or** long rest, Sorcery
Points only on a long rest. Sharing a type would have erased that
difference or forced an awkward per-instance override.

- **Two new sibling domains, `Rules/Classes/Ki/` and
  `Rules/Classes/SorceryPoints/`**, each the same "plain embedded value
  object, no catalog" shape as Rage/Sneak Attack — both are strictly
  single-class, so there was never a referenced-by-ID case to make.
  Each holds a dense `PointsByLevel` list (one grant per character
  level, 2 through 20 — 19 entries) rather than a sparse breakpoint
  list like Rage's own `UsesByLevel`/`DamageBonusByLevel`, because the
  value genuinely changes at *every* level here, not just at a few
  milestone levels — a dense list is what "read directly off the
  table" honestly produces for this particular shape, the same
  all-20-levels-present precedent the spell slot progression table
  already established, without inventing a "points equal your level"
  formula field the data model doesn't otherwise support.
- **`RecoversOnShortRest` is a plain bool on each type, not a shared
  enum** — `SpellSlotRecoveryRest` (`LongRest`/`ShortOrLongRest`)
  already exists on the Spellcasting domain, but its name is
  spell-slot-specific and reusing it here would read as claiming Ki
  points *are* spell slots. Renaming/relocating it into a shared
  location was considered and rejected as an unnecessary breaking
  change to an already-shipped public type for a two-value distinction
  that a local bool states just as clearly, matching Rage's own choice
  not to generalize a "recovery timing" concept when it only ever
  needed long-rest.
- Public API: four new public types (`KiPointsGrant`,
  `KiProgressionDetail`, `SorceryPointsGrant`,
  `SorceryPointsProgressionDetail`) and two new members on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1356 tests (was 1317; +39 new).

**Wild Shape (Druid) and Circle Forms (Circle of the Moon) converted
ninth and tenth, together — the first pass to combine a class-level
progression with its own subclass-level override in one PR**, because
reading Wild Shape's own text made clear the two features only make
sense read together: Circle Forms explicitly modifies one column of
Wild Shape's own table and leaves the other alone.

- **Wild Shape's own leveled table (2nd/4th/8th) captures three
  columns, not just the max CR number** — `WildShapeFormLimit` is
  `(CharacterLevel, MaxChallengeRating, AllowsFlyingSpeed,
  AllowsSwimmingSpeed)`, since the Beast Shapes table's own
  "Limitations" column ("No flying or swimming speed" → "No flying
  speed" → no limitation) is just as much a leveled fact as the CR cap
  and was read off the same table row. **The use count itself is flat,
  not leveled** — "you can use this feature twice" is one constant
  (`UsesPerRest = 2`) for the whole class, confirmed by reading the
  full feature text rather than assumed from the "recurring numeric
  progression" framing every item on this list has carried so far; not
  everything under a leveled table heading is itself leveled.
- **`MaxChallengeRating` is a plain `double`, not a new
  `ChallengeRating` type** — only three literal values ever appear
  (1/4, 1/2, 1, later 2-6 for Circle Forms), all exactly representable
  in binary floating point, and no other domain in this codebase
  models challenge rating at all yet. Inventing a fraction/CR value
  type for two call sites would be generality ahead of real need, the
  same restraint already applied to `RecoversOnShortRest` staying a
  plain bool instead of a shared enum.
- **Circle Forms only stores its own CR override, not a duplicate copy
  of the flying/swimming limitations** — its gateway sentence explicit
  ("you ignore the Max. CR column of the Beast Shapes table, but must
  abide by the other limitations there") decomposes cleanly into two
  independent facts: a subclass-level CR-by-level list
  (`CircleFormsProgressionDetail.MaxChallengeRatingByLevel`) and the
  *already-modeled* base `WildShapeProgressionDetail.FormLimitsByLevel`
  for everything else, combined by a consumer rather than duplicated in
  data. This is the first quantized progression where a compound cross-
  reference between a class-level and subclass-level feature turned out
  to decompose cleanly instead of needing to stay citation-only (the
  Sneak Attack/Rage precedent for compound rules) — verified by reading
  both features' full text side by side before concluding it decomposed
  rather than assuming it would.
- **Circle Forms' own 6th-level breakpoint is a formula in the book**
  ("your druid level divided by 3, rounded down"), the first quantized
  progression built from a computed rule rather than a literal table
  row. Rather than store the formula, every level 6-20 was hand-computed
  (`floor(level/3)`) and only the levels where the *result* actually
  changes were kept as explicit grants (6→2, 9→3, 12→4, 15→5, 18→6) —
  the same "read the rule, expand it, store the resulting table"
  treatment Ki/Sorcery Points' dense per-level list already established,
  just applied to a sparser breakpoint result instead of a value that
  changes every level.
- Public API: four new public types (`WildShapeFormLimit`,
  `WildShapeProgressionDetail`, `CircleFormsChallengeRatingGrant`,
  `CircleFormsProgressionDetail`) and one new member apiece on
  `ClassDefinition`/`SubclassDefinition`. Full gate green: Debug+Release
  build 0 warnings, 1388 tests (was 1356; +32 new).

**Paladin auras converted eleventh — Aura of Protection/Aura of
Courage (Paladin core) and Aura of Devotion (Oath of Devotion)/Aura
of Warding (Oath of the Ancients).** All four share the identical
"10 feet, expands to 30 feet at 18th level" range progression — a
genuine case (not just superficial coincidence) since the class
table's own 18th-level row is literally one shared "Aura
improvements" placeholder covering both core auras at once (already
noted under "Classes" above), and both oath auras separately restate
the same 10-foot/30-foot-at-18th numbers in their own prose, verified
by reading all four side by side rather than assumed from the shared
template.

- **A shared `AuraRange` struct, embedded by value into four separate
  per-aura types** (`AuraOfProtectionDetail`/`AuraOfCourageDetail` on
  `ClassDefinition`; `AuraOfDevotionDetail`/`AuraOfWardingDetail` on
  `SubclassDefinition`) — the range progression is genuinely identical
  across all four, unlike Ki/Sorcery Points' coincidental numbers, so
  reusing one small struct for that one shared fact is honest rather
  than premature generalization; the four wrapper types stay separate
  because each aura's own effect differs in kind (a numeric saving-throw
  bonus vs. three different condition immunities/resistances).
- **A real asymmetry caught by reading all four auras' full text
  side by side, not assumed from the shared range template:** Aura of
  Protection ("You must be conscious to grant this bonus"), Aura of
  Courage, and Aura of Devotion (both "while you are conscious") all
  gate on the paladin being conscious — Aura of Warding's own text has
  no such clause at all. `RequiresConsciousness` is a plain bool on
  all four types specifically so this real difference is visible in
  the schema (`false` on Warding) rather than assumed uniformly `true`
  from the other three.
- **Three pre-existing PHB citation errors caught and fixed while
  reading the source pages for this pass** (not something this task
  set out to audit, but cheap to fix once found, matching the
  provenance discipline in "Architecture" above): the existing
  `dnd5e2014.class-rule.aura-of-devotion` citation said page 87, its
  actual page 86; `aura-of-warding` said page 88, actual page 87; the
  `dnd5e2014.subclass.oath-of-the-ancients` and
  `dnd5e2014.subclass.oath-of-vengeance` subclass-level citations were
  each off by one for the same reason (87→86, 88→87) — all four
  verified directly against the cleanly-scanned PDF's own page
  footers, not from memory, the same discipline every prior citation
  in this project has used.
- Public API: five new public types (`AuraRange`,
  `AuraOfProtectionDetail`, `AuraOfCourageDetail`,
  `AuraOfDevotionDetail`, `AuraOfWardingDetail`) and two new members
  apiece on `ClassDefinition`/`SubclassDefinition`. Full gate green:
  Debug+Release build 0 warnings, 1412 tests (was 1388; +24 new).

**Bardic Inspiration die converted twelfth — the first quantized
progression with no personal resource pool at all**, structurally
different from Ki/Sorcery Points despite a superficially similar "a
die/points that scales with level" shape: a Bardic Inspiration die is
granted to *another* creature via a bonus action and is simply gone
once rolled — there's no cap, no rest-recovery rule, nothing that
looks like `UsesPerRest`/`RecoversOnShortRest` on Ki/Sorcery Points,
so `BardicInspirationProgressionDetail` doesn't carry either field.

- **Die *size* increases (d6→d8→d10→d12), not die *count*** — the
  breakpoint list (1st/5th/10th/15th, read off the Bard table) is a
  `BardicInspirationDieGrant` list validated the mirror image of Sneak
  Attack's own check: count must stay constant (always 1) while sides
  strictly increase, rather than Sneak Attack's constant sides with
  increasing count. Confirmed both dimensions from the actual table
  rather than assuming "leveled die progression" always means the same
  thing Sneak Attack's did.
- **`RangeFeet` (60) and `DurationMinutes` (10) captured alongside the
  leveled die**, the same "capture the full mechanical fact set, not
  just the leveled number" call as Rage's `DurationMinutes`/Sneak
  Attack's `RequiresFinesseOrRangedWeapon` — both are flat, unleveled
  facts read directly from the feature's own prose ("within 60 feet of
  you," "within the next 10 minutes"), not part of the Bard table
  itself.
- **Song of Rest's own die-size progression (2nd/9th/13th/17th,
  visible in the same table column) was deliberately left untouched**
  by this PR — it's a separate named feature from Bardic Inspiration
  with its own citation, not a second column of the same mechanic, so
  quantizing it is a distinct future item, not folded in here just
  because it happened to be on the same page.
- Public API: two new public types (`BardicInspirationDieGrant`,
  `BardicInspirationProgressionDetail`) and one new member on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1434 tests (was 1412; +22 new).

**Channel Divinity uses converted thirteenth — Cleric's own core
uses-per-rest progression (2nd level → 1, 6th → 2, 18th → 3,
recovering on a short or long rest), verified against the Cleric's
own page 58 text rather than assumed from the "typical" 5e Channel
Divinity shape.** Reused the exact same `ValidatePointsProgression`
helper `ClassDefinitionValidator` already built for Ki/Sorcery Points
— `ChannelDivinityUseGrant` is a plain `(CharacterLevel, UsesPerRest)`
pair, the identical shape those two already use, so no new validation
logic was needed, just a new call site.

- **Paladin's own "Channel Divinity" was checked and confirmed to be
  a genuinely different case, not an oversight for leaving unquantized
  here.** Paladin has a same-named class-level framework (already
  folded into `sacred-oath`'s own citation with no separate `RuleId`,
  per the "Classes" section above), but its own text never scales —
  it stays a flat one use, recovering on a short or long rest, for the
  Paladin's entire career. A constant isn't a progression, so there
  was nothing to quantize; this was verified by re-reading Paladin's
  own Channel Divinity paragraph specifically to check for a "twice at
  level X" clause like Cleric's, not assumed from the shared name.
  `ChannelDivinityProgressionDetail` stays Cleric-only, unprefixed,
  since the only real progression that exists belongs to one class.
- Public API: two new public types (`ChannelDivinityUseGrant`,
  `ChannelDivinityProgressionDetail`) and one new member on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1455 tests (was 1434; +21 new).

**Mystic Arcanum converted fourteenth — Warlock's own spell-tier-by-
level progression (11th → 6th-level spell, 13th → 7th, 15th → 8th,
17th → 9th, each usable once per long rest), verified against page
108 rather than assumed from the well-known "one 6th/7th/8th/9th
level slot equivalent" 5e shape.** The fourth progression in this
pass to reuse `ClassDefinitionValidator`'s shared
`ValidatePointsProgression` helper without any new validation logic —
`MysticArcanumGrant` is the same `(CharacterLevel, Value)` shape
Ki/Sorcery Points/Channel Divinity already established, just with the
value being a spell level (1-9) instead of a resource count.

- **`RecoversOnShortRest` stays `false` here, the third distinct
  value in this field's short history** (Ki: `true`, Sorcery Points:
  `false`, Channel Divinity: `true`) — Mystic Arcanum's own text says
  only "You regain all uses of Mystic Arcanum when you finish a long
  rest," no short-rest option at all, confirmed by reading the actual
  sentence rather than assuming a warlock resource follows Pact
  Magic's own short-or-long-rest pattern from the spell slot pass.
  Every one of these four leveled-progression PRs has now landed on
  whichever value the text actually says, not a class-family default.
- Public API: two new public types (`MysticArcanumGrant`,
  `MysticArcanumProgressionDetail`) and one new member on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1476 tests (was 1455; +21 new).

**Font of Magic conversion converted fifteenth — the Creating Spell
Slots table (sorcery point costs 2/3/5/6/7 for spell slot levels 1-5),
verified against the same page 101 already read during the Sorcery
Points pass, closing out every item that was on the original "rest of
the per-level numeric progressions" list.**

- **The leveled axis is spell slot level, not character level — the
  first quantized progression where that's true**, which is exactly
  why it could *not* reuse the shared `ValidatePointsProgression`
  helper the four previous progressions (Ki, Sorcery Points, Channel
  Divinity, Mystic Arcanum) all shared without any new code: that
  helper's `ValidateAscending` hardcodes "character level" into its
  own error text. Reusing it here would have produced a validation
  message that called a spell slot level a character level — caught
  before writing the data, not after, by checking what the shared
  helper's messages actually said rather than assuming the shape match
  alone was enough. `ValidateFontOfMagicConversion` is a small bespoke
  method instead, mirroring the same "shared helper only when the
  domain vocabulary genuinely matches" restraint already implicit in
  why Wild Shape/Circle Forms/Divine Strike never tried to reuse it in
  the first place.
- **The reverse conversion (spell slot → sorcery points, 1:1 with the
  slot's own level) was deliberately not given a field of its own** —
  unlike every other "capture the full mechanical fact set" case in
  this pass, there's no independent number here: the point value *is*
  the slot's level, a fact already fully expressed by the concept of
  "spell slot level" itself, not a new leveled quantity like
  `RangeFeet` or `DurationMinutes` was for Rage/Bardic Inspiration. A
  field that always just echoes an existing axis back would be
  redundant structure, not a new fact.
- Public API: two new public types (`FontOfMagicSlotCostGrant`,
  `FontOfMagicConversionDetail`) and one new member on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1496 tests (was 1476; +20 new).

**Song of Rest converted sixteenth — Bard's own healing-die progression
(1d6 at 2nd level, 1d8 at 9th, 1d10 at 13th, 1d12 at 17th), the one
addition to the original per-level-numeric-progressions list, closing
it out for good.** Verified against the same page 54 already read for
Bardic Inspiration (both features are on that page, but are genuinely
separate mechanics with separate citations, not two columns of one
table — confirmed by reading each one's own paragraph rather than
assuming from proximity).

- **Deliberately simpler than Bardic Inspiration's own shape** — Song
  of Rest has no range or duration numbers in its own text at all (it
  triggers automatically for "any friendly creatures who can hear your
  performance" during a short rest, with no explicit feet or minutes
  given the way Bardic Inspiration's "within 60 feet" and "within the
  next 10 minutes" were), so `SongOfRestProgressionDetail` carries only
  `DieByLevel`, no `RangeFeet`/`DurationMinutes` fields. This is the
  "capture the full mechanical fact set" discipline working in the
  other direction from usual — the fact set here genuinely only has one
  member, confirmed by checking rather than assumed absent.
  `ClassDefinitionValidator`'s own `ValidateSongOfRestProgression` is a
  near-duplicate of `ValidateBardicInspirationProgression` (same
  constant-count/ascending-sides checks) rather than a shared helper,
  matching the same "domain-specific error wording matters" call Font
  of Magic's own validator just established — a shared helper here
  would only save a few lines and these two dice progressions could
  plausibly diverge in a later pass.
- Public API: two new public types (`SongOfRestDieGrant`,
  `SongOfRestProgressionDetail`) and one new member on
  `ClassDefinition`. Full gate green: Debug+Release build 0 warnings,
  1515 tests (was 1496; +19 new).
- **Every per-level numeric progression identified during this entire
  pass — the original list plus both items discovered along the way
  (Wild Shape/Circle Forms' compound pairing, Song of Rest) — is now
  converted.**

**Metamagic converted seventeenth, resolving the "Not yet decided" note
above by proving out the choice-point-catalog shape on the smallest of
the six remaining catalogs first — the same reason Fighting Style was
chosen to prove the original shape.** All 8 options read directly off
page 102 (Careful, Distant, Empowered, Extended, Heightened, Quickened,
Subtle, Twinned Spell) — the first genuinely new top-level catalog
domain since Fighting Style itself, not an embedded value object on
`ClassDefinition` like everything else in this pass, because these are
*named options a player chooses from*, the same vocabulary-catalog
shape Fighting Style already established, not a per-class leveled fact.

- **A brand-new ID namespace, `dnd5e2014.metamagic-option.*`, with no
  prior citation to reconcile against** — unlike every progression
  converted so far, the existing `dnd5e2014.class-rule.metamagic`
  citation never had per-option sub-IDs (all 8 options were folded into
  one citation, the same choice-point treatment Fighting Style's own
  6 options got before *it* was quantized). That gateway citation on
  Sorcerer's own `LevelFeatures` stays completely untouched — the new
  `MetamagicOptionCatalog` is an additional, independent catalog, the
  identical relationship Fighting Style's own catalog has to
  `dnd5e2014.class-rule.fighting-style`.
- **No `AvailableToClassIds` list, unlike Fighting Style** — Metamagic
  is Sorcerer-only, so every option is implicitly available to the one
  class that has the feature at all; adding a list that would always
  contain exactly one entry would be structure with no discriminating
  power, the same reasoning that kept `RequiresConsciousness` off
  Warding but *on* the other three auras — add a field only where it
  actually varies.
- **Cost representation needed two mutually-exclusive shapes, not one
  flat number, because Twinned Spell's own cost isn't fixed** — seven
  of the eight options cost a flat number of sorcery points (1, 1, 1,
  1, 3, 2, 1), but Twinned Spell costs "a number of sorcery points
  equal to the spell's level (1 sorcery point if the spell is a
  cantrip)" — a cost that depends on which spell is being cast, not a
  per-option constant. `FixedSorceryPointCost: int?` /
  `CostEqualsSpellLevelWithCantripMinimum: bool` sit side by side,
  validated exactly-one-populated, the same "several typed nullable
  mechanism fields" shape Fighting Style's own five mechanisms
  established — proving that shape generalizes to a genuinely
  different kind of mechanism (a cost formula, not an effect) on the
  very first catalog tried.
- **The individual spell effects themselves (double range, reroll
  damage, disadvantage on a save, change casting time, drop
  components, target a second creature) were deliberately left
  unquantized** — this is a real, considered scope cut, not an
  oversight: unlike Fighting Style's six options, which all reduced to
  the *same small family* of mechanisms (a roll bonus, an AC bonus, a
  reroll, a reaction) that a handful of shared types could honestly
  cover, Metamagic's 8 effects are individually heterogeneous with no
  shared shape between them. Modeling them would mean either inventing
  a bespoke mechanism type per option (defeating the point of a shared
  catalog schema) or building the general effect DSL this project has
  explicitly rejected since Fighting Style's own design note. The
  sorcery point *cost* is the number this whole pass has been about
  quantizing; the prose effect stays exactly where every other
  citation's prose already lives — nowhere in this domain's own data,
  implicitly still covered by the untouched gateway citation.
- Public API: five new public types (`MetamagicOptionId`,
  `MetamagicOptionDefinition`, `MetamagicOptionCatalog`, plus the
  `Dnd5e2014Ruleset.MetamagicOptions` catalog member and
  `RulesetDefinitionSet.MetamagicOptions` on the internal definition
  set) — `RulesetDefinitionSet`/`Dnd5e2014RulesetLoader`/
  `Dnd5e2014Ruleset`/`CatalogIntegrityValidator` all wired through the
  exact same pattern Fighting Style's own catalog established, new
  embedded resource `Data/dnd5e2014/metamagic-options.json`. Full gate
  green: Debug+Release build 0 warnings, 1548 tests (was 1515; +33
  new — `MetamagicFoundationTests`/
  `MetamagicOptionDefinitionLoaderTests`/`MetamagicOptionDataFileTests`,
  the same three-file convention every domain follows).
**Pact Boon evaluated eighteenth and deliberately declined — the first
choice-point catalog that turned out not to need a catalog at all.**
The pre-Metamagic note above predicted Pact Boon would need real
sub-structure (calling out "substantial, multi-paragraph mechanics"),
which was true of the *prose* but turned out not to translate into
leveled numeric data once the full text (PHB pages 107-108) was
actually read side by side, the same "verify before assuming"
discipline this whole pass has run on:

- **Pact of the Chain** (a smarter familiar, special forms, forgo an
  attack to let it attack) has no number to capture at all beyond
  ordinary combat rules already implied elsewhere.
- **Pact of the Blade** (conjure a melee weapon, treat it as magical,
  a 1-hour ritual to bind a different magic weapon to the pact) has a
  5-foot/1-minute dismissal trigger and a 1-hour ritual duration, but
  both are compound trigger conditions embedded in prose — the same
  "not a single static fact" boundary that already kept Sneak Attack's
  alternative-to-advantage trigger and Rage's Persistent Rage
  unquantized, applied consistently here.
- **Pact of the Tome** grants three chosen cantrips — a real, isolated
  number, but a flat one-time constant with nothing to progress by
  level, and no second data point anywhere in this domain to justify a
  field for it alone.
- None of the three costs anything, has a save DC, or scales with
  level — the actual dividing line this whole pass has used
  throughout (a *leveled numeric progression* worth extracting,
  the same bar Metamagic's per-option sorcery-point cost cleared and
  Sneak Attack's alternative-trigger prose didn't) isn't cleared by
  any of them. Building `PactBoonOptionId`/`Definition`/
  `Catalog`/loader/tests here would reproduce
  `dnd5e2014.class-rule.pact-boon`'s own three-option citation with no
  new mechanical information attached — the same "don't build
  generality ahead of real need" call Rage's own embedded-vs-catalog
  decision already established, just resolved here as "don't build
  *anything* new" rather than "build the smaller of two shapes." The
  existing citation is left completely untouched; no code changed for
  this item.
**Battle Master maneuvers converted nineteenth — the predicted "real
sub-structure" case, confirmed, plus a fourth new choice-point-catalog
shape and a genuine citation-page correction found along the way.**
Unlike Metamagic's single cost axis or Pact Boon's "nothing to
quantize" outcome, all 16 maneuvers (read directly off PHB page 74,
alphabetically from Commander's Strike through Trip Attack) share a
real, mechanically meaningful axis Metamagic's own effects never had:
*where the rolled superiority die value gets applied*. This is a
second, larger proof point for the same "several typed nullable
mechanism fields" restraint Fighting Style pioneered and Divine
Strike/Metamagic each reused — the axis just turned out to be a single
enum rather than several mutually-exclusive numeric fields, since a
maneuver only ever routes its die one way.

- **Two separate new domains, not one** — `Rules/Classes/
  BattleMasterManeuvers/` (a new top-level catalog, `dnd5e2014.
  battle-master-maneuver.*`, the sixteen named choice options) and
  `Rules/Classes/CombatSuperiority/` (an embedded value object on
  `SubclassDefinition`, Battle Master-only, no sharing candidate) — the
  same class-level-fact/named-option-list split Metamagic already drew
  against Sorcerer's own Font of Magic vs. Metamagic options, applied
  here to Battle Master's Combat Superiority framework (maneuvers
  known, superiority dice count, superiority die size — all three
  progress at different levels, so three separate grant lists, the
  same multi-independent-axis shape Rage's own uses/damage-bonus split
  already established) vs. its sixteen named maneuvers.
- **`BattleMasterManeuverEffectTarget`, a 6-value enum** (DamageRoll,
  AttackRoll, ArmorClass, DamageReduction, TemporaryHitPoints,
  SecondaryTargetDamage) **captures where each maneuver's die value is
  spent**, read directly off each maneuver's own sentence rather than
  assumed from its name — eleven of sixteen add the die to a damage
  roll, but Precision Attack adds it to the *attack* roll, Evasive
  Footwork adds it to AC, Parry uses it as damage reduction against an
  incoming hit, Rally grants it as temporary hit points to an ally, and
  Sweeping Attack deals it as a wholly separate damage instance to a
  second nearby creature. `RequiresSavingThrow: AbilityId?` sits
  alongside it — five of sixteen (Disarming/Pushing/Trip Attack force a
  Strength save; Goading/Menacing Attack force a Wisdom save) — the
  same "each maneuver either has this fact or doesn't" nullable shape
  every other domain in this pass already uses.
- **Deliberately left unquantized, matching Metamagic's own precedent
  exactly:** each maneuver's actual secondary effect (disarm, frighten,
  knock prone, push up to 15 feet, extend reach by 5 feet, redirect an
  ally's reaction, grant advantage, restore an ally to half speed
  movement) — compound, per-maneuver prose with no shared shape across
  all sixteen, the same reasoning that kept Metamagic's 8 individual
  spell effects out of scope. The shared *save DC formula itself* (8 +
  proficiency bonus + Strength or Dexterity, the wielder's choice) also
  stays unquantized, consistent with every other class's own save-DC
  formula never being modeled in this pass (Ki, Sorcery Points, and
  every other resource with a save DC all leave the DC formula to the
  citation) — what's captured here is only the two facts that vary
  *per maneuver*, not the one formula shared by all of them.
- **A genuine citation-page correction, the same category of finding
  as the four caught during Paladin auras.** Reading PHB pages 73-75
  directly for this conversion found `combat-superiority`/
  `student-of-war`/`know-your-enemy` (all cited at page 74) actually
  start on page 73, and `improved-combat-superiority`/`relentless`
  (both cited at page 75) actually land on page 74 — a consistent
  off-by-one across all five `class-rule.json` entries plus the Battle
  Master subclass's own citation in `subclasses.json` (also corrected
  from 74 to 73). All six fixed in the same commit as the new domain,
  the same "fix it where you find it" precedent the auras pass set.
- Public API: two new public types per new domain
  (`BattleMasterManeuverId`/`Definition`/`EffectTarget`/`Catalog`, plus
  `CombatSuperiorityManeuversKnownGrant`/`DiceCountGrant`/
  `DieSizeGrant`/`ProgressionDetail`) and one new member each on the
  already-public `Dnd5e2014Ruleset`/`RulesetDefinitionSet` (the new
  catalog) and `SubclassDefinition` (the new embedded progression).
  `RulesetDefinitionSet`/`Dnd5e2014RulesetLoader`/`Dnd5e2014Ruleset`/
  `CatalogIntegrityValidator` wired through the exact same pattern
  Fighting Style's and Metamagic's own catalogs established (including
  a missing-`AbilityId` cross-reference check on
  `SavingThrowAbilityId`, the same shape `RequiresAllPrimaryAbilities`
  reference-checking already uses elsewhere), new embedded resource
  `Data/dnd5e2014/battle-master-maneuvers.json`. Full gate green:
  Debug+Release build 0 warnings, 1589 tests (was 1548; +41 new —
  `BattleMasterManeuverFoundationTests`/
  `BattleMasterManeuverDefinitionLoaderTests`/
  `BattleMasterManeuverDataFileTests` for the new catalog, plus
  Combat Superiority assertions folded into the existing
  `SubclassDataFileTests`/`SubclassDefinitionLoaderTests`/
  `SubclassFoundationTests`).
**Eldritch Invocations converted twentieth — by far the largest single
choice-point catalog quantized so far, and the first with a real,
independent class-level "known count" progression sitting alongside
it.** CLAUDE.md's own earlier estimate ("~20 options") undershot the
real PHB list badly: reading pages 110-111 directly found 32 named
invocations, not ~20 — another instance of the standing "verify
against the real text, not a remembered/estimated count" discipline
paying for itself. Two separate new domains, the same class-level/
choice-point split Battle Master's own pass established:

- **`Rules/Classes/EldritchInvocationsKnown/`** (embedded on
  `ClassDefinition`, Warlock-only) captures the Warlock table's own
  "Invocations Known" column as a sparse breakpoint list — 2nd→2,
  5th→3, 7th→4, 9th→5, 12th→6, 15th→7, 17th→8 — reusing
  `ValidatePointsProgression` directly (a character-level, monotonic
  count, exactly the shape that helper was built for). The table's own
  further clause ("you can choose one of the invocations you know and
  replace it with another... that you could learn at that level") is
  a swap mechanic, not a number, and stays citation-only, matching
  Metamagic's and Mystic Arcanum's own re-choice clauses.
- **`Rules/Classes/EldritchInvocations/`** (a new top-level catalog,
  `dnd5e2014.eldritch-invocation.*`, all 32 named options) captures
  each invocation's prerequisites as three independent, non-exclusive
  facts — `RequiresEldritchBlastCantrip: bool` (3 invocations),
  `RequiredMinimumLevel: int?` (13), and `RequiresPactBoon:
  WarlockPactBoon?` (5, three of which *also* carry a level) — rather
  than Battle Master's single mutually-exclusive enum, since a real
  invocation can and does stack more than one prerequisite type at
  once (Chains of Carceri needs both 15th level *and* Pact of the
  Chain). 11 of the 32 have no prerequisite at all.
- **`WarlockPactBoon`, a new 3-value enum (`Chain`/`Blade`/`Tome`),
  exists specifically because Pact Boon itself has no catalog to
  reference.** Pact Boon was deliberately declined as its own domain
  (see above) since none of its three options carried a leveled
  numeric fact — but five Eldritch Invocations still need to name
  *which* Pact Boon option they require, a real, checkable fact this
  pass is scoped to capture regardless of whether the thing being
  referenced got its own ID type. A small enum local to this domain
  was the honest fix, not a reason to retroactively build the Pact
  Boon catalog after all — the two decisions don't conflict, they
  answer different questions (does Pact Boon itself have quantifiable
  data of its own vs. does something else need to reference *which*
  Pact Boon option was chosen).
- Public API: two new public types per new domain
  (`EldritchInvocationId`/`Definition`/`Catalog`, plus
  `WarlockPactBoon`; `EldritchInvocationsKnownGrant`/
  `ProgressionDetail`) and one new member each on the already-public
  `Dnd5e2014Ruleset`/`RulesetDefinitionSet` (the new catalog) and
  `ClassDefinition` (the new embedded progression), wired through the
  same pattern every prior catalog/progression in this pass used. Full
  gate green: Debug+Release build 0 warnings, 1637 tests (was 1589;
  +48 new — `EldritchInvocationFoundationTests`/
  `EldritchInvocationDefinitionLoaderTests`/
  `EldritchInvocationDataFileTests` for the new catalog, plus Invocations
  Known assertions folded into the existing `ClassDataFileTests`).
**Elemental Disciplines converted twenty-first — Way of the Four
Elements' own choice-point catalog, and a second confirmation that
CLAUDE.md's own remembered option counts can't be trusted without
re-reading the page.** This section's own earlier text (see "Classes"
above) called it "18 individual elemental disciplines"; reading PHB
page 81 directly found 17, not 18 — a smaller discrepancy than
Eldritch Invocations' 32-vs-~20 gap, but the same lesson repeated
immediately after being learned: recount every time, regardless of
how confident an existing in-repo number looks. The same
catalog/class-level-progression split every subclass-level choice
point in this pass has used again:

- **`Rules/Classes/ElementalDisciplines/`** (a new top-level catalog,
  `dnd5e2014.elemental-discipline.*`, all 17 named disciplines from
  page 81) captures each discipline's `KiPointCost: int?` and
  `RequiredMinimumLevel: int?` — a simpler two-axis shape than
  Eldritch Invocations' three, since Monk disciplines have no
  cantrip-style or Pact-Boon-style gate to represent. 9 of the 17
  require 6th/11th/17th level; Elemental Attunement alone has neither
  a cost nor a level requirement (it's the one discipline granted
  automatically, not chosen). **Which spell each discipline casts
  (`burning hands`, `fireball`, `wall of stone`, ...) was deliberately
  left unquantized** — this project doesn't model spells as a domain
  at all, the same boundary that already kept Domain Spells/Circle
  Spells/Oath Spells and every caster's own spell list out of
  structured data; a discipline's spell reference has nowhere valid to
  point without inventing a `SpellId` type this pass isn't scoped to
  create.
- **`Rules/Classes/DiscipleOfTheElements/`** (embedded on
  `SubclassDefinition`, Way of the Four Elements only) captures two
  independent leveled facts read off the same page-80/81 spread as the
  feature's own prose: `DisciplinesKnownByLevel` (3rd→2 — Elemental
  Attunement plus one chosen discipline — then +1 at 6th/11th/17th,
  same recurring-choice-count shape Fighting Style's own "additional
  option" grants never needed to quantize but this pass's later,
  numbers-first entries do) and `MaxKiPointsPerSpellByLevel`, the
  class's own "Spells and Ki Points" table (5th→3, 9th→4, 13th→5,
  17th→6) governing how many extra ki points can upgrade a
  discipline's spell level — a real leveled table sitting right next
  to the discipline list that would have been easy to skip past as
  "just more prose" without reading the full page closely.
- Public API: two new public types per new domain
  (`ElementalDisciplineId`/`Definition`/`Catalog`;
  `DiscipleOfTheElementsDisciplinesKnownGrant`/`MaxKiPointsGrant`/
  `ProgressionDetail`) and one new member each on the already-public
  `Dnd5e2014Ruleset`/`RulesetDefinitionSet` (the new catalog) and
  `SubclassDefinition` (the new embedded progression), wired through
  the same pattern every prior catalog/progression in this pass used.
  Full gate green: Debug+Release build 0 warnings, 1681 tests (was
  1637; +44 new — `ElementalDisciplineFoundationTests`/
  `ElementalDisciplineDefinitionLoaderTests`/
  `ElementalDisciplineDataFileTests` for the new catalog, plus Disciple
  of the Elements assertions folded into the existing
  `SubclassDataFileTests`/`SubclassDefinitionLoaderTests`/
  `SubclassFoundationTests`).
**Channel Divinity options converted twenty-second — the sixth and
last choice-point catalog, closing out that entire sub-project.** All
10 named options across Cleric's 7 domains (Knowledge of the Ages,
Read Thoughts, Preserve Life, Radiance of the Dawn, Charm Animals and
Plants, Destructive Wrath, Invoke Duplicity, Cloak of Shadows, Guided
Strike, War God's Blessing — the same 10 CLAUDE.md's own "Cleric"
section already enumerated by name, so no count correction needed
this time, unlike Eldritch Invocations/Elemental Disciplines)  were
read directly off PHB pages 59-63. This is the *sixth* distinct
catalog shape this sub-project has produced, confirming the standing
rule ("verify every catalog's real shape, never assume a prior one
transfers") one more time:

- **Four independent, non-exclusive nullable facts** — `RangeFeet:
  int?`, `SavingThrowAbilityId: AbilityId?`, `DurationMinutes: int?`,
  `RollBonus: int?` — cover every option that reduces to a clean
  single number: Read Thoughts and Charm Animals and Plants each carry
  three of the four at once (range + save + duration); Guided Strike
  and War God's Blessing share the same flat `+10` roll bonus (the
  same accuracy-boost mechanic Fighting Style's own `RollBonus`
  shape already modeled, arrived at independently here rather than
  reused directly, since these are a different domain's own types);
  Destructive Wrath and Cloak of Shadows have **no** quantizable fact
  among the four and validate with all four null — the catalog still
  carries both as complete entries (Id/Name/Sources only) rather than
  omitting them, since the catalog's job is completing the option
  list, not that every entry contributes a number, the same reasoning
  Battle Master's own maneuver catalog and Metamagic's own option list
  already established.
- **Deliberately left unquantized: Preserve Life's "5 × cleric level"
  healing formula and Radiance of the Dawn's "2d10 + cleric level"
  damage formula** — both are linear-in-level formulas, a genuinely
  different shape from every leveled-breakpoint table this pass has
  captured elsewhere (Rage, Divine Strike, Combat Superiority, ...),
  and inventing a bespoke coefficient field for two options out of ten
  would have been exactly the kind of one-off complexity this pass has
  consistently avoided (Metamagic's 8 spell effects, Sneak Attack's
  alternative-advantage trigger, Pact of the Blade's dismissal
  condition all drew the same line). `RangeFeet` was still captured
  for both, since the range itself is a clean, simple, independent
  fact even though the damage/healing amount it applies to is not.
- Public API: one new public type set
  (`ChannelDivinityOptionId`/`Definition`/`Catalog`) and one new member
  each on the already-public `Dnd5e2014Ruleset`/`RulesetDefinitionSet`
  — no `ClassDefinition`/`SubclassDefinition` change this time, since
  (unlike every other choice point in this pass) Channel Divinity's
  own class-level "uses per rest" progression was already quantized
  back in PR #32, well before this sub-project of catalogs began; this
  PR only needed the options list itself. `CatalogIntegrityValidator`
  wired through the same pattern as every prior catalog (source
  validation plus a missing-`AbilityId` cross-reference check on
  `SavingThrowAbilityId`). Full gate green: Debug+Release build 0
  warnings, 1718 tests (was 1681; +37 new —
  `ChannelDivinityOptionFoundationTests`/
  `ChannelDivinityOptionDefinitionLoaderTests`/
  `ChannelDivinityOptionDataFileTests`).

**All six choice-point catalogs are now resolved, closing that
sub-project of the "Quantized mechanics" pass.** The six outcomes,
for reference the next time a similar choice point needs sizing up:
Fighting Style (the pattern's own proof case — typed mechanism
fields), Metamagic (cost-only), Pact Boon (verified and declined — no
catalog needed), Battle Master maneuvers (effect-target +
save-ability), Eldritch Invocations (three independent prerequisite
facts + a class-level known-count progression), Elemental Disciplines
(cost/level + a class-level disciplines-known-and-max-ki progression),
Channel Divinity options (four independent nullable facts, no
class-level companion needed). No two of the six ended up with an
identical shape — the standing lesson of this whole sub-project is
that a "choice point" is a *category* of feature, not a fixed
template, and the actual PHB text has to be read every single time to
find out which facts are real.

**Race traits converted twenty-third — the first quantization pass
over the Races domain, and it surfaced a genuine pre-existing bug
along the way, not just new content.** Read directly off PHB pages
20-43 (Dwarf/Hill Dwarf, Elf/High Elf/Wood Elf/Drow, Dragonborn), this
pass covers the real leveled/scalar numeric facts hiding inside
`RaceDefinition.TraitRuleIds`/`SubraceDefinition.TraitRuleIds`
citations — the same restrained selection discipline as every prior
item in this pass: capture what's a clean single number, leave
compound or non-modeled content (granted-spell names, since spells
aren't a domain here; Draconic Ancestry's own damage-type table,
already declared out of scope) in the citation.

- **Four new bare nullable/list fields directly on `RaceDefinition`
  and/or `SubraceDefinition`, no new wrapper types** — `int?
  DarkvisionRangeFeet` (60 for the six races that have it: Dwarf, Elf,
  Gnome, Half-Elf, Half-Orc, Tiefling; a `SubraceDefinition` override
  of 120 for Drow's own Superior Darkvision), `IReadOnlyList<DamageTypeId>
  ResistedDamageTypeIds` (Dwarf/Stout Halfling → poison, Tiefling →
  fire; Dragonborn's own resistance is deliberately left empty since
  it depends on the declined-to-model Draconic Ancestry choice), `int?
  TranceDurationHours` (Elf only, 4), and `int? HitPointBonusPerLevel`
  (Hill Dwarf's own Dwarven Toughness, +1). Bare fields rather than
  dedicated value-object types, since each is a single independent
  scalar shared by at most a handful of races/subraces — the same
  "don't build a wrapper type for one number" restraint
  `FightingStyleDefinition.ArmorClassBonus` and
  `ChannelDivinityOptionDefinition`'s own four scalar fields already
  established.
- **`Rules/Creatures/Races/BreathWeapon/`, a new embedded value object
  on `RaceDefinition` (Dragonborn only)**, is the one race trait with
  real sub-structure: a leveled damage-dice progression (1st→2d6,
  6th→3d6, 11th→4d6, 16th→5d6) plus `RecoversOnShortRest: bool`, the
  same shape as every other single-class leveled progression in this
  pass (Rage, Sneak Attack, Combat Superiority, ...). The breath
  weapon's actual damage *type* and shape (cone vs. line) still stay
  fully unquantized, since both depend on the Draconic Ancestry choice
  this project already decided not to model — the dice-count
  progression itself doesn't depend on which ancestry was chosen, so
  it was safe to capture without reopening that boundary.
- **A genuine pre-existing bug found and fixed, not introduced by this
  pass: Wood Elf's own Fleet of Foot speed override (35 feet) was
  never populated in `subraces.json`, even though
  `SubraceDefinition.Speed` already existed specifically for this
  purpose and a test (`CanonicalFile_OnlyWoodElfOverridesSpeed`,
  present since the original Races commit) already asserted the
  correct value.** The test had been silently red since the Races
  domain was first built — worth flagging because it's a reminder that
  "the test exists" and "the test passes" are different claims, and
  gate-running discipline only catches the gap if someone actually
  runs the suite and reads a failure rather than assuming green.
  Fixed in the same commit as everything else in this pass, verified
  directly against the PHB's own "Fleet of Foot. Your base walking
  speed increases to 35 feet" text rather than just trusting the
  pre-existing test's expected value blindly.
- Public API: one new public type set (`BreathWeaponProgressionDetail`
  plus `BreathWeaponDamageGrant`) and four new members each split
  across `RaceDefinition`/`SubraceDefinition` (the two darkvision
  fields, two resisted-damage-type fields, trance duration, hit point
  bonus, plus the one breath weapon progression). `RaceCatalogIntegrityValidator`
  gained a `damageTypeIds` parameter and cross-reference check for
  `ResistedDamageTypeIds`, the same missing-reference pattern every
  other domain's own catalog integrity check already uses. Full gate
  green: Debug+Release build 0 warnings, 1735 tests (was 1718; +17
  new — validator-rejection and data-file assertions folded into the
  existing `RaceFoundationTests`/`RaceDefinitionLoaderTests`/
  `RaceDataFileTests`/`SubraceFoundationTests`/
  `SubraceDefinitionLoaderTests`/`SubraceDataFileTests`, no new test
  files needed since Breath Weapon has no catalog of its own).

- **Remaining, tracked but not started:** a background audit (likely
  little to quantize — most background features are narrative/social,
  not numeric). This is the last item on the "Quantized mechanics"
  remaining list.

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
separate planning doc. As of Phase 25: equipment (weapons, armor, shields,
adventuring gear, tools, mounts, vehicles, trade goods), expenses
(lifestyles, food & drink, hospitality, mundane services), creature
vocabulary (abilities, skills, languages, sizes, conditions, damage types,
senses, alignments), races (all 9 PHB races plus all 9 subraces — see
"Races" above), **Classes (complete)**, and **Backgrounds (complete —
all 13 PHB backgrounds, see "Backgrounds" above)** are done. All 12 PHB classes
and all 40 of their subclasses/archetypes/domains/traditions/oaths/
patrons/circles/origins are built — Fighter (Champion, Battle Master,
Eldritch Knight), Barbarian (Path of the Berserker, Path of the Totem
Warrior), Monk (Way of the Open Hand, Way of Shadow, Way of the Four
Elements), Rogue (Thief, Assassin, Arcane Trickster), Bard (College of
Lore, College of Valor), Wizard (all 8 schools), Cleric (all 7 domains),
Warlock (the Archfey, the Fiend, the Great Old One), Druid (Circle of
the Land, Circle of the Moon), Ranger (Hunter, Beast Master), Paladin
(Oath of Devotion, Oath of the Ancients, Oath of Vengeance), and
Sorcerer (Draconic Bloodline, Wild Magic). See "Classes" above for the
full build-by-build reasoning trail; the short version of what twelve
classes' worth of evidence settled:

- **RuleId sharing is never assumed, always verified, against every
  previously-built class** — not just the most recently built one.
  `Land's Stride` (Ranger/Druid) and `Timeless Body` (Druid/Monk) were
  both discovered against classes separated by one or more intervening
  PRs, not neighbors. A single dropped word (Paladin's Fighting Style)
  was treated as a real difference, not scan noise, applying the same
  standard as a deliberate wording change (College of Valor's Extra
  Attack) — the two failure directions aren't symmetric, so uncertainty
  resolves toward keeping citations separate, not merging them.
- **A recurring template name is not by itself evidence for or against
  sharing.** Wizard's 8 school `Savant` features and Cleric's 5-way
  `Divine Strike` share an identical template but differ in the actual
  substituted content (which school, which damage type) and stayed
  split; Cleric's `Potent Spellcasting` (Knowledge/Light) and Tempest/
  War's `Bonus Proficiencies` were verified character-for-character
  identical and were shared. The only question that matters is whether
  the cited text is actually the same, every time, regardless of how
  the name pattern looks going in.
- **A choice point (Fighting Style, Eldritch Invocations, Pact Boon,
  Metamagic, Hunter's/Beast Master's own sub-features) always folds
  into one citation**, independent of how many options it offers (6 to
  20+) or how much text any individual option carries (one-liners to
  ~200-word mechanics like Pact of the Blade) — the deciding factor is
  the shape (a single named gateway offering sub-choices), never size.
- **A class's own core spellcasting citation is always separately
  prefixed per class** (never shared, even between two full casters),
  and never assumed to literally be table-named "Spellcasting" — Warlock
  calls it "Pact Magic." Two classes (Warlock, Sorcerer) have no Ritual
  Casting subsection at all, a real absence rather than an oversight
  each time.
- **A framework heading only earns its own `RuleId` if it has its own
  table row.** Cleric's `Channel Divinity` is its own 2nd-level row and
  kept a citation; Paladin's `Channel Divinity`/`Oath Spells` framework
  text is not a table row and folded into `Sacred Oath` instead. The
  same logic decided which recurring mechanics got repeated
  `LevelFeatures` entries (Wild Shape, Circle Forms, Aura of Protection/
  Courage) versus which stayed single-citation with the scaling left in
  prose (the oath-specific auras' own 18th-level range increase).
- **Real spell-list content (Domain Spells, Circle Spells, Oath Spells)
  never becomes structured spell references** — it stays inside
  whichever citation the surrounding feature already uses, consistent
  with spells not being a modeled domain yet.
- Two forward-references written during early builds both closed out
  correctly: Fighter's own note anticipating "Druid's nonmetal-only
  restriction" (stayed unmodeled, same category as unmodeled tool
  choices) and Bard's citation-precision caveat (resolved once a
  cleanly-scanned PDF surfaced, see the Bard section above).
- Two test patterns that would have kept growing a near-duplicate
  assertion per class — `ChosenAtLevel` (now 1st/2nd/3rd depending on
  class) and the shared-Ability-Score-Improvement class list — were each
  generalized into a single data-driven test keyed by class ID rather
  than accreting further one-offs; both absorbed the last several
  classes' worth of additions as one-line changes.

Backgrounds turned out to be the simplest domain built so far — no
ability scores, no level-gated features, no cross-class sharing question
at all (all 13 feature names were unique on the first pass) — but it
did surface one genuinely new find: **every one of the 13 backgrounds
grants exactly two fixed skill proficiencies, never a choice**, verified
across all 13 rather than assumed from the first few, which is why
`SkillProficiencyIds` is validated as an exact-two field instead of a
choice-count/option-list pair the way class skill choices are. Tool
proficiencies and starting equipment leaned on the already-established
unmodeled-gap precedent harder than any class did (nearly every
background has one or both), confirming that gap generalizes cleanly
rather than needing a background-specific carve-out.

Use the cleanly-scanned PHB PDF (`~/Downloads/Player's Handbook.pdf`,
reliable per-page footers) for any future citation work in this
document — not the archive.org OCR export used for the original Bard
pass (see "Classes" above, end of the Bard section, for why that
matters). Not yet started: spells, magic items, and combat/adventuring
rule prose beyond the existing rules citation index
(`Data/dnd5e2014/rules/`, split per-domain — see "Architecture" above).
Feats (and, by extension, Variant Human) are out of scope — they aren't
part of the free 2014 SRD this project's provenance model is built
around.

**"Complete" above means citation-complete, not mechanically
quantized.** Read "Quantized mechanics" below before assuming a class,
race, or background feature exposes real numbers rather than a page
reference — as of this writing, only Fighting Style, spellcasting slot
tables/abilities, Extra Attack, Rage, Sneak Attack, Divine Strike, Ki,
Sorcery Points, Wild Shape, Circle Forms, the Paladin auras, Bardic
Inspiration, Channel Divinity uses, Mystic Arcanum, Font of Magic
conversion, Song of Rest, and Metamagic have been converted; every
other named feature across Classes/Races/Backgrounds is still a
`RuleId` citation
with no
mechanical payload.

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
