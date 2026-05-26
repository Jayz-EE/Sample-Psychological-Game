using Microsoft.AspNetCore.Mvc;
using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private static GameState? _currentGame;
    private readonly ITimeManager _timeManager;
    private readonly INightSimulationService _nightSimulation;
    private readonly IRumorService _rumorService;
    private readonly IGameProgressionService _progressionService;

    private const int MAX_ACTIONS_PER_PHASE = 2;
    private const int ACTION_TIME_COST_MINUTES = 30;

    public GameController(ITimeManager timeManager, INightSimulationService nightSimulation, IRumorService rumorService, IGameProgressionService progressionService)
    {
        _timeManager = timeManager;
        _nightSimulation = nightSimulation;
        _rumorService = rumorService;
        _progressionService = progressionService;
    }

    [HttpPost("new")]
    public ActionResult<object> CreateNewGame()
    {
        _currentGame = InitializeGame();
        _progressionService.CheckWinConditions(_currentGame);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpGet("state")]
    public ActionResult<object> GetGameState()
    {
        if (_currentGame == null)
            return NotFound("No active game");
            
        _progressionService.CheckWinConditions(_currentGame);
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("advance-time")]
    public async Task<ActionResult<object>> AdvanceTime([FromBody] int minutes)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Status != GameStatus.InProgress)
            return BadRequest("Game is already over");

        if (minutes <= 0)
            return BadRequest("Minutes must be positive");

        await HandleTimeAdvancement(_currentGame, minutes);
        
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    private async Task HandleTimeAdvancement(GameState game, int minutes)
    {
        var previousPhase = game.CurrentPhase;
        var previousTime = game.CurrentTime;
        var totalMinutes = (int)previousTime.TotalMinutes + minutes;
        var daysElapsed = totalMinutes / (24 * 60);
        var shouldRunNight = CrossesNightStart(previousTime, minutes);

        game.CurrentTime = _timeManager.AdvanceTime(game.CurrentTime, minutes);
        game.CurrentDay += daysElapsed;

        if (shouldRunNight)
        {
            var result = await _nightSimulation.ExecuteNightPhase(game);
            game.Evidence.AddRange(result.GeneratedEvidence);
            game.Rumors.AddRange(result.GeneratedRumors);
        }
        
        var newPhase = _timeManager.GetCurrentPhase(game.CurrentTime);
        game.CurrentPhase = newPhase;

        if (newPhase != previousPhase)
        {
            ResetPhaseActionCounts(game);
        }
        
        _progressionService.UpdateFactionAlignments(game);
        _progressionService.CheckWinConditions(game);
    }

    private void ResetPhaseActionCounts(GameState game)
    {
        foreach (var npc in game.NPCs)
        {
            npc.PhaseActionCount = 0;
        }
        if (game.Player != null)
        {
            game.Player.PhaseActionCount = 0;
        }
    }

    private async Task<bool> ConsumeAction(GameState game, NPC actor)
    {
        if (actor.PhaseActionCount >= MAX_ACTIONS_PER_PHASE)
            return false;

        actor.PhaseActionCount++;
        await HandleTimeAdvancement(game, ACTION_TIME_COST_MINUTES);
        return true;
    }

    [HttpPost("consume-action")]
    public async Task<ActionResult<object>> PlayerConsumeAction()
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.Player == null) return BadRequest("No active player");

        if (await ConsumeAction(_currentGame, _currentGame.Player))
        {
            SyncSharedState();
            return Ok(ToClientGameState(_currentGame));
        }
        
        return BadRequest($"You have reached the maximum of {MAX_ACTIONS_PER_PHASE} actions for this phase.");
    }

    [HttpPost("npc-consume-action/{npcId}")]
    public async Task<ActionResult<object>> NpcConsumeAction(string npcId)
    {
        if (_currentGame == null) return NotFound("No active game");
        var npc = _currentGame.NPCs.FirstOrDefault(n => n.Id == npcId);
        if (npc == null) return NotFound("NPC not found");

        if (await ConsumeAction(_currentGame, npc))
        {
            SyncSharedState();
            return Ok(ToClientGameState(_currentGame));
        }
        
        return BadRequest($"NPC has reached the maximum of {MAX_ACTIONS_PER_PHASE} actions for this phase.");
    }

    [HttpPost("role-action")]
    public async Task<ActionResult<object>> PerformRoleAction([FromBody] PlayerRoleActionRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Status != GameStatus.InProgress)
            return BadRequest("Game is already over");

        if (_currentGame.Player == null)
            return BadRequest("No active player");

        if (string.IsNullOrWhiteSpace(request.Action))
            return BadRequest("Action is required");

        if (!request.IsFree && !await ConsumeAction(_currentGame, _currentGame.Player))
             return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        var actor = _currentGame.Player;
        var target = string.IsNullOrWhiteSpace(request.TargetNpcId)
            ? null
            : _currentGame.NPCs.FirstOrDefault(n => n.Id == request.TargetNpcId && n.Status == NPCStatus.Alive);

        ApplyRoleAction(_currentGame, actor, request.Action.Trim(), target, request.TargetNpcId);
        _progressionService.CheckWinConditions(_currentGame);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("rumor")]
    public async Task<ActionResult<object>> SpreadRumor([FromBody] PlayerRumorRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Player == null) return BadRequest("No active player");

        if (!await ConsumeAction(_currentGame, _currentGame.Player))
             return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        var target = _currentGame.NPCs.FirstOrDefault(n => n.Id == request.TargetNpcId && n.Status == NPCStatus.Alive);
        if (target == null)
            return BadRequest("Target NPC not found or unavailable");

        if (string.IsNullOrWhiteSpace(request.Context))
            return BadRequest("Rumor text is required");

        var rumor = _rumorService.GenerateRumor("player", target.Id, request.Context.Trim(), 50);
        _currentGame.Rumors.Add(rumor);
        _rumorService.SpreadRumor(_currentGame, rumor);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("heal")]
    public async Task<ActionResult<object>> HealNpc([FromBody] TargetNpcRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Player?.Role != RoleType.Doctor)
            return BadRequest("Only doctors can heal NPCs");

        if (!await ConsumeAction(_currentGame, _currentGame.Player))
             return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        var target = _currentGame.NPCs.FirstOrDefault(n => n.Id == request.TargetNpcId && n.Status == NPCStatus.Alive);
        if (target == null)
            return BadRequest("Target NPC not found or unavailable");

        target.Health = Math.Min(100, target.Health + 30);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("inventory/add")]
    public async Task<ActionResult<object>> AddPlayerInventoryItem([FromBody] InventoryItemRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Player == null)
            return BadRequest("No active player");

        if (!request.IsFree && !await ConsumeAction(_currentGame, _currentGame.Player))
             return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        var item = request.Item?.Trim().ToLowerInvariant();
        if (item is not ("crop" or "meat"))
            return BadRequest("Unsupported inventory item");

        var quantity = Math.Clamp(request.Quantity <= 0 ? 1 : request.Quantity, 1, 20);
        for (var i = 0; i < quantity; i++)
            _currentGame.Player.Inventory.Add(item);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("inventory/remove-all")]
    public ActionResult<object> RemoveAllPlayerInventoryItems([FromBody] InventoryItemRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Player == null)
            return BadRequest("No active player");

        var item = request.Item?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(item))
            return BadRequest("Inventory item is required");

        _currentGame.Player.Inventory = _currentGame.Player.Inventory
            .Where(i => !string.Equals(i, item, StringComparison.OrdinalIgnoreCase))
            .ToList();
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("inventory/consume")]
    public async Task<ActionResult<object>> ConsumePlayerInventoryItem([FromBody] InventoryItemRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (_currentGame.Player == null)
            return BadRequest("No active player");

        if (!request.IsFree && !await ConsumeAction(_currentGame, _currentGame.Player))
             return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        var item = request.Item?.Trim().ToLowerInvariant();
        var quantity = Math.Max(1, request.Quantity); // Default to 1 if not specified
        
        // Check if player has enough items
        var itemCount = _currentGame.Player.Inventory.Count(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase));
        if (itemCount < quantity)
            return BadRequest($"Player does not have enough {item} (has {itemCount}, needs {quantity})");

        // Remove the specified quantity
        for (int i = 0; i < quantity; i++)
        {
            var index = _currentGame.Player.Inventory.FindIndex(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _currentGame.Player.Inventory.RemoveAt(index);
            }
        }
        
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpGet("npcs")]
    public ActionResult<object> GetNPCs()
    {
        if (_currentGame == null)
            return NotFound("No active game");
            
        return Ok(_currentGame.NPCs.Select(ToClientNpc));
    }

    [HttpGet("evidence")]
    public ActionResult<object> GetEvidence()
    {
        if (_currentGame == null)
            return NotFound("No active game");
            
        return Ok(_currentGame.Evidence.Select(ToClientEvidence));
    }

    [HttpGet("rumors")]
    public ActionResult<List<Rumor>> GetRumors()
    {
        if (_currentGame == null)
            return NotFound("No active game");
            
        return Ok(_currentGame.Rumors);
    }



    private object ToClientGameState(GameState game)
    {
        return new
        {
            game.Id,
            game.CurrentDay,
            game.CurrentTime,
            game.CurrentPhase,
            game.Status,
            game.WinMessage,
            NPCs = game.NPCs.Select(ToClientNpc).ToList(),
            Player = game.Player,
            Evidence = game.Evidence.Select(ToClientEvidence).ToList(),
            game.Rumors,
            game.CouncilHistory,
            game.ConversationLogs,
            game.VillageResources,
            game.VillageFear,
            game.VillageCorruption,
            game.FoodSupply,
            game.EconomyStability,
            game.BlackMarketActive,
            game.ShopkeeperAlive,
            game.ShopkeeperProtectionDays,
            game.CreatedAt,
            game.LastUpdated
        };
    }

    private static object ToClientNpc(NPC npc)
    {
        return new
        {
            npc.Id,
            npc.Name,
            npc.HouseId,
            npc.Status,
            npc.Trust,
            npc.Suspicion,
            npc.Fear,
            npc.KnownFacts,
            npc.Rumors,
            npc.Goals,
            npc.Inventory,
            npc.DailySchedule,
            npc.NightActions,
            npc.BehaviorFlags,
            npc.Health,
            npc.Hunger,
            npc.PhaseActionCount,
            npc.CurrentLocation
        };
    }

    private static object ToClientEvidence(Evidence evidence)
    {
        return new
        {
            evidence.Id,
            evidence.Type,
            evidence.Location,
            evidence.Visibility,
            evidence.DecayTime,
            evidence.CreatedAt,
            evidence.Metadata
        };
    }

    private static bool CrossesNightStart(TimeSpan startTime, int minutes)
    {
        var start = (int)startTime.TotalMinutes;
        var end = start + minutes;
        const int nightStart = 21 * 60;

        for (var marker = nightStart; marker <= end; marker += 24 * 60)
        {
            if (marker > start && marker <= end)
                return true;
        }

        return false;
    }

    private static void SyncSharedState()
    {
        if (_currentGame == null) return;
        _currentGame.LastUpdated = DateTime.UtcNow;
        DialogueController.SetGameState(_currentGame);
        InvestigationController.SetGameState(_currentGame);
    }


    private static Alignment GetAlignment(RoleType role)
    {
        return role switch
        {
            RoleType.Detective or RoleType.Doctor or RoleType.Priest or RoleType.Prosecutor => Alignment.Good,
            RoleType.Witch or RoleType.Crawler or RoleType.Butcher or RoleType.Headless => Alignment.Evil,
            RoleType.Farmer or RoleType.Alchemist or RoleType.Hunter or RoleType.Scholar => Alignment.GoodNeutral,
            RoleType.Thief or RoleType.Voyeur or RoleType.Vagabond => Alignment.EvilNeutral,
            RoleType.Shopkeeper => Alignment.FixedNeutral,
            _ => Alignment.Neutral
        };
    }

    private void ApplyRoleAction(GameState game, NPC actor, string action, NPC? target, string? targetId = null)
    {
        var actionKey = action.Replace(" ", string.Empty);
        var location = actor.CurrentLocation;

        void AddEvidence(EvidenceType type, int visibility, string? targetId = null)
        {
            game.Evidence.Add(new Evidence
            {
                Id = Guid.NewGuid().ToString(),
                Type = type,
                Location = location,
                CreatedBy = actor.Id,
                Visibility = visibility,
                DecayTime = 2,
                CreatedAt = DateTime.UtcNow,
                Metadata = targetId == null ? new() : new() { ["targetNpcId"] = targetId }
            });
        }

        void AdjustWorld(int fear = 0, int corruption = 0, int food = 0, int economy = 0)
        {
            game.VillageFear = Math.Clamp(game.VillageFear + fear, 0, 100);
            game.VillageCorruption = Math.Clamp(game.VillageCorruption + corruption, 0, 100);
            game.FoodSupply = Math.Clamp(game.FoodSupply + food, 0, 200);
            game.EconomyStability = Math.Clamp(game.EconomyStability + economy, 0, 100);
            game.BlackMarketActive = game.EconomyStability < 35 || !game.ShopkeeperAlive;
        }

        void AdjustTarget(int trust = 0, int suspicion = 0, int fear = 0, int health = 0)
        {
            if (target == null) return;
            if (!target.Trust.ContainsKey(actor.Id)) target.Trust[actor.Id] = 50;
            if (!target.Suspicion.ContainsKey(actor.Id)) target.Suspicion[actor.Id] = 0;
            if (!target.Fear.ContainsKey(actor.Id)) target.Fear[actor.Id] = 0;
            target.Trust[actor.Id] = Math.Clamp(target.Trust[actor.Id] + trust, 0, 100);
            target.Suspicion[actor.Id] = Math.Clamp(target.Suspicion[actor.Id] + suspicion, 0, 100);
            target.Fear[actor.Id] = Math.Clamp(target.Fear[actor.Id] + fear, 0, 100);
            target.Health = Math.Clamp(target.Health + health, 0, 100);
            if (target.Health == 0) target.Status = NPCStatus.Dead;
        }

        switch (actionKey)
        {
            case "InvestigateHouse": AddEvidence(EvidenceType.ResearchNotes, 25, target?.Id); AdjustTarget(suspicion: 4); break;
            case "InterrogateNPC": AdjustTarget(trust: -3, suspicion: 6, fear: 3); break;
            case "AnalyzeEvidence": game.VillageFear = Math.Max(0, game.VillageFear - 3); AddEvidence(EvidenceType.ResearchNotes, 15); break;
            case "CompareAlibis": AdjustTarget(suspicion: 10); AddEvidence(EvidenceType.ResearchNotes, 20, target?.Id); break;
            case "HealNPC": AdjustTarget(trust: 12, health: 30); AddEvidence(EvidenceType.MedicalResidue, 20, target?.Id); break;
            case "DiagnoseInjury": AddEvidence(EvidenceType.MedicalResidue, 15, target?.Id); AdjustTarget(trust: 5); break;
            case "TreatPanic": AdjustTarget(trust: 8, fear: -20); AdjustWorld(fear: -5); break;
            case "BlessHouse": AddEvidence(EvidenceType.HolyMarkings, 25, target?.Id); AdjustWorld(corruption: -4, fear: -3); break;
            case "CalmVillagers": AdjustWorld(fear: -10); break;
            case "ConductRitual": AddEvidence(EvidenceType.IncenseSmoke, 35); AdjustWorld(corruption: -6); break;
            case "PublicAccusation": AdjustTarget(suspicion: 18, fear: 5); AdjustWorld(fear: 4); break;
            case "ReviewStatements": AddEvidence(EvidenceType.ResearchNotes, 15); break;
            case "DemandTestimony": AdjustTarget(suspicion: 8, fear: 8, trust: -5); AdjustWorld(fear: 3); break;
            case "BuyIngredients": AddEvidence(EvidenceType.RarePurchases, 35); AdjustWorld(economy: 2); break;
            case "SpreadFear": AdjustWorld(fear: 12); AddEvidence(EvidenceType.RarePurchases, 20); break;
            case "PlantRumors": if (target != null) game.Rumors.Add(new Rumor { Id = Guid.NewGuid().ToString(), SourceNpcId = actor.Id, TargetNpcId = target.Id, Context = "linked to unsettling village events", Truthfulness = 25, SpreadRate = 65, CreatedAt = DateTime.UtcNow, KnownBy = new() { actor.Id } }); break;
            case "HideInShadows": actor.BehaviorFlags.Add("Hidden"); AdjustTarget(suspicion: -8); break;
            case "ObserveNPCs": actor.KnownFacts.Add($"Observed {target?.Name ?? "someone"}'s schedule"); AddEvidence(EvidenceType.Footprints, 10, target?.Id); break;
            case "SellMeat": 
                // Remove all meat from inventory and add coins
                var meatCount = actor.Inventory.Count(i => i == "meat");
                actor.Inventory.RemoveAll(i => i == "meat");
                actor.Inventory.AddRange(Enumerable.Repeat("coin", meatCount));
                AddEvidence(EvidenceType.TransactionRecords, 20); 
                AdjustWorld(food: meatCount * 2, economy: meatCount * 2); 
                break;
            case "CleanTools": AddEvidence(EvidenceType.MedicalResidue, 15); AdjustTarget(suspicion: -6); break;
            case "TradeSupplies": AdjustWorld(economy: 4, food: 2); AddEvidence(EvidenceType.TransactionRecords, 20); break;
            case "ManifestBriefly": AdjustWorld(fear: 18, corruption: 4); AddEvidence(EvidenceType.ColdSpots, 45); break;
            case "SowCrops": AdjustWorld(food: 8); AddEvidence(EvidenceType.DamagedCrops, 10); break;
            case "HarvestCrops": actor.Inventory.AddRange(Enumerable.Repeat("crop", 5)); AdjustWorld(food: 10, economy: 2); break;
            case "SellProduce": 
                // Remove all crops from inventory and add coins
                var cropCount = actor.Inventory.Count(i => i == "crop");
                actor.Inventory.RemoveAll(i => i == "crop");
                actor.Inventory.AddRange(Enumerable.Repeat("coin", cropCount));
                AddEvidence(EvidenceType.TransactionRecords, 15); 
                AdjustWorld(food: cropCount * 2, economy: cropCount * 3); 
                break;
            case "FertilizeLand": AdjustWorld(food: 6); AddEvidence(EvidenceType.ChemicalResidue, 20); break;
            case "BrewPotions": actor.Inventory.Add("potion"); AddEvidence(EvidenceType.ChemicalResidue, 35); break;
            case "SellRemedies": 
                // Remove all potions from inventory and add coins
                var potionCount = actor.Inventory.Count(i => i == "potion");
                actor.Inventory.RemoveAll(i => i == "potion");
                actor.Inventory.AddRange(Enumerable.Repeat("coin", potionCount * 2));
                if (target != null) { AdjustTarget(trust: 10, health: 10); }
                AddEvidence(EvidenceType.TransactionRecords, 15);
                AdjustWorld(economy: potionCount * 3); 
                break;
            case "HuntAnimals": actor.Inventory.AddRange(Enumerable.Repeat("meat", 2)); AdjustWorld(food: 5); AddEvidence(EvidenceType.AnimalCarcass, 30); break;
            case "IdentifyTraces":
                var evidence = game.Evidence.FirstOrDefault(e => e.Id == targetId);
                if (evidence != null)
                {
                    var owner = game.NPCs.FirstOrDefault(n => n.Id == evidence.CreatedBy);
                    var ownerName = owner?.Name ?? "Unknown";
                    actor.KnownFacts.Add($"Identified that the {evidence.Type} at {evidence.Location} belongs to {ownerName}.");
                }
                break;
            case "AnalyzeRecords": AddEvidence(EvidenceType.ResearchNotes, 20); break;
            case "StudyPatterns": actor.KnownFacts.Add("Studied village behavior patterns"); AddEvidence(EvidenceType.ResearchNotes, 15); break;
            case "DecodeSymbols": AdjustWorld(corruption: -5); AddEvidence(EvidenceType.ResearchNotes, 25); break;
            case "ScoutHouses": AddEvidence(EvidenceType.Footprints, 30, target?.Id); AdjustTarget(suspicion: 8); break;
            case "TradeStolenGoods": 
                // Remove all stolen items (coins from theft) and convert to legitimate coins
                var stolenCount = actor.Inventory.Count(i => i == "coin" || i == "scrap");
                actor.Inventory.RemoveAll(i => i == "coin" || i == "scrap");
                actor.Inventory.AddRange(Enumerable.Repeat("coin", stolenCount));
                AddEvidence(EvidenceType.TransactionRecords, 35); 
                AdjustWorld(economy: -stolenCount, food: stolenCount); 
                break;
            case "ListenToRumors": actor.KnownFacts.Add("Collected rumors from the village"); break;
            case "SellInformation": 
                // Voyeur sells information for coins (knowledge is not removed, but gets paid)
                var infoValue = actor.KnownFacts.Count / 2; // More knowledge = more value
                actor.Inventory.AddRange(Enumerable.Repeat("coin", Math.Max(1, infoValue)));
                AddEvidence(EvidenceType.VisitorLogs, 20); 
                AdjustWorld(economy: 2);
                break;
            case "BegResources": actor.Inventory.Add("crop"); AdjustTarget(trust: -2, suspicion: 4); break;
            case "TradeRumors": if (target != null) game.Rumors.Add(new Rumor { Id = Guid.NewGuid().ToString(), SourceNpcId = actor.Id, TargetNpcId = target.Id, Context = "seen during odd hours", Truthfulness = 45, SpreadRate = 55, CreatedAt = DateTime.UtcNow, KnownBy = new() { actor.Id } }); break;
            case "SearchScrap": actor.Inventory.Add("scrap"); AddEvidence(EvidenceType.DisturbedDirt, 15); break;
            case "TradeResources": AdjustWorld(economy: 6, food: 2); AddEvidence(EvidenceType.TransactionRecords, 10); break;
            case "SellClues": 
                // Shopkeeper sells clues/evidence for coins
                var clueValue = game.Evidence.Count / 3; // More evidence = more clues to sell
                actor.Inventory.AddRange(Enumerable.Repeat("coin", Math.Max(1, clueValue)));
                AddEvidence(EvidenceType.VisitorLogs, 20); 
                AdjustWorld(economy: 3);
                break;
            case "SpreadRumors": AdjustWorld(fear: 3); break;
            
            // Faction & Win Condition Actions
            case "LeaveVillage":
                if (actor.Id == "player" && (actor.Alignment == Alignment.Neutral || actor.Alignment == Alignment.GoodNeutral || actor.Alignment == Alignment.EvilNeutral))
                {
                    game.Status = GameStatus.NeutralWin;
                    game.WinMessage = "You have managed to escape the village, leaving the darkness behind. You are free, but the village's fate remains uncertain.";
                }
                break;
            case "JoinGoodFaction":
                _progressionService.HandleFactionShift(game, actor.Id, true);
                break;
            case "JoinEvilFaction":
                _progressionService.HandleFactionShift(game, actor.Id, false);
                break;
                
            default: actor.KnownFacts.Add($"Prepared action: {action}"); break;
        }
    }

    private GameState InitializeGame()
    {
        var game = new GameState
        {
            CurrentDay = 1,
            CurrentTime = new TimeSpan(6, 0, 0),
            CurrentPhase = GamePhase.MorningDiscovery
        };

        // Initialize NPCs with random names and roles
        var maleNames = new[] { "Edgar Hollow", "Victor Crowe", "Elias Thorn", "Silas Moore", "Warren Black", "Jonas Vale", "Mathias Crane", "Oscar Flint" };
        var femaleNames = new[] { "Eliza Vane", "Clara Hollow", "Miriam Crowe", "Helena Ward", "Roselyn Pike", "Ada Marrow", "Lenora Fields", "Iris Vale" };
        var allNames = maleNames.Concat(femaleNames).ToList();
        
        var random = new Random();
        var roles = Enum.GetValues<RoleType>()
            .Where(r => r != RoleType.Shopkeeper)
            .OrderBy(_ => random.Next())
            .ToList();
        var npcCount = Math.Min(10, allNames.Count);
        var shuffledNames = allNames.OrderBy(_ => random.Next()).Take(npcCount).ToList();
        var shuffledRoles = roles.Take(npcCount + 1).ToList();
        
        for (int i = 0; i < npcCount; i++)
        {
            var role = shuffledRoles[i];
            var alignment = GetAlignment(role);
            
            game.NPCs.Add(new NPC
            {
                Id = $"npc_{i + 1:D3}",
                Name = shuffledNames[i],
                Role = role,
                Alignment = alignment,
                HouseId = $"house_{i + 1:D2}",
                Status = NPCStatus.Alive,
                CurrentLocation = "VillageCenter"
            });
        }
        
        // Add Shopkeeper
        game.NPCs.Add(new NPC
        {
            Id = "npc_shopkeeper",
            Name = "Tobias Reed",
            Role = RoleType.Shopkeeper,
            Alignment = Alignment.FixedNeutral,
            HouseId = "house_shop",
            Status = NPCStatus.Alive,
            CurrentLocation = "ShopkeeperHouse"
        });
        
        // Initialize player
        game.Player = new NPC
        {
            Id = "player",
            Name = "Player",
            Role = shuffledRoles[^1],
            Alignment = GetAlignment(shuffledRoles[^1]),
            HouseId = "house_player",
            Status = NPCStatus.Alive,
            CurrentLocation = "VillageCenter",
            Inventory = new List<string> { "coin", "coin", "coin", "coin", "coin" } // Start with 5 coins
        };
        
        return game;
    }
}

public class PlayerRoleActionRequest
{
    public string Action { get; set; } = string.Empty;
    public string? TargetNpcId { get; set; }
    public bool IsFree { get; set; } = false;
}

public class PlayerRumorRequest
{
    public string TargetNpcId { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}

public class TargetNpcRequest
{
    public string TargetNpcId { get; set; } = string.Empty;
}

public class InventoryItemRequest
{
    public string Item { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public bool IsFree { get; set; } = false;
}
