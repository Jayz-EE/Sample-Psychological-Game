# 🎮 E2E Test Quick Reference

## 🚀 Run Tests

```bash
# Easiest way
./run-e2e-tests.sh

# Or manually
cd tests/VillageOfAshes.E2ETests && dotnet test

# List all tests
dotnet test --list-tests

# Run specific test
dotnet test --filter "GoodFactionWins"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## 📋 4 Test Scenarios

| Test | What It Does | Duration |
|------|--------------|----------|
| `CompleteGamePlaythrough_GoodFactionWins` | Full game simulation with strategic play | 30-60s |
| `CompleteGamePlaythrough_EvilFactionWins` | Fast simulation, evil victory | 5-10s |
| `RoleSpecificActions_Detective` | Tests investigation APIs | 1-5s |
| `CouncilMechanics_AccusationsAndVoting` | Tests council system | 1-5s |

## ✅ What Gets Tested

**Game Systems:** Initialization, Time Management, Dialogue, Investigation, Night Simulation, Evidence, Rumors, Council, Death, Roles, Win Conditions, Suspicion, Trust

**Endpoints:** 14+ API endpoints tested

**Actions:** Create game, Advance time, Talk to NPCs, Investigate, Spread rumors, Heal, Make accusations, Vote

## 📊 Expected Output

```
=== PHASE 1: GAME INITIALIZATION ===
✓ Game created: Day 1, MorningDiscovery
✓ Player Role: Detective
✓ NPCs: 6 alive

=== PHASE 4: FIRST NIGHT SIMULATION ===
✓ Night completed: 6 -> 5 alive
💀 Deaths occurred: Emma (Vagabond)
🔍 Evidence collected: 3 pieces

🎮 GAME ENDED: GoodVictory
🎉 GOOD FACTION VICTORY!
✓ All assertions passed!
```

## 🐛 Fixed Bugs

1. Namespace conflict in `IDialogueService.cs`
2. Array `.Count` → `.Length` in `NpcDecisionService.cs`
3. Duplicate class definitions in `GameController.cs`
4. Private method accessibility in `GameController.cs`

## 📚 Documentation

- **E2E_TEST_GUIDE.md** - Comprehensive guide (read this first)
- **tests/VillageOfAshes.E2ETests/README.md** - Test project details
- **E2E_TESTS_CREATED.md** - What was created
- **TEST_QUICK_REFERENCE.md** - This file

## ✨ Features

- Simulates real gameplay
- Tests all major systems
- Detailed console logging
- Role-specific actions
- Strategic decision-making
- Win condition verification

## 🎯 Success Criteria

✅ Game initializes  
✅ Time advances  
✅ Night simulation runs  
✅ Evidence generated  
✅ NPCs die  
✅ Game ends  
✅ Win message set  

## 📈 Stats

- **Tests:** 4
- **Code:** 650+ lines
- **Systems:** 14+
- **Endpoints:** 14+
- **Status:** ✅ Ready

---

**Quick Start:** `./run-e2e-tests.sh` 🚀
