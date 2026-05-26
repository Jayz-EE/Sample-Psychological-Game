# 📋 Changelog

## Version 2.0 - Enhanced Social Deduction (Current)

### 🎭 Major Features Added

#### 1. Role-Agnostic Dialogue System
**Problem Solved**: Original dialogue system revealed NPC roles through role-specific conversations

**Solution**:
- ✅ Removed all role-specific dialogue filtering
- ✅ Expanded dialogue pool from 5 to 35+ unique dialogues
- ✅ All NPCs can use any dialogue regardless of role
- ✅ Observations are ambiguous (e.g., "I saw Jake outside" could be any role)

**Impact**:
- Vagabonds naturally blend in
- Butchers don't stand out in conversation
- Detectives don't reveal themselves
- True social deduction gameplay

#### 2. Expanded Dialogue Contexts
**Added 6 distinct contexts with multiple variations:**

| Context | Dialogues | Response Options |
|---------|-----------|------------------|
| Neutral | 5 | 5 |
| Suspicious | 8 | 6 |
| Fearful | 6 | 5 |
| Trusting | 5 | 5 |
| Aggressive | 4 | 5 |
| Rumor | 5 | 5 |
| **Total** | **33** | **31** |

#### 3. Ambiguous Council Statements
**Enhanced council system with 20+ statement variations:**

- Death comments (5 variations)
- Evidence observations (5 variations)
- NPC observations (9 variations)
- General suspicion (9 variations)

**Example**: "I saw Jake near Anna's house" - could be Detective investigating, Butcher stalking, or Vagabond wandering

#### 4. Role-Agnostic Accusations
**14+ accusation reasons that don't reveal roles:**

- Evidence-based (4 variations)
- Rumor-based (5 variations)
- Behavioral (14 variations)
- Intuition-based (5 variations)

#### 5. Expanded Rumor System
**20 ambiguous rumor contexts:**

- "Seen near the forest at night"
- "Acting suspiciously"
- "Wandering outside after dark"
- "Seen with blood on their clothes"
- "Lying about their whereabouts"
- And 15 more...

### 🔍 Investigation System (NEW)

#### Observation Tracking
- **Purpose**: Track what NPCs witness without revealing roles
- **Features**:
  - Ambiguous descriptions
  - Reliability scores (Detective: 80%, Others: 60%)
  - Gossip mechanics
  - Location tracking

#### Behavior Analysis
- **Purpose**: Analyze NPC patterns over time
- **Tracks**:
  - Night activity frequency
  - Location patterns
  - Council attendance
  - Defensive responses
  - Suspicious actions

#### Role Prediction Algorithm
- **Purpose**: Predict roles from behavior
- **Factors**:
  - Night activity patterns
  - Location preferences
  - Social behaviors
  - Suspicious actions
- **Output**: Probability distribution across roles

#### Behavior Comparison
- **Purpose**: Find behavioral similarities
- **Use Cases**:
  - Identify potential allies
  - Detect coordinated evil
  - Find role clusters

#### Investigation Summary
- **Purpose**: High-level investigation overview
- **Provides**:
  - Total observations
  - Total rumors
  - Total evidence
  - Most suspicious NPCs (ranked)
  - Most trusted NPCs (ranked)

### 🎯 New API Endpoints

```
Investigation System:
GET  /api/investigation/observations/about/{npcId}
GET  /api/investigation/observations/by/{npcId}
GET  /api/investigation/behavior/{npcId}
GET  /api/investigation/suspicious/{npcId}
GET  /api/investigation/predict-role/{npcId}
GET  /api/investigation/compare/{npc1}/{npc2}
GET  /api/investigation/summary
```

### 📊 Statistics

**Code Added:**
- 4 new entity classes
- 4 new service interfaces
- 2 new service implementations
- 1 new API controller
- ~1,500 lines of code

**Content Added:**
- 28 new dialogue entries
- 31 player response options
- 20 rumor contexts
- 20+ council statement variations
- 14+ accusation reasons

### 🎮 Gameplay Improvements

#### Strategic Depth
- **Before**: Limited deduction options
- **After**: Multiple investigation paths

#### Role Concealment
- **Before**: Roles revealed through dialogue
- **After**: Roles hidden, must be deduced

#### Replayability
- **Before**: Predictable patterns
- **After**: Unique investigations each game

#### Social Dynamics
- **Before**: Simple trust/suspicion
- **After**: Complex behavioral analysis

### 📚 Documentation Added

1. **DIALOGUE_SYSTEM.md** - Complete dialogue system documentation
2. **INVESTIGATION_FEATURES.md** - Investigation system guide
3. **CHANGELOG.md** - This file

### 🔧 Technical Improvements

#### Architecture
- Clean separation of concerns
- New service layer for investigations
- Extensible observation system

#### Code Quality
- Type-safe implementations
- Well-documented APIs
- Comprehensive examples

#### Performance
- Efficient pattern matching
- Optimized similarity calculations
- Minimal memory overhead

---

## Version 1.0 - Initial Release

### Core Features
- ✅ Time management system (5 phases)
- ✅ NPC management (6 NPCs + Player)
- ✅ Role system (6 roles)
- ✅ Night simulation engine
- ✅ Evidence generation (8 types)
- ✅ Suspicion calculation
- ✅ Basic dialogue system
- ✅ Rumor propagation
- ✅ Council meetings
- ✅ HTML/CSS/JS frontend
- ✅ RESTful API
- ✅ Swagger documentation

### Architecture
- Clean Architecture (3 layers)
- ASP.NET Core 10 Web API
- Dependency Injection
- Service Layer Pattern

### Documentation
- README.md
- QUICKSTART.md
- PROJECT_SUMMARY.md
- ARCHITECTURE.md
- FILE_MANIFEST.md

---

## Comparison: V1.0 vs V2.0

| Feature | V1.0 | V2.0 |
|---------|------|------|
| **Dialogues** | 5 | 35+ |
| **Response Options** | 8 | 31 |
| **Role Concealment** | Partial | Complete |
| **Investigation Tools** | None | 7 endpoints |
| **Behavior Tracking** | No | Yes |
| **Role Prediction** | No | Yes |
| **Pattern Analysis** | No | Yes |
| **Observation System** | No | Yes |
| **Council Variations** | 3 | 20+ |
| **Accusation Reasons** | 3 | 14+ |
| **Rumor Contexts** | 3 | 20 |
| **API Endpoints** | 8 | 15 |
| **Lines of Code** | ~3,000 | ~4,500 |

---

## Migration Guide (V1.0 → V2.0)

### Breaking Changes
None - V2.0 is fully backward compatible

### New Dependencies
None - uses existing .NET 10 framework

### Database Changes
None - still using in-memory state

### API Changes
- All V1.0 endpoints still work
- 7 new investigation endpoints added
- No changes to existing request/response formats

### Frontend Changes
None required - new features accessible via API

---

## Future Roadmap

### Version 2.1 (Planned)
- [ ] Secret system implementation
- [ ] Relationship tracking
- [ ] Alliance mechanics
- [ ] Betrayal system

### Version 2.2 (Planned)
- [ ] Memory system for NPCs
- [ ] Lie detection mechanics
- [ ] Contradiction tracking
- [ ] Timeline reconstruction

### Version 3.0 (Planned)
- [ ] Database persistence (EF Core)
- [ ] SignalR real-time updates
- [ ] Save/load game state
- [ ] Multiple game instances

### Version 3.1 (Planned)
- [ ] Enhanced frontend with investigation UI
- [ ] Visual behavior charts
- [ ] Interactive timeline
- [ ] Relationship graphs

### Version 4.0 (Planned)
- [ ] Machine learning for NPC behavior
- [ ] Adaptive difficulty
- [ ] Custom scenarios
- [ ] Mod support

---

## Contributors

Built with ❤️ using .NET 10

## License

Educational/Prototype Project

---

**Last Updated**: May 26, 2026
**Current Version**: 2.0
**Status**: ✅ Stable
