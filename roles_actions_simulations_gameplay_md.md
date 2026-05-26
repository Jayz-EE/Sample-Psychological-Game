# Roles_Actions_Simulations_Gameplay.md

# Complete Role System

This document defines:
- role actions
- day/night abilities
- traces left behind
- gameplay effects
- simulation logic
- progression impact
- suspicion generation
- world consequences

The systems are designed to remain modular, scalable, and data-driven.

---

# ROLE CATEGORIES

| Category | Description |
|---|---|
| Good | Preserve village and eliminate evil |
| Evil | Destroy village and eliminate resistance |
| Good Neutral | Independent goals with possible alliance |
| Evil Neutral | Self-serving roles causing instability |
| Fixed Neutral | Static world role influencing economy/info |

---

# GOOD ROLES

# Detective

## Gameplay Identity
Investigation and contradiction analysis.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Investigate House | Finds clues/evidence | Opened cabinets, disturbed objects | Increases clue discovery |
| Interrogate NPC | Gains statements | Witnessed conversations | Reveals contradictions |
| Analyze Evidence | Improves evidence accuracy | Investigation notes | Reduces false suspicion |
| Compare Alibis | Detects inconsistencies | Written comparisons | Raises suspicion logically |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Stakeout | Observe NPC movement | Footprints, lantern use | Reveals hidden actions |
| Track Target | Follow suspect | Trail marks | Gains hidden location logs |
| Secret Surveillance | Gather movement info | Hidden notes | Creates rumors |

---

## Risks
- Frequently seen investigating
- May be targeted by evil roles
- Can falsely accuse innocents

---

# Doctor

## Gameplay Identity
Protection and injury analysis.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Heal NPC | Restore health | Medical residue | Builds trust |
| Diagnose Injury | Identify wound source | Medical notes | Reveals attack type |
| Treat Panic | Reduce fear | Medicine use | Stabilizes NPC behavior |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Protect NPC | Prevent death | Bandages, medicine | Counters evil actions |
| Emergency Treatment | Heal hidden wounds | Bloody cloth | May hide clues |
| Secret Recovery | Conceal injuries | Medical waste | Creates suspicion |

---

## Risks
- Medicine shortages
- Helping evil roles accidentally
- Blood traces may implicate Doctor

---

# Priest

## Gameplay Identity
Spiritual protection and fear control.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Bless House | Temporary protection | Holy markings | Reduces curse chance |
| Calm Villagers | Reduce panic | Prayer gatherings | Stabilizes social trust |
| Conduct Ritual | Detect corruption | Incense smoke | Narrows evil suspects |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Purify Area | Remove curse effects | Burnt incense | Weakens Witch |
| Sense Evil Presence | Detect corruption zones | Ritual symbols | Reveals dangerous areas |
| Night Prayer | Lower village fear | Candles | Slows panic escalation |

---

## Risks
- Highly targeted by evil roles
- Failed rituals reduce trust

---

# Prosecutor

## Gameplay Identity
Public accusation and council influence.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Public Accusation | Raise suspicion | Council record | Influences voting |
| Review Statements | Detect lies | Written notes | Increases logical deductions |
| Demand Testimony | Force responses | Witnesses | Raises tension |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Secret Investigation | Gather hidden info | Seen wandering | Risky intelligence gathering |
| Collect Records | Compare evidence | Stolen documents | Reveals contradictions |

---

## Risks
- False accusations cause panic
- Can destabilize village rapidly

---

# EVIL ROLES

# Witch

## Gameplay Identity
Manipulation, curses, misinformation.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Buy Ingredients | Prepare rituals | Rare purchases | Creates suspicion |
| Spread Fear | Increase paranoia | Strange behavior | Weakens trust |
| Plant Rumors | Manipulate suspicions | Gossip spread | Frames targets |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Curse NPC | Debuff target | Ritual circles | Weakens victims |
| Plant False Evidence | Mislead investigations | Fake clues | Redirects suspicion |
| Perform Ritual | Increase corruption | Dark symbols | Escalates horror |

---

## Risks
- Ritual traces detectable by Priest
- Ingredient purchases suspicious

---

# Crawler

## Gameplay Identity
Stealth predator and fear generator.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Hide in Shadows | Avoid suspicion | Minimal traces | Hard to detect |
| Observe NPCs | Learn schedules | Strange sightings | Gains target data |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Stalk Target | Build fear | Claw marks | Raises panic |
| Ambush NPC | Kill target | Blood trails | Population reduction |
| Crawl Through Village | Terrorize villagers | Disturbed dirt | Fear escalation |

---

## Risks
- Animalistic traces reveal existence
- Loud attacks attract attention

---

# Butcher

## Gameplay Identity
Physical killer with economic disguise.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Sell Meat | Earn trust/resources | Blood stains | Supports economy |
| Clean Tools | Remove evidence | Water residue | Hides clues |
| Trade Supplies | Maintain cover | Shop records | Appears useful |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Kill NPC | Eliminate target | Blood pools | Reduces village population |
| Dispose Body | Hide evidence | Drag marks | Delays discovery |
| Harvest Flesh | Gain resources | Bone remains | Disturbing discoveries |

---

## Risks
- Blood evidence accumulates
- Missing meat patterns suspicious

---

# Headless

## Gameplay Identity
Supernatural terror and fear corruption.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Manifest Briefly | Terrify villagers | Panic rumors | Fear increase |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Haunt Area | Increase insanity | Cold spots | Alters NPC schedules |
| Terrorize NPC | Cause panic | Whispering sounds | Fear escalation |
| Curse Ground | Corrupt locations | Black residue | Dangerous zones |

---

## Risks
- Supernatural traces reveal evil presence
- Panic may unite villagers

---

# GOOD NEUTRAL ROLES

# Farmer

## Gameplay Identity
Food production and village stability.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Sow Crops | Future food supply | Fresh soil | Prevents famine |
| Harvest Crops | Gain food | Crop changes | Stabilizes economy |
| Sell Produce | Earn money | Trade records | Builds trust |
| Fertilize Land | Improve yield | Chemical smell | Better harvest rates |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Guard Crops | Prevent theft | Lantern light | Protects food |
| Hide Supplies | Secret reserves | Buried sacks | Creates suspicion |

---

## Goal
- Maintain food production
- Escape village or join faction

---

# Alchemist

## Gameplay Identity
Potion crafting and chemical manipulation.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Brew Potions | Create effects | Strange smells | Powerful support items |
| Buy Ingredients | Expand experiments | Rare purchases | Suspicious behavior |
| Sell Remedies | Gain trust | Medicine bottles | Resource economy |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Experiment | Random effects | Chemical residue | Unpredictable outcomes |
| Poison Resources | Cause illness | Tainted food | Panic generation |
| Distill Elixir | Create rare buffs | Glass fragments | Strong utility |

---

## Goal
- Complete research objectives
- Survive village collapse

---

# Hunter

## Gameplay Identity
Tracking and wilderness survival.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Hunt Animals | Gain meat | Carcasses | Food generation |
| Track Footprints | Find movement clues | Trail marks | Investigation support |
| Sell Meat | Earn money | Market logs | Resource economy |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Patrol Forest | Detect threats | Boot prints | Prevents ambushes |
| Set Traps | Slow enemies | Rope traps | Defensive utility |
| Follow Sounds | Discover activity | Broken branches | Clue discovery |

---

## Goal
- Protect self or village
- Escape alive

---

# Scholar (Former Hacker)

## Gameplay Identity
Knowledge and behavioral analysis.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Analyze Records | Discover inconsistencies | Open documents | Better deduction |
| Study Patterns | Predict actions | Written notes | Suspicion accuracy |
| Decode Symbols | Understand rituals | Research scraps | Counter Witch |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Secret Observation | Gather movement info | Hidden notes | Information advantage |
| Hidden Research | Discover secrets | Candles at night | Raises suspicion |

---

## Goal
- Discover hidden truth
- Survive long enough to escape

---

# EVIL NEUTRAL ROLES

# Thief

## Gameplay Identity
Resource theft and economic disruption.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Scout Houses | Plan theft | Seen wandering | Suspicion increase |
| Trade Stolen Goods | Earn money | Suspicious items | Economic instability |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Steal Resources | Gain supplies | Broken locks | Resource shortages |
| Sneak Into Houses | Gather info | Footprints | Suspicion generation |
| Pickpocket NPC | Steal valuables | Missing items | Distrust increase |

---

## Goal
- Accumulate wealth
- Escape village alive

---

# Voyeur

## Gameplay Identity
Information gathering and blackmail.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Listen to Rumors | Gain secrets | Seen eavesdropping | Information control |
| Sell Information | Earn resources | Gossip spread | Manipulates suspicion |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Spy on NPC | Learn hidden actions | Hidden footprints | Valuable intelligence |
| Observe Meetings | Discover alliances | Window marks | Political instability |

---

## Goal
- Gather valuable secrets
- Manipulate factions for survival

---

# Vagabond

## Gameplay Identity
Outcast survivalist and social scapegoat.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Beg Resources | Gain food/info | Public sightings | Mixed trust reactions |
| Trade Rumors | Spread information | Gossip chains | Suspicion shifts |
| Search Scrap | Find random items | Trash disturbances | Resource generation |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Sleep Outdoors | Avoid houses | Camp traces | Automatic suspicion |
| Wander Village | Observe activity | Footprints | Gains hidden clues |
| Sneak Around | Gather rumors | Seen movement | Easily blamed |

---

## Goal
- Survive 5 nights
- Escape village safely

---

# FIXED NEUTRAL ROLE

# Shopkeeper

## Gameplay Identity
Village economic and information center.

---

## Day Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Trade Resources | Stabilize economy | Transaction records | Resource distribution |
| Sell Clues | Provide information | Visitor logs | Suspicion influence |
| Spread Rumors | Shape village opinion | Gossip network | Alters trust |

---

## Night Actions

| Action | Result | Trace Left | Gameplay Impact |
|---|---|---|---|
| Lock Shop | Prevent theft | Closed shutters | Security increase |
| Record Visitors | Track suspicious NPCs | Written logs | Hidden evidence |

---

## Protection Mechanic
- Protected by talisman for 7 days
- Cannot be killed during protection

---

## After Death

### Economy Effects
- resource scarcity
- black market emergence
- food shortages
- higher theft rates

### Social Effects
- panic increase
- trust reduction
- rumor spread acceleration

### Gameplay Effects
- harder survival
- suspicion escalation
- unstable village state

---

# GLOBAL SIMULATION SYSTEM

# Simulation Flow

```text
1. Assign NPC Goals
2. Evaluate Hunger/Fear/Suspicion
3. Execute Role Actions
4. Generate Encounters
5. Spawn Evidence
6. Update NPC Memories
7. Spread Rumors
8. Calculate Suspicion
9. Update Relationships
10. Trigger Events
```

---

# KEY GAME PROGRESSION STATES

# Early Game

## Characteristics
- stable economy
- low panic
- incomplete information
- hidden alignments

## Focus
- introductions
- observations
- relationship building

---

# Mid Game

## Characteristics
- first deaths
- increased suspicion
- rumor escalation
- resource shortages

## Focus
- investigations
- alliances
- manipulation

---

# Late Game

## Characteristics
- village instability
- faction conflict
- panic behavior
- social collapse

## Focus
- survival
- accusations
- final faction dominance

---

# ENDGAME STATES

## Good Victory
- evil eliminated
- village stabilized

## Evil Victory
- village collapses
- good faction eliminated

## Neutral Escape Victory
- goal completed
- escaped alive

## Total Collapse
- famine
- panic
- mass death
- nobody truly wins

