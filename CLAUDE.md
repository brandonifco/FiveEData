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

**Nothing in progress. All five choice-point catalogs are closed**
(Metamagic, Battle Master maneuvers, Eldritch Invocations, Elemental
Disciplines, Channel Divinity — the last now 16 entries, not 10; see
below). The wider race/subclass/background feature-prose tail was
**scoped** in a systematic pass (2026-08-10) — see "Game-backend
quantization: scoping the race/subclass/background feature-prose
tail" — and has a large, ranked candidate pool. One slice (Paladin's 6
missing Channel Divinity options) is now also **built**; every other
candidate in the pool is scoped-not-built. Ask the user which to pick
up next.

Built and complete:

- Equipment (weapons, armor, shields, adventuring gear, tools, mounts,
  vehicles, trade goods)
- Expenses (lifestyles, food & drink, hospitality, mundane services)
- Creature vocabulary (abilities, skills, languages, sizes, conditions,
  damage types, senses, alignments)
- Races — all 9 PHB races, all 9 subraces
- Classes — all 12 PHB classes, all 40 subclasses
- Backgrounds — all 13 PHB backgrounds
- Quantized mechanics — the second pass (see that section). **Complete
  against its own enumerated list, not against a sweep** — a later sweep of
  all 305 `class-rule` entries found a real tail it never listed; see
  "Quantized mechanics: the remaining tail". Cantrips Known, Spells Known,
  and the Wizard spellbook (the follow-ups flagged during the spell slot
  pass) are also built; see "Quantized mechanics: cantrips known, spells
  known, and the Wizard spellbook"
- Spells — `MagicSchools` (the 8 schools, a closed official set, cited to
  the p.203 sidebar) and `SpellDefinition`. **All 27 cantrips and all
  spells of every level 1 through 9 are built — 361 spells, the complete
  real set.** See "Spells: the Trap the Soul appendix error" below for
  why 361 is the correct final count, not 362. **Every spell level, 0
  through 9, now also carries real damage/condition effect data** — 78
  of the 361 spells carry a `SpellDamageEffect` and/or
  `SpellConditionEffect` (59 damage grants, 24 condition grants; some
  spells, like Ray of Sickness and Evard's Black Tentacles, carry both).
  See the "Game-backend quantization: ... spell effects" sections below,
  one per level (cantrips through 9th) — cantrips and 1st level drove
  almost all of the schema (choosable damage types, half-damage-on-a-
  successful-save, a flat leveled-spell base damage independent of
  character level, and named `SpellConditionEffect` grants), 6th level
  added one more field (`FlatDamageBonus`, for the rare "dice + flat
  number" spell), and every other level reused the existing shapes with
  no schema growth at all. This closes gap 1 of the game-backend
  initiative below.
- Combat/adventuring rules — **all five scoped catalogs are built.**
  `CombatActions` (10 named actions), `Cover` (3 degrees), `TravelPace`
  (3 paces), `RestTypes` (2 rest types), `DowntimeActivities` (5 named
  activities). See "Combat/adventuring rules: scoping and the Actions in
  Combat catalog" and "Combat/adventuring rules: Cover, Travel Pace,
  Resting, and Downtime Activities" — everything else in PHB Chapters 8–9
  stays unbuilt by design (DM-adjudicated prose or linear-in-level
  formulas), not a gap to fill later.
- Conditions — **all 15 PHB conditions (Appendix A) now carry full
  mechanical payloads**, not just `Id`/`Name`/`Sources`. See "Game-backend
  quantization: Conditions" below.
- Gap 3 (feature-effect prose) — **all five choice-point catalogs
  built.** Metamagic (all 8 options), Battle Master maneuvers (all 16),
  Eldritch Invocations (all 32), Elemental Disciplines (all 17), and
  Channel Divinity options (**16**, not 10 — the original catalog only
  covered the 7 Cleric domains; Paladin's 3 oaths add 6 more) are done.
  See "Game-backend quantization: gap 3 scoping, and Metamagic
  effects", "Game-backend quantization: Battle Master maneuver
  effects", "Game-backend quantization: Eldritch Invocation effects",
  "Game-backend quantization: Elemental Disciplines and Channel
  Divinity effects", and "Game-backend quantization: scoping the
  race/subclass/background feature-prose tail" below. **This closes
  the five choice-point catalogs, not every feature-effect prose in
  the codebase** — a large, ranked pool of individual race/subclass/
  background features (outside these five catalogs) is scoped but
  mostly unbuilt; see the scoping section for the full list.

**"Complete" means citation-complete, not mechanically quantized.** Most
named features across Classes/Races/Backgrounds are still a `RuleId`
citation with no mechanical payload — the quantized pass covered leveled
numbers and choice-point options, not every feature. Check the inventory
under "Quantized mechanics" before assuming a feature exposes real numbers.

**A second, larger initiative started 2026-08-09: making the catalog
directly useful for a game backend**, not just citation-complete. An
audit found three real gaps, ranked by size: (1) Spells have zero effect
data — no damage dice, save DC/ability, or condition applied, only the
header block; (2) Conditions carried zero mechanical payload; (3) most
class/subclass/race/background feature *text* is unquantized prose
(Metamagic effects, maneuver secondary effects, invocation benefits).
Conditions (gap 2) went first — smallest, and a dependency for the other
two, since spell/feature effects constantly reference named conditions.
Spells (gap 1) went second, cantrips first; feature-effect prose (gap 3)
is next. **This deliberately reverses two lines stated below as settled
architecture**: "no generic effect DSL" and "Spells are not a modeled
domain" (the latter was already stale — `SpellId`/`SpellDefinition` exist
as a real domain; only the *effect* stayed unmodeled). The reversal is
narrower than it sounds: Conditions used the *same* "many typed fields on
one Definition" shape every other domain already uses, not a new DSL —
see "Game-backend quantization: Conditions" for why that shape held up
even at 26 fields.

**Gap 1 (Spells) is now fully closed — every level, cantrips through
9th, has real damage/condition effect data.** It was built cantrips
first (proving the shape on a small closed set, the same strategy the
original header-block build used), then level by level; see the ten
"Game-backend quantization: ... spell effects" sections below for the
full per-level build log. The two mechanism fields —
`SpellDamageEffect` (attack roll or saving throw, damage type,
half-damage-on-a-successful-save, and either a cantrip's
character-level progression or a leveled spell's flat base damage plus
optional `FlatDamageBonus`) and `SpellConditionEffect` (one or more
named Appendix A conditions gated by a saving throw) — cover 78 of the
361 spells; the schema itself was essentially settled after 1st level,
with only one further addition (`FlatDamageBonus`, at 6th level) across
the remaining eight levels. **Feature-effect prose (gap 3) is the
initiative's last remaining piece — now scoped and started.** See
"Game-backend quantization: gap 3 scoping, and Metamagic effects" below
for the ranked candidate list and the first slice (Metamagic, all 8
options).

Gate as of the last merge: Debug+Release build 0 warnings, **2989 tests**.

## Spells: the Trap the Soul appendix error

**"Trap the Soul" is not a real spell in this PHB printing — it was
never supposed to be part of the Spell Descriptions section, which is
why no description page exists for it.** It's confirmed *not findable*
anywhere in its correct alphabetical position in
`~/Downloads/Player's Handbook.pdf`: the text runs continuously from
Transport via Plants into Tree Stride with no gap, verified against
high-resolution renders of every page from p.279 through p.285 (the
entire T range) plus a full-text search of the whole book. Its only
appearance anywhere in the book is the Wizard class list on p.212 —
that appendix entry is itself the error, a leftover or misprint that
was never backed by an actual spell entry. This is the same "the
appendix is a summary list, not the primary source" rule that already
corrected Destructive Wave's name and the Warlock ability-score-
improvement table — here the appendix doesn't just get a detail wrong,
it names a spell that doesn't exist at all. **Treat this as settled,
not as a pending gap**: 8th level's real PHB count is 18, not 19, and
Wizard's real 8th-level count is 13, not 14 — both already reflected in
`SpellDataFileTests`. Do not build Trap the Soul from memory or from a
different source, and do not treat a future PDF that happens to include
it as corrective; this printing's absence is correct.

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

**Every level was added in alphabetical batches** — 62 first-level spells over
~38 pages was too much to read reliably in one pass, second level's 59 was no
better, and third level's 50 (split A–C/D–H/L–P/R–W, ~12–13 per batch for
tighter accuracy than the first two levels' ~15–16) confirmed the pattern a
third time. `SpellDataFileTests` pins the exact built closure, so a partial
level was asserted rather than implied while in progress. Levels 7–9 kept
the pattern at a smaller scale — two batches of ~8–10 each, since those
levels are much smaller than 1st–6th.
Per-level union counts are **not monotonically declining**: 62/59/50 for
1st–3rd, 35 at 4th, back up to 42 at 5th, down to **32 at 6th, 20 at 7th,
18 at 8th, then 16 at 9th** — the 8th-level appendix union naively reads
as 19, but one of its entries (Trap the Soul, Wizard) is an appendix
error with no real spell behind it; see "Spells: the Trap the Soul
appendix error". Paladin and Ranger's lists really do stop entirely at 6th, as
expected — but Warlock's Pact Magic *slots* cap at 5th level while the
class's own spell list keeps going through 9th, for spells eligible as a
Mystic Arcanum (one higher-level spell known and cast once per day with
no matching slot). The file's earlier claim that all three classes would
drop out together at 6th was wrong for Warlock specifically; verify the
real count each level rather than assuming the trend. Warlock's 7th-level
count (4) is smaller than 6th's (8), but 8th's (5) ticks back up, and
9th's (5, tying 8th) holds rather than either climbing or shrinking
further — confirming the hedge was right: Mystic Arcanum eligibility
tracks which spells are worth learning at each level, not a shrinking
slot count, so the four-level run (8/4/5/5) has no single trend to
extrapolate from.

Each batch drove schema additions from real content, never anticipation:

- **1st A–C:** `IsRitual` (Alarm, Comprehend Languages); area-of-effect
  ranges via `SpellRange.SelfWithArea` (Arms of Hadar's radius, Burning
  Hands and Color Spray's cones); `MaterialCostGoldPieces` (Chromatic Orb's
  50 gp diamond). A cantrip may never be a ritual — the validator enforces
  it.
- **1st D–F:** the `Reaction` casting-time unit (Feather Fall) and `Hour`
  (Find Familiar); `MaterialIsConsumed` (Find Familiar's charcoal and
  incense).
- **1st G–P:** the `Day` duration unit (Illusory Script's 10 days).
- **1st R–W:** the `Cube` area shape (Thunderwave's 15-foot cube) — the
  first area that is neither a cone nor a radius.
- **2nd A–C:** `SpellDuration.UntilDispelled()` and its `IsUntilDispelled`
  flag (Arcane Lock, Continual Flame) — a **third duration kind**, not an
  unbounded amount, so it carries neither amount nor unit and is never
  concentration or "up to". The validator enforces exactly one of the three
  kinds.
- **2nd D–H:** the `Line` area shape (Gust of Wind's 60-foot line) — which
  this file had expected Lightning Bolt to bring at third level, a reminder
  that the *first* spell to need a shape is rarely the memorable one. Find
  Steed's 10-minute casting time is the first whose amount is not 1; the
  field already allowed it, so no schema change was needed.
- **2nd I–P:** nothing. The first batch of the project to need no schema
  change at all — evidence the header-block shape has converged, not that
  the batch was read carelessly.
- **2nd R–Z:** nothing either. Two consecutive schema-quiet batches closed
  second level; treat the shape as settled and expect level 3 to add at
  most an area shape or two.
- **3rd A–C:** nothing either — the third schema-quiet batch in a row.
  Clairvoyance's range is printed as "1 mile", the first PHB range not
  stated in feet; `SpellRange.DistanceFeet` has no separate unit (unlike
  casting time and duration), so it's canonicalized to 5,280 — a unit
  conversion, not a derived total like Warding Bond's per-item cost, so it
  doesn't trip the "store what's printed" rule. Blink's duration is a flat,
  non-"up to" **1 minute** — the same flat-vs-concentration distinction
  Shield/True Strike already pinned at the Round unit, now shown at Minute
  too; no schema change needed since the shape already allows it.
- **3rd D–H:** nothing either — the fourth schema-quiet batch. Glyph of
  Warding's "Until dispelled or triggered" maps to the existing
  `IsUntilDispelled` flag, the same call already made for Magic Mouth; no
  new duration kind needed for a second termination condition. A re-read of
  the class spell list appendix while placing Hypnotic Pattern caught a
  column-boundary miss from the A–C batch: Wizard's 3rd-level count is 29,
  not the 28 first recorded below — the per-class figures are corrected.
- **3rd L–P:** two additions. `SpellAreaShape.Hemisphere` for Leomund's
  Tiny Hut's "Self (10-foot-radius hemisphere)" — geometrically distinct
  from a full-sphere `Radius` aura, so it earns its own value rather than
  reusing one. And a genuinely new problem: Plant Growth prints **"Casting
  Time: 1 action or 8 hours,"** two alternative casting times that produce
  different effects (an instant local burst vs. a slower, wider blessing).
  `SpellCastingTime` has no shape for "either/or." Declined rather than
  extended: the field stores the 1-action primary value, and the 8-hour
  alternate mode — which changes the spell's *effect*, not just its speed —
  stays in the citation, the same "content this project doesn't model as
  its own domain" line Rock Gnome's Tinker sits on. Revisit only if a
  second spell needs the same shape; one data point doesn't justify a new
  mechanism field. Lightning Bolt also became the second `Line` area,
  after Gust of Wind.
- **3rd R–W (closing the level):** one addition. Sending prints **"Range:
  Unlimited"** — no distance at all, not a large bounded `Distance`.
  `SpellRangeKind` gained an `Unlimited` member and `SpellRange.Unlimited()`
  factory, the same "self/touch/distance" enumeration shape extended by one
  case; Telepathy (8th level, unbuilt) uses the same word, confirming it's
  a real PHB category and not a one-off worth declining. Tongues is V+M
  with no S, joining the rarest V/S/M combination (5 spells across 4
  levels now). Third level is closed at exactly 50 — the class-list
  appendix union predicted this count before any R–W spell was read, and
  it held.
- **4th A–D:** nothing — the fifth schema-quiet batch. Dimension Door is
  Verbal-only, but that combination already existed (26 spells now); no new
  fact, so it earns no pinning test. Aura of Life and Aura of Purity reuse
  `SpellAreaShape.Radius` at 30 feet, the same shape Aura of Vitality
  already established.
- **4th D–I:** nothing — the sixth schema-quiet batch in a row. Fabricate
  and Hallucinatory Terrain both join the existing "10 minutes" casting-time
  group, no longer a rarity (5 spells now). **This batch's insertion pass
  caught its own mistake twice**: Divination and Grasping Vine were each
  first inserted after the wrong alphabetical neighbor (`Divine Favor` and
  `Grease` respectively, both a plausible-looking but incorrect anchor) —
  caught by the same `sorted?` check every batch runs before the gate, not
  by a human reviewer. The check exists precisely for this: verify it after
  every batch, not just when something looks off.
- **4th L–W (closing the level):** one decline. Leomund's Secret Chest
  prints **two separately-costed material items** in one description — a
  5,000 gp chest and a 50 gp replica, both required — not one figure or a
  per-item cost like Warding Bond's. No single number represents "the"
  cost, so `MaterialCostGoldPieces` stays null rather than picking one of
  the two figures: the same partial-decline shape Plant Growth's compound
  casting time used at third level, now the second instance of the same
  pattern. Locate Creature reaches six classes, matching Hold Person's
  second-level widest membership. Fourth level closed at exactly 35 — the
  class-list appendix union predicted this before any L–W spell was read,
  the same way third level's did.
- **5th A–C:** nothing — the seventh schema-quiet batch. Awaken's casting
  time is **"8 hours,"** the first Hour-unit casting time whose amount
  isn't 1 (Find Familiar and Glyph of Warding are both flat 1 hour);
  `SpellCastingTime` already allows any amount, so no change was needed —
  the same non-event as Find Steed's 10-minute amount at first level.
  Antilife Shell and Circle of Power reuse `SpellAreaShape.Radius`, and
  Cone of Cold reuses `Cone`, both already-established shapes.
- **5th C–G:** the busiest batch since first level. Creation prints
  **"Duration: Special"** (the real duration is a table keyed by the
  material created, 1 day down to 1 minute) and Dream prints **"Range:
  Special"** (reach is "same plane of existence as the target," a
  conditional rule, not a distance). Both got a proper flag —
  `SpellDuration.IsSpecial` and `SpellRangeKind.Special` — rather than a
  per-spell hack, the same "carries no amount/unit of its own" shape
  `IsUntilDispelled` already established; the validator now caps
  Instantaneous/UntilDispelled/Special at one true flag instead of two.
  **Adding `IsSpecial` as `[JsonRequired]` meant every one of the 244
  already-built spells needed the field added to stay loadable** — done
  as one scripted pass over the data file (insert `"isSpecial": false`
  after every `"isUntilDispelled"` key), verified byte-for-byte afterward
  to touch nothing else. This is the standing cost of a required field on
  a domain with this many entries; expect it again for the next
  compound-fact discovery. Contagion and Geas also use the `Day` duration
  unit (7 and 30 days), breaking `DayLongDurationsAreIllusoryScriptAnd
  GentleRepose`'s "always 10" assumption.
  **The Paladin spell list appendix prints "Destructive Smite," but the
  spell's own description page headers it "Destructive Wave"** — its real
  published name, and a plausible appendix slip given the Paladin list's
  six other "___ Smite" spells already built. The description page wins,
  same "errata and prose beat printing artifacts" rule as the Dwarf's
  throwing hammer; pinned by
  `DestructiveWaveIsNamedFromItsDescriptionNotTheAppendix` so a future
  pass doesn't "helpfully" rename it back.
- **5th G–P:** nothing new structurally, but two recurring-pattern
  confirmations. Legend Lore is the **second** spell (after Leomund's
  Secret Chest) with a two-item material bundle and no single cost figure
  — incense at 250 gp, four ivory strips at 50 gp each — declined the same
  way, `MaterialCostGoldPieces` null; unlike Leomund's Secret Chest, one
  item's consumption *is* stated ("which the spell consumes" on the
  incense), so `MaterialIsConsumed` is `true` even though the field can't
  represent "only one of the two items." Hallow is a plain **"Until
  dispelled"** with no trigger clause (unlike Magic Mouth/Glyph of
  Warding's "or triggered"), the fifth spell on that flag and still
  costed-and-consumed. Hallow (24) and Planar Binding (1) also extend the
  Hour-unit casting times past Awaken's 8, so the "amount 1" default is
  now the minority among Hour-unit spells (2 of 5).
- **5th R–W (closing the level):** nothing — the eighth schema-quiet
  batch. Teleportation Circle is V+M with no S, the sixth spell on that
  combination and the first non-cantrip/non-1st-level one since Tongues.
  Raise Dead and Reincarnate both add a third and fourth Hour-unit
  casting-time amount of 1, alongside Planar Binding's. Fifth level
  closed at exactly 42 — the class-list appendix union predicted this
  before any R–W spell was read, matching third and fourth level's same
  pattern. **All five levels 1–5 are now complete: 275 spells total.**
- **6th A–E:** one correction, no schema change. Drawmij's Instant
  Summons is the first "Until dispelled" spell whose material cost isn't
  paired with stated consumption — the PHB says a fresh sapphire is
  needed each casting but never uses the word "consumes," so
  `MaterialIsConsumed` stays `false` even though every prior
  until-dispelled spell (Arcane Lock, Continual Flame, Glyph of Warding,
  Hallow, Magic Mouth) was both costed *and* consumed. Contingency's
  10-day duration rejoins the existing 10-day group (Gentle Repose,
  Illusory Script) rather than adding a fourth span. Create Undead's
  "150 gp black onyx stone for each corpse" stores 150, not a
  multiplied total, the same per-item convention Warding Bond set.
- **6th F–M:** nothing structurally new, but two independent facts
  finally break apart on the same feature. Find the Path is the first
  Day-unit duration that requires concentration ("Concentration, up to
  1 day"), while Forbiddance stays a flat "1 day" — the same
  unit-doesn't-imply-shape distinction Shield/True Strike already pin at
  the Round unit. Magic Jar is the second "Until dispelled" spell (after
  Drawmij's Instant Summons) that's costed without being stated as
  consumed. Mass Suggestion (V+M, no S) is the seventh spell on that
  combination.
- **6th M–W (closing the level):** one addition, one correction, and one
  missed spell. Sunbeam's **"Self (60-foot line)"** is the third `Line`
  area after Gust of Wind and Lightning Bolt, and the first outside
  cantrip/1st/2nd level — no schema change needed, the shape already
  allowed it. Otto's Irresistible Dance and Word of Recall are both
  Verbal-only, joining True Strike/Vicious Mockery/Wrathful Smite on that
  rare combination. Programmed Illusion is the second "Until dispelled"
  spell that's costed without being stated as consumed (after Magic Jar).
  **Wall of Thorns was missed entirely on the first read-through** of this
  batch — it never made the initial per-page description pass even though
  it's on the Druid class list — and surfaced only because
  `ClassSixthLevelListHasExpectedSize`'s Druid count failed at 8 instead
  of 9 after the other ten spells were added. The PHB's real 6th-level
  count is 32, not the 31 this file previously stated; the class-list
  union math never actually promised the built-spell count would match a
  number carried over from before any M–W spell was read. **A second,
  unrelated error surfaced in the same pass:** Conjure Fey (built in the
  A-E batch) was tagged available to Wizard, but the Wizard class list on
  p.212 doesn't include it — only Druid and Warlock do. Fixed in the same
  commit, per the standing rule to fix an error where it's found rather
  than filing it separately. **All 32 of the PHB's sixth-level spells are
  now built: 307 spells total, levels 0–6 complete.**
- **7th A–M (starting level 7):** one schema addition. Mirage Arcane
  prints **"Range: Sight"** — reach is whatever the caster can see, not a
  bounded distance and not the same conditional-rule shape as Dream's
  Special. `SpellRangeKind` gained a `Sight` member and
  `SpellRange.Sight()` factory, the same "self/touch/distance" enumeration
  shape Unlimited and Special already extended. Etherealness is a flat
  **"Up to 8 hours" with no concentration** — the same unit-doesn't-imply-
  shape distinction Prestidigitation already pins at the Hour unit, now
  shown on a non-cantrip. Mordenkainen's Magnificent Mansion is the third
  spell (after Warding Bond and Legend Lore) with a per-item material
  cost — three items "each item worth at least 5 gp" — but unlike Legend
  Lore's two different figures, all three share one cost, so the printed 5
  gp is stored directly, the same convention Warding Bond set. Etherealness
  reaches five classes (Bard, Cleric, Sorcerer, Warlock, Wizard), the
  widest membership since Hold Person's six at second level.
- **7th P–T (closing the level):** two new Hour-unit casting times.
  Resurrection is a flat 1 hour, rejoining the existing amount-1 group;
  Simulacrum's **12 hours** is a fourth distinct Hour-unit amount (after
  1, 8, and 24). Plane Shift, Project Image, Resurrection, Sequester,
  Simulacrum, and Symbol add six more entries to the costed-materials
  list, and Sequester/Simulacrum/Symbol are all "Until dispelled" and
  both costed and consumed — Symbol's is technically "Until dispelled or
  triggered," the same trigger-clause shape Magic Mouth/Glyph of Warding
  already established. Prismatic Spray is the fourth `Cone` area (Self,
  60 feet), after Burning Hands, Color Spray, and Cone of Cold. Project
  Image prints **"Range: 500 miles"** — the second range printed in miles
  rather than feet (after Clairvoyance's 1 mile), canonicalized to
  2,640,000 feet by the same unit-conversion rule. **All 20 of the PHB's
  seventh-level spells are now built: 327 spells total, levels 0–7
  complete.**
- **8th A–F (starting level 8):** no schema additions, but two recurring-
  pattern confirmations and one new component combination. Clone is the
  third spell (after Warding Bond's per-item cost and Legend Lore/
  Leomund's Secret Chest's two-item decline) with a two-part material
  bundle — a 1,000 gp diamond and a 2,000 gp vessel — so
  `MaterialCostGoldPieces` stays declined; its consumed flesh component
  still makes `MaterialIsConsumed` true, the same partial shape Legend
  Lore already used. Control Weather's **"Self (5-mile radius)"** is the
  first area size printed in miles rather than feet, canonicalized to
  26,400 by the same unit-conversion rule Clairvoyance and Project Image
  already established. Demiplane is Somatic-only — no verbal, no
  material — the first non-cantrip spell on that combination (True
  Strike is the only cantrip on it).
- **8th G–T:** nine of the ten remaining spells, completing the level's
  real content at 18 — **Trap the Soul, the tenth name on the Wizard
  class list, is a PHB appendix error with no backing spell; see
  "Spells: the Trap the Soul appendix error" above. This is not an
  oversight to silently fix.** Telepathy's "Range:
  Unlimited" confirms Sending's range category is real and recurring,
  not a one-off, the same confirmation pattern as Mirage Arcane and
  Tsunami both landing on "Range: Sight" (Tsunami is the second, closing
  the loop this file predicted when Mirage Arcane was built at 7th).
  Tsunami's "Concentration, up to 6 rounds" is also the first Round-unit
  duration whose amount isn't 1 (Shield and True Strike are both 1) — the
  field already allowed it, the same non-event as Find Steed's 10-minute
  casting time. Holy Aura's material is costed (1,000 gp) but not stated
  as consumed.
- **9th A–P (starting level 9):** one addition, one new decline shape.
  Astral Projection prints **"Duration: Special"**, the second spell on
  that flag after Creation — its "special" is a compound end-condition
  rule (dismissal, dispel magic, 0 hit points, or a severed silver cord),
  the same "leave the compound rule in the citation" call Creation's
  table already made. Astral Projection's material is also costed **per
  creature affected** ("for each creature you affect... one jacinth
  worth at least 1,000 gp and one ornately carved bar of silver worth at
  least 100 gp") — Clone's two-item shape plus a multiplier neither
  field can represent, so `MaterialCostGoldPieces` stays null. A second,
  more novel decline: **Imprisonment breaks the "until dispelled is
  always costed" pattern** eleven prior spells held — its cost is
  printed as "500 gp per Hit Die of the target," a formula rather than a
  flat figure, so it declines the same way Clone and Astral Projection
  do, just triggered by a formula instead of a multi-item bundle or a
  multiplier. Meteor Swarm's "Range: 1 mile" is the third range printed
  in miles (after Clairvoyance, Project Image), canonicalized to 5,280
  feet by the same rule.
- **9th P–W (closing the level, and the Spells domain's level coverage):**
  no schema additions. Storm of Vengeance's "Range: Sight" is the third
  spell on that category (after Mirage Arcane and Tsunami), closing out
  the pattern predicted when Mirage Arcane was built at 7th level.
  Shapechange's material is costed (1,500 gp jade circlet, worn rather
  than expended) but not stated as consumed — the "costed, not consumed"
  quadrant Chromatic Orb already established, still showing up at the
  very top of the level range. **All 16 of the PHB's ninth-level spells
  are now built. Every PHB spell level, 0 through 9, is now fully
  built: 361 spells total — the complete real set, since Trap the
  Soul was never a real spell to begin with (see "Spells: the Trap the
  Soul appendix error").**

**Material cost and consumption are independent fields, and all four
combinations now exist** — Chromatic Orb costed-not-consumed, Identify the
same at 100 gp, Illusory Script and Find Familiar both, and Protection from
Evil and Good consumed with no stated cost. **Three** PHB phrasings carry a
cost: "worth at least X gp", "X gp worth of", and — new at second level —
a bare "worth X gp" (Continual Flame's ruby dust).

**`MaterialCostGoldPieces` holds the figure the PHB prints, never a derived
total.** Warding Bond's "a pair of platinum rings worth at least 50 gp each"
stores 50, not 100; the "each" survives in `MaterialDescription`, so a
consumer that wants the total can still compute it. Pinned by
`WardingBondStoresThePrintedPerItemCostNotTheTotal`.

**A spell entry is cited to the page bearing its name heading**, even when
its stat block or body runs onto the next page. Armor of Agathys set this
precedent at first level; Alter Self and Branding Smite follow it at second.
This is the one place the general "cite where the body text starts" rule is
read as "where the entry starts" — a spell's citation backs its header
block, and the header block begins with the name.

**A reaction spell's trigger stays in the citation.** The PHB states it as
prose ("which you take when you or a creature within 60 feet of you falls");
only the unit is data, consistent with never storing rules text.

**Still omitted until content needs them:** area shapes beyond
`Cone`/`Radius`/`Cube`/`Line`/`Hemisphere` — the PHB also uses a cylinder
(Flame Strike) and a wall, neither of which a built spell has reached yet.

**`AvailableToClassIds` comes from the Chapter 11 class spell lists
(pp.207–210), never from the spell description** — the description never
names its classes. Hold Person reaches six classes, the widest membership
built so far; the Paladin smites reach exactly one. Those four pages are laid
out in **four narrow columns**, not the two the description pages use — read
them as quadrant crops or a column's continuation gets missed. The 2nd-level
union of all eight class lists is **59 spells**; the per-class counts are
Bard 22, Cleric 17, Druid 18, Paladin 8, Ranger 13, Sorcerer 24, Warlock 12,
Wizard 34. The 3rd-level union is **50 spells**; the per-class counts are
Bard 16, Cleric 20, Druid 13, Paladin 10, Ranger 5, Sorcerer 20, Warlock 12,
Wizard 29 — read off the same pp.207–210 appendix, whose PDF page number is
the printed page number **plus one** in the current
`~/Downloads/Player's Handbook.pdf` (verify this offset again if the PDF is
ever replaced). The Wizard figure was first recorded as 28 and corrected to
29 during the D–H batch — a four-column class list page is genuinely easy
to miss one line on; re-verify a list against the image again before
trusting a count carried over from an earlier batch. The 4th-level union is
**35 spells**; the per-class counts are Bard 8, Cleric 8, Druid 16, Paladin
6, Ranger 5, Sorcerer 10, Warlock 4, Wizard 23. The 5th-level union is
**42 spells**; the per-class counts are Bard 16, Cleric 13, Druid 14,
Paladin 6, Ranger 4, Sorcerer 11, Warlock 4, Wizard 23 — every list grew
or held from 4th to 5th, since it's Paladin and Ranger's *last* level
before their lists stop entirely, which is why the union rose instead of
continuing to fall. The 6th-level union is **32 spells**; the per-class
counts are Bard 7, Cleric 10, Druid 9, Paladin 0, Ranger 0, Sorcerer 10,
Warlock 8, Wizard 20 — the Wizard figure was first recorded as 17 (a
four-column class list page split its 6th-level entries across a column
boundary, the same kind of miss the 3rd-level Wizard count made) and
corrected once the M–W batch's own read of the page caught it. Paladin
and Ranger drop to zero as expected, but
**Warlock does not** — its 8 sixth-level entries are spells eligible for
Mystic Arcanum (a feature that grants one 6th–9th-level spell known, cast
once per day with no spell slot), even though Pact Magic's actual slots
never exceed 5th level. Read this distinction again at 7th–9th: Warlock's
per-level counts there come from the same Mystic Arcanum eligibility, not
from slots, and should keep shrinking (one eligible spell learned per
level from 11th to 17th) rather than tracking the full-caster pattern.

**A duration unit does not imply a duration shape.** Shield and True Strike
are both 1 round, but Shield's is flat while True Strike's is concentration
and "up to". Pinned by
`ShieldsRoundDurationIsFlatWhileTrueStrikesIsUpTo`.

Not started: everything in PHB Chapters 8–9 outside the five scoped
catalogs (Actions in Combat, Cover, Travel Pace, Resting, Downtime
Activities) — by design, not a gap. See "Combat/adventuring rules:
scoping and the Actions in Combat catalog" for the full inventory.
**Magic items are explicitly
out of scope**: they're 2014 DMG content (Chapter 7: Treasure), not PHB —
confirmed again 2026-08-09 when asked to build them, since this project's
scope is the PHB specifically. Revisit only if the project's scope statement
itself is deliberately widened to include the DMG, not as a one-off
exception. Feats (and by extension Variant Human) are out of scope — not
part of the free 2014 SRD this project's provenance model is built around.

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
  the Paladin auras work, six during Battle Master maneuvers, ten during the
  Fighter quantized pass). **Off-by-one errors cluster but do not run
  uniformly** — the Fighter block had seven rules one page late and three more
  one page late from a different starting page, while three neighbours in
  between were already correct. Check each entry against the image; never
  extrapolate a shift across a range. Fix them where you find them, in the
  same commit.
- Errata and body prose beat printing artifacts. The Dwarf trait's printed
  "throwing hammer" is corrected to "light hammer" by official errata, and
  that's what's stored. The Warlock table omits a 19th-level Ability Score
  Improvement row that the feature's own text explicitly names — the prose
  wins, pinned by
  `PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission`.
  This extends to a spell's own *name*: the Paladin class spell list
  appendix prints "Destructive Smite," but the spell's own description
  page headers it "Destructive Wave" — its real published name. The
  appendix is a summary list; the description page is the primary source,
  same as prose beating a table.

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
- **Embedded on `ClassDefinition`:** Action Surge, Indomitable, Rage, Brutal
  Critical, Fast Movement, Favored Enemy, Natural Explorer, Sneak Attack, Ki,
  Martial Arts, Unarmored Movement, Sorcery Points, Wild Shape, Bardic
  Inspiration, Song of Rest, Magical Secrets, Channel Divinity uses, Destroy
  Undead, Mystic Arcanum, Font of Magic conversion, Aura of Protection, Aura
  of Courage, Eldritch Invocations known, Cantrips Known, Spells Known,
  Wizard Spellbook.
- **Embedded on `SubclassDefinition`:** Divine Strike, Circle Forms, Combat
  Superiority, Disciple of the Elements, Aura of Devotion, Aura of Warding,
  Additional Magical Secrets (the same `MagicalSecretsProgressionDetail` the
  Bard uses), Portent, Draconic Resilience, Improved Critical, Shadow Step,
  Hurl Through Hell, Wrath of the Storm, Thunderbolt Strike, Shadow Arts and
  Quivering Palm ki costs, Draconic Presence's sorcery point cost, Bend Luck.
- **On `RaceDefinition`/`SubraceDefinition`:** `DarkvisionRangeFeet`,
  `ResistedDamageTypeIds`, `TranceDurationHours`, `HitPointBonusPerLevel`,
  the subrace `Speed` override, the embedded Breath Weapon progression, and
  Savage Attacks, Relentless Endurance, and Lucky.
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

## Quantized mechanics: the remaining tail

The pass above closed when its enumerated list was done. A later sweep of all
305 `class-rule` entries found features carrying clean numbers that were never
on that list — so **"the pass is complete" meant list-complete, and the
distinction is load-bearing.** Martial Arts, Unarmored Movement, Brutal
Critical, Fast Movement, Action Surge, Indomitable, Destroy Undead, Favored
Enemy, Natural Explorer, Magical Secrets, Additional Magical Secrets, Portent,
and Draconic Resilience are done — all of Tier A. The Tier B list below is
verified-as-candidates by name only and still needs values read off the page
images.

**Tier A, Tier B, and the Race tail are all closed. The sweep is done.** Tier
C stays citation by design.

**The Race tail held three features, not the one the list named.** Savage
Attacks (1 extra crit die, melee weapon), Relentless Endurance (drop to 1 hp,
long rest), and Halfling Lucky (reroll a natural 1, must use the new roll).
Savage Attacks stores a *count* rather than a `DiceExpression` for the same
reason Brutal Critical does — the die size comes from the weapon.

**Rock Gnome's Tinker is declined despite carrying numbers.** 1 hour and 10 gp
to build, AC 5, 1 hp, 24 hours, up to three active — but those describe a
*constructed object*, and objects are not a modeled domain here; its three
device options are per-option effect prose besides. Pinned by
`CanonicalFile_RockGnomeTinkerStaysCitationOnly`. This is the "content this
project doesn't model as its own domain" line, not the "no number" line that
covers Pact Boon and Indomitable Might — revisit if a crafting or object
domain ever exists.

**Two features here were explicitly declined during the original pass, and
both declines predate the precedent that now covers them** — Brutal Critical's
scaling was left "in the prose" and Destroy Undead's CR table was cited as
precedent for leaving Font of Magic's table inline, both decided before Sneak
Attack and Wild Shape existed. Both have since been reversed and converted.
**A decline is only as good as the precedents available when it was made;
re-read old declines against newer shapes rather than treating them as
settled.**

**One progression may span two separately cited features.** Portent and
Greater Portent are two `RuleId`s, but Greater Portent's entire mechanical
content is "you roll three d20s for your Portent feature, rather than two" —
so one `PortentProgression` carries both rows (2nd → 2, 14th → 3) while both
citations stay in `LevelFeatures`. Pinned by
`CanonicalFile_GreaterPortentSuppliesTheFourteenthLevelPortentRow`. Split the
progression only when the second feature is a genuinely separate resource.

**The same detail type can serve a class and a subclass when the mechanic is
the same.** `MagicalSecretsProgressionDetail` sits on both `ClassDefinition`
(Bard) and `SubclassDefinition` (College of Lore); `CountsAgainstSpellsKnown`
is `true` for one and `false` for the other, which is exactly the difference
the PHB states. A field that distinguishes two real instances beats two
near-identical types.

**A leveled choice count is stored as a cumulative total, not an increment.**
Favored Enemy is 1/2/3 at levels 1/6/14, not 1/+1/+1 — the same convention
every other progression already uses, and what `ValidatePointsProgression`'s
ascending check assumes.

**Quantizing a feature means re-reading its table row, not just its numbers.**
Destroy Undead's `LevelFeatures` recorded only 5th level while the Cleric
table's Features column names it at 5/8/11/14/17; the gap surfaced only
because the table image was read for the CR values. Check the Features column
against `LevelFeatures` for every feature a pass touches.

**An armor gate is read per feature, never generalized.** Rage and Fast
Movement stop at *heavy* armor (`RequiresNotWearingHeavyArmor`); Monk's
Martial Arts and Unarmored Movement are blocked by *any* armor and by a
shield. Pinned by `CanonicalFile_BarbarianFastMovementGatesOnHeavyArmorOnly`.

**`RecoversOnShortRest` can differ between two features on the same table
row.** Fighter's Action Surge and Indomitable both step up at 17th level;
Action Surge returns on a short rest, Indomitable only on a long one. Pinned
by `CanonicalFile_FighterActionSurgeAndIndomitableRecoverOnDifferentRests` —
the strongest case yet for the standing rule that the rest is read per
feature, never inferred from the class.

**Tier B — flat scalars, the `DarkvisionRangeFeet` shape.** Done on
`ClassDefinition`: Blindsense, Reliable Talent, Feral Senses, Divine Sense,
Improved Divine Smite, Primal Champion. Done on `SubclassDefinition`: Improved
Critical, Shadow Step, Hurl Through Hell, Wrath of the Storm, Thunderbolt
Strike. The fixed ki and sorcery-point costs are done. **Tier B is closed.**

**A cost and a regain are different fields.** Perfect Self *regains* 4 ki and
Sorcerous Restoration *regains* 4 sorcery points, while Stunning Strike and
Shadow Arts *spend* theirs — hence `…KiPointsRegained` versus `…KiCost`. Empty
Body needed a detail object rather than a scalar because one feature buys two
different things at two different prices (4 ki to turn invisible, 8 to cast
astral projection).

**Quivering Palm is a Way of the Open Hand feature, not a Monk class
feature** — the Monk table's 17th-level row reads "Monastic Tradition
feature". Its ki cost lives on `SubclassDefinition`. Pinned by
`CanonicalFile_QuiveringPalmBelongsToWayOfTheOpenHand`, because the PHB prints
it in the same column flow as the class features.

**One progression descends.** Improved Critical's threshold goes 19 at 3rd to
18 at 15th, because a *lower* crit threshold is the improvement — the only
progression in the codebase whose value falls as level rises. It gets a
bespoke descending validator, flagged in a comment on the method and pinned by
`Validator_RejectsImprovedCriticalProgressionWithRisingThreshold`. Don't
"align" it to `ValidatePointsProgression`.

**Indomitable Might was a listed Tier B candidate and carries no number.**
"If your total for a Strength check is less than your Strength score, you can
use that score in place of the total" is a comparison rule, not a value.
Declined and pinned by
`CanonicalFile_BarbarianIndomitableMightStaysCitationOnly`, the same call Pact
Boon earned. **A tier listing is a candidate list, not a promise** — read the
page before building the field.

**Three features share the phrase "aware of … within N feet" and no other
fact.** Blindsense 10 ft (requires hearing), Feral Senses 30 ft (also negates
unseen-attack disadvantage), Divine Sense 60 ft (recovers on a long rest).
Each got its own detail type carrying its own second fact rather than a shared
"awareness range" type.

**Tier C — stays citation**, on the settled linear-in-level line: Lay on
Hands, Second Wind, Arcane Recovery, Natural Recovery, Divine Intervention,
Arcane Ward, Dark One's Blessing, Wholeness of Body, Slow Fall, Deflect
Missiles, Survivor, Divine Smite's slot scaling.

**A class table's numeric column and its Features column are two different
lists and may legitimately disagree — neither is a stale copy of the other.**
Monk proved both directions at once: Unarmored Movement's speed grows at
2/6/10/14/18 while the Features column names it at 2 and 9 (the 9th-level
entry is the vertical-surfaces clause, which carries no number), and Martial
Arts is a single 1st-level feature row whose die grows at 5/11/17 with no row
at all. Pinned by
`CanonicalFile_MonkUnarmoredMovementSpeedLevelsDifferFromFeatureLevels` and
`CanonicalFile_MonkGrantsMartialArtsOnlyAtFirstLevelDespiteDieUpgrades` —
both exist to stop a future pass "aligning" one list to the other.

**Verify every remembered count, page number, and existing field's
completeness against the actual text, repeatedly — including numbers written
in this file.** This is not a hypothetical risk. Eldritch Invocations turned
out to be 32, not the ~20 this document claimed. Elemental Disciplines are
17, not the 18 it claimed. Two passes found real pre-existing citation page
errors. The race pass found `CanonicalFile_OnlyWoodElfOverridesSpeed` had
been **silently failing since the original Races commit** — Wood Elf's 35-foot
speed override was never populated even though the field and the test both
existed. "The test exists" and "the test passes" are different claims.

## Quantized mechanics: cantrips known, spells known, and the Wizard spellbook

The deferred follow-up job flagged during the spell slot pass — closed, no
sweep needed since the two fields only ever apply to the 8 casting classes.
`CantripsKnownProgressionDetail` and `SpellsKnownProgressionDetail` are
independent embedded value objects on `ClassDefinition` (own
`Rules/Classes/CantripsKnown`/`SpellsKnown` folders, no catalog, mapped by a
`*DataMapper`, validated through the existing `ValidatePointsProgression`
helper) — same shape as Sorcery Points, not a new mechanism.

**Cantrips Known and Spells Known are mutually exclusive with "prepares from
the full list," and don't always co-occur with each other.** Six classes
have a Cantrips Known table column: Bard, Cleric, Druid, Sorcerer, Warlock,
Wizard — every class that gets cantrips at all, including three (Cleric,
Druid, Wizard) that prepare their leveled spells and have no Spells Known
count. Four classes have a Spells Known column: Bard, Ranger, Sorcerer,
Warlock. Ranger has Spells Known but no Cantrips Known (rangers get no PHB
cantrips). Wizard has Cantrips Known but no Spells Known — spells are
learned into a spellbook instead, a different mechanic with its own detail
type (`WizardSpellbookDetail`, below). Paladin, the other half-caster, has
neither: no cantrips, and it
prepares from its full list like Cleric/Druid. Cantrips Known breakpoints are
identical across all six classes' shape (three rows, at 1st/4th/10th level)
but not identical in value — Sorcerer starts at 4, Cleric/Wizard at 3,
Bard/Druid/Warlock at 2.

**Spells Known tables plateau — they are not strictly increasing at every
listed row the way Sorcery Points or Ki are.** Bard repeats a value three
separate times (11th=12th, 15th=16th, 18th=19th=20th all hold at 22);
Sorcerer and Warlock each repeat at multiple points too.
`ValidatePointsProgression` requires strictly ascending values between
consecutive grants, so a plateau level is simply omitted from
`SpellsKnownByLevel` — the same sparse, breakpoints-only convention Rage and
Extra Attack already established for milestone-driven tables, now shown on
a table whose breakpoints don't fall on a clean pattern. Verify every
plateau against the rendered table image directly; skimming a
low-resolution render mis-groups adjacent equal values easily. Ranger's
Spells Known starts at 2nd level, not 1st — rangers gain no
spellcasting at all until 2nd level, so `SpellsKnownByLevel`'s first grant is
`(2, 2)`, the same "progression doesn't have to start at level 1" shape
Sorcery Points' 2nd-level start already established.

**The Wizard's spellbook is a flat rate, not a leveled table, because the
PHB never prints it as one.** Unlike Cantrips Known/Spells Known, "Spellbook"
and "Learning Spells of 1st Level and Higher" are prose paragraphs, not a
Wizard-table column — p.114: "At 1st level, you have a spellbook containing
six 1st-level wizard spells of your choice," and "Each time you gain a
wizard level, you can add two wizard spells of your choice to your
spellbook." `WizardSpellbookDetail` is therefore a flat two-scalar value
object (`StartingSpellCount`, `SpellsAddedPerLevelAfterFirst`), the same
`FastMovementDetail` shape — no per-level grant list, no
`ValidatePointsProgression` call, since there's no table row per level to
encode. **`SpellsAddedPerLevelAfterFirst` reads "gain a level" as excluding
1st level itself** — you don't "gain" your starting level, you begin at it,
so the +2 growth starts at 2nd level (6 at 1st, +2 every level through
20th, for 44 total at 20th). This is the standard/intended reading, not an
official errata correction; the field name spells out the exclusion so a
future reader can't apply it at level 1 by mistake. The variable costs to
copy a found spell into the book (2 hours + 50 gp per spell level) or
duplicate your own book (1 hour + 10 gp per spell level) are linear-in-level
formulas and stay in the citation, the same call already made for Preserve
Life and Radiance of the Dawn.

## Combat/adventuring rules: scoping and the Actions in Combat catalog

The first work against the last item on the deferred list. **This is a big
domain and was scoped, not built wholesale** — read the actual PHB table of
contents for Chapter 8 (Adventuring, p.181) and Chapter 9 (Combat, p.189)
before assuming what's in scope:

- **Chapter 8: Adventuring** — Time, Movement, The Environment, Social
  Interaction, Resting, Between Adventures.
- **Chapter 9: Combat** — The Order of Combat, Movement and Position,
  Actions in Combat, Making an Attack, Cover, Damage and Healing, Mounted
  Combat, Underwater Combat.

Most of this is DM-adjudicated prose or a linear-in-level formula (falling
damage, jump distance, attack rolls, breath-holding) — the same "belongs in
the citation" line Preserve Life and Radiance of the Dawn already draw. A
handful of sections are genuinely **closed, named sets** worth their own
catalog, the same shape Conditions/Magic Schools/Alignments already use.
Five were identified as candidates, ranked by cleanliness:

| Candidate | Entries | Status |
| --- | --- | --- |
| Actions in Combat | Attack, Cast a Spell, Dash, Disengage, Dodge, Help, Hide, Ready, Search, Use an Object (10) | **Built** |
| Cover | Half, Three-Quarters, Total (3) | **Built** |
| Travel Pace | Fast, Normal, Slow (3) | **Built** |
| Resting | Short Rest, Long Rest (2) | **Built** |
| Between Adventures (Downtime Activities) | Crafting, Practicing a Profession, Recuperating, Researching, Training (5) | **Built** |

**All five candidates are now built — this table is closed, not a queue.**
Everything else in Chapters 8–9 stays fully unbuilt — no catalog, no bare
`RuleId` citation index either, since nothing in the codebase currently
needs to reference it. If a sixth candidate ever gets proposed, re-scope
from the actual page images rather than trusting this table blindly; a
past scoping pass already found real numbers in places a first skim would
have called DM-adjudicated prose (Cover's AC/Dex-save bonuses).

**`CombatActions` is a plain closed-vocabulary catalog** — `Id`/`Name`/
`Sources` only, no mechanical payload, the exact `MagicSchoolDefinition`
shape (not `SpellDefinition`'s richer header-block shape, since none of the
10 actions carries a fact worth a shared field: Dash's extra movement is
just "equals your speed," not an independent number). Cited to **p.192**
for the first seven (Attack, Cast a Spell, Dash, Disengage, Dodge, Help,
Hide) and **p.193** for the last three (Ready, Search, Use an Object) —
the two-column layout puts Attack through Hide on the first page and the
remaining three plus "Making an Attack" on the next. Section string is
`"Chapter 9: Combat — Actions in Combat — {Name}"`, matching Magic Schools'
per-entry section-suffix convention.

**This is the first domain that isn't a sibling of Equipment/Expenses/
Creatures/Classes/Races/Backgrounds/Spells** — it lives at `Rules/Combat/
CombatActions/`, a new top-level domain, with its catalog wrapper at
`Rules/Catalog/CombatActionCatalog.cs` like every other catalog. Wiring a
brand-new closed-set domain touches five places beyond its own five-piece
folder: `RulesetDefinitionSet` (raw definitions, appended as the new last
constructor parameter), `Dnd5e2014RulesetLoader` (embedded-resource
constant + `Load()` call + threading through both the `RulesetDefinitionSet`
and `Dnd5e2014Ruleset` constructor calls), `Dnd5e2014Ruleset` itself (the
public catalog property), `CatalogIntegrityValidator` (one `ValidateSources`
loop — no cross-domain reference checks needed here, since nothing yet
references a `CombatActionId`), and the `.csproj` embedded-resource entry.
Ten pre-existing tests across `CatalogIntegrityTests.cs` and sibling
`*CatalogIntegrityTests.cs`/`*RuleAssociationIntegrityTests.cs` files
construct `RulesetDefinitionSet` directly with named arguments and needed
`combatActions: []` added — expect the same fan-out for the next new
top-level domain.

## Combat/adventuring rules: Cover, Travel Pace, Resting, and Downtime Activities

The other four catalogs from the scoping table above, built in one pass
immediately after Actions in Combat. **Unlike Actions in Combat, none of
these four share Magic Schools' bare `Id`/`Name`/`Sources` shape** — every
one carries real typed fields, because every one of these sections actually
prints numbers, not just named prose blocks.

**Two live under a new `Rules/Adventuring/` top-level domain, not
`Rules/Combat/`** — `TravelPace`, `Resting`, and `DowntimeActivities` are
Chapter 8 (Adventuring) content; `Cover` joins `CombatActions` under
`Rules/Combat/` since it's Chapter 9. Splitting by PHB chapter rather than
dumping everything under one `Rules/Combat/` mirrors how `Rules/Spells` and
`Rules/Classes` already stay separate top-level domains despite spells
being cast by classes.

**`CoverDefinition`** (p.196, 3 entries) has two independent nullable bonus
fields (`ArmorClassBonus`, `DexteritySavingThrowBonus`) plus a
`PreventsBeingTargeted` bool — Half and Three-Quarters populate the
bonuses (+2/+2 and +5/+5) and leave the bool false; Total cover flips the
bool and leaves both bonuses null. The two bonus fields are always numerically
identical for a given degree in this printing, but they're still modeled
as two fields rather than one shared value — the PHB states them as two
separate mechanical benefits (AC *and* Dexterity saves) that happen to
share a number here, not one fact printed once.

**`TravelPaceDefinition`** (p.182, 3 entries) is the first domain with three
parallel distance fields (`FeetPerMinute`/`MilesPerHour`/`MilesPerDay`) plus
an independent `PassiveWisdomPerceptionPenalty` (int?, only Fast: 5) and
`AllowsStealth` (bool, only Slow) — two unrelated effects on the same table
column ("Effect"), so they get two unrelated fields rather than one shared
"effect" enum.

**`RestTypeDefinition`** (p.186, 2 entries) — `MinimumDurationHours` (1 for
Short, 8 for Long), `CooldownHours` (int?, only Long: 24 — "can't benefit
from more than one long rest in a 24-hour period"), and
`MinimumHitPointsToBenefit` (int?, only Long: 1). **The Hit Dice regained on
a long rest ("half of the character's total number of them") is declined,
not quantized** — it's a formula relative to a value this project already
models elsewhere (total Hit Dice, driven by class level), the same
linear-in-level-formula line Preserve Life and Radiance of the Dawn already
draw, even though the multiplier itself (one-half) is a flat constant.

**`DowntimeActivityDefinition`** (p.187, 5 entries) uses the Channel
Divinity Options shape — several independent nullable scalars
(`RequiredDays`, `CostPerDayGoldPieces`, `SavingThrowAbilityId`,
`SavingThrowDC`, `MarketValueProgressPerDayGoldPieces`), validated so a
saving-throw ability and its DC are both present or both absent together.
Crafting only populates `MarketValueProgressPerDayGoldPieces` (5) — its
"raw materials cost half the market value" is declined the same way Long
Rest's Hit Dice fraction is, a ratio applied to a value the player chooses
per-attempt, not a flat fact. Recuperating populates `RequiredDays` (3) and
the saving throw pair (Constitution, DC 15). Researching and Training both
populate `CostPerDayGoldPieces` (1); only Training also has a fixed
`RequiredDays` (250) — Researching's duration is explicitly DM-determined,
so it stays null rather than guessing a placeholder. **Practicing a
Profession declines every field** — its outcome is which Lifestyle tier you
qualify for (modest/comfortable/wealthy, gated on org membership or
Performance proficiency), not a number — and still gets a full catalog
entry, the same "declined but still enumerated" precedent Pact Boon set.

All four reuse the exact wiring fan-out `CombatActions` established
(`RulesetDefinitionSet` → `Dnd5e2014RulesetLoader` → `Dnd5e2014Ruleset` →
`CatalogIntegrityValidator` → `.csproj`), done together in one pass since
by this point the fan-out shape was already known. `DowntimeActivities` is
the only one of the four with a cross-domain reference
(`SavingThrowAbilityId` against the ability catalog), validated in
`CatalogIntegrityValidator` the same way Battle Master maneuvers already
validate their own saving-throw ability.

## Game-backend quantization: Conditions

The first slice of the game-backend initiative described in "Current
state" above. `ConditionDefinition` went from `Id`/`Name`/`Sources` (a
bare closed-vocabulary entry, same shape as `CombatActionDefinition`) to
26 additional fields capturing every mechanical bullet point in PHB
Appendix A (pp.290–292) for all 15 conditions. Existing citations didn't
change — same IDs, same page/section — this was purely additive.

**The shape is still "many typed fields on one Definition," the same
precedent `ClassDefinition` (40+ fields) already set — not a new DSL.**
What makes Conditions look different is density: unlike most Definitions,
where a handful of fields populate per instance, several conditions
populate 6–8 of the 26 fields simultaneously (Unconscious is the densest).
That ruled out the "several mechanism fields, validator enforces exactly
one populated" shape `FightingStyleDefinition`/`DivineStrike` use — these
facts aren't alternative representations of one mechanic, they're
independent facts that legitimately co-occur, so no "exactly one
populated" constraint was added.

**Two shared value types moved to reusable locations, since both are
core D&D vocabulary certain to recur once Spells/Features get their own
quantization pass:**

- `RollModifier` (`None`/`Advantage`/`Disadvantage`) lives in
  `Rules/Common/`, not under Conditions, since advantage/disadvantage is
  the single most-referenced mechanic in the whole ruleset.
- `SpeechRestriction` (`None`/`CanOnlySpeakFalteringly`/`CannotSpeak`) and
  `ExhaustionLevelEffect` stay under `Rules/Creatures/Conditions/`, since
  neither generalizes beyond conditions the way advantage/disadvantage
  does.

**Exhaustion is structurally different from the other 14 and gets its own
nested `ExhaustionEffectDetail`** (`LevelEffects` — exactly 6, one per
level, self-validated in the constructor since a wrong count is a data
error, not a "some campaigns differ" case — plus
`RecoversOneLevelPerLongRest` and `RecoveryRequiresFoodAndDrink`). Every
other condition has this field `null`. The six level effects are
cumulative in play ("suffers the effect of its current level as well as
all lower levels" — p.291), but the stored list holds each level's
*incremental* new effect exactly as the PHB table prints it; resolving
cumulative effects at a given level is a consuming engine's job, not this
library's.

**One fact was deliberately declined, matching the project's existing
"compound conditional" line.** Prone's attack-roll-against text is
range-conditional — advantage within 5 feet, disadvantage beyond — not a
flat `RollModifier` value the way every other condition's is. Rather than
inventing a distance-conditional shape for one condition,
`AttackRollsAgainstTheCreature` stays `None` for Prone and the real rule
stays in the citation, the same "content this project doesn't model as
its own domain" call Plant Growth's compound casting time and Prone's
own "ends if grappler is incapacitated"-style entailments already made.

**`PreventsActionsAndReactions` is set directly on every condition that
includes it, not resolved through a reference.** Paralyzed, Petrified,
Stunned, and Unconscious all say "is incapacitated (see the condition)" —
rather than modeling that as a reference to the `incapacitated` condition
a consumer would have to resolve, the flag is set `true` directly on all
five conditions (including Incapacitated itself). This matches the
project's standing preference for flattening a derived/compound fact into
direct data over requiring a lookup chain (the same reasoning Warding
Bond's per-item cost already established).

**Constructing a `ConditionDefinition` now uses named arguments at every
call site — loader, tests, everywhere** — a deliberate, reasoned
deviation from this codebase's usual positional-constructor-call style.
Every other multi-field constructor in this project mixes types
(`DiceExpression`, `AbilityId`, `int?`, ...), so a transposed positional
argument usually fails to compile. Here, ~20 of the 26 new parameters are
plain `bool`, so a transposition would silently compile and silently
corrupt data. Named arguments are the correct trade against this
codebase's normal terseness, not a style inconsistency to "fix" later.

## Game-backend quantization: Spell cantrip effects

The first slice of gap 1 (Spells have zero effect data). Cantrips were
chosen to prove the shape for the same reason they proved the header-block
shape originally: a complete, closed, small set. `SpellDefinition` gained
one new nullable field, `DamageEffect` (type `SpellDamageEffect`, under
`Rules/Spells/`), populated on the 11 of 27 cantrips that actually deal
damage on their own. The other 16 — utility and buff cantrips — carry
`DamageEffect: null`; existing header-block data and citations didn't
change.

**`SpellDamageEffect` follows the Battle Master maneuver / Divine Strike
precedent, not the Conditions precedent.** Unlike Conditions' "several
independent facts that legitimately co-occur," a spell's damage-resolution
mechanic is genuinely alternative — PHB p.202 ("Attack Rolls and Saving
Throws") splits every damaging spell into "make an attack roll" *or*
"target makes a saving throw," never both. So `AttackRollType`
(`SpellAttackRollType?`: `Melee`/`Ranged`) and `SavingThrowAbilityId`
(`AbilityId?`) are mutually exclusive, validated as "exactly one populated"
in the constructor itself (matching `SpellComponents`/`SpellDuration`'s
self-validating style, not a `SpellDefinitionValidator` method — the
closer precedent here is `DivineStrikeProgressionDetail`, an embedded
value object with a public constructor that validates its own invariants,
not a top-level `Definition` with an internal one).

**The character-level damage-tier list (`DamageByCharacterLevel`, a new
`SpellDamageTierGrant` record: `CharacterLevel` + `DiceExpression`) reuses
`DivineStrikeDamageGrant`'s exact shape but as its own Spells-domain type**
— each domain owns its progression grant type even when the shape is
identical, the same call `CantripsKnownProgressionDetail`/
`SpellsKnownProgressionDetail` already made as two separate types. Every
PHB damage cantrip states its own breakpoints explicitly in its
description text ("increases by 1d6 when you reach 5th level (2d6), 11th
level (3d6), and 17th level (4d6)"), so the table is expanded from the
spell's own prose, not derived from the general p.201 "Cantrips" rule.
The constructor enforces: at least one tier, the first at character level
1, strictly ascending character levels, a constant die size across every
tier, and a strictly increasing die count — the same "expand formulas into
tables" discipline `DivineStrikeDamageGrant` and Circle Forms already
established.

**Eldritch Blast is the one damage cantrip whose damage doesn't scale by
die count at all — it stays flat 1d10 and gains extra beams instead** (2
at 5th, 3 at 11th, 4 at 17th, each a separate attack roll). A single-tier
`DamageByCharacterLevel` (just character level 1) represents this
correctly with no schema change — the ascending-tier validation is
trivially satisfied by one entry. **Beam count itself is declined, not
modeled**: it's a targeting-multiplicity fact, not one of the three the
initiative's audit named (damage dice, save DC/ability, condition
applied), and no other PHB cantrip shares the shape, so it stays in the
citation rather than earning a bespoke field for one spell.

**Every per-spell secondary rider stays in the citation, unquantized** —
Chill Touch's no-healing clause, its bonus disadvantage against undead,
Shocking Grasp's advantage-vs-metal-armor and no-reactions clauses, Ray of
Frost's speed reduction, Sacred Flame's cover-ignoring clause, Thorn
Whip's pull, Vicious Mockery's disadvantage-on-next-attack. This is the
same call already made for Battle Master maneuvers' secondary effects and
Metamagic's 8 spell effects: individually heterogeneous, no shared shape,
so modeling them means a bespoke type per spell or the DSL this project
rejects. The three facts this pass does capture (damage type, attack-roll-
or-save, damage dice) are exactly the ones the initiative's audit named as
the real gap; everything else is scope creep against that audit.

**Two cantrips carry a saving throw that isn't a damage effect, and both
are declined for the same "not this pass's scope" reason.** Light's
conditional Dexterity save ("if you target an object held or worn by a
hostile creature") only triggers on a specific targeting choice, the same
range/context-conditional shape Prone's attack-roll text already declined
for Conditions — no `SpellDamageEffect` fits a save with no damage behind
it anyway. Minor Illusion's "Intelligence (Investigation) check against
your spell save DC" is an ability check, not a saving throw, and belongs
to a different, unbuilt mechanic (opposed checks against a DC) entirely.
Guidance and Resistance's 1d4 bonus-to-a-roll buffs were also declined —
real numbers, but buffs, not damage, and outside what the audit asked for.

**All 27 cantrips were re-read from the rendered page images for this
pass, not carried over from the original header-block build** — the same
"verify every remembered fact against the actual text, repeatedly" rule
the Quantized mechanics tail section already established, applied here to
a second pass over already-built content rather than new content.

## Game-backend quantization: 1st-level spell effects

The second slice of gap 1, extending cantrip damage effects to a real
leveled-spell level. All 62 1st-level spells were re-read from the
rendered page images. 10 have a `SpellDamageEffect`, 6 have a new
`SpellConditionEffect`, one (Ray of Sickness) has both, and 47 have
neither. This pass grew `SpellDamageEffect` and added
`SpellConditionEffect` because cantrips alone hadn't shown enough real
content to justify the shapes — the same "verify against real content,
don't build ahead of it" discipline the whole project follows.

**A leveled spell's saving throw takes half damage on success; a
cantrip's takes none.** Every 1st-level save-based damage spell (Arms of
Hadar, Burning Hands, Dissonant Whispers, Hellish Rebuke, Thunderwave)
prints "half as much damage on a successful one" — none of the 11 damage
cantrips built earlier ever had this line, so the field didn't exist yet.
`HalfDamageOnSuccessfulSave` is a plain bool, constructor-enforced to
`false` whenever there's no saving throw at all (an attack-roll effect
can't have "half on save" by definition).

**A leveled spell's damage doesn't scale by character level — cantrips'
`DamageByCharacterLevel` doesn't apply, so a second shape,
`BaseDamage: DiceExpression?`, sits beside it.** The two are mutually
exclusive (constructor-enforced), not a shared "amount" field with a
kind flag: a cantrip's damage-die count climbing at 5th/11th/17th
character level and a leveled spell's flat damage at its own printed
level are different facts on different axes, and forcing one shape onto
both would have meant lying about what "character level 1" means for a
spell whose damage never depends on character level. The leveled spell's
own "At Higher Levels" spell-slot-upcast scaling — the real analog of a
cantrip's scaling — stays in the citation, the same linear-in-slot-level
formula call already made for Preserve Life, Radiance of the Dawn, and
every other scaling formula this project declines to expand.

**Chromatic Orb needed a choosable damage type, so `DamageTypeId` became
nullable and gained a sibling `ChoosableDamageTypeIds`.** "You choose
acid, cold, fire, lightning, poison, or thunder" is a real, closed list,
the same `FixedDamageTypeId`/`ChoosableDamageTypeIds` shape
`DivineStrikeProgressionDetail` already established for the same "fixed
type or list of choosable types" fact. Constructor-enforced: exactly one
of the two populated, and the choosable list must not be empty when
present.

**`SpellConditionEffect` is a new, independent nullable field on
`SpellDefinition` (`ConditionEffect`), not folded into
`SpellDamageEffect`.** A spell's condition-imposing mechanic (Charm
Person's Wisdom save into `charmed`) and its damage mechanic are
genuinely separate facts that can co-occur — Ray of Sickness deals damage
via a ranged spell attack and *separately* poisons on a failed
Constitution save decoupled from that attack roll — so a spell can carry
`DamageEffect`, `ConditionEffect`, both, or neither, the same
"independent nullable mechanism fields side by side" shape the project
already uses elsewhere. The type itself is
`ConditionIds: IReadOnlyList<ConditionId>` (non-empty, no duplicates) +
`SavingThrowAbilityId: AbilityId` (required, not nullable — see below).
Tasha's Hideous Laughter is the reason `ConditionIds` is a list, not a
singular field: one failed save imposes both `prone` and
`incapacitated` together.

**`SavingThrowAbilityId` on `SpellConditionEffect` is required, not
nullable, and Sleep is the reason it stays that way.** Every
condition-imposing spell found across all 62 has a save gating the
condition — except Sleep, whose hit-point-pool targeting ("roll 5d8...
starting with the creature with the lowest current hit points...") has
no saving throw anywhere in its text. Rather than relaxing the field to
accommodate one outlier, Sleep is declined the same way Color Spray's
hit-point-pool blinding was declined at cantrip level: a unique,
non-recurring compound mechanic, not a reason to weaken a field every
other real spell needs.

**Magic Missile is declined, not modeled, despite dealing real damage.**
It auto-hits with no attack roll and no saving throw at all ("You don't
need to make an attack roll"), which the schema doesn't represent (every
`SpellDamageEffect` built so far resolves via exactly one of the two);
and its "1d4 + 1 force damage" per dart needs a flat modifier
`DiceExpression` has never carried, since every dice fact built anywhere
in this project so far has been pure `Xd Y` with no added constant. Both
gaps are real, but Magic Missile is the only 1st-level spell that needs
either one, so it's declined rather than bending two established types
for a single spell — the same "one data point doesn't justify a new
mechanism field" call Plant Growth's compound casting time already made.
Revisit if a second auto-hit or flat-modifier spell turns up at a later
level.

**Weapon-attack-rider spells are declined as a group, the same call
Shillelagh made at cantrip level.** Ensnaring Strike, Hail of Thorns,
Searing Smite, Thunderous Smite, and Wrathful Smite are all a bonus
action that buffs your *next weapon attack* rather than being an
independent spell attack or save in their own right — the spell doesn't
resolve anything itself until a separate weapon attack (already modeled
via `WeaponDefinition`) lands. None of the five get a `SpellDamageEffect`.

**Per-spell secondary riders stay in the citation, unquantized — same
rule as cantrips, now confirmed at a second level.** Arms of Hadar's
no-reactions clause, Dissonant Whispers' forced movement, Guiding Bolt's
advantage-on-next-attack, Hellish Rebuke and Witch Bolt's status as
reaction/recurring-tick spells, Ray of Sickness's poisoned duration,
Tasha's Hideous Laughter's repeatable end-of-turn save — all individually
heterogeneous, all declined, all in the citation. Witch Bolt's "deal 1d12
automatically each turn" follow-up damage is declined the same way; only
the spell's own initial-hit `SpellDamageEffect` is captured.

**Bane, Command, Compelled Duel, Divine Favor, Hex, and Hunter's Mark are
all saving-throw or no-save spells with zero damage or condition of their
own** — pure debuffs, behavioral compulsions, or weapon-attack-damage
riders (Divine Favor and Hex both buff a *later* weapon hit, the same
"rider, not the spell's own effect" reasoning that declined the five
smite-style spells above, just without the bonus-action setup). None
carry either mechanism field.

## Game-backend quantization: 2nd-level spell effects

The third slice of gap 1. All 59 2nd-level spells were classified against
the shapes 1st level already established — **no schema change was
needed this time**, the first level of the effect-data pass where the
existing fields covered every real fact found. 7 spells get a
`SpellDamageEffect` (Cordon of Arrows, Flame Blade, Flaming Sphere,
Melf's Acid Arrow, Moonbeam, Scorching Ray, Shatter), 3 get a
`SpellConditionEffect` (Crown of Madness → `charmed`, Hold Person →
`paralyzed`, Web → `restrained`), and 49 get neither.

**Cordon of Arrows confirms `HalfDamageOnSuccessfulSave` is read per
spell, not inferred from "has a saving throw."** Its Dexterity save
prints no "half as much damage on a successful one" clause — a failed
save takes 1d6 piercing, a successful one takes none — the first
1st-level-schema field this pass exercised in its `false` state on a
real saving-throw spell rather than only ever seeing `true`.

**A third recurring decline confirmed: automatic zone/trigger damage
with no attack roll or saving throw at all.** Cloud of Daggers (4d4
slashing on entering its area), Heat Metal (2d8 fire the instant the
object is touched, no roll of any kind), and Spike Growth (2d4 piercing
per 5 feet moved through the area) all deal damage as an automatic
consequence of position or contact, never through the "make an attack
roll" or "target makes a saving throw" split `SpellDamageEffect` models.
This is the same shape Magic Missile declined at 1st level for a
different reason (its auto-hit was a *targeted* dart with no roll);
these three are environmental/hazard damage instead, but the underlying
gap is identical — the schema has no field for "no roll of any kind."
Three real instances across two levels is enough to note as a confirmed
pattern, not yet enough to justify a schema change per the standing "one
data point doesn't justify a new mechanism field" rule — revisit if a
future level's density of these grows large enough to be worth a
dedicated `AutomaticDamage` shape instead of a per-spell decline.

**Spiritual Weapon is declined for the same reason every "dice + ability
modifier" spell has been declined project-wide.** Its melee spell attack
deals "1d8 + your spellcasting ability modifier" force damage — the die
is clean, but the total also depends on a modifier this project has
never stored (Cure Wounds, Healing Word, and every other "Xd Y + ability
modifier" heal were declined the same way before this initiative even
started, since the modifier isn't a fact about the spell, it's a fact
about the caster).

**Melf's Acid Arrow and Scorching Ray both capture only the clean base
fact and decline the rest, the same "per-spell secondary rider" call as
Witch Bolt's recurring tick.** Melf's Acid Arrow's initial 4d4 acid hit
is captured; its follow-up 2d4 at the end of the target's next turn, and
its unusual "half damage even on a miss" clause, stay in the citation —
no field exists for damage-on-a-miss anywhere in this schema, and one
spell isn't reason enough to add one. Scorching Ray's per-ray 2d6 fire
is captured; the "three separate rays, each its own attack roll" fact is
declined the same way Eldritch Blast's beam count was declined at
cantrip level — a targeting-multiplicity fact, not a damage-amount fact.

**Blindness/Deafness is declined despite being a clean, otherwise
single-condition spell**, because the condition itself is a caster's
choice between two ("the target is either blinded or deafened — your
choice"), and `SpellConditionEffect.ConditionIds` today means "all of
these are imposed together" (Tasha's Hideous Laughter's AND semantics),
not "one of these, chooser's pick." A choosable-condition shape mirroring
`SpellDamageEffect`'s `DamageTypeId`/`ChoosableDamageTypeIds` split would
fit, but Blindness/Deafness is the only spell across the 148 spells with
effect data built so far (27 cantrips + 62 1st-level + 59 2nd-level)
that needs it — declined per the same one-data-point rule, not because
the shape wouldn't work.

## Game-backend quantization: 3rd-level spell effects

The fourth slice of gap 1. All 50 3rd-level spells classified against
the shapes already established — **zero schema changes needed**, the
second level in a row where 1st level's shapes covered everything. 7
spells get a `SpellDamageEffect` (Call Lightning, Conjure Barrage,
Fireball, Lightning Bolt, Spirit Guardians, Vampiric Touch, Wind Wall),
3 get a `SpellConditionEffect` (Fear → `frightened`, Hypnotic Pattern →
`charmed` + `incapacitated` together, Sleet Storm → `prone`), and 40
get neither.

**Conjure Barrage and Spirit Guardians are the second and third spells
to use `ChoosableDamageTypeIds`, after Chromatic Orb at 1st level** —
Conjure Barrage picks from bludgeoning/piercing/slashing (matching the
weapon that created it), Spirit Guardians from radiant/necrotic. Two
more real instances confirm the choosable-type shape is a recurring
PHB pattern, not a one-off Chromatic Orb needed alone.

**Wind Wall is the first damage spell built whose saving throw is
Strength** — every prior saving-throw damage spell (cantrips through
3rd level) used Dexterity, Constitution, or Wisdom; nothing about the
schema assumed a specific ability, but this is the first real spell to
exercise Strength in that slot.

**Two more weapon-attack-rider spells confirm that decline pattern
again: Blinding Smite and Lightning Arrow.** Both buff a weapon attack
that hasn't happened yet rather than resolving their own attack or save
— the same reasoning that declined Ensnaring Strike, Hail of Thorns,
Searing Smite, Thunderous Smite, and Wrathful Smite at 1st level.
Lightning Arrow is the most elaborate of the group (the triggering
weapon attack deals 4d8 lightning instead of its normal damage, *and*
a separate Dexterity save against nearby creatures deals 2d8 more), but
the core reason for declining is unchanged: the spell's own effect never
resolves independently of a weapon attack this project already models
through `WeaponDefinition`.

**Hunger of Hadar and Glyph of Warding are both declined as compound/
multi-mode mechanics, the same call already made for Sanctuary and
Color Spray.** Hunger of Hadar deals automatic cold damage with no roll
at all *and* separately gates acid damage behind a Dexterity save —
two different damage-resolution mechanics on one spell, which no single
`SpellDamageEffect` can represent since the type is built around
exactly one resolution mechanic per instance. Glyph of Warding's
explosive-runes mode has real damage-and-save numbers, but the spell
also has an entirely different spell-glyph mode with no numbers at all,
and which mode applies is chosen at cast time — modeling only one mode
would misrepresent the spell as always working that way.

**Stinking Cloud's incapacitating effect is declined, not modeled as
`poisoned`.** Its saving-throw failure causes the creature to spend its
action "retching and reeling" — a real, specific mechanical consequence,
but not language that names the `poisoned` condition the way Ray of
Sickness or Web's text names theirs. Modeling it as `poisoned` would be
inferring a condition tag the spell's own words don't use; better to
leave the mechanic in the citation than assert a citation this project
can't back with the printed text.

## Game-backend quantization: 4th-level spell effects

The fifth slice of gap 1, and the first level where a spell needed both
mechanism fields populated at once. 3 of the 35 4th-level spells get a
`SpellDamageEffect` (Blight, Evard's Black Tentacles, Wall of Fire), 3
get a `SpellConditionEffect` (Dominate Beast → `charmed`, Evard's Black
Tentacles → `restrained`, Phantasmal Killer → `frightened`), and 30 get
neither.

**Evard's Black Tentacles is the first spell where `DamageEffect` and
`ConditionEffect` are gated by the *same* saving throw**, not two
independent ones the way Ray of Sickness's attack-roll damage and
separate Constitution-save poison were. The schema doesn't need to
express "these two facts share one roll" — each field already carries
its own `SavingThrowAbilityId` independently, and here they simply
happen to match (both Dexterity), which is exactly what the PHB text
describes: one failed save triggers both the 3d6 bludgeoning damage and
the restrained condition together. Like Cordon of Arrows, its damage has
no half-on-a-successful-save clause — success negates the damage and the
restrain both.

**Phantasmal Killer splits cleanly into a captured condition and a
declined damage rider.** Its initial Wisdom save gates `frightened`
directly, so that's captured as a `SpellConditionEffect`. But the
psychic damage isn't part of that same resolution — the target repeats
a Wisdom save every subsequent turn, and only a failure on *that*
recurring save deals 4d10 psychic damage. This is the same
recurring-save-rider shape Witch Bolt's per-turn tick and Tasha's
Hideous Laughter's repeatable end-of-turn save already declined; only
the fact gated by the spell's initial cast is captured.

**Two genuinely new one-off shapes surfaced this level, and both are
declined rather than extended into the schema:**

- **Guardian of Faith deals a flat 20 radiant damage, not a dice
  expression.** Every damage fact captured anywhere in this project so
  far — cantrips through 4th level — has been `DiceExpression` (`XdY`).
  A bare integer has no representation in `BaseDamage: DiceExpression?`,
  and one spell isn't reason enough to add a parallel flat-damage field.
  Declined; revisit if a second flat-damage spell appears.
- **Ice Storm deals two damage types at once, not one fixed type or a
  choice of one.** `DamageTypeId`/`ChoosableDamageTypeIds` both assume
  exactly one type applies per casting (a fixed type, or the caster
  picks one from a list) — Ice Storm's 2d8 bludgeoning *and* 4d6 cold
  always both apply together, a third shape neither field represents.
  Declined for the same reason; the two established shapes cover
  "always this one type" and "caster picks one," not "always both."

**Banishment and Dimension Door are both declined as compound/
conditional mechanics, extending the Sanctuary/Color Spray/Hunger of
Hadar precedent.** Banishment's Charisma save leads to one of two
different outcomes depending on the target's home plane (incapacitated
in a demiplane vs. banished home with no incapacitation at all) — which
branch applies isn't a fact about the spell, it's conditional on the
target. Dimension Door's 4d6 force damage only triggers on the specific
edge case of teleporting into an occupied space; the spell's normal
case deals no damage at all, so capturing the edge case as if it were
the spell's effect would misrepresent the common case.

**Mordenkainen's Faithful Hound and Otiluke's Resilient Sphere are
declined for two different, already-established reasons.** The hound's
bite is a summoned creature acting on its own initiative over multiple
turns, the same "the summoned creature's actions aren't the spell's own
effect" reasoning that already declined Animate Dead and every
conjure-a-creature spell. Otiluke's Resilient Sphere traps a creature
in a sphere, but the PHB's own word for this is "enclosed," never
"restrained" — the same discipline Stinking Cloud's decline already
established: don't infer a condition tag the text doesn't use.

## Game-backend quantization: 5th-level spell effects

The sixth slice of gap 1. All 42 5th-level spells were classified
against the shapes already established — **no schema change was
needed**, the third level in a row. 5 spells get a `SpellDamageEffect`
(Cloudkill, Cone of Cold, Conjure Volley, Contact Other Plane, Insect
Plague), 3 get a `SpellConditionEffect` (Destructive Wave → `prone`,
Dominate Person → `charmed`, Hold Monster → `paralyzed`), and 34 get
neither.

**Conjure Volley is the fourth spell to use `ChoosableDamageTypeIds`**
(bludgeoning/piercing/slashing, matching its ammunition), confirming
Conjure Barrage's 3rd-level pattern rather than being a one-off.
**Contact Other Plane is the first Intelligence-save damage spell**,
and like Cordon of Arrows and Evard's Black Tentacles, its save carries
no half-on-a-successful-save clause — a failed save deals 6d6 psychic
damage and imposes a bespoke "insane" status, a successful one avoids
both entirely. The "insane" status itself is declined, not modeled as
any Appendix A condition, since it isn't one — it's a bespoke
"can't take actions, can't understand speech, speaks only in
gibberish" block with no formal condition name attached.

**Destructive Wave confirms `DamageEffect` and `ConditionEffect` really
are independent, not a package deal.** Its failed save knocks the
target prone (captured as a `ConditionEffect`) *and* deals 5d6 thunder
damage plus 5d6 radiant-or-necrotic damage together — the same
two-simultaneous-damage-types shape Ice Storm declined at 4th level, so
its `DamageEffect` stays null. The two mechanism fields don't rise or
fall together: a spell's clean fact is captured even when its messier
sibling fact on the very same saving throw has to be declined.
Flame Strike is a second, plainer instance of the same
two-simultaneous-types decline (4d6 fire and 4d6 radiant, both always
apply), with no condition to salvage this time.

**Telekinesis is declined for a new reason: its "restrained" effect
resolves through a contested check, not a standard save.** The target
doesn't roll its own saving throw against the caster's DC — the caster
makes a spellcasting-ability check *against* the target's Strength
saving throw as the opposing roll. `SpellConditionEffect.SavingThrowAbilityId`
assumes the PHB's ordinary "target makes a saving throw" shape, which
this spell's contested-roll mechanic isn't; modeling it as an ordinary
save would misstate how the roll actually works.

**Bigby's Hand and Animate Objects both extend the
summoned/controllable-construct decline already established by
Spiritual Weapon and Mordenkainen's Faithful Hound.** Bigby's Hand's
Clenched Fist option deals a clean 4d8 force damage on a hit, but the
attack is one of several optional bonus-action effects (grapple, push,
interpose) for a conjured hand the caster controls turn after turn —
the same "recurring, optional, not-the-spell's-single-effect" shape
already declined, not a new one-off worth re-litigating.

## Game-backend quantization: 6th-level spell effects

The seventh slice of gap 1, and the first level since 1st that actually
grew `SpellDamageEffect` rather than reusing it unchanged. 9 of the 32
6th-level spells get a `SpellDamageEffect` (Blade Barrier, Chain
Lightning, Circle of Death, Disintegrate, Harm, Otiluke's Freezing
Sphere, Sunbeam, Wall of Ice, Wall of Thorns), 2 get a
`SpellConditionEffect` (Flesh to Stone → `restrained`, Sunbeam →
`blinded`), and 22 get neither.

**`FlatDamageBonus: int?` is a new field, added because Disintegrate is
the second real "dice + flat modifier" spell, confirming the pattern
1st level's Magic Missile note explicitly flagged for revisit.**
Disintegrate's "10d6 + 40 force damage" needs the same shape Magic
Missile's declined "1d4 + 1" did, but unlike Magic Missile, Disintegrate
has no other disqualifying trait — it resolves via an ordinary Dexterity
saving throw with `BaseDamage` already capturing the dice, so only the
flat add was missing. The field is constructor-validated to require
`BaseDamage` (never `DamageByCharacterLevel`, since no cantrip has
needed a flat add) and to be `null` or a positive integer — the
overwhelming majority of spells still carry `null`. **Every one of the
43 already-built populated `damageEffect` entries needed the field
added to stay loadable**, the same "add a required field, do one
scripted backfill pass over the data file" cost the original
header-block build already paid when `SpellDuration.IsSpecial` was
added (5th-level spells, back when this project was still building
header blocks, not effect data) — done here as a single regex insertion
after every `baseDamage` value, verified to touch nothing else. Magic Missile itself stays declined — the flat-modifier
gap is now closed, but its auto-hit (no attack roll, no saving throw at
all) is a second, still-unaddressed reason, and fixing only one of two
disqualifying traits isn't worth revisiting the spell on its own.

**Sunbeam is the second spell (after Evard's Black Tentacles) where
`DamageEffect` and `ConditionEffect` share one saving throw** — a
failed Constitution save deals 6d8 radiant damage (half on success) and
separately blinds the target (no half-blindness concept, so the
condition simply doesn't apply on a successful save). Two real
instances now confirm this dual-effect shape is a recurring PHB
pattern, not a one-off.

**Flesh to Stone captures only its initial restrain, not the
escalation to petrified.** The spell's real mechanic is a Constitution
save into `restrained`, followed by up to three more Constitution saves
at the end of each of the target's turns; failing three of those turns
turns it to stone, subject to the *petrified* condition instead. Modeling only the initial
gate is the same call already made for Hold Person/Hold Monster's
repeat-save-to-end mechanic — the repeated saves are a rider on the
initial effect, not a fact the initial `SpellConditionEffect` needs to
carry.

**Forbiddance is a fourth instance of the automatic zone/trigger damage
decline**, after Cloud of Daggers, Heat Metal, and Spike Growth — a
chosen creature type takes 5d10 radiant-or-necrotic damage the instant
it enters the warded area, with no attack roll or saving throw
anywhere in the mechanic.

**Eyebite and Otto's Irresistible Dance are both declined as
condition-shaped effects that don't cleanly fit
`SpellConditionEffect`.** Eyebite lets the caster choose one of three
named effects before casting (Asleep → unconscious, Panicked →
frightened, Sickened → a bespoke disadvantage effect with no condition
name at all) — a fixed-at-cast-time choice among options that aren't
uniformly real conditions, the same reasoning that declined
Blindness/Deafness's two-option choice at 2nd level. Otto's
Irresistible Dance imposes a forced-dancing debuff with Prone-like
mechanical consequences (disadvantage on Dexterity saves, advantage to
attackers) but the PHB text never calls it any Appendix A condition by
name — modeling it as `prone` would be inferring a tag the spell's own
words don't use, the same discipline Otiluke's Resilient Sphere's
"enclosed, not restrained" decline already established.

## Game-backend quantization: 7th-level spell effects

The eighth slice of gap 1. All 20 7th-level spells classified against
the shapes already established — no schema change needed. 3 spells get
a `SpellDamageEffect` (Delayed Blast Fireball, Finger of Death, Fire
Storm), and for the first level in the whole pass, **zero spells get a
`SpellConditionEffect`** — not because 7th level has no condition-shaped
content, but because every candidate this level offers is too compound
to fit the field cleanly (see below). 17 of the 20 spells get neither
mechanism field.

**Finger of Death is the second `FlatDamageBonus` spell, and the first
to pair it with `HalfDamageOnSuccessfulSave`.** Disintegrate's "10d6 +
40" had no half-on-success clause; Finger of Death's "7d8 + 30 necrotic
damage... or half as much damage on a successful one" is the first
real spell to combine both facts, confirming the two fields are
independent the way the schema already assumed rather than requiring
a retrofit.

**Divine Word, Prismatic Spray, and Symbol are all declined as
compound/multi-mode mechanics, extending the Glyph of Warding precedent
to its most extreme cases yet.** Divine Word imposes a different effect
based on the target's current hit points: 50 hp or fewer, deafened;
40 or fewer, deafened and blinded; 30 or fewer, blinded, deafened, and
stunned; 20 or fewer, killed instantly — four different outcomes
gated by a fact about the target, not the spell. Symbol lets the caster
choose one of seven glyph effects at creation (Death's 10d10 necrotic
damage, Discord, Fear's frightened, Hopelessness, Insanity, Pain's
incapacitated, Sleep's unconscious, Stunning's stunned), each with its
own saving-throw ability and its own damage-or-condition shape — no
single `SpellDamageEffect`/`SpellConditionEffect` pair can represent
"the caster picks one of seven completely different effects when the
glyph is made." Prismatic Spray is the most compound spell built so
far: a d8 roll per target picks one of eight rays, each with its own
damage type or condition (four different damage types, a
restrained-toward-petrified escalation, blinded, banishment, or two
rerolls) — even further from a single resolution mechanic than Symbol.

**Mordenkainen's Sword is a fourth instance of the controllable-
construct decline**, after Spiritual Weapon, Bigby's Hand, and
Mordenkainen's Faithful Hound — a clean 3d10 force hit on a melee spell
attack, but delivered by a conjured weapon the caster repositions and
re-attacks with on a bonus action every turn, not a single resolved
spell effect.

**Forcecage and Plane Shift are both declined for the same "no damage,
no named condition" reason.** Forcecage's initial trapping requires no
saving throw at all — only a later escape *attempt* (teleportation)
triggers a Charisma save — and being trapped in a cage is never called
`restrained` or any other Appendix A condition. Plane Shift's
unwilling-creature option resolves through a melee spell attack that,
on a hit, banishes the target to another plane with no damage dealt at
all, the same "attack roll used for banishment, not damage" shape
Dispel Evil and Good's dismissal option already declined.

## Game-backend quantization: 8th-level spell effects

The ninth slice of gap 1, still no schema changes needed. 3 of the 18
8th-level spells get a `SpellDamageEffect` (Incendiary Cloud, Sunburst,
Tsunami), 3 get a `SpellConditionEffect` (Dominate Monster → `charmed`,
Earthquake → `prone`, Sunburst → `blinded`), and 13 get neither.

**Sunburst is the third spell to share one saving throw between
`DamageEffect` and `ConditionEffect`**, after Evard's Black Tentacles
(4th level) and Sunbeam (6th level) — a failed Constitution save deals
12d6 radiant damage (half on success) and separately blinds the
target. Three real instances across three different levels confirms
this dual-effect shape as a genuine recurring PHB pattern, not a
coincidence tied to any one spell level.

**Earthquake's knocked-prone effect is captured even though it's a
per-turn repeating trigger, because each instance is still a clean
"Dexterity save or prone" fact** — "at the end of each turn you spend
concentrating on it, each creature on the ground in the area must make
a Dexterity saving throw. On a failed save, the creature is knocked
prone." This is different from Cloud of Daggers/Heat Metal/Spike
Growth's declined automatic-zone-damage shape: those have no roll at
all, while Earthquake's repeating trigger is still save-gated every
time, so it fits `SpellConditionEffect` the same way a single-trigger
spell would.

**Power Word Stun is the first spell whose condition-shaped effect is
declined for having no saving throw gating its *initial* application at
all.** "If the target has 150 hit points or fewer, it is stunned.
Otherwise, the spell has no effect" — an automatic, hit-point-threshold
gate with no roll of any kind, the condition-effect analog of the
automatic-damage decline already established for Cloud of Daggers and
friends. The spell's own Constitution save exists only to *end* the
stun early each turn, the same "repeat save to end, not to avoid"
rider already declined for Hold Person/Hold Monster and Flesh to
Stone — but here there's no qualifying initial save at all for
`SpellConditionEffect.SavingThrowAbilityId` to point at, so the whole
effect is declined rather than just the repeat-save rider.

**Antipathy/Sympathy and Feeblemind are both declined, for two
different reasons already established elsewhere.** Antipathy/Sympathy
is a compound, mode-dependent effect (its Antipathy and Sympathy modes
behave oppositely, and only affects creature types the caster
specifies), the same "too compound/multi-mode" call that declined
Glyph of Warding, Symbol, and Prismatic Spray. Feeblemind reduces a
target's Intelligence and Charisma scores to 1 — a real, save-gated
effect, but a bespoke ability-score reduction rather than any named
Appendix A condition, the same "don't infer a condition tag the text
doesn't use" discipline that declined Otto's Irresistible Dance.

## Game-backend quantization: 9th-level spell effects (closing gap 1)

The tenth and final slice of gap 1, closing the Spells effect-data
initiative. Of the 16 9th-level spells, only **Weird** gets any effect
data — a `SpellDamageEffect` and a `SpellConditionEffect`, both gated
by the same Wisdom save (4d10 psychic damage, half on success, and
`frightened`). This is the sparsest level in the whole pass: the other
15 spells are dominated by exotic utility, transformation, and
"ultimate" multi-mode effects that don't fit either mechanism field,
not because the schema is missing something, but because 9th-level PHB
content genuinely trends away from the clean "attack roll or saving
throw, damage or condition" shape most other levels' combat spells use.

**Weird is the fourth spell to share one saving throw between
`DamageEffect` and `ConditionEffect`**, after Evard's Black Tentacles
(4th), Sunbeam (6th), and Sunburst (8th) — four real instances spanning
four different levels confirms this dual-effect shape as a genuine,
recurring part of the PHB's design vocabulary, not a coincidence tied
to any one spell level.

**Power Word Kill is the most extreme instance yet of the automatic-
effect decline**, more extreme than Power Word Stun's 8th-level
version: "If the creature has 100 hit points or fewer, it dies.
Otherwise, the spell has no effect" carries no attack roll, no saving
throw, and no dice damage expression at all — an unconditional
hit-point-threshold kill-or-nothing effect with nothing for either
mechanism field to hold.

**Meteor Swarm is a second 9th-level-scale instance of the
two-simultaneous-damage-types decline** (20d6 fire and 20d6
bludgeoning, both always apply), joining Ice Storm, Destructive Wave,
and Flame Strike from earlier levels — the same shape, now confirmed
at the top of the level range too.

**Imprisonment, Prismatic Wall, and Storm of Vengeance are all declined
as compound/multi-mode mechanics**, the same call already made for
Glyph of Warding, Symbol, and Prismatic Spray: each offers several
caster-chosen or round-by-round effects (Imprisonment's five
imprisonment forms, Prismatic Wall's seven color-coded layers, Storm of
Vengeance's per-round choice of thunder/acid/hail/lightning effects),
with different save abilities and different damage-or-condition shapes
depending on which option applies — never one resolution mechanic a
single `SpellDamageEffect`/`SpellConditionEffect` pair could represent.

**Gap 1 is closed.** All 361 real PHB spells (0 through 9th level) are
built with header-block data, and 78 of them now carry real
damage-and/or-condition effect data reflecting everything the PHB
actually specifies in a clean, quantizable shape. The full inventory:
`SpellDamageEffect` on 59 spells, `SpellConditionEffect` on 24, with
overlap on the spells noted throughout these ten sections. Gap 3
(feature-effect prose) is the initiative's next and final piece.

## Game-backend quantization: gap 3 scoping, and Metamagic effects

Gap 3 was only ever an audit note ("Metamagic effects, maneuver secondary
effects, invocation benefits") — unlike Combat/Adventuring, it had never
been read against the real PHB pages and turned into a ranked candidate
list before work started. **Scoped 2026-08-10** by reading pp.74–75
(Battle Master maneuvers), pp.102–103 (Metamagic), and pp.109–110
(Eldritch Invocations) directly, the same "read the actual pages before
assuming what's in scope" discipline the Combat/Adventuring scoping pass
already established:

| Candidate | Entries | Status |
| --- | --- | --- |
| Metamagic | 8 | **Built** |
| Battle Master maneuvers | 16 | **Built** |
| Eldritch Invocations | 32 | **Built** |

**Eldritch Invocations turned out to be the richest candidate by far,
and needed a new cross-domain reference that didn't exist when gap 3
was first named.** 19 of its 32 options (not the ~14 first estimated
during scoping — verify every count against the page images, not a
scoping-time guess) are literally "you can cast `<spell>` at will /
once per long rest / using a warlock spell slot" — a clean
`GrantedSpellId: SpellId?` reference into the Spells catalog, which
wasn't buildable when the "Spells are not a modeled domain" line in
"What becomes structured data vs. a citation" was written but is now,
since `SpellId`/`SpellDefinition` are a real domain (see the "Current
state" note on the game-backend initiative reversing that line). See
"Game-backend quantization: Eldritch Invocation effects" below for the
full build. **Elemental Disciplines and Channel Divinity options were
built next, closing this predicted tail** — see "Game-backend
quantization: Elemental Disciplines and Channel Divinity effects" for
the full build, including one further discovery neither prediction
made: Channel Divinity's Read Thoughts also grants a spell
(`suggestion`), and reading the real pages surfaced several
non-spell-grant facts (damage/save/push/condition effects, a
maximize-damage flag, an imposed-condition-plus-duration-trigger shape)
that the "mostly cast a spell" prediction undersold. Race/subclass/
background feature prose outside these five choice-point catalogs is a
separate, unscoped tail.

**Metamagic (all 8 options, p.102–103) is gap 3's first slice, chosen
for the same reason cantrips and Conditions were chosen first: smallest
closed set, proves the shape.** `MetamagicOptionDefinition` gained 9 new
fields — `ProtectsCreatureCountUpToSpellcastingModifier` (Careful
Spell), `DoublesRange` + `TouchRangeBecomesFeet` (Distant Spell, one
option needing two independent facts since it's genuinely two
alternative range modifications depending on the target spell's own
range), `RerollsDiceCountUpToSpellcastingModifier` (Empowered Spell),
`DoublesDurationMaxHours` (Extended Spell, capped at 24),
`GrantsDisadvantageOnFirstSavingThrow` (Heightened Spell),
`ChangesCastingTimeToBonusAction` (Quickened Spell),
`RemovesVerbalAndSomaticComponents` (Subtle Spell), and
`TargetsSecondCreatureInRange` (Twinned Spell, which was already fully
inferable from the existing cost field but now reads as an explicit
fact rather than an implication). Every field but the option's own is
`false`/`null`; existing IDs and citations didn't change — purely
additive, the same pattern the Conditions and Spell-effect passes both
followed.

**Shape: independent nullable/bool scalars, no "exactly one populated"
constraint — the `ChannelDivinityOptionDefinition` precedent, not
`FightingStyleDefinition`/`DivineStrikeProgressionDetail`'s.** Each of
the 8 options populates only 1 or 2 of the 9 new fields; they're not
alternative representations of one mechanic the way a weapon's
damage/range/versatile-damage fields are, so no exclusivity check was
added — only bounds validation on the two new numeric fields (both must
be positive when present).

**"Up to your Charisma modifier (minimum of one)" recurs twice (Careful
Spell's creature count, Empowered Spell's reroll count) and got two
separate, purpose-named bool fields rather than one shared type** — the
same "don't invent a type for two call sites" call `MaxChallengeRating`
already made; the two facts count different things (creatures vs. dice)
even though the scaling rule is identical.

**Three facts stay declined, all restrictions on which spell the option
can target rather than facts about the option itself:** Distant Spell
only applies to a spell "that has a range of 5 feet or greater" (or
touch), Extended Spell only to a duration of "1 minute or longer",
Twinned Spell only to a spell that "targets only one creature and
doesn't have a range of self". These are eligibility gates a consuming
engine would check against the chosen spell's own already-modeled
`SpellRange`/`SpellDuration`, not new data to store redundantly on the
option — the same reasoning that never made `RuleId` sharing carry a
class's option count. **One genuine compound-exception clause was also
declined:** Empowered Spell's "you can use Empowered Spell even if you
have already used a different Metamagic option during the casting of
the spell" — an exception to the p.102 framework rule that only one
Metamagic option normally applies per cast — stays in the citation, the
same "compound conditional" line Prone's attack-roll text and Plant
Growth's alternate casting time already sit on.

**All 8 citations were re-verified against the page images while
scoping, not just the effect text** — page 102 already matches where
"CAREFUL SPELL" through "TWINNED SPELL" actually start (confirmed
against the PDF's real footer, which for this section runs one page
behind the PDF's own page index); no citation error found this time,
but the check is run every pass regardless of outcome.

## Game-backend quantization: Battle Master maneuver effects

Gap 3's second slice, built against the ranked candidate list above.
`BattleMasterManeuverDefinition` already carried `EffectTarget` +
`SavingThrowAbilityId` from the original Quantized pass; this slice adds
the secondary effect every maneuver's saving throw actually gates, read
from p.75. 10 of the 16 maneuvers gained at least one new fact; the
other 6 (Commander's Strike, Evasive Footwork, Parry, Precision Attack,
Rally, Riposte) stay exactly as before — either an action-economy
redirect (who attacks, not a number) or an ability-modifier addition
already declined project-wide (Parry's "+ your Dexterity modifier",
Rally's "+ your Charisma modifier", the same reasoning that already
declined Cure Wounds' and Spiritual Weapon's ability-modifier terms).

**Two new cross-domain references, both reusing existing catalogs
instead of inventing bespoke types:** `ImposedConditionId: ConditionId?`
(Menacing Attack → frightened, Trip Attack → prone, both real Appendix A
condition names the PHB text uses directly — the same "don't infer a
condition tag the text doesn't use" discipline the Spells effect passes
already established, satisfied here because both maneuvers say the word)
and `MaximumTargetSizeId: CreatureSizeId?` (Pushing Attack and Trip
Attack's shared "if the target is Large or smaller" gate — a real
mechanical fact, since Huge/Gargantuan creatures are simply immune to
being pushed or tripped by these maneuvers, not fluff). Both catalogs
already existed for other reasons (Conditions' gap-2 quantization, the
creature-size vocabulary), so wiring them in was two more
`CatalogIntegrityValidator` checks, not new domains.

**A new duration shape: `BattleMasterManeuverDurationTrigger`
(`EndOfYourNextTurn` / `StartOfYourNextTurn`), independent of
`SpellDuration`.** Three maneuvers bound their secondary effect to the
attacker's own next turn, and the PHB uses two distinct phrasings for
it, not one: Goading Attack and Menacing Attack both read "until the
end of your next turn," Distracting Strike reads "before the start of
your next turn" — a shorter window, not the same fact reworded. This
doesn't fit `SpellDuration` (that type is calendar/real-time: rounds,
minutes, hours, days, until dispelled) — a combat-turn-relative
duration is a genuinely different axis, so it got its own small enum
rather than stretching `SpellDuration` to cover a case it was never
built for. Kept scoped to Battle Master maneuvers for now, the same
"never build the general shape by default" discipline `RollModifier`'s
move to `Rules/Common` didn't get applied to `SpeechRestriction`;
revisit only if a second domain needs the identical concept. **The
duration trigger is validator-enforced to never appear without a
secondary effect it could bound** (`ImposedConditionId`,
`GrantsAdvantageToNextAttackAgainstTarget`, or
`ImposesDisadvantageOnAttacksAgainstOthers`), the same paired-field
discipline `DowntimeActivityDefinition` already established for a
saving-throw ability and its DC. **The reverse isn't required** — Trip
Attack's `prone` has no stated expiration in the maneuver's own text
(Prone ends the normal way, standing up), so a condition can be imposed
with no duration trigger at all.

**Five more one-off scalars, one per maneuver, no shared shape between
them:** `ForcesDroppedItem` (Disarming Attack), `ReachIncreaseFeet: 5`
(Lunging Attack), `PushDistanceFeet: 15` (Pushing Attack, alongside its
size gate), `SecondaryTargetRangeFeet: 5` (Sweeping Attack — only the
"within 5 feet of the original target" half of its range clause; the
"and within your reach" half is a formula relative to the wielder's own
weapon, already modeled via `WeaponDefinition`, and stays declined the
same way Parry's ability-modifier addition does), and
`AllowsAllyReactionMovement` (Maneuvering Attack). **Two facts stay
declined for reasons already established elsewhere:**
`GrantsAdvantageOnNextAttackRoll` (Feinting Attack) needed no new
numeric field at all, just a bool — the "advantage on your next attack
roll" is self-limiting with no duration to track. And Maneuvering
Attack's "move up to half its speed" is declined outright, not
partially captured — it's a formula relative to a value this project
already models elsewhere (the ally's own speed), the exact same
reasoning that declined the Long Rest's "half your total Hit Dice";
only the boolean fact that the reaction movement exists at all is
captured, not the fraction.

**All 16 citations were re-verified against the page images while
building this slice** — page 74 already matches where "COMMANDER'S
STRIKE" through "TRIP ATTACK" actually start (the PDF's real footer for
this section, like Metamagic's, runs one page behind the PDF's own page
index); no citation error found.

## Game-backend quantization: Eldritch Invocation effects

Gap 3's third and final scoped slice, closing the ranked candidate
table. `EldritchInvocationDefinition` already carried three prerequisite
facts (`RequiresEldritchBlastCantrip`, `RequiredMinimumLevel`,
`RequiresPactBoon`); this slice adds each invocation's actual *benefit*,
read from the rendered page images at pp.110–111 — **not from
`pdftotext`**, since this is a two-page, four-column layout and
`pdftotext -layout`'s column-interleaving genuinely scrambled the
reading order on a first pass (confirmed by rendering both pages to
300 dpi PNGs with `pdftoppm` and reading them directly, the same
"locate with text, read values from the image" split the citation rules
already mandate — this domain just needed the image for the *locating*
step too, not only the values). 28 of the 32 invocations gained at
least one new fact; 4 (Book of Ancient Secrets, Gaze of Two Minds, One
with Shadows, Voice of the Chain Master) stay exactly as before, all
genuinely compound/multi-part mechanics — the richest slice of gap 3 by
a wide margin, as the scoping pass predicted.

**The headline addition: `GrantedSpellId: SpellId?` paired with
`EldritchInvocationCastingFrequency?` (`AtWill` /
`OncePerLongRestUsingASpellSlot`) — this project's first-ever
cross-domain reference *into* the Spells catalog from outside it.**
Every prior spell cross-reference ran the other direction (a spell's
own `AvailableToClassIds`); this is the first definition anywhere that
points *at* a `SpellId`. 19 of the 32 invocations are a clean "cast
`<spell>`" grant — 12 `AtWill` (Armor of Shadows, Ascendant Step, Beast
Speech, Chains of Carceri, Eldritch Sight, Fiendish Vigor, Mask of Many
Faces, Master of Myriad Forms, Misty Visions, Otherworldly Leap, Visions
of Distant Realms, Whispers of the Grave) and 7
`OncePerLongRestUsingASpellSlot` (Bewitching Whispers, Dreadful Word,
Minions of Chaos, Mire the Mind, Sculptor of Flesh, Sign of Ill Omen,
Thief of Five Fates) — both fields validator-paired (present together or
neither), the same discipline `DowntimeActivityDefinition`'s
saving-throw/DC pair and this project's other paired-field checks
already established. `CatalogIntegrityValidator` gained its first
`HashSet<SpellId>` (built from `definitions.Spells`) to back the
reference-integrity check, the same shape every other cross-domain
check in that validator already uses.

**A third fact splits the 12 `AtWill` grants down the middle, and it's
a real PHB distinction, not noise:** `WaivesMaterialComponents` is
`true` for exactly 6 of the 12 (Armor of Shadows, Ascendant Step, Chains
of Carceri, Fiendish Vigor, Misty Visions, Otherworldly Leap — all
printed "at will, without expending a spell slot **or material
components**") and `false` for the other 6 (Beast Speech, Eldritch
Sight, Mask of Many Faces, Master of Myriad Forms, Visions of Distant
Realms, Whispers of the Grave — printed "at will, without expending a
spell slot" with no components clause at all, meaning the caster still
needs to provide them normally). Reading `pdftotext`'s garbled column
order would have made this an easy detail to miss or homogenize;
verified per-entry against the rendered image instead.

**Two invocations add the spellcasting ability modifier straight to a
damage roll, mirroring Metamagic's Careful/Empowered Spell "up to your
Charisma modifier" shape rather than the declined "dice + modifier"
line Parry/Rally/Cure Wounds sit on** — because here the *entire*
damage fact is the modifier, no dice at all. `AddsSpellcastingModifierToDamage`
is `true` for Agonizing Blast (boosts Eldritch Blast's own force
damage, no new damage type introduced, so `ExtraDamageTypeId` stays
null) and Lifedrinker (adds a wholly new necrotic damage source
alongside the pact weapon's own damage, so `ExtraDamageTypeId` = necrotic).
**Lifedrinker's printed "(minimum 1)" clause is not stored as a
separate field** — Agonizing Blast's identical mechanic prints no such
clause, and the project's "store what's printed, don't infer" rule
means the difference stays in the citation rather than being
normalized away or invented for the entry that lacks it.

**`SkillProficiencyIds` reuses the exact `IReadOnlyList<SkillId>` shape
`BackgroundDefinition` already established**, populated only for
Beguiling Influence (Deception, Persuasion) — the same "two fixed
skills, no choice" pattern every background grant already uses, just on
a fourth domain now instead of a third.

**Four more one-off scalars, no shared shape between them, the same
"several independent purpose-named fields" discipline Metamagic and
Battle Master maneuvers both already used:** `DarknessVisionRangeFeet:
120` (Devil's Sight) and `TrueSightRangeFeet: 30` (Witch Sight) are two
different vision mechanics that don't share a field, the same
"Blindsense/Feral Senses/Divine Sense each get their own detail type"
precedent from the Quantized-mechanics tail; `EldritchBlastRangeFeet:
300` (Eldritch Spear) and `EldritchBlastPushDistanceFeet: 10` (Repelling
Blast) are both scoped specifically to Eldritch Blast rather than named
generically, since neither fact applies to any other spell an
invocation could ever touch. `CanReadAllWriting` (Eyes of the Rune
Keeper) and `GrantsSecondPactWeaponAttack` (Thirsting Blade) round out
the slice — each a real, singular, quantizable fact used by exactly one
invocation, the same standing precedent Metamagic's
`ProtectsCreatureCountUpToSpellcastingModifier` already set: a fact
used once still earns a field if it's genuinely a fact, not a
compound/DM-adjudicated rule.

**Chains of Carceri's creature-type restriction and per-target
cooldown stay declined, riding on top of a captured core grant** — "You
can cast *hold monster* at will—targeting a celestial, fiend, or
elemental—...You must finish a long rest before you can use this
invocation on the same creature again" captures cleanly as
`GrantedSpellId`/`AtWill`/`WaivesMaterialComponents`, but the
creature-type filter and the unusual *per-target* (not per-caster)
cooldown are riders no field represents, the same "per-spell secondary
rider stays unquantized" call already made throughout the Spells
effect-data passes.

**Book of Ancient Secrets is declined for the same reason Wizard's own
spellbook-copying costs were declined, and the two turn out to state
the identical formula.** Its "choose two 1st-level ritual spells from
any class's list" grant is an open-ended choice, not a single
`GrantedSpellId` — the same "caster picks from a list" shape that
already declined Symbol and Prismatic Spray's per-option choices at the
Spells pass. Its later "add other ritual spells" paragraph prices
transcription at 2 hours + 50 gp per spell level — verified against
`WizardSpellbookDetail`'s own declined copying-cost prose and found to
be the exact same figures, not a coincidence worth re-deriving as a new
fact.

**Gaze of Two Minds and Voice of the Chain Master both extend the
controllable/perceiving-through-another-creature decline already
established for Spiritual Weapon, Bigby's Hand, and Mordenkainen's
Faithful Hound** — each is a multi-turn perception/communication link
maintained action-by-action, not a single resolved effect. **One with
Shadows is declined as a conditional trigger** (invisibility gated on
being in dim light or darkness, ended by moving/acting/reacting), the
same shape Prone's range-conditional attack-roll text and Plant
Growth's alternate casting time already sit on.

**All 32 citations were re-verified against the rendered page images
while building this slice** — pages 110 and 111 already match where
each invocation's own name heading starts (the same one-page-behind
footer offset Metamagic and Battle Master maneuvers both showed); no
citation error found. **Gap 3's three originally scoped candidates were
all built at this point**; the predicted follow-on tail (Elemental
Disciplines, Channel Divinity options) was picked up next — see
"Game-backend quantization: Elemental Disciplines and Channel Divinity
effects" below. Every class/subclass/race/background feature outside
these five choice-point catalogs remains unquantized and unscoped.

## Game-backend quantization: Elemental Disciplines and Channel Divinity effects

The predicted follow-on to gap 3's three scoped candidates, built
immediately after Eldritch Invocations closed them. Both catalogs
already carried partial quantization from the original Quantized pass
(`ElementalDisciplineDefinition` had `KiPointCost`/`RequiredMinimumLevel`;
`ChannelDivinityOptionDefinition` had `RangeFeet`/`SavingThrowAbilityId`/
`DurationMinutes`/`RollBonus`) — this slice adds each option's actual
*effect*, the same shift gap 3's other three catalogs made. Both were
read from rendered page images, not `pdftotext`: Elemental Disciplines'
17 entries turned out to fit on one single two-column page (p.81, PDF
index 82); Channel Divinity's 10 span five different pages (pp.59–63,
one per cleric domain) since each option is that domain's own 2nd-level
feature, not a grouped list like the other four gap-3 catalogs.

**Elemental Disciplines: 12 of 17 are a clean `GrantedSpellId` grant,
confirming the prediction — but 3 more are combat techniques with their
own damage/save/rider facts the prediction didn't anticipate, and 2
stay fully declined.** The 12 spell grants (Breath of Winter → cone of
cold, Clench of the North Wind → hold person, Eternal Mountain Defense
→ stoneskin, Fist of Four Thunders → thunderwave, Flames of the Phoenix
→ fireball, Gong of the Summit → shatter, Mist Stance → gaseous form,
Ride the Wind → fly, River of Hungry Flame → wall of fire, Rush of the
Gale Spirits → gust of wind, Sweeping Cinder Strike → burning hands,
Wave of Rolling Earth → wall of stone) need no `CastingFrequency`
companion field the way Eldritch Invocations did — every one is simply
"spend N ki points to cast `<spell>`," and `KiPointCost` already covers
the cost, so `GrantedSpellId` stands alone here.

**Fist of Unbroken Air and Water Whip are the same "3d10 bludgeoning,
save for half" shape confirmed on a second and third instance, reusing
`SavingThrowAbilityId`/`BaseDamage`/`BaseDamageTypeId`/
`HalfDamageOnSuccessfulSave`/`RangeFeet` — new fields on
`ElementalDisciplineDefinition`, not shared with `SpellDamageEffect`,
per the standing "each domain owns its own type even when the shape is
identical" rule.** Where the two diverge is where one stays clean and
the other gets declined: Fist of Unbroken Air's "push up to 20 feet
away *and* knock prone" is a clean AND, captured as
`PushDistanceFeet`/`ImposedConditionId` both populated (the existing
"two independent fields populated together = both apply" convention
Evard's Black Tentacles and Sunbeam already established for spells).
Water Whip's "either knock it prone *or* pull it up to 25 feet closer"
is a genuine OR — a caster's choice this project has no field for
(`SpellConditionEffect.ConditionIds` only ever meant AND, per
Blindness/Deafness's decline at 2nd level) — so Water Whip captures
only its clean damage/save facts and declines the prone-or-pull choice
entirely, rather than populating fields that would misstate an OR as an
AND.

**Fangs of the Fire Snake is a weapon-attack rider, not a save-based
effect, and gets its own pair of one-off fields:** `ReachIncreaseFeet:
10` and `ChangesUnarmedDamageTypeId: fire`. Its further "spend 1 more ki
point for an extra 1d10 fire damage" clause stays declined — a single,
non-recurring conditional-cost rider, the same "one data point doesn't
justify a new field" call Magic Missile's flat modifier and Chromatic
Orb's minimum-one clause already made elsewhere.

**Elemental Attunement and Shape the Flowing River stay fully declined,
both for the established "caster picks from several options, no single
resolution mechanic" reason** — Elemental Attunement offers four minor
sensory/utility effects, Shape the Flowing River is a multi-mode
terrain tool (raise, lower, dig, wall, pillar) bounded only by a shared
formula ("half the area's largest dimension"). Same call as Symbol and
Prismatic Spray at the Spells pass.

**Channel Divinity: a new cross-domain grant (`GrantedSpellId` again,
this time on `ChannelDivinityOptionDefinition`) turned up in a place
neither the original scoping nor the Eldritch Invocations prediction
named.** Read Thoughts's text ends with "you can use your action to end
this effect and cast the *suggestion* spell on the creature without
expending a spell slot. The target automatically fails its saving
throw against the spell" — a real spell grant, gated on the option's own
mind-reading effect already being active rather than framed as
"at will" or "once per long rest" the way Eldritch Invocations phrases
its grants. No `CastingFrequency` companion field was added here: unlike
Eldritch Invocations' repeatable grants, Channel Divinity options are
already bounded by the class's own Channel Divinity use economy
(`ChannelDivinityUseGrant`), so a per-option frequency fact would be
redundant. The one new fact this option's text actually adds —
`AutomaticallyFailsGrantedSpellSave: bool` — is captured directly.

**A new shared duration shape moved out of Battle Master maneuvers and
into `Rules/Common`, the second-domain trigger CLAUDE.md's Battle
Master section explicitly flagged to watch for.** Cloak of Shadows
("you become invisible until the end of your next turn") needs the
exact same combat-turn-relative duration Menacing Attack and Goading
Attack already used. `BattleMasterManeuverDurationTrigger` is now
`NextTurnDurationTrigger` in `Rules/Common/`, used by both
`BattleMasterManeuverDefinition.SecondaryEffectDurationTrigger` and the
new `ChannelDivinityOptionDefinition.ConditionDurationTrigger` — the
same "a mechanic common enough to show up in a second domain graduates
to shared vocabulary" reasoning that already moved `RollModifier` to
`Rules/Common`, now demonstrated for real rather than just predicted.
Cloak of Shadows also reuses `ImposedConditionId` (invisible) — a field
this pass added to `ChannelDivinityOptionDefinition` for the first
time, populated a second time by Charm Animals and Plants (charmed, on
a failed Wisdom save, no duration trigger since the condition's own
1-minute-or-damage end condition is already captured via the existing
`DurationMinutes` field).

**Destructive Wrath needed a field no other gap-3 catalog has: a flat
"maximize this roll instead of rolling it" flag.** "When you roll
lightning or thunder damage, you can use your Channel Divinity to deal
maximum damage, instead of rolling" is genuinely a new mechanism shape
— not a bonus, not a reroll, a replacement of the roll with its maximum
— so it earns its own bool, `MaximizesDamageRoll`, rather than being
forced into `RollBonus` (which adds to a roll, not replaces it).

**Six options needed no new fields at all, confirming the original
four scalar fields already covered them fully:** Knowledge of the Ages,
Preserve Life, Radiance of the Dawn, Invoke Duplicity, Guided Strike,
and War God's Blessing were all re-verified against the rendered page
images and matched their existing `RangeFeet`/`SavingThrowAbilityId`/
`DurationMinutes`/`RollBonus` data exactly. Invoke Duplicity's illusion-
duplicate mechanic (30-foot placement range, 120-foot leash, advantage
when both caster and illusion flank a target) stays partially declined
beyond its two already-captured numbers, the same "controllable/
perceiving" shape already declined for Bigby's Hand and Spiritual
Weapon.

**All citations were re-verified against the rendered page images while
building this slice** — Elemental Disciplines' page 81 and Channel
Divinity's pp.59–63 all matched their existing citations exactly; no
error found. **This closed the two-catalog tail Eldritch Invocations'
build predicted, completing all five choice-point catalogs** — but see
the next section: Channel Divinity itself turned out to be
undercounted, and the wider race/subclass/background feature-prose
tail got its own real scoping pass next.

## Game-backend quantization: scoping the race/subclass/background feature-prose tail

Gap 3's audit line ("most class/subclass/race/background feature text
is unquantized prose") was never actually turned into a ranked
candidate table the way Combat/Adventuring and the five choice-point
catalogs were — it just got read as "build the five obvious choice-point
catalogs" and stopped there. This pass (2026-08-10) did the table:
extracted all 355 `RuleId`s across `class-rule.json` (305),
`race-rule.json` (37), and `background-rule.json` (13), cross-referenced
every one against everything this file already documents as quantized
or declined, and triaged the genuinely-never-assessed remainder against
real D&D 5e domain knowledge (not yet page-image-verified except where
noted as built below — that verification is the gate before building
any other item in this list, same as every other pass in this project).

**Headline finding: the "Channel Divinity options (10)" catalog from
the original Quantized pass was never complete — it only ever covered
the 7 Cleric domains.** Paladin's three oaths (Devotion, Ancients,
Vengeance) each grant two Channel Divinity options of their own —
Sacred Weapon, Turn the Unholy, Nature's Wrath, Turn the Faithless,
Abjure Enemy, Vow of Enmity — and none of them existed anywhere in the
codebase, not even as a citation-only entry in the standalone catalog
(their `class-rule.json` framework citations existed and cited real
pages, but the mechanical data itself was simply never captured). This
is now fixed — see the next section — and the catalog is 16 entries,
not 10. **This is the second time a "closed, N-item" catalog turned out
to be undercounted** (Eldritch Invocations was ~14 estimated vs. 19
real); treat every catalog count in this file as re-verifiable, not
just spell/page counts.

**The rest of the pool, condensed to what's buildable.** Four
recurring shapes account for most of the real candidates found; none
of the specific numbers below have been checked against page images
yet:

- **More choice-point catalogs never built, same shape as Fighting
  Style/Battle Master maneuvers:** Circle of the Totem Warrior
  (Barbarian) has three separate Bear/Eagle/Wolf choice points (Totem
  Spirit 3rd, Aspect of the Beast 6th, Totemic Attunement 14th); Ranger
  Hunter has four (Hunter's Prey, Defensive Tactics, Multiattack,
  Superior Hunter's Defense); Monk's Open Hand Technique (3rd level) is
  a 3-option Flurry of Blows rider; Wizard's The Third Eye (Divination
  14th) and Transmuter's Stone (Transmutation 6th) are both 4-option
  utility choices.
- **More `GrantedSpellId` candidates, reusing the exact shape gap 3's
  tail established:** Drow Magic and Infernal Legacy (racial per-level
  spell grants, same shape as Eldritch Invocations' at-will/once-per-
  rest split), Natural Illusionist (Forest Gnome, Minor Illusion
  cantrip), Thousand Forms (Druid Circle of the Moon, Alter Self at
  will), Shapechanger (Wizard Transmutation, Polymorph once per rest),
  Light Domain's and Circle of the Land's Bonus Cantrip.
- **Rich individual features with real damage/save/condition/duration
  facts**, the same shape as the maneuvers/invocations work: Warlock
  patron features (Fey Presence, Misty Escape, Beguiling Defenses, Dark
  Delirium for Archfey; Entropic Ward, Awakened Mind for Great Old
  One), Wizard school features (Hypnotic Gaze, Instinctive Charm for
  Enchantment; Overchannel, Sculpt Spells, Empowered Evocation for
  Evocation), Barbarian Berserker (Relentless Rage's escalating DC,
  Intimidating Presence), Sorcerer Draconic Bloodline (Elemental
  Affinity), and Rogue (Second-Story Work, Assassinate, Infiltration
  Expertise, Death Strike).
- **A recurring "X uses per rest, equal to an ability modifier
  (minimum one)" resource shape** appears on Warding Flare (Cleric
  Light), War Priest (Cleric War), and Cleansing Touch (Paladin base)
  — checked against the codebase and confirmed **not yet captured
  anywhere, including on the already-"quantized" Wrath of the Storm**
  (`WrathOfTheStormDetail` has no uses-per-rest field at all). This
  would be a new field shape, not a reuse of an existing one.

**One genuinely new structural gap, bigger than a data-entry slice:**
racial weapon/armor/skill proficiency grants (Elf Weapon Training,
Dwarven Combat Training, Dwarven Armor Training, Drow Weapon Training,
Keen Senses, Skill Versatility, ...) have no field anywhere on
`RaceDefinition`/`SubraceDefinition` — proficiency grants were only
ever modeled on `ClassDefinition`. Building this needs a real design
decision (a new field shape mirroring the class weapon-proficiency
category-plus-named-exceptions pattern, or something narrower), not
just verifying numbers against a page.

**A large fraction of the "not yet mentioned" 355 turned out to
already be accounted for once cross-referenced properly** — this
matters because a naive name-match against this file undercounts by
category, not just by typo. Subclass-gateway `RuleId`s (Martial
Archetype, Primal Path, Otherworldly Patron, Sorcerous Origin, Sacred
Oath, ...) were never going to be quantized — the subclass itself is
the modeled entity. Every "Channel Divinity: X" `class-rule.json` entry
is a framework citation whose real data lives in the separate
standalone catalog, not a second copy of the same gap. Every school's
`Savant` entry is already covered collectively by the existing
"Deliberately prefixed splits" note, and its actual halved-cost fact
is correctly still declined since the base spellbook-copying cost it
halves is itself a declined formula (`WizardSpellbookDetail`). Several
features that read as new by name are already captured under a
different field name entirely — Dwarven Toughness is `HitPointBonusPerLevel`,
Superior Darkvision is `DarkvisionRangeFeet`, Fleet of Foot is the
subrace `Speed` override, Draconic Ancestry/Damage Resistance back the
already-quantized Breath Weapon and `ResistedDamageTypeIds`. **Verify
against the field, not the feature name, before assuming something is
missing** — this cost real time during this pass (Combat Superiority
looked unmentioned only because its name wraps across two source
lines; a naive substring search missed it).

**Backgrounds show no strong candidates** — the domain's own
structural simplicity (13 backgrounds, one feature each, already fully
triaged into the 4 existing scalars during the original pass) held up;
nothing found here contradicts "Backgrounds: structurally the simplest
domain" above.

## Game-backend quantization: Paladin's missing Channel Divinity options

The first slice built from the scoping pass above, chosen for the same
reason Metamagic went first at gap 3: smallest, and here specifically
because it's not a new candidate so much as a **correction to an
already-"closed" catalog** — `ChannelDivinityOptionCatalog` goes from
10 entries to 16. All 6 verified against rendered page images at
pp.86–88 (Oath of Devotion, Oath of the Ancients, Oath of Vengeance),
matching the pages `class-rule.json`'s existing framework citations
for these features already used.

**Five new fields on `ChannelDivinityOptionDefinition`, each earned by
a real, distinct fact — no shared shape invented ahead of the content:**

- `AddsSpellcastingModifierToAttackRolls: bool` (Sacred Weapon) — adds
  Charisma modifier to attack rolls with an imbued weapon; the
  printed "(with a minimum bonus of +1)" floor stays declined, the
  same "store what's printed, don't infer a clause the text doesn't
  restate elsewhere" call Agonizing Blast's missing minimum-1 clause
  already made.
- `BrightLightRadiusFeet`/`DimLightRadiusFeet: int?` (Sacred Weapon) —
  20/20, the first "bright light X, dim light Y beyond" pair captured
  anywhere in this codebase; always populated together, validator-paired
  the same way `BaseDamage`/`BaseDamageTypeId` already are.
- `ChoosableSavingThrowAbilityIds: IReadOnlyList<AbilityId>` (Nature's
  Wrath) — "a Strength or Dexterity saving throw (its choice)," the
  first "target's choice of ability" fact this project has captured.
  Mirrors `SpellDamageEffect`'s fixed-or-choosable-list shape
  (`DamageTypeId?`/`ChoosableDamageTypeIds`) but for saving-throw
  ability instead of damage type — same shape, new axis, validator-
  enforced mutually exclusive with the existing `SavingThrowAbilityId`
  and required to hold at least two options (a "choice" of one isn't a
  choice).
- `GrantsAdvantageOnAttackRollsAgainstTarget: bool` (Vow of Enmity) —
  named to match `BattleMasterManeuverDefinition`'s existing
  `GrantsAdvantageToNextAttackAgainstTarget`/`GrantsAdvantageOnNextAttackRoll`
  convention exactly, the same mechanic on a third domain now.

**Turn the Unholy and Turn the Faithless needed no new fields at
all** — both fit the existing `RangeFeet`(30)/`SavingThrowAbilityId`(wisdom)/
`DurationMinutes`(1) shape exactly, the same shape the base Cleric Turn
Undead framework uses. Neither captures the "turned" status itself as
an `ImposedConditionId`, because **Turned is not one of the 15
Appendix A conditions** — it's a bespoke effect described inline in
the feature text, so (matching the "don't infer a condition tag the
text doesn't use" discipline already established for Otiluke's
Resilient Sphere and Otto's Irresistible Dance) it stays undescribed
structurally, the same way it would if this were a spell effect.

**Two riders stay declined on real, otherwise-captured options, both
matching existing decline categories:** Abjure Enemy's "fiends and
undead have disadvantage on this saving throw" is a creature-type-
conditional save modifier (the same shape already declined for Chains
of Carceri's creature-type restriction), and its "on a successful
save, the creature's speed is halved" is a partial-success rider (the
same "per-option secondary rider stays unquantized" call made
throughout the Spells effect-data passes). Nature's Wrath's repeat-
save-to-escape-the-restrain mechanic stays declined too, the same
"repeat save to end, not to avoid" rider already declined for Hold
Person/Hold Monster and Flesh to Stone.

**All 6 citations were verified against the rendered page images, not
carried over from the pre-existing `class-rule.json` framework
citations** — they matched exactly (pp.86, 87, 88), but the check ran
regardless of that outcome, the same discipline every prior slice in
this file followed.

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
