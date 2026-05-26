# 📁 File Manifest

Complete list of all files in the Village of Ashes project.

## 📚 Documentation (5 files)

| File | Purpose |
|------|---------|
| `README.md` | Main project documentation |
| `QUICKSTART.md` | Getting started guide |
| `PROJECT_SUMMARY.md` | Implementation summary |
| `ARCHITECTURE.md` | Technical architecture documentation |
| `prototype_md_social_horror_village_simulation.md` | Original game design document |

## 🏗️ Solution Files (1 file)

| File | Purpose |
|------|---------|
| `VillageOfAshes.sln` | Visual Studio solution file |

## 🎮 API Project (8 files)

### Controllers (2 files)
- `src/VillageOfAshes.API/Controllers/GameController.cs` - Game lifecycle management
- `src/VillageOfAshes.API/Controllers/DialogueController.cs` - NPC interaction

### Configuration (3 files)
- `src/VillageOfAshes.API/appsettings.json` - Application settings
- `src/VillageOfAshes.API/appsettings.Development.json` - Development settings
- `src/VillageOfAshes.API/Properties/launchSettings.json` - Launch profiles

### Core Files (2 files)
- `src/VillageOfAshes.API/Program.cs` - Application entry point
- `src/VillageOfAshes.API/VillageOfAshes.API.csproj` - Project file

### Frontend (1 file)
- `src/VillageOfAshes.API/wwwroot/index.html` - Single-page application

## 🎯 Core Project (18 files)

### Entities (5 files)
- `src/VillageOfAshes.Core/Entities/NPC.cs` - NPC entity
- `src/VillageOfAshes.Core/Entities/Evidence.cs` - Evidence entity
- `src/VillageOfAshes.Core/Entities/Rumor.cs` - Rumor entity
- `src/VillageOfAshes.Core/Entities/Dialogue.cs` - Dialogue entities
- `src/VillageOfAshes.Core/Entities/GameState.cs` - Game state entity

### Enums (6 files)
- `src/VillageOfAshes.Core/Enums/Alignment.cs` - Good/Evil/Neutral
- `src/VillageOfAshes.Core/Enums/RoleType.cs` - Character roles
- `src/VillageOfAshes.Core/Enums/NPCStatus.cs` - NPC status states
- `src/VillageOfAshes.Core/Enums/GamePhase.cs` - Game phases
- `src/VillageOfAshes.Core/Enums/EvidenceType.cs` - Evidence types
- `src/VillageOfAshes.Core/Enums/DialogueContext.cs` - Dialogue contexts

### Roles (1 file)
- `src/VillageOfAshes.Core/Roles/RoleDefinition.cs` - Role definitions and abilities

### Service Interfaces (6 files)
- `src/VillageOfAshes.Core/Services/ITimeManager.cs` - Time management interface
- `src/VillageOfAshes.Core/Services/INightSimulationService.cs` - Night simulation interface
- `src/VillageOfAshes.Core/Services/ISuspicionCalculator.cs` - Suspicion calculation interface
- `src/VillageOfAshes.Core/Services/IDialogueService.cs` - Dialogue service interface
- `src/VillageOfAshes.Core/Services/IRumorService.cs` - Rumor service interface
- `src/VillageOfAshes.Core/Services/ICouncilService.cs` - Council service interface

### Project File (1 file)
- `src/VillageOfAshes.Core/VillageOfAshes.Core.csproj` - Project file

## 🔧 Infrastructure Project (7 files)

### Service Implementations (6 files)
- `src/VillageOfAshes.Infrastructure/Services/TimeManager.cs` - Time management implementation
- `src/VillageOfAshes.Infrastructure/Services/NightSimulationService.cs` - Night simulation implementation
- `src/VillageOfAshes.Infrastructure/Services/SuspicionCalculator.cs` - Suspicion calculation implementation
- `src/VillageOfAshes.Infrastructure/Services/DialogueService.cs` - Dialogue service implementation
- `src/VillageOfAshes.Infrastructure/Services/RumorService.cs` - Rumor service implementation
- `src/VillageOfAshes.Infrastructure/Services/CouncilService.cs` - Council service implementation

### Project File (1 file)
- `src/VillageOfAshes.Infrastructure/VillageOfAshes.Infrastructure.csproj` - Project file

## ⚙️ VS Code Configuration (3 files)

| File | Purpose |
|------|---------|
| `.vscode/launch.json` | Debug configuration |
| `.vscode/tasks.json` | Build tasks |
| `.vscode/settings.json` | Editor settings |

## 📊 Statistics

### Total Files: 42

#### By Category:
- **Documentation**: 5 files
- **Source Code (C#)**: 27 files
  - Controllers: 2
  - Entities: 5
  - Enums: 6
  - Roles: 1
  - Service Interfaces: 6
  - Service Implementations: 6
  - Entry Point: 1
- **Frontend**: 1 file (HTML/CSS/JS)
- **Configuration**: 7 files
- **VS Code**: 3 files

#### By Project:
- **VillageOfAshes.API**: 8 files
- **VillageOfAshes.Core**: 18 files
- **VillageOfAshes.Infrastructure**: 7 files
- **Documentation**: 5 files
- **Configuration**: 4 files

### Lines of Code (Estimated)

| Component | Files | Approx. Lines |
|-----------|-------|---------------|
| Core Entities | 5 | ~400 |
| Core Enums | 6 | ~100 |
| Core Services (Interfaces) | 6 | ~150 |
| Infrastructure Services | 6 | ~1,200 |
| API Controllers | 2 | ~400 |
| Frontend | 1 | ~600 |
| Role Definitions | 1 | ~150 |
| **Total** | **27** | **~3,000** |

## 🎯 Key Files to Understand

### For Game Design:
1. `prototype_md_social_horror_village_simulation.md` - Original design
2. `src/VillageOfAshes.Core/Roles/RoleDefinition.cs` - Role mechanics
3. `src/VillageOfAshes.Infrastructure/Services/NightSimulationService.cs` - Core simulation

### For Architecture:
1. `ARCHITECTURE.md` - System design
2. `src/VillageOfAshes.API/Program.cs` - Application setup
3. `src/VillageOfAshes.Core/Entities/GameState.cs` - State structure

### For Development:
1. `QUICKSTART.md` - Getting started
2. `README.md` - Comprehensive guide
3. `.vscode/launch.json` - Debug configuration

### For API Usage:
1. `src/VillageOfAshes.API/Controllers/GameController.cs` - Game endpoints
2. `src/VillageOfAshes.API/Controllers/DialogueController.cs` - Dialogue endpoints
3. `src/VillageOfAshes.API/wwwroot/index.html` - Frontend example

## 📦 Dependencies

### NuGet Packages:
- **Swashbuckle.AspNetCore** (10.1.7) - OpenAPI/Swagger documentation
- **Microsoft.AspNetCore.OpenApi** (Built-in) - OpenAPI support

### Framework:
- **.NET 10.0** - Latest .NET framework

## 🔄 Build Artifacts (Excluded)

The following directories are generated during build and excluded from version control:
- `bin/` - Compiled binaries
- `obj/` - Intermediate build files
- `.vs/` - Visual Studio cache
- `*.user` - User-specific settings

## 📝 Notes

### File Naming Conventions:
- **PascalCase** for C# files (e.g., `GameController.cs`)
- **UPPERCASE** for documentation (e.g., `README.md`)
- **camelCase** for JSON files (e.g., `appsettings.json`)

### Project Structure:
```
Village/
├── .vscode/                    # VS Code configuration
├── src/
│   ├── VillageOfAshes.API/    # Presentation layer
│   ├── VillageOfAshes.Core/   # Domain layer
│   └── VillageOfAshes.Infrastructure/  # Infrastructure layer
├── Documentation files         # *.md files
└── VillageOfAshes.sln         # Solution file
```

### Clean Architecture Layers:
1. **Core** (Domain) - No dependencies
2. **Infrastructure** - Depends on Core
3. **API** - Depends on Core and Infrastructure

---

**All files accounted for and documented. ✅**
