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
- **Citation precision is coarser here than for the first four classes.**
  Earlier classes' page citations were verified against a cleanly paginated
  PHB PDF with reliable per-page footers. The PDF available for this pass
  is an archive.org full-text OCR export whose embedded page-footer digits
  are largely missing or corrupted; the class's overall page range (52–55)
  was instead anchored against a separate, cleanly-extracted table of
  contents (confirming Bard starts the PHB page immediately after Barbarian's
  own citations end, and Cleric starts immediately after), with individual
  feature-to-page assignments within that range reconstructed from content
  flow rather than a literal per-feature footer read. Treat Bard's citations
  as range-accurate but not independently re-verified at the same
  per-paragraph precision as Fighter/Barbarian/Monk/Rogue's; re-check
  against a cleaner scan if a consumer ever needs that precision.

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
separate planning doc. As of Phase 17: equipment (weapons, armor, shields,
adventuring gear, tools, mounts, vehicles, trade goods), expenses
(lifestyles, food & drink, hospitality, mundane services), creature
vocabulary (abilities, skills, languages, sizes, conditions, damage types,
senses, alignments), and races (all 9 PHB races plus all 9 subraces — see
"Races" above) are complete. Classes is started but far from complete:
Fighter (all 3 subclasses — Champion, Battle Master, Eldritch Knight),
Barbarian (both subclasses — Path of the Berserker, Path of the Totem
Warrior), Monk (all 3 subclasses — Way of the Open Hand, Way of Shadow,
Way of the Four Elements), Rogue (all 3 subclasses — Thief, Assassin,
Arcane Trickster), and Bard (both subclasses — College of Lore, College
of Valor) are built — see "Classes" above, including the cross-class
`RuleId` sharing question, resolved with evidence from all five (most
recently: Ability Score Improvement confirmed a third time verbatim
against Bard's; Expertise turned out to be a genuine Bard/Rogue name
collision, not a share, retroactively prefixing Rogue's previously-bare
entry the same way Unarmored Defense's collision was handled). Bard is
also the first full spellcaster built, confirming ahead of schedule that
a class's own core `Spellcasting` feature reduces to one citation the
same way Eldritch Knight's and Arcane Trickster's subclass-only
spellcasting already did. The other 7 PHB classes (Cleric, Druid,
Paladin, Ranger, Sorcerer, Warlock, Wizard) are not yet built — note that
all of them are full or half spellcasters like Bard, so the core-
`Spellcasting`-as-single-citation approach carries forward directly; when
picking one up, re-derive RuleId cross-class-sharing decisions against
the precedent in "Classes" above (default to sharing a generic-named
mechanic like Ability Score Improvement/Extra Attack/Evasion unless the
actual PHB text diverges; prefix on a name collision with a different
mechanic, like Unarmored Defense/Expertise/Bonus Proficiencies) rather
than assuming it's settled for good — re-verify by reading the real text,
the same discipline that's paid off at both the class level and the
individual-feature level so far. Bard's own page citations were
reconstructed from a noisier PDF source than earlier classes (see
"Classes" above, end of the Bard section) — worth re-verifying against a
cleaner scan before treating them as paragraph-precise. Not yet started:
backgrounds, spells, magic items, and combat/adventuring rule prose
beyond the existing rules citation index (`Data/dnd5e2014/rules/`, split
per-domain — see "Architecture" above). Feats (and, by extension,
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
