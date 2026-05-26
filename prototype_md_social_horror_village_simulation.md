# Prototype.md

# Project Title
Village of Ashes (Working Title)

---

# Core Concept

A single-player psychological social horror simulation game where NPCs secretly possess randomized hidden roles.

Every night, hidden backend simulations execute role actions, movements, interactions, crimes, evidence creation, rumors, and social influence.

The player must:
- survive
- investigate
- manipulate trust/suspicion
- complete role goals
- identify threats
- decide alliances

The game revolves around uncertainty, deduction, hidden information, and emergent storytelling.

---

# Core Gameplay Loop

## Night Phase
Backend simulation executes:
- role actions
- movements
- killings
- thefts
- spying
- rituals
- resource gathering
- evidence spawning
- rumor generation

Player visibility is limited.

---

## Morning Discovery Phase
Player and NPCs observe:
- missing villagers
- opened doors
- blood traces
- footprints
- damaged crops
- stolen resources
- suspicious activity

---

## Village Council Phase (7:00 AM - 8:00 AM)
NPCs converse.
NPCs question each other.
Player selects fixed dialogue responses.

Hidden systems update:
- trust
- suspicion
- fear
- rumors
- memory
- faction alignment

---

## Day Phase
Allowed role-specific actions occur.
Examples:
- farming
- investigations
- trading
- healing
- crafting
- spying
- stealing
- hunting

---

## Repeat Until
- Good faction survives
- Evil faction dominates
- Neutral escapes
- Village collapses

---

# Prototype Scope (MVP)

## Initial NPC Count
6 NPCs + Player + Shopkeeper

---

## Initial Roles

### Good
- Detective
- Doctor

### Evil
- Butcher

### Neutral
- Vagabond
- Farmer

### Fixed Neutral NPC
- Shopkeeper

---

# Village Layout

## Core Areas
- Village Center
- Council Hall
- Shopkeeper House
- Forest Edge
- Church
- Farmland
- Hunter Trail
- Abandoned Shed

---

# Time System

| Time | Event |
|---|---|
| 6:00 AM | Morning discovery |
| 7:00 AM - 8:00 AM | Village council |
| 8:00 AM - 6:00 PM | Day actions |
| 6:00 PM - 9:00 PM | Limited movement |
| 9:00 PM - 5:00 AM | Night simulation |

---

# NPC Data Structure

```json
{
  "npc_id": "npc_001",
  "name": "Edgar Hollow",
  "role": "Detective",
  "alignment": "Good",
  "house_id": "house_03",
  "status": "Alive",
  "trust": {},
  "suspicion": {},
  "fear": {},
  "known_facts": [],
  "rumors": [],
  "goals": [],
  "inventory": [],
  "daily_schedule": [],
  "night_actions": [],
  "behavior_flags": []
}
```

---

# Random NPC Name Pool

## Male
- Edgar Hollow
- Victor Crowe
- Elias Thorn
- Silas Moore
- Warren Black
- Cedric Vale
- Tobias Reed
- Lucien Graves
- Rowan Pike
- Damien Frost

## Female
- Eliza Vane
- Clara Hollow
- Miriam Crowe
- Helena Ward
- Roselyn Pike
- Evelyn Ash
- Lydia Graves
- Beatrice Thorn
- Selene Vale
- Violet Reed

---

# Role System

# GOOD ROLES

## Detective

### Day Actions
- Investigate House
- Examine Evidence
- Question NPC
- Review Rumors

### Night Actions
- Track Movement
- Stakeout

### Passive
- Higher clue accuracy

### Weakness
- High suspicion when frequently investigating

---

## Doctor

### Day Actions
- Heal NPC
- Diagnose Illness

### Night Actions
- Protect NPC
- Treat Wounds

### Passive
- Detect injury causes

### Weakness
- Medical supplies limited

---

# EVIL ROLES

## Butcher

### Day Actions
- Sell Meat
- Clean Tools
- Trade Resources

### Night Actions
- Kill NPC
- Harvest Meat
- Dispose Body

### Passive
- Strong physical attacks

### Weakness
- Blood evidence risk

---

# NEUTRAL ROLES

## Vagabond

### Day Actions
- Beg Information
- Trade Rumors
- Search Scrap

### Night Actions
- Sleep Outdoors
- Observe Activity
- Sneak Around

### Goal
- Survive 5 nights and escape village

### Weakness
- Constant suspicion due to homelessness

---

## Farmer

### Day Actions
- Sow Crops
- Fertilize
- Harvest
- Sell Produce

### Night Actions
- Protect Crops
- Hide Supplies

### Goal
- Maintain food supply for X days

### Weakness
- Crop destruction creates panic

---

# FIXED NPC ROLE

## Shopkeeper

### Status
Fixed neutral NPC.
Never assigned to player.

### Functions
- Trade hub
- Resource hub
- Information hub
- Rumor distribution

### Protection
Protected by talisman for 7 days.

### After Death
- Economy collapses
- Resources become scarce
- Theft increases
- Suspicion rises
- Information reliability decreases

---

# Dialogue Dictionary System

# Dialogue Structure

```json
{
  "dialogue_id": "dlg_suspicious_01",
  "context": "Suspicious",
  "speaker_role": "Detective",
  "emotion": "Defensive",
  "conditions": [
    "player_suspicion > 50"
  ],
  "lines": [
    "You ask too many questions.",
    "I saw you near the forest.",
    "You were not home last night."
  ],
  "effects": {
    "trust": -5,
    "suspicion": 10
  }
}
```

---

# Dialogue Context Categories

## Neutral
- greetings
- daily talk
- weather
- food
- routines

## Suspicious
- accusations
- defensive responses
- confrontation
- denial

## Fearful
- panic
- anxiety
- paranoia
- desperation

## Trusting
- confession
- secret sharing
- alliance
- warnings

## Aggressive
- threats
- hostility
- intimidation

## Rumor
- hearsay
- uncertain information
- speculation

---

# Player Dialogue Options

## Example

NPC Question:
"Where were you last night?"

Player Responses:
- "I stayed inside my house."
- "I searched the forest."
- "I heard noises near the church."
- "That is none of your concern."

---

# Dialogue Effects

Every dialogue option may modify:

| System | Effect |
|---|---|
| Trust | NPC confidence |
| Suspicion | Doubt increase |
| Fear | Anxiety increase |# Prototype.md

# Project Title
Village of Ashes (Working Title)

---

# Core Concept

A single-player psychological social horror simulation game where NPCs secretly possess randomized hidden roles.

Every night, hidden backend simulations execute role actions, movements, interactions, crimes, evidence creation, rumors, and social influence.

The player must:
- survive
- investigate
- manipulate trust/suspicion
- complete role goals
- identify threats
- decide alliances

The game revolves around uncertainty, deduction, hidden information, and emergent storytelling.

---

# Core Gameplay Loop

## Night Phase
Backend simulation executes:
- role actions
- movements
- killings
- thefts
- spying
- rituals
- resource gathering
- evidence spawning
- rumor generation

Player visibility is limited.

---

## Morning Discovery Phase
Player and NPCs observe:
- missing villagers
- opened doors
- blood traces
- footprints
- damaged crops
- stolen resources
- suspicious activity

---

## Village Council Phase (7:00 AM - 8:00 AM)
NPCs converse.
NPCs question each other.
Player selects fixed dialogue responses.

Hidden systems update:
- trust
- suspicion
- fear
- rumors
- memory
- faction alignment

---

## Day Phase
Allowed role-specific actions occur.
Examples:
- farming
- investigations
- trading
- healing
- crafting
- spying
- stealing
- hunting

---

## Repeat Until
- Good faction survives
- Evil faction dominates
- Neutral escapes
- Village collapses

---

# Prototype Scope (MVP)

## Initial NPC Count
6 NPCs + Player + Shopkeeper

---

## Initial Roles

### Good
- Detective
- Doctor

### Evil
- Butcher

### Neutral
- Vagabond
- Farmer

### Fixed Neutral NPC
- Shopkeeper

---

# Village Layout

## Core Areas
- Village Center
- Council Hall
- Shopkeeper House
- Forest Edge
- Church
- Farmland
- Hunter Trail
- Abandoned Shed

---

# Time System

| Time | Event |
|---|---|
| 6:00 AM | Morning discovery |
| 7:00 AM - 8:00 AM | Village council |
| 8:00 AM - 6:00 PM | Day actions |
| 6:00 PM - 9:00 PM | Limited movement |
| 9:00 PM - 5:00 AM | Night simulation |

---

# NPC Data Structure

```json
{
  "npc_id": "npc_001",
  "name": "Edgar Hollow",
  "role": "Detective",
  "alignment": "Good",
  "house_id": "house_03",
  "status": "Alive",
  "trust": {},
  "suspicion": {},
  "fear": {},
  "known_facts": [],
  "rumors": [],
  "goals": [],
  "inventory": [],
  "daily_schedule": [],
  "night_actions": [],
  "behavior_flags": []
}
```

---

# Random NPC Name Pool

## Male
- Edgar Hollow
- Victor Crowe
- Elias Thorn
- Silas Moore
- Warren Black
- Cedric Vale
- Tobias Reed
- Lucien Graves
- Rowan Pike
- Damien Frost

## Female
- Eliza Vane
- Clara Hollow
- Miriam Crowe
- Helena Ward
- Roselyn Pike
- Evelyn Ash
- Lydia Graves
- Beatrice Thorn
- Selene Vale
- Violet Reed

---

# Role System

# GOOD ROLES

## Detective

### Day Actions
- Investigate House
- Examine Evidence
- Question NPC
- Review Rumors

### Night Actions
- Track Movement
- Stakeout

### Passive
- Higher clue accuracy

### Weakness
- High suspicion when frequently investigating

---

## Doctor

### Day Actions
- Heal NPC
- Diagnose Illness

### Night Actions
- Protect NPC
- Treat Wounds

### Passive
- Detect injury causes

### Weakness
- Medical supplies limited

---

# EVIL ROLES

## Butcher

### Day Actions
- Sell Meat
- Clean Tools
- Trade Resources

### Night Actions
- Kill NPC
- Harvest Meat
- Dispose Body

### Passive
- Strong physical attacks

### Weakness
- Blood evidence risk

---

# NEUTRAL ROLES

## Vagabond

### Day Actions
- Beg Information
- Trade Rumors
- Search Scrap

### Night Actions
- Sleep Outdoors
- Observe Activity
- Sneak Around

### Goal
- Survive 5 nights and escape village

### Weakness
- Constant suspicion due to homelessness

---

## Farmer

### Day Actions
- Sow Crops
- Fertilize
- Harvest
- Sell Produce

### Night Actions
- Protect Crops
- Hide Supplies

### Goal
- Maintain food supply for X days

### Weakness
- Crop destruction creates panic

---

# FIXED NPC ROLE

## Shopkeeper

### Status
Fixed neutral NPC.
Never assigned to player.

### Functions
- Trade hub
- Resource hub
- Information hub
- Rumor distribution

### Protection
Protected by talisman for 7 days.

### After Death
- Economy collapses
- Resources become scarce
- Theft increases
- Suspicion rises
- Information reliability decreases

---

# Dialogue Dictionary System

# Dialogue Structure

```json
{
  "dialogue_id": "dlg_suspicious_01",
  "context": "Suspicious",
  "speaker_role": "Detective",
  "emotion": "Defensive",
  "conditions": [
    "player_suspicion > 50"
  ],
  "lines": [
    "You ask too many questions.",
    "I saw you near the forest.",
    "You were not home last night."
  ],
  "effects": {
    "trust": -5,
    "suspicion": 10
  }
}
```

---

# Dialogue Context Categories

## Neutral
- greetings
- daily talk
- weather
- food
- routines

## Suspicious
- accusations
- defensive responses
- confrontation
- denial

## Fearful
- panic
- anxiety
- paranoia
- desperation

## Trusting
- confession
- secret sharing
- alliance
- warnings

## Aggressive
- threats
- hostility
- intimidation

## Rumor
- hearsay
- uncertain information
- speculation

---

# Player Dialogue Options

## Example

NPC Question:
"Where were you last night?"

Player Responses:
- "I stayed inside my house."
- "I searched the forest."
- "I heard noises near the church."
- "That is none of your concern."

---

# Dialogue Effects

Every dialogue option may modify:

| System | Effect |
|---|---|
| Trust | NPC confidence |
| Suspicion | Doubt increase |
| Fear | Anxiety increase |
| Rumor Spread | Information propagation |
| Faction Alignment | Political leaning |
| Memory | NPC remembers statement |

---

# Suspicion System

# Suspicion Sources

## Direct Evidence
- witnessed murder
- blood traces
- stolen goods
- missing items
- footprints

## Behavioral Evidence
- wandering at night
- avoiding council
- contradictory statements
- suspicious trades
- entering forbidden areas

## Social Evidence
- rumors
- accusations
- alliances
- repeated conflicts

---

# Suspicion Calculation

```text
Final Suspicion =
Base Suspicion
+ Witness Evidence
+ Rumor Weight
+ Contradiction Weight
+ Role Bias
+ RNG Modifier
```

---

# NPC Knowledge Layers

## True Facts
Actual backend simulation events.

## Witnessed Facts
Events personally observed.

## Assumed Facts
NPC deductions.

## Rumors
Shared uncertain information.

## False Information
Lies and manipulation.

---

# Backend Simulation Logic

# Simulation Cycle

```text
FOR EACH NIGHT:

1. Assign Role Actions
2. Select NPC Targets
3. Execute Movement
4. Generate Encounters
5. Spawn Evidence
6. Update NPC Memories
7. Generate Rumors
8. Calculate Suspicion Changes
9. Save World State
```

---

# RNG Weighted Decision System

## Example

```text
If NPC is Hungry:
+20 chance to steal food

If NPC trusts Player:
-15 suspicion gain

If NPC witnessed blood:
+30 fear

If Vagabond seen outside:
+25 suspicion
```

---

# Evidence System

## Evidence Types
- blood
- footprints
- broken locks
- stolen items
- damaged crops
- ritual markings
- weapon traces
- corpse wounds

---

# Evidence Data Structure

```json
{
  "evidence_id": "ev_001",
  "type": "Blood",
  "location": "forest_edge",
  "created_by": "npc_004",
  "visibility": 70,
  "decay_time": 2,
  "linked_role": "Butcher"
}
```

---

# Rumor System

# Rumor Structure

```json
{
  "rumor_id": "rumor_001",
  "source_npc": "npc_003",
  "target_npc": "npc_001",
  "truthfulness": 60,
  "spread_rate": 40,
  "context": "Seen near forest at night"
}
```

---

# Rumor Categories

- suspicious movement
- missing supplies
- hidden meetings
- strange sounds
- blood sightings
- cursed activity
- secret trades

---

# Council System

# Council Rules

## Schedule
7:00 AM - 8:00 AM

## Functions
- questioning
- accusation
- rumor discussion
- defense
- voting
- trust updates

---

# Council Interaction Example

```text
Detective:
"Someone entered the forest after midnight."

Farmer:
"I heard footsteps near the church."

Vagabond:
"You always blame me first."

Player Choices:
1. Defend Vagabond
2. Accuse Hunter
3. Stay Silent
4. Redirect Topic
```

---

# Council Consequences

Possible outcomes:
- trust gain
- suspicion increase
- alliance creation
- public panic
- false accusations
- execution vote
- rumor spread

---

# Shopkeeper System

# Shopkeeper Services

## Buy
- food
- medicine
- tools
- candles
- clues
- rumors

## Sell
- crops
- meat
- stolen goods
- rare materials

## Information
- suspicious sightings
- recent visitors
- missing supplies
- rumor confirmation

---

# Shopkeeper Death Consequences

## Economy Effects
- no stable trading
- food scarcity
- increased theft
- black market emergence

## Social Effects
- panic increase
- trust decrease
- suspicion increase

---

# Conversation Logs

# Log Structure

```json
{
  "log_id": "conv_001",
  "timestamp": "Day_03_07:20AM",
  "participants": ["Player", "Edgar Hollow"],
  "context": "Council",
  "dialogue": [
    {
      "speaker": "Edgar Hollow",
      "line": "Where were you during the night?"
    },
    {
      "speaker": "Player",
      "line": "I stayed home."
    }
  ],
  "effects": {
    "trust": -5,
    "suspicion": 10
  }
}
```

---

# Council Logs

# Council Record Structure

```json
{
  "council_day": 3,
  "accusations": [
    {
      "source": "Victor Crowe",
      "target": "Vagabond",
      "reason": "Seen outside at night"
    }
  ],
  "votes": [],
  "public_suspicion": {}
}
```

---

# Win Conditions

# Good Victory
- eliminate evil roles
- preserve village stability

---

# Evil Victory
- outnumber good faction
- collapse village order

---

# Neutral Victory
- complete personal goal
- escape village alive

---

# Technical Architecture

# Recommended Systems

## Core Systems
- Time Manager
- NPC Manager
- Role Manager
- Dialogue Manager
- Suspicion Manager
- Rumor Manager
- Evidence Manager
- Council Manager
- Economy Manager
- Save System

---

# Recommended Unity Architecture

## Suggested Patterns
- ScriptableObjects
- Event-driven systems
- State machines
- Data-driven dictionaries
- Modular managers

---

# Prototype Priority Order

## Phase 1
- Time system
- NPC schedules
- Village movement
- Day/night cycle

## Phase 2
- Hidden role assignment
- Dialogue system
- Suspicion system
- Council prototype

## Phase 3
- Evidence generation
- Rumor system
- Shopkeeper economy
- Role actions

## Phase 4
- Polish
- balancing
- atmosphere
- sound design
- save system

---

# Core Design Philosophy

The game should never directly reveal truth.

Players must:
- observe behavior
- compare statements
- investigate clues
- interpret rumors
- manage trust
- survive uncertainty

Fear should come from:
- paranoia
- misinformation
- social collapse
- hidden motives
- incomplete knowledge

NOT from constant jumpscares.


| Rumor Spread | Information propagation |
| Faction Alignment | Political leaning |
| Memory | NPC remembers statement |

---

# Suspicion System

# Suspicion Sources

## Direct Evidence
- witnessed murder
- blood traces
- stolen goods
- missing items
- footprints

## Behavioral Evidence
- wandering at night
- avoiding council
- contradictory statements
- suspicious trades
- entering forbidden areas

## Social Evidence
- rumors
- accusations
- alliances
- repeated conflicts

---

# Suspicion Calculation

```text
Final Suspicion =
Base Suspicion
+ Witness Evidence
+ Rumor Weight
+ Contradiction Weight
+ Role Bias
+ RNG Modifier
```

---

# NPC Knowledge Layers

## True Facts
Actual backend simulation events.

## Witnessed Facts
Events personally observed.

## Assumed Facts
NPC deductions.

## Rumors
Shared uncertain information.

## False Information
Lies and manipulation.

---

# Backend Simulation Logic

# Simulation Cycle

```text
FOR EACH NIGHT:

1. Assign Role Actions
2. Select NPC Targets
3. Execute Movement
4. Generate Encounters
5. Spawn Evidence
6. Update NPC Memories
7. Generate Rumors
8. Calculate Suspicion Changes
9. Save World State
```

---

# RNG Weighted Decision System

## Example

```text
If NPC is Hungry:
+20 chance to steal food

If NPC trusts Player:
-15 suspicion gain

If NPC witnessed blood:
+30 fear

If Vagabond seen outside:
+25 suspicion
```

---

# Evidence System

## Evidence Types
- blood
- footprints
- broken locks
- stolen items
- damaged crops
- ritual markings
- weapon traces
- corpse wounds

---

# Evidence Data Structure

```json
{
  "evidence_id": "ev_001",
  "type": "Blood",
  "location": "forest_edge",
  "created_by": "npc_004",
  "visibility": 70,
  "decay_time": 2,
  "linked_role": "Butcher"
}
```

---

# Rumor System

# Rumor Structure

```json
{
  "rumor_id": "rumor_001",
  "source_npc": "npc_003",
  "target_npc": "npc_001",
  "truthfulness": 60,
  "spread_rate": 40,
  "context": "Seen near forest at night"
}
```

---

# Rumor Categories

- suspicious movement
- missing supplies
- hidden meetings
- strange sounds
- blood sightings
- cursed activity
- secret trades

---

# Council System

# Council Rules

## Schedule
7:00 AM - 8:00 AM

## Functions
- questioning
- accusation
- rumor discussion
- defense
- voting
- trust updates

---

# Council Interaction Example

```text
Detective:
"Someone entered the forest after midnight."

Farmer:
"I heard footsteps near the church."

Vagabond:
"You always blame me first."

Player Choices:
1. Defend Vagabond
2. Accuse Hunter
3. Stay Silent
4. Redirect Topic
```

---

# Council Consequences

Possible outcomes:
- trust gain
- suspicion increase
- alliance creation
- public panic
- false accusations
- execution vote
- rumor spread

---

# Shopkeeper System

# Shopkeeper Services

## Buy
- food
- medicine
- tools
- candles
- clues
- rumors

## Sell
- crops
- meat
- stolen goods
- rare materials

## Information
- suspicious sightings
- recent visitors
- missing supplies
- rumor confirmation

---

# Shopkeeper Death Consequences

## Economy Effects
- no stable trading
- food scarcity
- increased theft
- black market emergence

## Social Effects
- panic increase
- trust decrease
- suspicion increase

---

# Conversation Logs

# Log Structure

```json
{
  "log_id": "conv_001",
  "timestamp": "Day_03_07:20AM",
  "participants": ["Player", "Edgar Hollow"],
  "context": "Council",
  "dialogue": [
    {
      "speaker": "Edgar Hollow",
      "line": "Where were you during the night?"
    },
    {
      "speaker": "Player",
      "line": "I stayed home."
    }
  ],
  "effects": {
    "trust": -5,
    "suspicion": 10
  }
}
```

---

# Council Logs

# Council Record Structure

```json
{
  "council_day": 3,
  "accusations": [
    {
      "source": "Victor Crowe",
      "target": "Vagabond",
      "reason": "Seen outside at night"
    }
  ],
  "votes": [],
  "public_suspicion": {}
}
```

---

# Win Conditions

# Good Victory
- eliminate evil roles
- preserve village stability

---

# Evil Victory
- outnumber good faction
- collapse village order

---

# Neutral Victory
- complete personal goal
- escape village alive

---

# Technical Architecture

# Recommended Systems

## Core Systems
- Time Manager
- NPC Manager
- Role Manager
- Dialogue Manager
- Suspicion Manager
- Rumor Manager
- Evidence Manager
- Council Manager
- Economy Manager
- Save System

---

# Recommended Unity Architecture

## Suggested Patterns
- ScriptableObjects
- Event-driven systems
- State machines
- Data-driven dictionaries
- Modular managers

---

# Prototype Priority Order

## Phase 1
- Time system
- NPC schedules
- Village movement
- Day/night cycle

## Phase 2
- Hidden role assignment
- Dialogue system
- Suspicion system
- Council prototype

## Phase 3
- Evidence generation
- Rumor system
- Shopkeeper economy
- Role actions

## Phase 4
- Polish
- balancing
- atmosphere
- sound design
- save system

---

# Core Design Philosophy

The game should never directly reveal truth.

Players must:
- observe behavior
- compare statements
- investigate clues
- interpret rumors
- manage trust
- survive uncertainty

Fear should come from:
- paranoia
- misinformation
- social collapse
- hidden motives
- incomplete knowledge

NOT from constant jumpscares.

