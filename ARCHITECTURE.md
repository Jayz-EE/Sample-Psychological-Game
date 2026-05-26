# 🏗️ Architecture Documentation

## System Overview

Village of Ashes follows **Clean Architecture** principles with clear separation between domain logic, business rules, and infrastructure concerns.

## Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                        Presentation Layer                    │
│  ┌────────────────────┐         ┌─────────────────────────┐ │
│  │   HTML/CSS/JS      │         │   API Controllers       │ │
│  │   Frontend         │◄────────┤   - GameController      │ │
│  │   (wwwroot/)       │  HTTP   │   - DialogueController  │ │
│  └────────────────────┘         └─────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Application Layer                       │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              Service Interfaces (Core)                │   │
│  │  - ITimeManager                                       │   │
│  │  - INightSimulationService                           │   │
│  │  - ISuspicionCalculator                              │   │
│  │  - IDialogueService                                  │   │
│  │  - IRumorService                                     │   │
│  │  - ICouncilService                                   │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                        Domain Layer                          │
│  ┌──────────────────────────────────────────────────────┐   │
│  │                  Domain Entities                      │   │
│  │  - NPC          - Evidence      - Rumor              │   │
│  │  - GameState    - Dialogue      - CouncilRecord      │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │                  Domain Enums                         │   │
│  │  - RoleType     - GamePhase     - EvidenceType       │   │
│  │  - Alignment    - NPCStatus     - DialogueContext    │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │                  Role Definitions                     │   │
│  │  - Detective    - Doctor        - Butcher            │   │
│  │  - Vagabond     - Farmer        - Shopkeeper         │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │            Service Implementations                    │   │
│  │  - TimeManager                                        │   │
│  │  - NightSimulationService                            │   │
│  │  - SuspicionCalculator                               │   │
│  │  - DialogueService                                   │   │
│  │  - RumorService                                      │   │
│  │  - CouncilService                                    │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow

### Game Initialization Flow
```
User clicks "New Game"
    │
    ▼
Frontend (index.html)
    │ POST /api/game/new
    ▼
GameController.CreateNewGame()
    │
    ├─► Initialize GameState
    ├─► Assign random roles to NPCs
    ├─► Create player character
    ├─► Set starting time (6:00 AM)
    └─► Return GameState
    │
    ▼
Frontend updates UI
```

### Time Advancement Flow
```
User clicks "Advance Time"
    │
    ▼
Frontend (index.html)
    │ POST /api/game/advance-time
    ▼
GameController.AdvanceTime()
    │
    ├─► TimeManager.AdvanceTime()
    │   └─► Calculate new time
    │
    ├─► TimeManager.ShouldTransitionPhase()
    │   └─► Check if phase change needed
    │
    └─► If Night Phase:
        │
        ▼
    NightSimulationService.ExecuteNightPhase()
        │
        ├─► AssignRoleActions()
        ├─► ExecuteMovements()
        ├─► ExecuteRoleActions()
        │   ├─► ExecuteKillAction()
        │   ├─► ExecuteProtectAction()
        │   └─► ExecuteTrackAction()
        ├─► GenerateEncounters()
        ├─► SpawnEvidence()
        ├─► GenerateRumors()
        └─► UpdateNPCStates()
    │
    ▼
Return updated GameState
    │
    ▼
Frontend updates UI
```

### Suspicion Calculation Flow
```
Evidence Generated
    │
    ▼
SuspicionCalculator.UpdateSuspicionFromEvidence()
    │
    ├─► For each NPC:
    │   │
    │   ├─► CalculateWitnessEvidence()
    │   │   └─► Check proximity to evidence
    │   │
    │   ├─► CalculateRumorWeight()
    │   │   └─► Sum rumor influences
    │   │
    │   ├─► CalculateContradictions()
    │   │   └─► Check statement conflicts
    │   │
    │   ├─► CalculateRoleBias()
    │   │   └─► Apply role modifiers
    │   │
    │   └─► Add RNG modifier
    │
    └─► Update NPC.Suspicion dictionary
```

### Dialogue Flow
```
User requests dialogue with NPC
    │
    ▼
Frontend
    │ GET /api/dialogue/npc/{id}
    ▼
DialogueController.GetDialogueOptions()
    │
    ├─► DetermineDialogueContext()
    │   └─► Check suspicion/trust/fear levels
    │
    └─► DialogueService.GenerateDialogue()
        │
        ├─► GetAvailableDialogues()
        │   └─► Filter by context and conditions
        │
        └─► GenerateOptions()
            └─► Create response choices
    │
    ▼
Return DialogueExchange
    │
    ▼
User selects response
    │
    ▼
Frontend
    │ POST /api/dialogue/respond
    ▼
DialogueController.RespondToDialogue()
    │
    └─► DialogueService.ApplyDialogueEffects()
        │
        ├─► Update NPC.Trust
        ├─► Update NPC.Suspicion
        ├─► Update NPC.Fear
        └─► Generate rumor if needed
```

## Component Responsibilities

### Core Layer (Domain)

#### Entities
- **NPC**: Represents villagers with roles, stats, and relationships
- **GameState**: Central game state container
- **Evidence**: Physical clues left by actions
- **Rumor**: Social information propagation
- **Dialogue**: Conversation structures
- **CouncilRecord**: Historical council data

#### Enums
- Define valid states and types
- Ensure type safety
- Document game constants

#### Role Definitions
- Define role abilities and constraints
- Specify win conditions
- Document role behaviors

### Infrastructure Layer

#### TimeManager
- **Responsibility**: Game time and phase management
- **Key Methods**:
  - `GetCurrentPhase()`: Determine current game phase
  - `AdvanceTime()`: Move time forward
  - `ShouldTransitionPhase()`: Check for phase changes

#### NightSimulationService
- **Responsibility**: Execute night phase simulation
- **Key Methods**:
  - `ExecuteNightPhase()`: Main simulation loop
  - `AssignRoleActions()`: Determine NPC actions
  - `ExecuteKillAction()`: Process murders
  - `SpawnEvidence()`: Generate clues

#### SuspicionCalculator
- **Responsibility**: Calculate and update suspicion levels
- **Key Methods**:
  - `CalculateSuspicion()`: Multi-factor calculation
  - `UpdateSuspicionFromEvidence()`: Evidence-based updates
  - `GetPublicSuspicionRankings()`: Community perception

#### DialogueService
- **Responsibility**: Generate and manage conversations
- **Key Methods**:
  - `GenerateDialogue()`: Create dialogue exchanges
  - `ApplyDialogueEffects()`: Update relationships
  - `EvaluateConditions()`: Check dialogue availability

#### RumorService
- **Responsibility**: Rumor generation and propagation
- **Key Methods**:
  - `GenerateRumor()`: Create new rumors
  - `SpreadRumor()`: Propagate through social network
  - `GetRumorsAbout()`: Query rumors by target

#### CouncilService
- **Responsibility**: Manage council meetings
- **Key Methods**:
  - `StartCouncilSession()`: Initialize council
  - `ProcessAccusation()`: Handle accusations
  - `ResolveCouncil()`: Determine outcomes

### API Layer

#### GameController
- **Responsibility**: Game lifecycle management
- **Endpoints**:
  - `POST /api/game/new`: Start new game
  - `GET /api/game/state`: Get current state
  - `POST /api/game/advance-time`: Progress time
  - `GET /api/game/npcs`: List NPCs
  - `GET /api/game/evidence`: List evidence
  - `GET /api/game/rumors`: List rumors

#### DialogueController
- **Responsibility**: NPC interaction
- **Endpoints**:
  - `GET /api/dialogue/npc/{id}`: Get dialogue options
  - `POST /api/dialogue/respond`: Submit response

## Design Patterns

### Dependency Injection
```csharp
// Services registered in Program.cs
builder.Services.AddSingleton<ITimeManager, TimeManager>();
builder.Services.AddSingleton<INightSimulationService, NightSimulationService>();

// Injected into controllers
public GameController(ITimeManager timeManager, INightSimulationService nightSimulation)
{
    _timeManager = timeManager;
    _nightSimulation = nightSimulation;
}
```

### Service Layer Pattern
```csharp
// Interface defines contract
public interface ISuspicionCalculator
{
    int CalculateSuspicion(NPC observer, NPC target, GameState gameState);
}

// Implementation provides logic
public class SuspicionCalculator : ISuspicionCalculator
{
    public int CalculateSuspicion(NPC observer, NPC target, GameState gameState)
    {
        // Complex calculation logic
    }
}
```

### Strategy Pattern (Roles)
```csharp
// Each role has different behavior
public static class RoleDefinitions
{
    public static readonly Dictionary<RoleType, RoleDefinition> Roles = new()
    {
        { RoleType.Detective, new RoleDefinition { /* ... */ } },
        { RoleType.Butcher, new RoleDefinition { /* ... */ } }
    };
}
```

## State Management

### Current Implementation
- **In-Memory State**: Static `GameState` object
- **Shared Between Controllers**: Via static helper method
- **Session-Based**: Single game instance

### Future Enhancement
```csharp
// Database persistence with EF Core
public class GameStateRepository : IGameStateRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<GameState> GetByIdAsync(string id)
    {
        return await _context.GameStates
            .Include(g => g.NPCs)
            .Include(g => g.Evidence)
            .Include(g => g.Rumors)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
```

## Security Considerations

### Current State
- No authentication/authorization
- Single-player only
- Local state management

### Production Recommendations
1. **Authentication**: Add JWT or cookie-based auth
2. **Authorization**: Role-based access control
3. **Input Validation**: Validate all user inputs
4. **Rate Limiting**: Prevent API abuse
5. **HTTPS**: Enforce secure connections
6. **CORS**: Restrict to known origins

## Performance Considerations

### Optimization Strategies
1. **Caching**: Cache frequently accessed data
2. **Async Operations**: Use async/await throughout
3. **Lazy Loading**: Load data on demand
4. **Pagination**: Limit result set sizes
5. **Indexing**: Database indexes for queries

### Scalability Path
```
Current: Single Instance
    │
    ▼
Add: Redis Cache
    │
    ▼
Add: Database (PostgreSQL/SQL Server)
    │
    ▼
Add: SignalR for Real-Time
    │
    ▼
Add: Load Balancer
    │
    ▼
Add: Microservices (if needed)
```

## Testing Strategy

### Unit Tests (Recommended)
```csharp
[Fact]
public void CalculateSuspicion_WithHighEvidence_ReturnsHighValue()
{
    // Arrange
    var calculator = new SuspicionCalculator();
    var observer = new NPC { /* ... */ };
    var target = new NPC { /* ... */ };
    var gameState = new GameState { /* ... */ };
    
    // Act
    var suspicion = calculator.CalculateSuspicion(observer, target, gameState);
    
    // Assert
    Assert.InRange(suspicion, 60, 100);
}
```

### Integration Tests (Recommended)
```csharp
[Fact]
public async Task NightSimulation_WithButcher_GeneratesDeathAndEvidence()
{
    // Arrange
    var service = new NightSimulationService();
    var gameState = CreateGameStateWithButcher();
    
    // Act
    var result = await service.ExecuteNightPhase(gameState);
    
    // Assert
    Assert.NotEmpty(result.Deaths);
    Assert.Contains(result.GeneratedEvidence, e => e.Type == EvidenceType.Blood);
}
```

## Deployment

### Development
```bash
dotnet run --project src/VillageOfAshes.API
```

### Production
```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet VillageOfAshes.API.dll
```

### Docker (Future)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "VillageOfAshes.API.dll"]
```

## Monitoring & Logging

### Recommended Additions
1. **Serilog**: Structured logging
2. **Application Insights**: Telemetry
3. **Health Checks**: Endpoint monitoring
4. **Metrics**: Performance tracking

---

**Architecture designed for maintainability, scalability, and extensibility.**
