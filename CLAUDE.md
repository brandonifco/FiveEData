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
why the quantization work below exists — a citation alone can't run a game.

**This file is loaded into context every session, so it pays for brevity.**
Record the *durable rule* a decision established — a new cross-domain
reference shape, a `RuleId` sharing precedent, a new mechanism shape, a
decline category — never a narrative of the build. The build-by-build trail
lives in `git log --oneline`; two earlier long-form versions of this file are
recoverable with `git show e2bd672:CLAUDE.md` and `git show b96ed1d:CLAUDE.md`.

## Current state

Gate as of the last merge: Debug+Release build 0 warnings, **3302 tests**.

**Built and complete:**

- Equipment (weapons, armor, shields, adventuring gear, tools, mounts,
  vehicles, trade goods) and Expenses (lifestyles, food & drink, hospitality,
  mundane services)
- Creature vocabulary (abilities, skills, languages, sizes, conditions,
  damage types, senses, alignments). **All 15 conditions carry full
  mechanical payloads**, not just `Id`/`Name`/`Sources`.
- Races (9 races, 9 subraces), Classes (12 classes, 40 subclasses),
  Backgrounds (13)
- **Spells — all 361 real PHB spells, levels 0 through 9**, with header
  blocks, and 78 of them with damage/condition effect data (59
  `SpellDamageEffect`, 24 `SpellConditionEffect`, some both). See "Spells".
  361, not 362 — Trap the Soul is an appendix error; see that section.
- Combat/Adventuring — the five scoped catalogs: `CombatActions` (10),
  `Cover` (3), `TravelPace` (3), `RestTypes` (2), `DowntimeActivities` (5).
  Everything else in PHB Chapters 8–9 is unbuilt **by design**.
- Character Advancement (p.15) — the 20-row XP/level/proficiency-bonus table.
- Concentration (p.203) — a singleton rules object; see that section.
- Tool proficiency grants on Class/Subclass/Race/Background, and the
  Multiclassing Proficiencies table (p.164) — see those sections.
- Quantized mechanics — leveled numbers, choice-point catalogs, and the
  feature-prose tail. **11 choice-point catalogs exist** (see the table under
  "Quantized mechanics").

**"Complete" means citation-complete, not mechanically quantized.** Many
named features across Classes/Races/Backgrounds are still a `RuleId` citation
with no mechanical payload. Check the inventory under "Quantized mechanics"
before assuming a feature exposes real numbers, and **verify against the
field, not the feature name** — several features that read as unquantized by
name are already captured under a different field (Dwarven Toughness is
`HitPointBonusPerLevel`, Superior Darkvision is `DarkvisionRangeFeet`, Fleet
of Foot is the subrace `Speed` override).

**In progress: the rules-chapters scoping pass** (see that section).
Character Advancement, Multiclassing Proficiencies, and Concentration are
built. The encumbrance/size multipliers are the only candidate left.

**Out of scope, settled:** magic items (2014 DMG Chapter 7, not PHB —
re-confirmed 2026-08-09 when asked); feats and Variant Human (not in the free
2014 SRD this project's provenance model is built around); variant
backgrounds and Suggested Characteristics tables (an optional reskin isn't a
new named entity). Revisit only if the project's scope statement itself is
deliberately widened, not as a one-off exception.

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

Every catalog domain (weapons, armor, languages, conditions, ...) follows the
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

Top-level domains are siblings, split by PHB chapter rather than by what
consumes what: `Rules/Equipment`, `Rules/Expenses`, `Rules/Creatures`,
`Rules/Classes`, `Rules/Backgrounds`, `Rules/Spells`, `Rules/Combat`
(Chapter 9), `Rules/Adventuring` (Chapter 8), `Rules/Characters` (Chapter 1),
and `Rules/Common` (shared vocabulary). Races sit under
`Rules/Creatures/Races/` since they consume creature vocabulary; Classes and
Backgrounds do not, since a class/background is a player-build concept in its
own right. Character advancement is deliberately **not** under
`Rules/Classes/` — the table is universal, and filing it under one class
domain would misstate that.

**A parent/child pair is two sibling definitions, never nested.** The child
carries a back-reference ID to its parent and is validated and cataloged
independently — `Tool`/`ToolFamilyId` set the shape, `Race`/`Subrace` and
`Class`/`Subclass` follow it. Follow it again for any future parent/child
domain rather than nesting the child inside the parent's definition.

**Not every domain is a catalog.** `ArmorUsageRules`, `MountVehicleRules`,
and `CharacterAdvancementRules` are singleton rules objects exposed directly
on `Dnd5e2014Ruleset` as bare properties. **Reach for the singleton shape
when the natural key isn't a string ID** (character advancement's key is an
integer level; minting `dnd5e2014.character-advancement.level-1` would be
ceremony with no consumer) **or when the content is a handful of flat
constants** rather than a list of named entries.

**A new cross-domain reference may need its `HashSet` built earlier in
`CatalogIntegrityValidator`, not added.** Both the racial proficiency pass
and the tool proficiency pass found the set they needed already constructed
further down the same method for a later consumer; the fix is to move the
existing construction up, not to declare a second one.

**Wiring a brand-new top-level domain touches five places beyond its own
folder:** `RulesetDefinitionSet` (raw definitions, appended as the new last
constructor parameter), `Dnd5e2014RulesetLoader` (embedded-resource constant
+ `Load()` call + threading through both constructor calls), `Dnd5e2014Ruleset`
(the public property), `CatalogIntegrityValidator` (a `ValidateSources` loop,
plus any cross-domain reference checks), and the `.csproj` entry. Roughly ten
pre-existing integrity tests construct `RulesetDefinitionSet` directly with
named arguments and need the new parameter added — expect that fan-out every
time.

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

If a single prefix's file grows into its own monolith — `class-rule.json`
(305 entries) is the likely first candidate — split that one file further
(e.g. `rules/class-rule/<class-slug>.json`) and fold it into the same merge
list; the merge step doesn't care how many files it's fed.

### Provenance discipline

`CatalogIntegrityValidator` (and domain-specific integrity validators like
`CreatureVocabularyCatalogIntegrityValidator`, `RaceCatalogIntegrityValidator`,
`ClassCatalogIntegrityValidator`, `BackgroundCatalogIntegrityValidator`)
check every definition's `SourceReference.DocumentId` resolves to a loaded
`SourceDocument`, and that cross-domain ID references resolve too. **Any new
cross-domain reference field gets the same check wired in.**

**A new closed official domain does not need its own
`Official*SemanticValidator`.** That runtime-validator pattern
(`OfficialCreatureVocabularySemanticValidator` and its expense/weapon
siblings) exists only for the older domains, where it hardcodes the
exact expected set of IDs/names/citations and rejects even *extra* entries
(non-`dnd5e2014.*` extension IDs are exempt). Every catalog added since
Fighting Style asserts its exact closure in `<Domain>DataFileTests` instead.
Follow the newer pattern.

### Citation rules, learned the hard way

- Use the cleanly-scanned PHB PDF (`~/Downloads/Player's Handbook.pdf`,
  reliable per-page footers) — **not** the archive.org OCR export, whose
  page-footer digits are missing or corrupted.
- **Read values off the page images, never off `pdftotext`.** That PDF's own
  text layer is OCR and is badly noisy — "Id6" for 1d6, "leveI" for level,
  "I action" for 1 action. It is fine for *locating* a heading (grep it to
  find the page, much cheaper than paging through images), but every value
  that lands in a data file must come from the rendered page (`pdftoppm` at
  300 dpi). For multi-column layouts, `pdftotext -layout` scrambles reading
  order badly enough that even the *locating* step needs the image.
- **The printed footer page and the PDF page index differ, and the offset is
  section-dependent** — Chapters 2, 3, and 11's appendix have each shown a
  one-page offset in this printing. Verify the offset per section, and
  re-verify it entirely if the PDF is ever replaced.
- Cite the page where a feature's own substantial body text **starts**, not
  where it ends and not wherever a stray heading or stat line surfaces first.
  The PHB's two-column layout regularly puts a proficiency block a page ahead
  of the feature it belongs to. **Verify the heading, not the payload** —
  The Third Eye's correct p.116 citation was nearly "fixed" into an error
  because all four of its options render on p.117.
- **A spell entry is cited to the page bearing its name heading**, even when
  its stat block or body runs onto the next page. This is the one place the
  general rule is read as "where the entry starts" — a spell's citation backs
  its header block, and the header block begins with the name.
- **Verify every page number against the real text, every time.** Separate
  passes found genuine pre-existing off-by-one errors (four during the
  Paladin auras work, six during Battle Master maneuvers, ten during the
  Fighter quantized pass, one on Overchannel). **Off-by-one errors cluster
  but do not run uniformly** — the Fighter block had seven rules late from
  one starting page and three more late from a different one, with three
  correct neighbours in between. Check each entry against the image; never
  extrapolate a shift across a range. Fix them where you find them, in the
  same commit.
- **Two-column pages mix owners.** A feature printed directly above or beside
  another subclass's entries can belong to a different owner entirely:
  Quivering Palm is Way of the Open Hand's, not the Monk's
  (`CanonicalFile_QuiveringPalmBelongsToWayOfTheOpenHand`); Thousand Forms is
  Circle of the Moon's, not the Land's
  (`CanonicalFile_PreservesCircleOfTheMoonThousandForms`); Relentless Rage is
  a base Barbarian feature, not the Berserker's. Cross-check `LevelFeatures`
  before writing data, not after.
- **Errata and body prose beat printing artifacts, and a description page
  beats a summary appendix.** The Dwarf's printed "throwing hammer" is
  corrected to "light hammer" by official errata. The Warlock table omits a
  19th-level Ability Score Improvement row the feature's own text names — the
  prose wins (`PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission`).
  The Paladin spell list prints "Destructive Smite"; the spell's own
  description page headers it "Destructive Wave", which is its real name
  (`DestructiveWaveIsNamedFromItsDescriptionNotTheAppendix`). Taken to its
  limit, the appendix can name something that doesn't exist at all — see
  "Spells: the Trap the Soul appendix error".
- **Verify every remembered count, page number, and existing field's
  completeness against the actual text, repeatedly — including numbers
  written in this file.** Eldritch Invocations turned out to be 32, not the
  ~20 this file once claimed; Elemental Disciplines 17, not 18; Channel
  Divinity options 16, not 10. The race pass found
  `CanonicalFile_OnlyWoodElfOverridesSpeed` had been **silently failing since
  the original Races commit** — the field was never populated even though the
  field and the test both existed. **"The test exists" and "the test passes"
  are different claims.**

## Content modeling rules

### What becomes structured data vs. a citation

`RuleDefinition` is `Id`/`Name`/`Sources` — nothing else. A feature's prose
never lives in this repo; only a citation index does.

- **A leveled numeric fact belongs in structured data.**
- **A compound formula, a DM-adjudicated range, or content this project
  doesn't model as its own domain belongs in the citation.**
- **Spells *are* a modeled domain — `SpellId` and `SpellDefinition` are real
  types, and other domains reference them.** This file asserted the opposite
  for a long time ("there is no `SpellId` type"), which was true when written
  and is now false. Domain Spells, Circle Spells, and Oath Spells are still
  citation-only, but that's a not-yet-built gap, not a modeling stance.
- **A fact used exactly once still earns a field if it's genuinely a flat
  fact** (Metamagic's `ProtectsCreatureCountUpToSpellcastingModifier`,
  `SpellGrant.CastAtSpellLevel`). The one-data-point rule below governs new
  *mechanism shapes*, not new fields on an existing shape.
- **One data point doesn't justify a new mechanism shape.** When a single
  entry needs a shape nothing else needs, decline it and note the shape for
  revisit. Two independent real instances is the bar
  (`FlatDamageBonus` was declined for Magic Missile at 1st level and added
  when Disintegrate needed it at 6th; `ChoosableConditionIds` was declined
  three times before Fey Presence and Dark Delirium arrived together).
- **A decline is only as good as the precedents available when it was
  made.** Brutal Critical's scaling and Destroy Undead's CR table were both
  declined before Sneak Attack and Wild Shape existed, and both were later
  reversed. Re-read old declines against newer shapes rather than treating
  them as settled.

### The decline taxonomy

These categories recur constantly. When declining something, name which one
it falls under rather than re-deriving the reasoning:

- **Linear-in-level or ability-modifier formulas** — Preserve Life's
  `5 × cleric level`, Radiance of the Dawn's `2d10 + level`, Lay on Hands,
  Second Wind, Arcane Recovery, Natural Recovery, Divine Intervention, Arcane
  Ward, Dark One's Blessing, Wholeness of Body, Slow Fall, Deflect Missiles,
  Survivor, Divine Smite's slot scaling, every spell's "At Higher Levels"
  upcast, the Wizard's spell-copying costs, every "Xd Y + your ability
  modifier" damage or heal (Cure Wounds, Spiritual Weapon, Parry, Rally).
- **Formulas relative to a value already modelled elsewhere** — a Long Rest's
  "half your total Hit Dice", Maneuvering Attack's "half its speed", any
  "within your reach" clause (`WeaponDefinition` already models reach).
  Corollary: **don't store what's derivable from an already-modelled field**
  (Sculpt Spells' "1 + the spell's level" is a bool, not a number).
- **Compound conditionals** — Prone's range-conditional attack rolls,
  Plant Growth's "1 action *or* 8 hours" alternate casting mode, One with
  Shadows' trigger, Totemic Attunement (Bear)'s immunity clause.
- **Multi-mode / caster-picks-one-of-N** — Symbol, Prismatic Spray,
  Prismatic Wall, Imprisonment, Storm of Vengeance, Glyph of Warding,
  Antipathy/Sympathy, Eyebite, Elemental Attunement, Shape the Flowing River,
  Book of Ancient Secrets' open spell choice, Circle of the Land's
  "one cantrip of your choice". A caster-chosen *condition* is now capturable
  via `ChoosableConditionIds`; a caster-chosen whole *effect* is not.
- **Content this project doesn't model as a domain** — Rock Gnome's Tinker
  (constructed objects; pinned by
  `CanonicalFile_RockGnomeTinkerStaysCitationOnly`), carrying capacity, jump
  distance, opposed ability checks against a DC.
- **A comparison rule with no value** — Indomitable Might ("if your total is
  less than your Strength score, use the score") is not a number. Pinned by
  `CanonicalFile_BarbarianIndomitableMightStaysCitationOnly`.
- **Riders on a captured core fact** — per-target cooldowns (Chains of
  Carceri, Intimidating Presence), creature-type filters, repeat-saves-to-end
  (Hold Person, Flesh to Stone, Nature's Wrath), damage-on-a-miss, follow-up
  tick damage (Witch Bolt, Melf's Acid Arrow), partial-success riders. Capture
  the clean initial fact; leave the rider in the citation.
- **Action-economy redirects** — who attacks, not a number: Commander's
  Strike, Riposte, Retaliation, Stand Against the Tide.
- **Weapon-attack riders** — a spell or feature that only buffs a *later*
  weapon attack (the five 1st-level smites, Blinding Smite, Lightning Arrow,
  Divine Favor, Hex, Shillelagh).
- **Controllable constructs and perception links** — Spiritual Weapon,
  Bigby's Hand, Mordenkainen's Faithful Hound and Sword, Invoke Duplicity,
  Gaze of Two Minds, Voice of the Chain Master, every conjure-a-creature
  spell. The summoned thing's turn-by-turn actions aren't the spell's own
  effect.
- **Don't infer a tag the text doesn't use.** Stinking Cloud's "retching and
  reeling" is not `poisoned`; Otiluke's Resilient Sphere "encloses", it does
  not `restrain`; Otto's Irresistible Dance is not `prone`; Feeblemind is an
  ability-score reduction, not a condition. "Turned" is not one of the 15
  Appendix A conditions at all.
- **Two damage types that both always apply** — Ice Storm, Destructive Wave,
  Flame Strike, Meteor Swarm. `DamageTypeId`/`ChoosableDamageTypeIds` cover
  "always this one" and "caster picks one", not "always both".
- **No roll of any kind** — automatic zone/trigger damage (Cloud of Daggers,
  Heat Metal, Spike Growth, Forbiddance), auto-hit targeting (Magic Missile),
  and hit-point-threshold effects (Power Word Stun, Power Word Kill).
  `SpellDamageEffect` is built around exactly one of attack-roll or save.
  Five real instances exist across the PHB — enough to note as a confirmed
  pattern, not yet enough for a dedicated `AutomaticDamage` shape. Revisit if
  a future domain's density grows.
- **Contested checks** — Telekinesis resolves caster check vs. target's save,
  not the ordinary "target makes a saving throw" shape.
- **Two separately-costed material items** — Leomund's Secret Chest, Legend
  Lore, Clone, Astral Projection (per-creature multiplier), Imprisonment
  (per-Hit-Die formula). `MaterialCostGoldPieces` stays null rather than
  picking one of two figures.
- **Store what's printed, don't infer.** Lifedrinker prints "(minimum 1)" and
  Agonizing Blast doesn't, for the same mechanic — the difference stays in
  the citation rather than being normalized away or invented for the entry
  that lacks it. Same for Sacred Weapon's "(with a minimum bonus of +1)".

**An option with nothing to quantify still gets a catalog entry**
(Id/Name/Sources) — the catalog's job is completing the option list. Pact
Boon, Practicing a Profession, and Stand Against the Tide set that precedent
(`StandAgainstTheTide_IsEnumeratedWithNoMechanismFields`).

### Unmodeled gaps (deliberate, not oversights)

Starting equipment and Druid's nonmetal-armor restriction have no fields.
Each was a considered call: adding one would be generality ahead of a real
consumer.

**The tool-proficiency gap is now open and fully populated across all four
owners** — the Multiclassing Proficiencies table was the first real
downstream consumer it ever had, and rather than declining two of its twelve
rows, the grant fields were built. See "Tool proficiency grants".

**Two features now reference rules this project doesn't model.** Aspect of
the Beast (Bear) sets `DoublesCarryingCapacity` with no carrying-capacity
domain to double; Second-Story Work sets
`AddsDexterityModifierToRunningJumpDistance` with no jump-distance rule.
Both are honest records of what the book says and neither is a bug, but they
are *dangling mechanics* rather than dangling references — the concrete
argument for the encumbrance and jump candidates in the open scoping table.

### `RuleId` sharing — the single most important discipline here

**Default to a shared, unprefixed `RuleId` for a mechanic bearing a generic
name; prefix and split only when a specific class's actual PHB text
diverges. Verify by reading the real text side by side, every time, against
*every* previously-built class — not just the most recent one.**

- **A recurring template name is not evidence either way.** A template with a
  substituted word stays split, because the substitution *is* the mechanic
  (Wizard's 8 school `Savant` features, Cleric's 5-way `Divine Strike`,
  Warlock's 3 `Expanded Spell List`). A template with zero difference shares
  (`Potent Spellcasting` across Knowledge/Light).
- **Uncertainty resolves toward keeping separate.** The failure directions
  aren't symmetric: wrongly merging two possibly-distinct texts is a
  correctness bug; keeping one redundant citation is not. A single dropped
  word ("a *particular* style of fighting") kept `paladin-fighting-style`
  split even though the word carries no mechanical weight.
- **Trigger level alone never blocks sharing** — the level lives in
  `LevelFeatures`, not the citation. But a difference in the cited sentence
  does (`college-of-valor-extra-attack`'s "Starting at 6th" vs. the shared
  entry's "Beginning at 5th").
- **A shared choice-point `RuleId` doesn't promise every class offers the
  same options.** Fighter and Ranger share `fighting-style` while offering 6
  and 4 options.
- **Before minting a `RuleId` for a feature named like an existing one, check
  whether it's a choice-point *option* rather than a feature in its own
  right.** Hunter's Evasion and Uncanny Dodge are sub-options inside Superior
  Hunter's Defense — they live as catalog entries, not as new or extended
  `RuleId`s, and that is not a collision with the standalone `evasion` and
  `uncanny-dodge` rules.
- **Two features sharing a name is not evidence they share a mechanic; read
  both.** Light Domain's Bonus Cantrip names a specific cantrip; Circle of
  the Land's is an open choice.
- Collisions get caught late as well as early. When one surfaces,
  retroactively rename the incumbent too rather than leaving one mechanic
  squatting on the generic ID (`expertise` → `rogue-expertise` when Bard's
  arrived).

**Current shared, unprefixed entries** (add a `SourceReference`, don't mint a
new ID): `ability-score-improvement` (10 — every class but the two prefixed
below), `extra-attack` (Barbarian/Monk/Ranger/Paladin), `fighting-style`
(Fighter/Ranger), `evasion` (Monk/Rogue), `lands-stride` (Ranger/Circle of
the Land), `potent-spellcasting` (Knowledge/Light),
`martial-and-heavy-armor-proficiency` (Tempest/War).

**Deliberately prefixed splits** (don't "helpfully" consolidate):
`fighter-`/`rogue-ability-score-improvement`, `fighter-extra-attack`,
`college-of-valor-extra-attack`, `paladin-fighting-style`,
`barbarian-`/`monk-unarmored-defense`, `rogue-`/`bard-expertise`,
`monk-`/`druid-timeless-body`, every school's `Savant`, every domain's
`Divine Strike`, every patron's `Expanded Spell List`, every subclass's
`Bonus Proficiencies`, and every class's own core spellcasting. Pinned by
`EverySchoolSavantFeatureIsItsOwnDistinctRuleId`.

### Class/subclass structure rules

- **Core spellcasting is always one citation, always prefixed per class,
  never shared** — even between two full casters. It isn't always table-named
  "Spellcasting" (Warlock's is "Pact Magic"). Check for a Ritual Casting
  subsection each time rather than assuming: Warlock and Sorcerer genuinely
  have none.
- **A choice point folds into one citation**, independent of option count (3
  to 32) or per-option length. The deciding factor is the shape — a single
  named gateway offering sub-choices — never size.
- **A framework heading earns its own `RuleId` only if it has its own table
  row.** Cleric's `Channel Divinity` is a 2nd-level row and kept a citation;
  Paladin's same-named framework text isn't, and folded into `sacred-oath`.
  The table likewise decides whether a recurring mechanic gets repeated
  `LevelFeatures` entries or stays one citation with the scaling in prose.
- **A class table's numeric column and its Features column are two different
  lists and may legitimately disagree — neither is a stale copy of the
  other.** Monk proved both directions at once: Unarmored Movement's speed
  grows at 2/6/10/14/18 while the Features column names it at 2 and 9;
  Martial Arts is a single 1st-level feature row whose die grows at 5/11/17
  with no row at all. Pinned by
  `CanonicalFile_MonkUnarmoredMovementSpeedLevelsDifferFromFeatureLevels` and
  `CanonicalFile_MonkGrantsMartialArtsOnlyAtFirstLevelDespiteDieUpgrades` —
  both exist to stop a future pass "aligning" one list to the other.
- **Quantizing a feature means re-reading its table row, not just its
  numbers.** Destroy Undead's `LevelFeatures` recorded only 5th level while
  the Cleric table's Features column names it at 5/8/11/14/17.
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
  who also have no armor proficiency). Pinned by
  `DoesNotDuplicateWizardsWeaponProficiencyIdList`.

### Backgrounds

Structurally the simplest domain: a flat, one-shot grant with no ability
scores, no size/speed, no level-gated features. **Every one of the 13 grants
exactly two fixed skill proficiencies, never a choice** — which is why
`SkillProficiencyIds` is validated as exactly-two rather than a
choice-count/option-list pair like class skill choices. Language grants are
always "N of your choice" (0, 1, or 2), never named languages. The domain was
fully triaged during the original quantized pass into its four scalars
(`SustainedLifestyleId`, `AdditionalPeopleFedPerDay`, `GuildDuesGoldPerMonth`,
`FastTravelSpeedMultiplier`); a later scoping sweep found no further
candidates.

## Spells

Per p.202 ("Casting a Spell"), a spell entry's header block is *name, level,
school, casting time, range, components, duration* — that's what
`SpellDefinition` stores, plus `AvailableToClassIds` and the two effect
fields below. `MagicSchools` holds the 8 schools (a closed official set,
cited to the p.203 sidebar).

**Every level was added in alphabetical batches of ~10–15**, because 62
first-level spells over ~38 pages is too much to read reliably in one pass.
`SpellDataFileTests` pins the exact built closure, so a partial level is
asserted rather than implied. **Keep batching for any future re-read.** Run
the `sorted?` check after every batch, not just when something looks off — it
caught two wrong alphabetical insertions that a human reviewer didn't.

### Header-block shape

Cantrips proved the shape because they are a complete, closed set exercising
every school and six V/S/M combinations — which is why **components are three
independent bools, not an enum**.

- **Duration has three mutually exclusive kinds**, validator-enforced:
  instantaneous, until-dispelled (`IsUntilDispelled`, which also covers
  "until dispelled or triggered"), and special (`IsSpecial`, for a duration
  the book states as a table or a compound end-condition). Beyond that,
  "up to" and concentration are independent axes. **"Up to" is not a synonym
  for concentration** — Prestidigitation is "Up to 1 hour" with no
  concentration; concentration is always an "up to" duration.
- **A duration unit does not imply a duration shape.** Shield and True Strike
  are both 1 round, but Shield's is flat while True Strike's is concentration
  and "up to". Pinned by `ShieldsRoundDurationIsFlatWhileTrueStrikesIsUpTo`.
  The same flat-vs-concentration split recurs at Minute (Blink), Hour
  (Etherealness), and Day (Forbiddance vs. Find the Path).
- **Casting time** carries a unit (`Action`/`BonusAction`/`Reaction`/
  `Minute`/`Hour`) and any amount, not just 1. **A reaction spell's trigger
  stays in the citation** — the PHB states it as prose; only the unit is data.
- **Range kinds:** `Self`, `Touch`, `Distance`, `Unlimited` (Sending,
  Telepathy), `Sight` (Mirage Arcane, Tsunami, Storm of Vengeance), and
  `Special` (Dream — reach stated as a conditional rule, not a distance).
  `SelfWithArea` carries an area shape.
- **Area shapes built:** `Cone`, `Radius`, `Cube`, `Line`, `Hemisphere`.
  Still omitted until content needs them: the cylinder (Flame Strike) and the
  wall. **The first spell to need a shape is rarely the memorable one** —
  Gust of Wind brought `Line`, not Lightning Bolt.
- **Distances are canonicalized to feet.** "1 mile" stores 5,280; Project
  Image's 500 miles stores 2,640,000; Control Weather's 5-mile radius stores
  26,400. This is a unit conversion, not a derived total, so it doesn't trip
  the store-what's-printed rule.
- **`IsRitual`** — a cantrip may never be a ritual; the validator enforces it.
- **Materials: `MaterialCostGoldPieces` and `MaterialIsConsumed` are
  independent, and all four combinations exist.** `MaterialCostGoldPieces`
  **holds the figure the PHB
  prints, never a derived total** — Warding Bond's "a pair of platinum rings
  worth at least 50 gp each" stores 50, not 100; the "each" survives in
  `MaterialDescription`. Pinned by
  `WardingBondStoresThePrintedPerItemCostNotTheTotal`. Three PHB phrasings
  carry a cost: "worth at least X gp", "X gp worth of", and a bare "worth
  X gp".

**Adding a `[JsonRequired]` field to `SpellDefinition` means backfilling
every already-built entry in the data file.** This has been paid twice
(`IsSpecial` across 244 spells, `FlatDamageBonus` across 43 populated damage
effects) — each done as one scripted insertion verified byte-for-byte to
touch nothing else. Expect that cost for the next required field.

### Effect data

Two independent nullable fields on `SpellDefinition`; a spell can carry
either, both, or neither.

- **`SpellDamageEffect`** — `AttackRollType` (`Melee`/`Ranged`) *or*
  `SavingThrowAbilityId`, constructor-enforced as exactly one, per p.202's
  own split; `DamageTypeId` *or* `ChoosableDamageTypeIds` (same exactly-one
  rule); `HalfDamageOnSuccessfulSave`, forced `false` when there's no save;
  `DamageByCharacterLevel` (a cantrip's tier list) *or* `BaseDamage` (a
  leveled spell's flat dice), again exactly one; and `FlatDamageBonus`
  (requires `BaseDamage`).
- **`SpellConditionEffect`** — `ConditionIds` (non-empty, no duplicates) +
  a **required** `SavingThrowAbilityId`. **`ConditionIds` means AND**
  (Tasha's Hideous Laughter imposes prone *and* incapacitated together), never
  "one of these".

Durable rules from that pass:

- **A leveled spell's saving throw usually takes half damage on success; a
  cantrip's never does — but read it per spell.** Cordon of Arrows, Evard's
  Black Tentacles, and Contact Other Plane all have saves with no half-damage
  clause.
- **A cantrip's character-level scaling and a leveled spell's flat damage are
  different axes, not one field with a kind flag.** Forcing one shape onto
  both would lie about what "character level 1" means for a spell whose
  damage never depends on character level. Eldritch Blast is a single-tier
  `DamageByCharacterLevel` (it gains beams, not dice); the beam count itself
  is declined as targeting multiplicity, the same call as Scorching Ray's
  three rays.
- **`DamageEffect` and `ConditionEffect` are independent, not a package
  deal.** Four spells gate both on the *same* saving throw (Evard's Black
  Tentacles, Sunbeam, Sunburst, Weird) — the schema needs no "these share one
  roll" concept, since each field carries its own ability and they simply
  match. And a spell's clean fact is captured even when its messier sibling
  fact on the very same save has to be declined (Destructive Wave).
- **Sleep is why `SpellConditionEffect.SavingThrowAbilityId` stays
  non-nullable** — its hit-point-pool targeting has no save anywhere, so it's
  declined rather than weakening a field every other spell needs.
- Levels 7 and 9 are sparse by nature: high-level PHB content trends toward
  exotic, multi-mode, "ultimate" effects that genuinely don't fit the clean
  "attack roll or save, damage or condition" shape.

### Class spell lists

**`AvailableToClassIds` comes from the Chapter 11 class spell lists
(pp.207–210), never from the spell description** — the description never
names its classes. Those pages are laid out in **four narrow columns**, not
the two the description pages use — **read them as quadrant crops or a
column's continuation gets missed.** That exact miss happened three separate
times (Wizard's 3rd-level count, Wizard's 6th-level count, and a spell,
Wall of Thorns, omitted entirely until a per-class count test failed).

Per-level union counts are **not monotonically declining**: 62/59/50/35/42/
32/20/18/16 for levels 1–9. Per-class counts are pinned in
`SpellDataFileTests` (`ClassCantripListHasExpectedSize` and
`Class<N>LevelListHasExpectedSize`) — read them there rather than trusting a
number carried over from an earlier batch.

**Paladin and Ranger's lists stop entirely after 6th level, but Warlock's
does not** — Pact Magic's *slots* cap at 5th while the class list keeps going
through 9th, for spells eligible as a Mystic Arcanum. Warlock's per-level
counts there track Mystic Arcanum eligibility (8/4/5/5 for 6th–9th), not a
shrinking slot count, so they have no single trend to extrapolate from.

### Spells: the Trap the Soul appendix error

**"Trap the Soul" is not a real spell in this PHB printing.** It's confirmed
*not findable* anywhere in its correct alphabetical position: the text runs
continuously from Transport via Plants into Tree Stride, verified against
high-resolution renders of every page from p.279 through p.285 plus a
full-text search of the whole book. Its only appearance is the Wizard class
list on p.212 — that appendix entry is itself the error. **Treat this as
settled, not a pending gap**: 8th level's real count is 18, not 19, and
Wizard's real 8th-level count is 13, not 14. Do not build it from memory or
another source, and do not treat a future PDF that includes it as corrective.

## Quantized mechanics

Extracting the actual numbers a running game needs from features previously
stored as citations only. Always **additive** — every citation touched was
left in place.

**Inventory — what actually carries numbers.** Anything not listed is
citation-only. **Verify against the definition types, not this list.**

- **Catalogs referenced by ID:** `SpellSlotProgressionId` (+
  `SpellcastingAbilityId`) and `ExtraAttackProgressionId`, on both
  `ClassDefinition` and `SubclassDefinition`.
- **Choice-point catalogs** (standalone, not referenced from a definition) —
  see the table below.
- **Embedded on `ClassDefinition`:** Action Surge, Indomitable, Rage, Brutal
  Critical, Fast Movement, Favored Enemy, Natural Explorer, Sneak Attack, Ki,
  Martial Arts, Unarmored Movement, Sorcery Points, Wild Shape, Bardic
  Inspiration, Song of Rest, Magical Secrets, Channel Divinity uses, Destroy
  Undead, Mystic Arcanum, Font of Magic conversion, Aura of Protection, Aura
  of Courage, Eldritch Invocations known, Cantrips Known, Spells Known,
  Wizard Spellbook, Blindsense, Reliable Talent, Feral Senses, Divine Sense,
  Improved Divine Smite, Primal Champion, Cleansing Touch uses, Relentless
  Rage.
- **Embedded on `SubclassDefinition`:** Divine Strike, Circle Forms, Combat
  Superiority, Disciple of the Elements, Aura of Devotion, Aura of Warding,
  Additional Magical Secrets, Portent, Draconic Resilience, Improved
  Critical, Shadow Step, Hurl Through Hell, Wrath of the Storm, Thunderbolt
  Strike, Shadow Arts and Quivering Palm ki costs, Draconic Presence's
  sorcery point cost, Bend Luck, Warding Flare, War Priest uses, and the
  Barbarian/Rogue/Warlock/Wizard/Sorcerer feature detail types (Frenzy,
  Intimidating Presence, Second-Story Work, Assassinate, Infiltration
  Expertise, Death Strike, Fey Presence, Misty Escape, Beguiling Defenses,
  Dark Delirium, Awakened Mind, Entropic Ward, Thought Shield, Create Thrall,
  Hypnotic Gaze, Instinctive Charm, Split Enchantment, Alter Memories, Sculpt
  Spells, Potent Cantrip, Empowered Evocation, Overchannel, Elemental
  Affinity, Dragon Wings).
- **On `RaceDefinition`/`SubraceDefinition`:** `DarkvisionRangeFeet`,
  `ResistedDamageTypeIds`, `TranceDurationHours`, `HitPointBonusPerLevel`,
  the subrace `Speed` override, Breath Weapon, Savage Attacks, Relentless
  Endurance, Lucky, the racial proficiency fields, and `InnateSpellGrants`.
- **On `BackgroundDefinition`:** the four scalars listed under "Backgrounds".
- **Singletons:** `CharacterAdvancementRules`, `ArmorUsageRules`,
  `MountVehicleRules`.

### Shape selection

- **A fact shared across multiple classes → a catalog referenced by ID.**
  `SpellSlotProgressionId` (4 rows), `ExtraAttackProgressionId` (2 rows).
- **A single-class/single-race/single-background fact → a bare field or a
  small embedded value object**, mapped inline by a `*DataMapper`, no
  top-level catalog.
- **One feature with several facts → its own `*Detail` type** (the
  `ShadowStepDetail`/`WardingFlareDetail` precedent), rather than loose fields
  on the owning definition. **One feature with exactly one fact and nothing
  to pair it with → a bare field** (`ImpostorRequiredStudyHours`,
  `SplitEnchantmentTargetsSecondCreature`) — a wrapper record holding one
  bool is ceremony.
- **Add a field only where it actually varies.** `RequiresConsciousness`
  exists on the auras because Aura of Warding genuinely lacks the clause;
  Metamagic has no `AvailableToClassIds` because it's Sorcerer-only; three
  choice-point catalogs carry no `RequiredLevel` because all their options
  sit at one level.
- **Each domain owns its own type even when the shape is identical.**
  `CantripsKnownProgressionDetail`/`SpellsKnownProgressionDetail`,
  `SpellDamageTierGrant` vs. `DivineStrikeDamageGrant`, and
  `DarkvisionRangeFeet` duplicated across three domains all follow this. A
  shape repeating is not by itself a reason to extract a shared type.
- **Reuse existing types over new number spaces.** `DiceExpression` carries
  dice; damage types, conditions, sizes, abilities, skills, weapons, spells,
  and travel paces are all catalogued IDs. **When a feature's text names a
  value some other catalog already enumerates, reference the catalog.**
  Conversely, don't invent a type for two call sites — `MaxChallengeRating`
  stayed a plain `double`, and `RecoversOnShortRest` stayed a plain `bool`
  rather than relocating the spell-slot-specific `SpellSlotRecoveryRest` enum
  and breaking a shipped public type.
- **Never build the general shape by default** — build only what the real
  content needs.

### Mechanism representation — no generic effect DSL

5e's mechanics are too heterogeneous for one flat `Effect` type to cover
honestly. Instead a definition carries **several typed, nullable mechanism
fields side by side**. Two variants, chosen by what the facts actually are:

- **Mutually exclusive** (validator/constructor enforces exactly one
  populated) when the fields are alternative representations of *one*
  mechanic — `WeaponDefinition`'s damage/range/versatile,
  `FightingStyleDefinition`, Metamagic's cost, Divine Strike's damage,
  `SpellDamageEffect`'s attack-or-save.
- **Independent and co-occurring** (no exclusivity check) when they're
  separate facts that legitimately apply together — `ConditionDefinition`'s
  26 fields, `ChannelDivinityOptionDefinition`, Eldritch Invocations'
  stackable prerequisites, `DowntimeActivityDefinition`. **Two independent
  fields populated together means both apply.**

**Paired fields are validator-enforced present-together-or-neither**:
saving-throw ability + DC, granted spell + casting frequency, base damage +
damage type, bright-light + dim-light radius, clear-sight range + its
detail-equivalent range, multiattack kind + its range, a duration trigger +
something for it to bound.

**Fixed-or-choosable is a recurring axis, now on five fields**: damage types
(`SpellDamageEffect`), saving-throw abilities
(`ChannelDivinityOptionDefinition`), conditions (`ChoosableConditionIds`),
and Transmuter's Stone's resistances. A choosable list must hold **at least
two** options — a "choice" of one isn't a choice. **`ChoosableConditionIds`
(OR) and `ImposedConditionIds` (AND) are deliberately different fields with
the same list shape, disambiguated by name.**

**Inverse pairs exist and stay separate:** `MaximumTargetSizeId` (Trip
Attack: "Large or smaller") vs. `MinimumTargetSizeId` (Giant Killer: "Large
or larger"); `HalfDamageOnSuccessfulSave` vs. `HalfDamageOnFailedSave`
(Hunter's Evasion). Don't collapse either pair into one field with a
direction flag — no single option ever needs both, and a field name that says
which branch it means is what stops a silent miscapture.
`ResistsAllDamageExceptTypeId` is the same idea for resistance: storing the
12 other damage types would be a derived total.

### Supporting rules

- **Capture the full mechanical fact set, not just the leveled number** —
  Rage's `DurationMinutes`/`RequiresNotWearingHeavyArmor`, Sneak Attack's
  `OncePerTurn`/`RequiresFinesseOrRangedWeapon`. But verify the set rather
  than assuming: Song of Rest genuinely has only a die.
- **An armor gate is read per feature, never generalized.** Rage and Fast
  Movement stop at *heavy* armor; Monk's Martial Arts and Unarmored Movement
  are blocked by *any* armor and by a shield. Pinned by
  `CanonicalFile_BarbarianFastMovementGatesOnHeavyArmorOnly`.
- **`RecoversOnShortRest` is read per feature, never inferred from the class
  family** — Ki `true`, Sorcery Points `false`, Channel Divinity `true`,
  Mystic Arcanum `false`, despite Warlock's Pact Magic recovering on a short
  rest. It can even differ between two features on the *same table row*:
  Fighter's Action Surge returns on a short rest, Indomitable only on a long
  one (`CanonicalFile_FighterActionSurgeAndIndomitableRecoverOnDifferentRests`).
- **A cost and a regain are different fields** — `…KiCost` vs.
  `…KiPointsRegained`. Empty Body needed a detail object rather than a scalar
  because one feature buys two things at two prices.
- **`int?` over sentinels** — Rage's 20th-level "Unlimited" uses `null`.
- **A leveled choice count is a cumulative total, not an increment** —
  Favored Enemy is 1/2/3 at levels 1/6/14.
- **Dense vs. sparse grant lists follow the source table.** Ki/Sorcery Points
  change every level → one entry per level. Rage/Extra Attack change at
  milestones → breakpoints only. Spell slot progressions carry all 20 levels
  even where empty.
- **Expand formulas into tables** where the breakpoints are the fact worth
  storing (Circle Forms' "level divided by 3"). But **don't** expand a pure
  arithmetic function the book prints alongside the table (ability score →
  modifier).
- **Progressions are validated for monotonicity per column, and not every
  column follows the same rule.** `ValidatePointsProgression` requires
  strictly ascending, so plateau levels are simply omitted from a sparse list
  (Spells Known). Where all rows must be kept, a non-decreasing validator is
  used instead (proficiency bonus; pinned by
  `Validator_AcceptsAPlateauedProficiencyBonusButNotAFall`). **One
  progression descends** — Improved Critical's threshold goes 19 → 18,
  because a lower crit threshold is the improvement; it has a bespoke
  descending validator, pinned by
  `Validator_RejectsImprovedCriticalProgressionWithRisingThreshold`. Don't
  "align" it. A progression also doesn't have to start at level 1 (Ranger's
  Spells Known starts at 2nd).
- **Don't reuse a shared validator helper whose error vocabulary doesn't
  match.** `ValidatePointsProgression` hardcodes "character level" into its
  messages, so Font of Magic — whose axis is *spell slot* level — got a
  bespoke method. Check what a helper's messages say, not just that the shape
  matches.
- **One progression may span two separately cited features.** Greater
  Portent's entire content is "roll three d20s rather than two", so one
  `PortentProgression` carries both rows while both citations stay in
  `LevelFeatures` (`CanonicalFile_GreaterPortentSuppliesTheFourteenthLevelPortentRow`).
  Split only when the second feature is a genuinely separate resource.
- **The same detail type can serve a class and a subclass when the mechanic
  is the same** — `MagicalSecretsProgressionDetail` sits on both, with
  `CountsAgainstSpellsKnown` distinguishing them. A field that distinguishes
  two real instances beats two near-identical types.
- **Named constructor arguments where transposition would compile.**
  `ConditionDefinition` uses them at every call site because ~20 of its 26
  parameters are plain `bool`. Elsewhere this codebase's mixed-type
  constructors fail to compile on a transposition, so positional calls are
  fine. This is a reasoned deviation, not an inconsistency to "fix".

### Cantrips known, spells known, and the Wizard spellbook

`CantripsKnownProgressionDetail` and `SpellsKnownProgressionDetail` are
independent embedded value objects on `ClassDefinition` (own
`Rules/Classes/CantripsKnown`/`SpellsKnown` folders, no catalog, mapped by a
`*DataMapper`, validated through `ValidatePointsProgression`) — the same
shape as Sorcery Points, not a new mechanism. They only ever apply to the 8
casting classes, so no sweep was needed.

**The two are mutually exclusive with "prepares from the full list", and
don't always co-occur with each other.** Six classes have a Cantrips Known
column (Bard, Cleric, Druid, Sorcerer, Warlock, Wizard — every class that
gets cantrips at all, including three that prepare their leveled spells and
have no Spells Known count). Four have a Spells Known column (Bard, Ranger,
Sorcerer, Warlock). Ranger has Spells Known but no cantrips; Wizard has
cantrips but no Spells Known; Paladin has neither. Cantrips Known breakpoints
share one shape across all six (1st/4th/10th) but not one set of values.

**The Wizard's spellbook is a flat rate, not a leveled table, because the
PHB never prints it as one** — "Spellbook" and "Learning Spells of 1st Level
and Higher" are prose paragraphs (p.114), not a table column. So
`WizardSpellbookDetail` is a flat two-scalar value object
(`StartingSpellCount`, `SpellsAddedPerLevelAfterFirst`), the
`FastMovementDetail` shape, with no per-level grant list.
**`SpellsAddedPerLevelAfterFirst` reads "gain a level" as excluding 1st
level** — you don't "gain" your starting level, you begin at it, so the +2
starts at 2nd (6 at 1st, 44 at 20th). The field name spells out the exclusion
so a future reader can't apply it at level 1 by mistake.

### The choice-point catalogs

A choice point earns a catalog **only if at least one option carries a real
quantizable fact** — Pact Boon proved the pattern isn't automatic. A "choice
point" is a category of feature, not a template: read the actual page every
time. No two of these share a schema.

| Catalog | Shape captured |
| --- | --- |
| Fighting Style (6) | typed mechanism fields + `AvailableToClassIds` |
| Metamagic (8) | sorcery point cost + 9 independent per-option effect fields |
| Pact Boon (3) | **declined** — no option carries a quantizable fact |
| Battle Master maneuvers (16) | effect target + save + condition/size/push/reach one-offs |
| Eldritch Invocations (32) | 3 stackable prerequisites + granted spell + one-offs |
| Elemental Disciplines (17) | ki cost + minimum level + granted spell or damage/save |
| Channel Divinity options (16) | independent nullable scalars, extended twice |
| Totem Warrior options (9) | shared rage/armor gates + per-animal one-offs |
| Hunter options (11) | per-option one-offs across 4 choice points |
| Open Hand Technique options (3) | save + condition/push/reaction denial |
| The Third Eye options (4) | three sense ranges + a language bool |
| Transmuter's Stone options (4) | four unrelated always-on benefits |

- **Several choice points can share one catalog.** Totem Warrior's three and
  Hunter's four each became one catalog, because they share a subclass, a
  page, and an option vocabulary, and `RequiredLevel` already discriminates.
  **Split only when the choice points genuinely differ in option vocabulary,
  not merely in level.** Conversely, two catalogs on the same class stay
  separate when their benefit sets are disjoint (The Third Eye vs.
  Transmuter's Stone) — shared option *text* is not evidence of a shared
  mechanic.
- **A catalog carries no grouping field for which subclass an option belongs
  to** — the ID prefix and the citation do that work.
- **Name options plainly when the names are globally unique; qualify them
  only when the option vocabulary repeats across choice points** (Totem
  Warrior's Bear/Eagle/Wolf).
- **A gate that cleanly partitions a catalog is load-bearing data, not
  decoration** — all six Totem Spirit and Totemic Attunement options require
  raging; none of the three Aspect of the Beast options do. Pinned by
  `AspectOfTheBeastOptions_AreTheOnlyOptionsNotRequiringRage`.
- **Open Hand Technique's and Transmuter's Stone's option names are
  synthesized, not quoted.** The PHB prints those options as unnamed bullet
  points. Names like "Knock Prone", "Push", "Prevent Reactions", "Speed
  Increase", and "Damage Resistance" are descriptive labels chosen here so
  the options can be addressed at all. **Do not "correct" them against the
  PHB expecting to find them, and do not treat them as citable book terms.**
  Every other catalog's names come straight from bold headings.
- **Treat every catalog count in this file as re-verifiable.** Two "closed,
  N-item" catalogs turned out undercounted (Eldritch Invocations, Channel
  Divinity options, which only ever covered the 7 Cleric domains and missed
  Paladin's 6).
- **Per-option effect prose stays unquantized** where it's individually
  heterogeneous with no shared shape. Shared formulas (every save DC of the
  "8 + proficiency + ability modifier" form) stay unquantized too — capture
  only what varies per option. **The exception is a printed literal:**
  Relentless Rage prints DC 10 rising by 5 per use, so
  `InitialSavingThrowDC`/`SavingThrowDCIncreasePerUse`/`ResetsOnShortRest`
  are real data. The distinction is whether the book prints a number or a
  formula.

### Shared vocabulary in `Rules/Common`

**The bar for promoting a type is two independent domains needing the
identical concept** — not one domain plus a prediction. Promoted so far:

- `RollModifier` (`None`/`Advantage`/`Disadvantage`) — the most-referenced
  mechanic in the ruleset.
- `NextTurnDurationTrigger` (`EndOfYourNextTurn`/`StartOfYourNextTurn`) —
  combat-turn-relative duration, a genuinely different axis from
  `SpellDuration`'s calendar/real-time units. Four consumers now. The PHB
  uses two distinct phrasings, not one reworded: "until the end of your next
  turn" vs. "before the start of your next turn".
- `AbilityModifierUsesGrant` (`AbilityId` + `RecoversOnLongRest`) — the
  recurring "a number of times equal to your [ability] modifier (minimum
  once), regained on a long rest" resource; promoted on 4 independently
  verified instances. This one **replaced** an existing field
  (`WrathOfTheStormDetail.RecoversOnLongRest`) because the bool alone was
  half the fact. `MaxChallengeRating` staying a plain `double` was about the
  absence of a reuse case, not a rule against ever breaking a shipped field.
- `SpellGrant` + `SpellGrantFrequency` (`AtWill`/`OncePerDay`) — innate
  racial/subclass spell grants. A **list**, not a scalar, because Drow Magic
  and Infernal Legacy each grant three spells unlocking at 1st/3rd/5th level.
  Deliberately **not** merged with `EldritchInvocationDefinition`'s
  `GrantedSpellId`/`CastingFrequency` pair: an invocation grants one spell
  with no level gate and recharges on a *rest*; racial grants recharge on a
  *day*. Same idea, genuinely different facts.
  **Ritual-only casting is a real third frequency axis the enum lacks** —
  Spirit Seeker and Spirit Walker need it; add it when a second feature does.
- `ToolProficiencyChoice` — see "Tool proficiency grants". Promoted on
  three owning domains at once (Class, Subclass, Race) with a fourth
  (Background) already known, the clearest case yet for the two-domain bar.
- `SpeechRestriction` and `ExhaustionLevelEffect` deliberately stayed under
  `Rules/Creatures/Conditions/` — neither generalizes.
- `HunterMultiattackKind` deliberately stayed scoped to its own domain.

### Tool proficiency grants

Built on `ClassDefinition`, `SubclassDefinition`, `RaceDefinition`, and
`BackgroundDefinition` — all four owners. Two fields per owner:
`ToolProficiencyIds` (a fixed grant — Rogue's thieves' tools, Assassin's
disguise kit *and* poisoner's kit) and a nullable
`ToolProficiencyChoice` in `Rules/Common`, the sixth type promoted there.

**`ToolProficiencyChoice` states a choice two mutually exclusive ways, both
real in the PHB:** by family (`ToolFamilyIds`) or by an explicit named subset
(`ToolOptionIds`). The Dwarf's "the artisan's tools of your choice: smith's
tools, brewer's supplies, or mason's tools" is *not* a family choice — it
names three of the seventeen — which is the whole reason the second field
exists. The constructor rejects both-or-neither, a repeated entry, an explicit
list of one ("a choice of one is not a choice", the same rule
`ChoosableSavingThrowAbilityIds` uses), and a count that isn't smaller than
the number of explicit options.

**`ToolFamilyIds` is a list because the Monk's choice genuinely spans two
families** — "one type of artisan's tools **or** one musical instrument". No
other grant does. Pinned by
`CanonicalFile_MonkChoiceSpansTwoToolFamilies` and
`CanonicalFile_DwarfChoiceNamesThreeExplicitTools`.

**A fixed grant is not a one-option choice.** Druid's herbalism kit and
Rogue's thieves' tools populate the ID list with `ToolProficiencyChoice`
null, pinned by `CanonicalFile_FixedGrantsAreNotModelledAsChoices`. **The two
fields are independent and co-occur** — Criminal's "One type of gaming set,
thieves' tools" populates both, pinned by
`CanonicalFile_CriminalCombinesAFixedGrantWithAChoice`.

**Backgrounds signal "no tool proficiency" by omitting the line, not by
printing "None".** Every class block prints "Tools: None" where there is no
grant; Acolyte and Sage simply have no Tool Proficiencies line at all. Both
mean an empty list, but don't expect the same printed shape when reading a
new domain's stat block.

**Grants live outside the obvious owner's block — scan for the mechanic, not
the domain.** Reading only the twelve class proficiency blocks would have
found four of the seven grants; Dwarf's Tool Proficiency (p.20), Battle
Master's Student of War (p.73), and Assassin's Bonus Proficiencies (p.97) sit
in race traits and subclass features. Grep the tool *names* across the whole
book before concluding a proficiency sweep is complete.

**Vehicle proficiency is not a tool proficiency here.** The Tools table
(p.154) prints one row, "Vehicles (land or water)", whose cost and weight are
both `*` and whose footnote defers to the Mounts and Vehicles section — a
pointer, not an entry. Backgrounds meanwhile grant "vehicles (land)" and
"vehicles (water)" separately, a distinction `VehicleDefinition.Kind`
(`Land`/`Water`) already models. So no vehicle entries were added to
`tools.json` (still 37); `BackgroundDefinition.VehicleProficiencyKinds`
reuses `VehicleKind` instead, populated for Folk Hero (Land), Sailor (Water),
and Soldier (Land) and pinned by
`CanonicalFile_VehicleProficiencyUsesVehicleKindNotATool`.
**Check whether an existing catalog already carries the axis before widening
a different one.**

### Multiclassing proficiencies

The p.164 table is an embedded `MulticlassingProficiencyGrant` on
`ClassDefinition`, not a catalog — its natural key is the class that already
owns the row, so a catalog keyed by `ClassId` would duplicate it.

**Sorcerer and Wizard carry `null`, not an empty grant.** Both print an
em-dash; the constructor rejects a grant that grants nothing, so "grants
nothing" is expressed by the absence of the object. Pinned by
`CanonicalFile_SorcererAndWizardGrantNothing`.

**A skill choice has two forms and they are not interchangeable.** Bard
grants "one skill of your choice" (unrestricted); Ranger and Rogue grant "one
skill from the class's skill list". Hence `SkillChoiceFromClassSkillList`
beside the count, validator-rejected when the count is zero.

**Barbarian grants shields with no armor category at all**, which is why the
shield flag stays independent of the category list — the same shape
`ClassDefinition`'s own starting proficiencies use. Monk grants "Simple
weapons, shortswords", the category-plus-named-exception shape again.

**The grant is a strict subset of the class's own starting proficiencies,
and that is asserted rather than assumed** —
`CanonicalFile_GrantsAreAStrictSubsetOfStartingProficiencies` checks every
populated row against the class it belongs to, which catches a transcription
slip no per-row assertion would.

**Druid's "(druids will not wear armor or use shields made of metal)" stays
in the citation**, the same deliberate unmodelled gap the Druid's own
starting proficiencies already leave.

### Concentration

`ConcentrationRules` (p.203) is a singleton on `Dnd5e2014Ruleset`, the third
after `ArmorUsageRules`/`MountVehicleRules` and `CharacterAdvancementRules` —
a handful of flat constants with no named entries to key.

**The saving throw DC is stored as its printed floor plus its printed
divisor, never as a formula or a precomputed table.** "10 or half the damage
you take, whichever number is higher" becomes `MinimumSavingThrowDC` (10) and
`DamageDivisorForSavingThrowDC` (2); the max() is the consuming engine's job.
This is the same "store what's printed" line `MaterialCostGoldPieces` sits on.

**`EndedByConditionIds` is a real cross-domain reference, not an inferred
tag** — the bullet names `incapacitated` outright, so it resolves against the
Conditions catalog rather than being modelled as a bespoke bool.

**A required singleton with cross-domain references is a new wiring hazard.**
Unlike a list-shaped domain, which contributes no references when empty, a
non-nullable singleton *always* contributes them — so ten minimal-set
integrity tests across unrelated domains (expenses, vocabulary) suddenly
needed the referenced ability and condition in their definition sets.
`TestConcentration.RequiredAbility()`/`RequiredCondition()` exist for exactly
that, beside `TestConcentration.Create()` itself — the same shared-helper
precedent `TestCharacterAdvancement` set. **Expect this fan-out for the next
required singleton that references another domain, and prefer nullable or
list-shaped domains when the content genuinely allows it.**

**The DM's environmental-phenomena paragraph is declined** — "The DM might
also decide that certain environmental phenomena... require you to succeed on
a DC 10 Constitution saving throw" prints a real DC, but the trigger is
explicitly DM-discretionary, the same line every other DM-adjudicated rule
sits on. The section is cited to **p.203** even though that paragraph runs
onto p.204: the heading and body start on 203.

**Whether a given spell requires concentration is not stored here** — that
lives on each spell's own `SpellDuration`.

### Cross-domain references into other catalogs

`GrantedSpellId`/`SpellGrant` point *into* the Spells catalog from Eldritch
Invocations, Elemental Disciplines, Channel Divinity, Races, Subraces, and
Subclasses — the first references that run that direction (a spell's own
`AvailableToClassIds` runs the other way). Battle Master maneuvers, Totem
Warrior options, and several feature details reference `ConditionId`,
`CreatureSizeId`, `TravelPaceId`, `AbilityId`, `SkillId`, `WeaponId`, and
`DamageTypeId`. Each needs a `HashSet<…Id>` in `CatalogIntegrityValidator`.

**`CatalogIntegrityTests.PublishedCatalog_HasNoDanglingReferences` only
covers what it actually loads.** Its `CreateDefinitionSet` helper has
repeatedly passed empty lists for domains it didn't yet need (`spells`,
`magicSchools`, `conditions`, `travelPaces`, and now `tools`/`toolFamilies`)
— so a new cross-reference into one of those was invisible to it, three
separate times, while the test stayed green.
**Check what that test loads before trusting its green status to cover a new
reference**, and add a negative test that pins the check actually fires
(`MissingTotemWarriorTravelPaceReference_IsRejected`).

### Feature placement

**A scoping table is a candidate list, not a placement decision, and its
groupings are the least-verified thing in it.** Three misattributions were
caught by cross-checking `LevelFeatures` before writing data: Skill
Versatility (race, not subrace — Half-Elf has no subraces in this schema),
Relentless Rage (base Barbarian, not Berserker), and The Third Eye (10th
level, not 14th). A fourth kind of error is the silent one: Skill
Versatility's property was first added to `SubraceDefinition` with **no
constructor parameter feeding it** — a dead field nothing would have flagged
until a consumer hit an always-null value. Caught in self-review, not by a
test.

Notes worth keeping from specific placements:

- **Racial proficiency grants are narrower than the class shape on purpose.**
  No `WeaponProficiencyCategories` field exists — every racial grant names
  specific weapons. `SkillProficiencyChoiceCount` has no companion option
  list, because Half-Elf's "two skills of your choice" is unrestricted; the
  *absence* of the options field is itself the fact "any skill qualifies".
- **Two subraces sharing an identical proficiency list repeat the data
  rather than sharing an ID** — the same "template with zero difference"
  shape `RuleId`s use, just expressed as data.
- **A list is used only when the count is genuinely variable.** Mindless
  Rage's immunity is a `ConditionId` *list* (two conditions apply together);
  Beguiling Defenses' is a bare `ConditionId` (exactly one).
- **`InnateSpellcastingAbilityId` sits on Race/Subrace but not Subclass** — a
  subclass's granted spell uses the class's already-modelled
  `SpellcastingAbilityId`, and restating it would duplicate a held fact.
- **"Add the spellcasting modifier to damage" is a bool, not the declined
  "dice + modifier" line**, when the modifier *is* the whole fact (Agonizing
  Blast, Lifedrinker, Empowered Evocation, Second-Story Work's jump bonus).
  Elemental Affinity captures the bool with no damage type, because its type
  was already chosen by a different, declined feature.

## Conditions

`ConditionDefinition` carries 26 mechanical fields beyond `Id`/`Name`/
`Sources`, covering every bullet in PHB Appendix A (pp.290–292) for all 15
conditions. **The shape is still "many typed fields on one Definition", the
same precedent `ClassDefinition` (40+ fields) set — not a new DSL.** What
makes it look different is density: several conditions populate 6–8 fields
simultaneously (Unconscious is densest), which is exactly why no "exactly one
populated" constraint applies here.

- **Exhaustion is structurally different and gets its own nested
  `ExhaustionEffectDetail`** — `LevelEffects` (exactly 6, self-validated in
  the constructor since a wrong count is a data error, not a "some campaigns
  differ" case), `RecoversOneLevelPerLongRest`, `RecoveryRequiresFoodAndDrink`.
  The stored list holds each level's *incremental* new effect exactly as the
  table prints it; resolving the cumulative stack is a consuming engine's job.
- **`PreventsActionsAndReactions` is set directly on every condition that
  includes it, not resolved through a reference.** Paralyzed, Petrified,
  Stunned, and Unconscious all say "is incapacitated (see the condition)";
  the flag is set `true` on all five (including Incapacitated itself). This
  matches the standing preference for flattening a derived fact into direct
  data over requiring a lookup chain.
- **Prone's attack-roll rule is declined** — advantage within 5 feet,
  disadvantage beyond, is range-conditional, not a flat `RollModifier`.

## Combat, Adventuring, and Characters

**Chapters 8–9 were scoped, not built wholesale.** Most of both chapters is
DM-adjudicated prose or a linear-in-level formula (falling damage, jump
distance, attack rolls, breath-holding). Five sections were genuinely closed,
named sets and all five are built. **That table is closed, not a queue** —
everything else in Chapters 8–9 stays fully unbuilt, with no catalog and no
bare `RuleId` citation index either, since nothing needs to reference it. If
a sixth candidate is ever proposed, re-scope from the page images rather than
trusting a past skim; the first scoping pass found real numbers in Cover,
which a skim would have called prose.

- **`CombatActions`** (10, pp.192–193) is a plain closed-vocabulary catalog —
  `Id`/`Name`/`Sources` only, the `MagicSchoolDefinition` shape. None of the
  10 carries a fact worth a field (Dash's extra movement is just "equals your
  speed"). Section string is `"Chapter 9: Combat — Actions in Combat —
  {Name}"`, matching Magic Schools' per-entry suffix convention.
- **`CoverDefinition`** (3, p.196) — `ArmorClassBonus` and
  `DexteritySavingThrowBonus` are modelled as two fields even though they're
  numerically identical for every degree in this printing, because the PHB
  states them as two separate benefits that happen to share a number. Plus
  `PreventsBeingTargeted` for Total cover.
- **`TravelPaceDefinition`** (3, p.182) — three parallel distance fields plus
  `PassiveWisdomPerceptionPenalty` (Fast only) and `AllowsStealth` (Slow
  only): two unrelated effects sharing one table column get two unrelated
  fields, not one "effect" enum.
- **`RestTypeDefinition`** (2, p.186) — `MinimumDurationHours`,
  `CooldownHours`, `MinimumHitPointsToBenefit`.
- **`DowntimeActivityDefinition`** (5, p.187) — independent nullable scalars,
  validated so a saving-throw ability and its DC are both present or both
  absent. Practicing a Profession populates nothing and still gets an entry.
- **`CharacterAdvancementRules`** (20 rows, p.15) — this project's first
  Chapter 1 citation, and a singleton rather than a catalog (its natural key
  is an integer level). **Experience points and proficiency bonus are stored
  as printed, and the "2 + (level − 1) / 4" relationship is asserted in a
  test rather than used to generate the data** — the formula holds for this
  printing but is stated nowhere in the book; generating from it would
  silently manufacture any row the book disagreed with. *Tiers of Play* is
  declined on the book's own authority ("The tiers don't have any rules
  associated with them"). The maximum ability score of 20 is real but belongs
  to Ability Score Improvement, not here.

`TestCharacterAdvancement` is a shared test helper — ten integrity tests
construct `RulesetDefinitionSet` directly and need a non-null value they
aren't otherwise exercising.

## Open work: the rules-chapters scoping pass (2026-08-10)

Scoped the territory the feature-prose pass never touched — Chapter 1's
advancement table, Chapter 6 (Multiclassing), Chapter 7 (Using Ability
Scores), and Chapter 10 (Spellcasting's own rules, as opposed to
`SpellDefinition` header blocks).

| Candidate | Page | Verdict |
| --- | --- | --- |
| Character Advancement | 15 | **Built** |
| Multiclassing Proficiencies | 164 | **Built** |
| Encumbrance variant + size/strength multipliers | 176 | **Buildable, partial** |
| Concentration rules | 203 | **Built** |
| Multiclass Spellcaster slot table | 165 | **Declined — duplicate** |
| Ability score → modifier table | 173 | **Declined — pure formula** |
| Advantage/Disadvantage stacking | 173 | **Declined — engine behavior** |
| Jump distances | 182 | **Declined — formula** |
| Feats | 165–170 | Out of scope (not in the free SRD) |

- **Multiclassing Proficiencies (p.164) is built** — see "Multiclassing
  proficiencies" below. Its two tool-proficiency rows are what forced the
  tool-proficiency gap open rather than being declined.
- **The Multiclass Spellcaster table (p.165) is byte-identical to the
  already-built `full-caster` progression** — all 20 rows compared
  programmatically against `spell-slot-progressions.json`, zero differences. The derivation rule around it reduces
  to a per-class caster fraction already 1:1 with `SpellSlotProgressionId`.
  **Do not "add the missing multiclass table"; check it against `full-caster`
  first.**
- **Carrying capacity is mostly formula, but two parts are not.** Str × 15
  and its 2× push/drag/lift stay declined; the **size multiplier** is a real
  table keyed to `CreatureSizeId`, and **Variant: Encumbrance** carries real
  speed penalties. Partial build only.
- **Concentration is built** — see "Concentration" below.

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
single test keyed by class ID after accreting near-duplicates.

**Add every new loader and validator to `PublicApiBoundaryTests`.** That
convention silently lapsed for five catalogs, leaving internal types
unguarded. A new domain's `*DataFileTests` should also assert that
`Dnd5e2014Ruleset.Instance` exposes the same closure as the on-disk file —
data-file tests read from disk, so nothing else catches a data file added
without its `<EmbeddedResource>` csproj entry.

**Some tests exist specifically to block a plausible-looking
consolidation** — don't delete them as redundant. They're named inline
throughout this file; the pattern is a `CanonicalFile_*` or
`Preserves*`/`Does Not*` name asserting that two similar-looking things stay
different.

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
   decision established, not a narrative of the build.
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
