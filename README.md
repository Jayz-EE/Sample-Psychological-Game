# 🏚️ Village of Ashes

A single-player psychological social horror simulation game built with ASP.NET Core Web API.

## 🎮 Game Concept

Village of Ashes is a social deduction horror game where NPCs possess hidden roles. Every night, a backend simulation executes role actions, movements, crimes, and evidence generation. Players must investigate, manipulate trust/suspicion, and survive through deduction and emergent storytelling.

## 🏗️ Architecture

### Project Structure

```
VillageOfAshes/
├── src/
│   ├── VillageOfAshes.API/          # Web API & Frontend
│   │   ├── Controllers/             # API endpoints
│   │   ├── wwwroot/                 # Static HTML/JS frontend
│   │   └── Program.cs               # Application entry point
│   ├── VillageOfAshes.Core/         # Domain models & interfaces
│   │   ├── Entities/                # Game entities (NPC, Evidence, Rumor, etc.)
│   │   ├── Enums/                   # Game enumerations
│   │   ├── Roles/                   # Role definitions
│   │   └── Services/                # Service interfaces
│   └── VillageOfAshes.Infrastructure/ # Service implementations
│       └── Services/                # Concrete implementations
└── VillageOfAshes.sln               # Solution file
```

### Clean Architecture Layers

1. **Core Layer** - Domain entities, enums, and service interfaces
2. **Infrastructure Layer** - Service implementations and business logic
3. **API Layer** - Controllers, endpoints, and presentation

## 🎯 Core Systems

### 1. Time Management System
- Manages game phases (Night, Morning, Council, Day, Evening)
- Advances time and triggers phase transitions
- Coordinates simulation execution

### 2. Night Simulation Engine
- Executes role-specific actions
- Generates NPC movements
- Creates evidence and encounters
- Spawns rumors
- Processes deaths and events

### 3. Suspicion System
- Calculates suspicion based on multiple factors:
  - Witness evidence
  - Rumor weight
  - Contradictions
  - Role bias
  - RNG modifiers
- Updates dynamically based on events

### 4. Enhanced Dialogue System ⭐ NEW
- **Role-Agnostic Design**: All NPCs can use any dialogue
- **35+ Unique Dialogues**: Expanded dialogue pool
- **Ambiguous Observations**: Statements don't reveal roles
- **6 Dialogue Contexts**: Neutral, Suspicious, Fearful, Trusting, Aggressive, Rumor
- **5-6 Response Options**: Per context with varied effects
- **Strategic Depth**: Players must deduce roles from patterns, not words

### 5. Rumor System
- Generates rumors based on events
- 20+ ambiguous rumor contexts
- Spreads rumors through social networks
- Truthfulness and spread rate mechanics
- Affects NPC suspicion levels

### 6. Council System
- Morning council meetings (7:00 AM - 8:00 AM)
- Ambiguous NPC statements (20+ variations)
- Role-agnostic accusations
- Voting mechanics
- Alliance formation
- Execution system

## 🎭 Roles

### Good Roles
- **Detective**: Investigates crimes, tracks movements
- **Doctor**: Heals and protects villagers

### Evil Roles
- **Butcher**: Kills villagers at night

### Neutral Roles
- **Vagabond**: Survives and escapes
- **Farmer**: Maintains food supply
- **Shopkeeper**: Fixed NPC, manages economy

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Any modern web browser

### Running the Game

1. **Build the solution:**
```bash
dotnet build
```

2. **Run the API:**
```bash
cd src/VillageOfAshes.API
dotnet run
```

3. **Open the game:**
Navigate to `http://localhost:5000` in your browser

### API Endpoints

#### Game Management
- `POST /api/game/new` - Start a new game
- `GET /api/game/state` - Get current game state
- `POST /api/game/advance-time` - Advance time by minutes
- `GET /api/game/npcs` - Get all NPCs
- `GET /api/game/evidence` - Get all evidence
- `GET /api/game/rumors` - Get all rumors

#### Dialogue
- `GET /api/dialogue/npc/{npcId}` - Get dialogue options for NPC
- `POST /api/dialogue/respond` - Respond to dialogue

## 🎲 Game Mechanics

### Time System
| Time | Phase | Description |
|------|-------|-------------|
| 9:00 PM - 5:00 AM | Night Simulation | Backend executes role actions |
| 6:00 AM | Morning Discovery | Players discover events |
| 7:00 AM - 8:00 AM | Village Council | NPCs discuss and vote |
| 8:00 AM - 6:00 PM | Day Actions | Role-specific actions |
| 6:00 PM - 9:00 PM | Evening | Limited movement |

### Evidence Types
- Blood
- Footprints
- Broken Locks
- Stolen Items
- Damaged Crops
- Ritual Markings
- Weapon Traces
- Corpse Wounds

### Suspicion Calculation
```
Final Suspicion = Base Suspicion
                + Witness Evidence
                + Rumor Weight
                + Contradiction Weight
                + Role Bias
                + RNG Modifier
```

### Win Conditions

**Good Victory:**
- Eliminate all evil roles
- Preserve village stability

**Evil Victory:**
- Outnumber good faction
- Collapse village order

**Neutral Victory:**
- Complete personal role objectives
- Escape village alive

## 🎨 Frontend Features

- Real-time game state display
- NPC status tracking
- Evidence viewer
- Rumor system
- Event log
- Time advancement controls
- Responsive design with dark horror theme

## 🔧 Technical Details

### Technologies Used
- **Backend**: ASP.NET Core 10 Web API
- **Frontend**: Vanilla JavaScript, HTML5, CSS3
- **Architecture**: Clean Architecture with DDD principles
- **Patterns**: Repository pattern, Dependency Injection, Service layer

### Key Design Decisions

1. **Singleton Services**: Game services are registered as singletons for state persistence
2. **In-Memory State**: Current implementation uses in-memory game state (can be extended to database)
3. **Event-Driven**: Night simulation generates events that cascade through systems
4. **Weighted RNG**: All random decisions use weighted probabilities for realistic behavior

## 📈 Future Enhancements

### Phase 1 (MVP) ✅
- [x] Time system
- [x] NPC management
- [x] Role system
- [x] Night simulation
- [x] Evidence generation
- [x] Suspicion system
- [x] Basic frontend
- [x] **Enhanced dialogue system (35+ dialogues)**
- [x] **Role-agnostic conversations**
- [x] **Ambiguous observations and accusations**

### Phase 2 (Planned)
- [ ] Full dialogue system with branching conversations
- [ ] Council voting UI
- [ ] Player action system
- [ ] Save/Load game state
- [ ] Database persistence (Entity Framework Core)

### Phase 3 (Advanced)
- [ ] SignalR for real-time updates
- [ ] Advanced AI decision-making
- [ ] Multiple game scenarios
- [ ] Sound effects and atmosphere
- [ ] Mobile-responsive improvements

### Phase 4 (Polish)
- [ ] Achievements system
- [ ] Statistics tracking
- [ ] Multiple difficulty levels
- [ ] Custom game rules
- [ ] Mod support

## 🎯 Design Philosophy

> "The game should never directly reveal truth. Players must observe behavior, compare statements, investigate clues, interpret rumors, manage trust, and survive uncertainty."

Fear comes from:
- Paranoia
- Misinformation
- Social collapse
- Hidden motives
- Incomplete knowledge

NOT from constant jumpscares.

## 📝 Development Notes

### Adding New Roles

1. Add role to `RoleType` enum
2. Define role in `RoleDefinitions.cs`
3. Implement role actions in `NightSimulationService.cs`
4. Add role-specific dialogue in `DialogueService.cs`

### Adding New Evidence Types

1. Add type to `EvidenceType` enum
2. Update evidence generation in `NightSimulationService.cs`
3. Update suspicion calculation in `SuspicionCalculator.cs`

### Extending Dialogue System

1. Add new context to `DialogueContext` enum
2. Create dialogue entries in `DialogueService.InitializeDialogues()`
3. Add response options in `GenerateOptions()`

## 🐛 Known Issues

- Game state is not persisted (resets on server restart)
- No authentication/authorization
- Single game instance only
- Limited error handling in frontend

## 📄 License

This is a prototype/educational project. Feel free to use and modify.

## 🤝 Contributing

This is a learning project, but suggestions and improvements are welcome!

## 📧 Contact

For questions or feedback about this implementation, please refer to the original design document: `prototype_md_social_horror_village_simulation.md`

---

**Built with ❤️ and ☠️ using .NET 10**
