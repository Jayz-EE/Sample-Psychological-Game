# 🎮 Village of Ashes - E2E Test Guide

## Overview

This guide explains the comprehensive End-to-End (E2E) test suite that simulates complete playthroughs of Village of Ashes, testing all game mechanics from initialization to win conditions.

## 🚀 Quick Start

### Run All Tests
```bash
./run-e2e-tests.sh
```

### Run Specific Test
```bash
cd tests/VillageOfAshes.E2ETests
dotnet test --filter "CompleteGamePlaythrough_GoodFactionWins"
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## 📋 Test Scenarios

### 1. Main E2E Test: Good Faction Victory
**Test Name:** `CompleteGamePlaythrough_GoodFactionWins_ByEliminatingButcher`

**What It Does:**
Simulates a complete game where the player strategically investigates, talks to NPCs, spreads rumors, and works to eliminate the Butcher.

**Game Flow:**
1. **Initialization** - Creates new game, verifies setup
2. **Investigation** - Uses dialogue and investigation APIs
3. **Time Progression** - Advances through multiple game phases
4. **Night Simulation** - Experiences deaths and evidence generation
5. **Council Participation** - Makes accusations at village council
6. **Strategic Actions** - Performs role-specific actions (Detective investigation, Doctor healing)
7. **Rumor Spreading** - Strategically influences NPC opinions
8. **Win Condition** - Continues until game ends with victory or defeat

**Duration:** 30-60 seconds

**Key Assertions:**
- Game progresses past Day 1
- Evidence is generated during night phases
- NPCs die or get executed
- Game reaches an end state
- Win message is populated

### 2. Evil Faction Victory
**Test Name:** `CompleteGamePlaythrough_EvilFactionWins_ByOutnumberingGood`

**What It Does:**
Fast-forwards through multiple days to let the Butcher eliminate villagers and potentially win.

**Game Flow:**
1. Create new game
2. Rapidly advance through 10+ days
3. Let natural game mechanics play out
4. Verify game ends appropriately

**Duration:** 5-10 seconds

### 3. Detective Role Investigation
**Test Name:** `RoleSpecificActions_Detective_CanInvestigate`

**What It Does:**
Tests all investigation-related API endpoints.

**Tests:**
- Get investigation summary
- Analyze NPC behavior
- Detect suspicious activity
- Use investigation tools

**Duration:** 1-5 seconds

### 4. Council Mechanics
**Test Name:** `CouncilMechanics_AccusationsAndVoting_WorkCorrectly`

**What It Does:**
Tests the village council system.

**Tests:**
- Advance to council time
- Make player accusations
- Verify council mechanics work

**Duration:** 1-5 seconds

## 🔍 What Gets Tested

### Game Initialization
- ✅ New game creation
- ✅ Player role assignment
- ✅ NPC generation with roles
- ✅ Initial game state validity

### Time Management
- ✅ Time advancement (1 hour, 2 hours, full day)
- ✅ Phase transitions (Night → Morning → Council → Day → Evening)
- ✅ Day progression
- ✅ Correct phase calculations

### Dialogue System
- ✅ Get dialogue options from NPCs
- ✅ Context-appropriate dialogues
- ✅ Response selection
- ✅ Dialogue effects applied

### Investigation System
- ✅ Investigation summary retrieval
- ✅ Behavior pattern analysis
- ✅ Suspicious behavior detection
- ✅ Observation tracking

### Night Simulation
- ✅ Role actions execution (Butcher kills, Doctor heals, etc.)
- ✅ Evidence generation
- ✅ Death mechanics
- ✅ NPC movements

### Evidence & Clues
- ✅ Evidence creation during night
- ✅ Evidence types (Blood, Footprints, etc.)
- ✅ Location tracking
- ✅ Visibility percentages

### Rumor System
- ✅ Player can spread rumors
- ✅ Rumors affect NPC suspicion
- ✅ Rumor propagation
- ✅ Truthfulness mechanics

### Council System
- ✅ Council timing (7:00-8:00 AM)
- ✅ NPC statements
- ✅ Player accusations
- ✅ Voting mechanics
- ✅ Execution system

### Role-Specific Actions
- ✅ Detective: Investigation
- ✅ Doctor: Healing NPCs
- ✅ Role-specific objectives

### Win Conditions
- ✅ Good faction win (Butcher eliminated)
- ✅ Evil faction win (outnumber good)
- ✅ Game status transitions
- ✅ Win messages

### API Endpoints Tested
- `POST /api/game/new`
- `GET /api/game/state`
- `POST /api/game/advance-time`
- `GET /api/game/npcs`
- `GET /api/game/evidence`
- `GET /api/game/rumors`
- `GET /api/dialogue/npc/{npcId}`
- `POST /api/dialogue/respond`
- `GET /api/investigation/summary`
- `GET /api/investigation/behavior/{npcId}`
- `GET /api/investigation/suspicious/{npcId}`
- `POST /api/game/rumor`
- `POST /api/game/heal`
- `POST /api/game/council/player-action`

## 📊 Test Output

### Console Output Example

```
=== PHASE 1: GAME INITIALIZATION ===
✓ Game created: Day 1, MorningDiscovery
✓ Player Role: Detective
✓ NPCs: 6 alive
  - John: Farmer (GoodNeutral)
  - Sarah: Doctor (Good)
  - Michael: Butcher (Evil)
  - Emma: Vagabond (TrueNeutral)
  - David: Shopkeeper (FixedNeutral)
  - Lisa: Farmer (GoodNeutral)

=== PHASE 2: EARLY GAME - INVESTIGATION ===
✓ Talking to John...
  NPC says: I've been tending to the crops, trying to keep things...
  Selected response and got effects
✓ Investigating behavior patterns...
  Total Observations: 0
  Total Evidence: 0

=== PHASE 3: ADVANCE TO COUNCIL ===
✓ Advanced to: 7:00 - VillageCouncil

=== PHASE 4: FIRST NIGHT SIMULATION ===
✓ Advanced to: 9:00 - NightSimulation
✓ Executing night phase...
✓ Night completed: 6 -> 5 alive
💀 Deaths occurred:
  - Emma (Vagabond)
🔍 Evidence collected: 3 pieces
  - Blood at Michael's House
  - Footprints at Village Square
  - Weapon Traces at Emma's House

=== PHASE 5: DAY 2 COUNCIL ===
✓ Council started on Day 2
🎯 Most suspicious NPCs:
  - Michael: 45% suspicion
  - David: 12% suspicion
✓ Player accuses the Butcher at council...
  Accusation made successfully

=== PHASE 6: STRATEGIC GAMEPLAY ===
🔍 Investigated Michael: 3 suspicious findings
💬 Spread rumor about the Butcher
📅 Day 3 - DayActions
  Alive: 4/6

=== PHASE 7: GAME COMPLETION ===
🎮 GAME ENDED:
  Status: GoodVictory
  Final Day: 5
  Final Phase: DayActions
  Win Message: The Butcher has been eliminated! Good faction wins!

📊 FINAL STATISTICS:
  NPCs Alive: 3
  NPCs Dead: 3
  Evidence Collected: 8
  Rumors Spread: 12

👥 FINAL NPC STATUS:
  ✓ John: Farmer (GoodNeutral) - Alive
  ✓ Sarah: Doctor (Good) - Alive
  💀 Michael: Butcher (Evil) - Dead
  💀 Emma: Vagabond (TrueNeutral) - Dead
  ✓ David: Shopkeeper (FixedNeutral) - Alive
  💀 Lisa: Farmer (GoodNeutral) - Dead

=== RUNNING ASSERTIONS ===
✓ All assertions passed!
✓ E2E test completed successfully!
🎉 GOOD FACTION VICTORY! The Butcher was eliminated!
```

## 🛠️ Troubleshooting

### Test Failures

**"Game ended early"**
- Check win condition logic in `GameProgressionService`
- Verify NPC death mechanics aren't too aggressive
- Check day/phase progression

**"No evidence generated"**
- Verify `NightSimulationService` is creating evidence
- Check that night phase is executing properly
- Ensure evidence types are configured

**"Game never ends"**
- Add max day limit in test (currently 15 days)
- Check win condition calculations
- Verify faction alignment logic

**"API connection failed"**
- Ensure `Program.cs` has `public partial class Program {}`
- Verify `InternalsVisibleTo` in API csproj
- Check that all services are registered

### Build Errors

**"Type 'Dialogue' conflict"**
- Already fixed in this update
- Fully qualified namespace in `IDialogueService.cs`

**"Array Count error"**
- Already fixed in this update
- Changed `.Count` to `.Length` for arrays

**"Duplicate class definitions"**
- Already fixed in this update
- Removed duplicate DTO classes

## 📈 Performance Benchmarks

Expected performance on a standard development machine:

| Operation | Expected Time |
|-----------|--------------|
| Game Initialization | < 100ms |
| Time Advance (1 hour) | < 50ms |
| Night Simulation | < 200ms |
| Dialogue Exchange | < 50ms |
| Investigation Query | < 30ms |
| Council Action | < 100ms |
| **Full E2E Test** | **30-60 seconds** |

## 🎯 Success Criteria

For a test run to be considered successful:

1. ✅ All assertions pass
2. ✅ Game progresses through multiple days
3. ✅ At least one night simulation executes
4. ✅ Evidence is generated
5. ✅ NPCs die or get executed
6. ✅ Game reaches an end state
7. ✅ No unhandled exceptions
8. ✅ Win message is set

## 🔄 CI/CD Integration

### GitHub Actions

```yaml
name: E2E Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  e2e-tests:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Run E2E Tests
      run: ./run-e2e-tests.sh
    
    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: test-results
        path: '**/TestResults/*.trx'
```

## 📝 Extending Tests

### Add New Test Scenario

```csharp
[Fact]
public async Task YourNewScenario_Condition_ExpectedOutcome()
{
    // Arrange
    var newGameResponse = await _client.PostAsJsonAsync("/api/game/new", new { });
    var gameState = await newGameResponse.Content.ReadFromJsonAsync<GameStateDto>();
    
    // Act
    // ... perform specific actions
    
    // Assert
    gameState.Should().NotBeNull();
    // ... your assertions
}
```

### Test Specific Player Role

```csharp
// Keep creating games until you get desired role
GameStateDto? gameState = null;
int attempts = 0;
while (gameState?.player?.role != "Detective" && attempts++ < 50)
{
    var response = await _client.PostAsJsonAsync("/api/game/new", new { });
    gameState = await response.Content.ReadFromJsonAsync<GameStateDto>();
}

gameState.player.role.Should().Be("Detective");
```

### Test Edge Cases

- Multiple NPCs dying in one night
- All good NPCs eliminated
- Player death scenarios
- Council with no accusations
- Voting ties
- Auto-time progression

## 🎓 Learning from Tests

The E2E tests serve as:
- **Living Documentation** - Shows how the game actually works
- **Integration Examples** - Demonstrates API usage
- **Quality Assurance** - Catches regressions
- **Performance Baseline** - Establishes expected timings

## 📖 Related Documentation

- [README.md](README.md) - Main project documentation
- [tests/VillageOfAshes.E2ETests/README.md](tests/VillageOfAshes.E2ETests/README.md) - Test project README
- [QUICKSTART.md](QUICKSTART.md) - Game quick start guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture overview

## 🤝 Contributing

When adding new features:
1. Write E2E tests for new endpoints
2. Ensure all existing tests pass
3. Update this guide with new test scenarios
4. Add console logging for important actions

---

**Happy Testing! May your tests always pass! 🎮✅💀**
