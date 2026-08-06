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
separate planning doc. As of Phase 24: equipment (weapons, armor, shields,
adventuring gear, tools, mounts, vehicles, trade goods), expenses
(lifestyles, food & drink, hospitality, mundane services), creature
vocabulary (abilities, skills, languages, sizes, conditions, damage types,
senses, alignments), races (all 9 PHB races plus all 9 subraces — see
"Races" above), and **Classes (complete)** are done. All 12 PHB classes
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

Use the cleanly-scanned PHB PDF (`~/Downloads/Player's Handbook.pdf`,
reliable per-page footers) for any future citation work in this
document — not the archive.org OCR export used for the original Bard
pass (see "Classes" above, end of the Bard section, for why that
matters). Not yet started: backgrounds, spells, magic items, and
combat/adventuring rule prose beyond the existing rules citation index
(`Data/dnd5e2014/rules/`, split per-domain — see "Architecture" above).
Feats (and, by extension, Variant Human) are out of scope — they aren't
part of the free 2014 SRD this project's provenance model is built
around.

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
