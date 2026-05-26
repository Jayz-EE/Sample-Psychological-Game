# 🔍 Advanced Investigation Features

## Overview

The investigation system adds deep analytical capabilities for players to deduce roles through behavioral analysis, observation tracking, and pattern recognition - without relying on role-revealing dialogue.

## New Features

### 1. Observation System

**Purpose**: Track what NPCs witness during night phases

#### Observation Entity
```csharp
public class Observation
{
    public string ObserverId { get; set; }      // Who saw it
    public string TargetId { get; set; }        // Who was observed
    public string Location { get; set; }        // Where it happened
    public string Description { get; set; }     // Ambiguous description
    public int Reliability { get; set; }        // 0-100
    public bool Shared { get; set; }            // Has been gossiped
    public List<string> SharedWith { get; set; } // Who knows
}
```

#### Key Features
- **Ambiguous Descriptions**: "Jake moving around suspiciously" (doesn't reveal role)
- **Reliability Scores**: Detective observations are more reliable (80%) vs others (60%)
- **Gossip Mechanics**: Observations can be shared, creating rumors
- **Location Tracking**: Where NPCs were seen

#### API Endpoints
```
GET /api/investigation/observations/about/{npcId}  - Get all observations about an NPC
GET /api/investigation/observations/by/{npcId}     - Get observations made by an NPC
```

### 2. Behavior Analysis System

**Purpose**: Track and analyze NPC behavior patterns over time

#### Behavior Pattern Entity
```csharp
public class BehaviorPattern
{
    public Dictionary<string, int> NightLocations { get; set; }  // Location frequency
    public Dictionary<string, int> DayActivities { get; set; }   // Activity frequency
    public int TimesSeenAtNight { get; set; }
    public int TimesAvoidedCouncil { get; set; }
    public int TimesDefensive { get; set; }
    public int TimesHelpful { get; set; }
    public List<string> SuspiciousActions { get; set; }
}
```

#### Tracked Behaviors

**Night Activity**
- Frequency of being seen outside
- Locations visited
- Patterns of movement

**Council Behavior**
- Attendance record
- Defensive responses
- Helpful contributions

**Suspicious Actions**
- Keywords: blood, weapon, sneak, hide, flee, threaten
- Recorded with location and context

#### API Endpoints
```
GET /api/investigation/behavior/{npcId}        - Get behavior pattern
GET /api/investigation/suspicious/{npcId}      - Get suspicious behavior analysis
GET /api/investigation/predict-role/{npcId}    - Predict role from behavior
GET /api/investigation/compare/{npc1}/{npc2}   - Compare two NPCs' behaviors
GET /api/investigation/summary                 - Get investigation summary
```

### 3. Role Prediction Algorithm

**Purpose**: Predict NPC roles based on behavioral patterns

#### Prediction Factors

**Detective Indicators**
- Seen at night frequently (+20)
- Few suspicious actions (+15)
- Multiple location visits

**Doctor Indicators**
- Helpful behavior (+25)
- Visits multiple locations (+15)
- Low defensive responses

**Butcher Indicators**
- Suspicious actions (+30)
- Defensive when questioned (+20)
- Avoids council meetings (+15)

**Vagabond Indicators**
- Very frequent night activity (+25)
- Many different locations (+20)
- Avoids council (+10)

**Farmer Indicators**
- Frequent farmland visits (+30)
- Helpful behavior (+15)
- Consistent patterns

#### Example Output
```json
{
  "Detective": 35,
  "Doctor": 15,
  "Butcher": 40,
  "Vagabond": 5,
  "Farmer": 5
}
```

### 4. Behavior Comparison

**Purpose**: Compare two NPCs to find similarities

#### Similarity Scoring
- **80-100**: Very similar - possibly same role type
- **60-79**: Similar - overlapping activities
- **40-59**: Some similarities
- **20-39**: Different patterns
- **0-19**: Completely different

#### Use Cases
- Identify potential allies (similar good roles)
- Find suspicious pairs (coordinated evil)
- Detect role clusters

### 5. Investigation Summary

**Purpose**: High-level overview of investigation progress

#### Summary Data
```json
{
  "totalObservations": 45,
  "totalRumors": 12,
  "totalEvidence": 8,
  "mostSuspiciousNpcs": [
    { "npcName": "Jake", "averageSuspicion": 75 },
    { "npcName": "Anna", "averageSuspicion": 62 }
  ],
  "mostTrustedNpcs": [
    { "npcName": "Marcus", "averageTrust": 80 },
    { "npcName": "Jenna", "averageTrust": 72 }
  ]
}
```

## Strategic Gameplay

### Investigation Workflow

#### Phase 1: Observation Collection (Days 1-2)
1. Advance time through nights
2. Collect observations via API
3. Note who was seen where
4. Track frequency patterns

#### Phase 2: Pattern Analysis (Days 2-3)
1. Request behavior patterns for suspects
2. Analyze suspicious behavior lists
3. Compare similar NPCs
4. Build hypotheses

#### Phase 3: Role Prediction (Days 3-4)
1. Use prediction API for top suspects
2. Cross-reference with evidence
3. Validate against observations
4. Narrow down killer

#### Phase 4: Accusation (Day 4+)
1. Present findings at council
2. Build consensus
3. Vote for execution
4. Verify outcome

### Example Investigation

#### Day 1 Night
**Observations Generated:**
- Anna saw Jake at Forest (Reliability: 75%)
- Marcus saw Jake at Church (Reliability: 60%)
- Jake saw Anna at Forest (Reliability: 55%)

#### Day 2 Analysis
**Jake's Behavior Pattern:**
```
Night Locations: { "Forest": 2, "Church": 1 }
Times Seen at Night: 3
Suspicious Actions: ["carrying something in the dark"]
```

**Prediction:**
```
Butcher: 45%
Vagabond: 35%
Detective: 15%
```

#### Day 3 Comparison
**Jake vs Marcus Similarity: 15%**
- Interpretation: "Completely different behavior"
- Conclusion: Not same role type

#### Day 4 Decision
**Evidence:**
- High night activity
- Suspicious actions
- 45% Butcher prediction
- Blood found near Forest

**Action:** Accuse Jake at council

## Integration with Existing Systems

### Dialogue System
- Observations can trigger suspicious dialogue
- Behavior patterns influence NPC responses
- Predictions affect trust/suspicion

### Suspicion System
- Observations add to suspicion scores
- Behavior patterns modify calculations
- Predictions influence voting

### Council System
- Observations shared during statements
- Behavior analysis used in accusations
- Predictions guide voting decisions

## API Usage Examples

### Get Suspicious Behavior
```bash
GET /api/investigation/suspicious/npc_001
```

**Response:**
```json
[
  "Jake has been seen outside at night 5 times",
  "Jake has avoided council meetings 2 times",
  "Jake has been defensive 4 times when questioned",
  "Jake has performed 2 suspicious actions"
]
```

### Predict Role
```bash
GET /api/investigation/predict-role/npc_001
```

**Response:**
```json
{
  "Detective": 20,
  "Doctor": 10,
  "Butcher": 50,
  "Vagabond": 15,
  "Farmer": 5
}
```

### Compare NPCs
```bash
GET /api/investigation/compare/npc_001/npc_002
```

**Response:**
```json
{
  "npc1Name": "Jake",
  "npc2Name": "Anna",
  "similarityScore": 65,
  "interpretation": "Similar behavior - may have overlapping activities"
}
```

### Investigation Summary
```bash
GET /api/investigation/summary
```

**Response:**
```json
{
  "totalObservations": 45,
  "totalRumors": 12,
  "totalEvidence": 8,
  "mostSuspiciousNpcs": [
    { "npcId": "npc_001", "npcName": "Jake", "averageSuspicion": 75 },
    { "npcId": "npc_003", "npcName": "Marcus", "averageSuspicion": 62 }
  ],
  "mostTrustedNpcs": [
    { "npcId": "npc_002", "npcName": "Anna", "averageTrust": 80 },
    { "npcId": "npc_004", "npcName": "Jenna", "averageTrust": 72 }
  ]
}
```

## Benefits

### 1. Deep Strategic Gameplay
- Multiple data sources for deduction
- Pattern recognition challenges
- Long-term observation rewards

### 2. No Role Reveals
- All analysis is probabilistic
- Ambiguous observations
- Multiple interpretations possible

### 3. Emergent Narratives
- Unique investigation paths
- Player-driven conclusions
- Unpredictable outcomes

### 4. Replayability
- Different behavior patterns each game
- Various investigation strategies
- Multiple solution paths

## Advanced Strategies

### The Detective Approach
1. Collect all observations systematically
2. Build comprehensive behavior profiles
3. Use prediction algorithm heavily
4. Cross-reference with evidence

### The Social Approach
1. Focus on trust/suspicion rankings
2. Build alliances with trusted NPCs
3. Share observations strategically
4. Use council dynamics

### The Pattern Approach
1. Track location frequencies
2. Identify anomalies
3. Compare similar behaviors
4. Eliminate impossibilities

### The Evidence Approach
1. Correlate observations with evidence
2. Track who was near crime scenes
3. Analyze timing patterns
4. Build circumstantial cases

## Future Enhancements

### Planned Features
- **Secret System**: NPCs can discover and share secrets
- **Relationship Tracking**: Alliance and betrayal mechanics
- **Memory System**: NPCs remember past interactions
- **Lie Detection**: Identify contradictions in statements
- **Forensic Analysis**: Detailed evidence examination
- **Timeline Reconstruction**: Piece together night events

---

**The investigation system transforms Village of Ashes into a true detective game where careful observation and analysis lead to victory. 🔍🕵️**
