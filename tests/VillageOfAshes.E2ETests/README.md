# Village of Ashes - E2E Tests

## Overview

This test project contains comprehensive End-to-End (E2E) tests that simulate complete playthroughs of the Village of Ashes game, attempting to win through various strategies.

## Test Scenarios

### 1. `CompleteGamePlaythrough_GoodFactionWins_ByEliminatingButcher`

The main E2E test that simulates a full game playthrough with the following phases:

**Phase 1: Game Initialization**
- Creates a new game
- Verifies initial game state
- Identifies NPCs and their roles

**Phase 2: Early Game Investigation**
- Talks to NPCs to gather information
- Uses the dialogue system
- Analyzes behavior patterns using investigation APIs

**Phase 3: Advance to Council**
- Progresses game time
- Enters council phase

**Phase 4: First Night Simulation**
- Advances through day to night
- Executes night simulation
- Checks for deaths and evidence generation

**Phase 5: Day 2 Council - Accusations**
- Participates in village council
- Makes accusations based on suspicion levels
- Uses investigation data to identify suspects

**Phase 6: Strategic Gameplay**
- Performs role-specific actions (Detective investigations, Doctor healing)
- Spreads strategic rumors
- Progresses through multiple days (up to 15 days)
- Continuously investigates and adapts strategy

**Phase 7: Game Completion**
- Verifies game ended properly
- Checks win conditions
- Displays final statistics

### 2. `CompleteGamePlaythrough_EvilFactionWins_ByOutnumberingGood`

Fast-forward simulation that:
- Creates a new game
- Rapidly advances through multiple days
- Allows the Butcher to eliminate villagers
- Tests evil victory condition

### 3. `RoleSpecificActions_Detective_CanInvestigate`

Focused test for Detective role capabilities:
- Tests investigation API endpoints
- Verifies behavior analysis
- Checks suspicious behavior detection

### 4. `CouncilMechanics_AccusationsAndVoting_WorkCorrectly`

Tests council system:
- Advances to council phase
- Makes accusations
- Tests council mechanics

## Running the Tests

### Prerequisites

- .NET 10 SDK installed
- All project dependencies restored

### Run All Tests

```bash
cd tests/VillageOfAshes.E2ETests
dotnet test
```

### Run Specific Test

```bash
dotnet test --filter "FullyQualifiedName~CompleteGamePlaythrough_GoodFactionWins"
```

### Run with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run and Generate Coverage Report

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Test Architecture

### Technology Stack

- **xUnit**: Test framework
- **FluentAssertions**: Assertion library for readable test assertions
- **Microsoft.AspNetCore.Mvc.Testing**: WebApplicationFactory for integration testing
- **HttpClient**: API communication

### Key Features

- **In-Memory Testing**: Uses WebApplicationFactory to spin up the API in memory
- **No External Dependencies**: All tests run against the actual API without mocking
- **Comprehensive Coverage**: Tests entire game flow from start to finish
- **Role-Based Testing**: Tests different player roles and their specific actions
- **Strategy Simulation**: Implements realistic game strategies

## Expected Outcomes

### Success Criteria

✅ Game initializes successfully  
✅ Time advancement works correctly  
✅ Night simulation executes and generates evidence  
✅ NPCs die or get executed during gameplay  
✅ Investigation APIs return data  
✅ Dialogue system functions  
✅ Council mechanics work (accusations, voting)  
✅ Game reaches an end condition (win/loss)  
✅ Win message is set  

### Typical Test Duration

- Main E2E test: 30-60 seconds
- Fast simulation: 5-10 seconds
- Role-specific tests: 1-5 seconds
- Council mechanics: 1-5 seconds

## Debugging Tests

### View Console Output

The tests include extensive console logging:
- Game state changes
- NPC status updates
- Evidence collection
- Investigation findings
- Strategic actions

Run with:
```bash
dotnet test --logger "console;verbosity=normal"
```

### Common Issues

**Test Timeout**
- Increase timeout in test settings if game takes too long
- Check for infinite loops in game logic

**Game Doesn't End**
- Verify win condition logic in GameProgressionService
- Check NPC death mechanics in NightSimulationService

**API Connection Errors**
- Ensure Program.cs has `public partial class Program {}`
- Verify InternalsVisibleTo in API project

## Extending Tests

### Adding New Test Scenarios

```csharp
[Fact]
public async Task YourNewTest_Scenario_ExpectedOutcome()
{
    // Arrange
    var response = await _client.PostAsJsonAsync("/api/game/new", new { });
    var gameState = await response.Content.ReadFromJsonAsync<GameStateDto>();
    
    // Act
    // ... perform game actions
    
    // Assert
    gameState.Should().NotBeNull();
    // ... add your assertions
}
```

### Testing Specific Roles

Modify the game initialization or repeatedly create games until you get the desired player role:

```csharp
GameStateDto? gameState = null;
int attempts = 0;
while ((gameState?.player?.role != "Detective" && attempts++ < 50))
{
    var response = await _client.PostAsJsonAsync("/api/game/new", new { });
    gameState = await response.Content.ReadFromJsonAsync<GameStateDto>();
}
```

### Testing Edge Cases

- Test game with auto-time enabled
- Test rapid time advancement
- Test all NPCs dying
- Test player death scenarios
- Test council with no accusations
- Test voting tie scenarios

## Test Data and Assertions

### Key Assertions

```csharp
// Game should progress
finalState.day.Should().BeGreaterThan(1);

// Evidence should be generated
finalState.evidence.Count.Should().BeGreaterThan(0);

// Game should end
finalState.status.Should().NotBe("InProgress");

// Win message should exist
finalState.winMessage.Should().NotBeNullOrEmpty();

// Some deaths should occur
var deadCount = finalState.npcs.Count(n => n.status == "Dead");
deadCount.Should().BeGreaterThan(0);
```

### Performance Benchmarks

Expected performance for main E2E test:
- Game initialization: < 100ms
- Time advancement: < 50ms per call
- Night simulation: < 200ms
- Total test time: 30-60 seconds

## CI/CD Integration

### GitHub Actions Example

```yaml
- name: Run E2E Tests
  run: |
    cd tests/VillageOfAshes.E2ETests
    dotnet test --logger "trx;LogFileName=test-results.trx"
```

### Azure DevOps Example

```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: 'tests/VillageOfAshes.E2ETests/*.csproj'
    arguments: '--configuration Release'
```

## Contributing

When adding new features to the game:

1. Add corresponding E2E test scenarios
2. Ensure all existing tests still pass
3. Add assertions for new win conditions
4. Document new test scenarios in this README

## License

Same as main project - Educational/Prototype project.

---

**Happy Testing! 🎮🔍💀**
