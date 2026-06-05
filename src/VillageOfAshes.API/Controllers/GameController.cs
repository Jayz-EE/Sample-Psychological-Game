using Microsoft.AspNetCore.Mvc;
using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;
using VillageOfAshes.Core.Dialogue;

namespace VillageOfAshes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private static GameState? _currentGame;
    private readonly ITimeManager _timeManager;
    private readonly INightSimulationService _nightSimulation;
    private readonly IDaySimulationService _daySimulation;
    private readonly IRumorService _rumorService;
    private readonly IGameProgressionService _progressionService;
    private readonly ICouncilService _councilService;
    private readonly INpcDecisionService _npcDecisions;

    private const int MAX_ACTIONS_PER_PHASE = 2;
    private const int ACTION_TIME_COST_MINUTES = 30;

    public GameController(
        ITimeManager timeManager,
        INightSimulationService nightSimulation,
        IDaySimulationService daySimulation,
        IRumorService rumorService,
        IGameProgressionService progressionService,
        ICouncilService councilService,
        INpcDecisionService npcDecisions)
    {
        _timeManager = timeManager;
        _nightSimulation = nightSimulation;
        _daySimulation = daySimulation;
        _rumorService = rumorService;
        _progressionService = progressionService;
        _councilService = councilService;
        _npcDecisions = npcDecisions;
    }

    [HttpPost("new")]
    public ActionResult<object> CreateNewGame([FromBody] NewGameRequest? request = null)
    {
        _currentGame = InitializeGame();
        
        // Configure automation settings if provided
        if (request != null)
        {
            _currentGame.AutoTimeEnabled = request.AutoTimeEnabled;
            _currentGame.AutoTimeIntervalSeconds = Math.Clamp(request.AutoTimeIntervalSeconds, 1, 60);
            _currentGame.AutoTimeIncrementMinutes = Math.Clamp(request.AutoTimeIncrementMinutes, 5, 360);
            _currentGame.PauseOnCouncil = request.PauseOnCouncil;
            _currentGame.PauseOnDeath = request.PauseOnDeath;
            _currentGame.PauseOnPlayerAction = request.PauseOnPlayerAction;
            _currentGame.LastAutoAdvance = DateTime.UtcNow;
        }
        
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
            await OnPhaseEntered(game, previousPhase, newPhase);
        }
        
        _progressionService.UpdateFactionAlignments(game);
        _progressionService.CheckWinConditions(game);
    }

    private async Task OnPhaseEntered(GameState game, GamePhase previousPhase, GamePhase newPhase)
    {
        if (newPhase == GamePhase.VillageCouncil && previousPhase != GamePhase.VillageCouncil)
        {
            _npcDecisions.RefreshAllNpcSuspicions(game);
            game.ActiveCouncil = await _councilService.StartCouncilSession(game);
            foreach (var statement in game.ActiveCouncil.Statements.ToList())
            {
                var speaker = game.NPCs.FirstOrDefault(n => n.Id == statement.NpcId);
                game.RecentEvents.Add($"Council Day {game.CurrentDay}: {speaker?.Name ?? "Villager"} — {statement.Statement}");

                // All NPCs analyze these initial statements
                _npcDecisions.AnalyzeStatement(game, statement.NpcId, statement.Statement);
            }
        }

        if (previousPhase == GamePhase.VillageCouncil && newPhase != GamePhase.VillageCouncil && game.ActiveCouncil != null)
        {
            if (!game.ActiveCouncil.Resolved)
            {
                if (!game.ActiveCouncil.VotingPhase)
                    _councilService.StartVoting(game, game.ActiveCouncil);
                var outcome = _councilService.ResolveCouncil(game, game.ActiveCouncil);
                if (!string.IsNullOrEmpty(outcome.ExecutedNpcId))
                {
                    var executed = FindPerson(game, outcome.ExecutedNpcId);
                    if (executed != null)
                        game.RecentEvents.Add($"Council Day {game.CurrentDay}: {executed.Name} was burned for retribution. Revealed role: {outcome.RevealedRole}.");
                }
            }
            if (string.IsNullOrEmpty(game.PendingPranksterRevealNpcId))
                game.ActiveCouncil = null;
        }

        if (newPhase == GamePhase.DayActions && previousPhase != GamePhase.DayActions)
        {
            var dayResult = _daySimulation.ExecuteDayPhase(game);
            game.Rumors.AddRange(dayResult.GeneratedRumors);
            foreach (var evt in dayResult.Events.Take(6))
                game.RecentEvents.Add($"Day {game.CurrentDay}: {evt}");
            if (game.RecentEvents.Count > 24)
                game.RecentEvents = game.RecentEvents.TakeLast(24).ToList();
        }
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

        if (target != null && IsEvilAttackAction(request.Action) && IsEvilAligned(actor) && IsEvilAligned(target))
            return BadRequest("Evil actions cannot target evil-aligned characters.");

        var resultMessage = ApplyRoleAction(_currentGame, actor, request.Action.Trim(), target, request.TargetNpcId);
        _progressionService.CheckWinConditions(_currentGame);
        SyncSharedState();
        
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = resultMessage
        });
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
        
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = $"You spread a rumor about {target.Name}."
        });
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
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = $"You healed {target.Name} and restored their strength."
        });
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
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = $"Added {quantity} {item} to your inventory."
        });
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

        var count = _currentGame.Player.Inventory.Count(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase));
        _currentGame.Player.Inventory = _currentGame.Player.Inventory
            .Where(i => !string.Equals(i, item, StringComparison.OrdinalIgnoreCase))
            .ToList();
        SyncSharedState();
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = $"Removed all {count} {item} from your inventory."
        });
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
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = $"Consumed {quantity} {item}."
        });
    }

    [HttpPost("clear-notifications")]
    public ActionResult<object> ClearPlayerNotifications()
    {
        if (_currentGame == null)
            return NotFound("No active game");

        _currentGame.PlayerNotifications.Clear();
        SyncSharedState();
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            success = true
        });
    }


    [HttpPost("auto-time/configure")]
    public ActionResult<object> ConfigureAutoTime([FromBody] AutoTimeConfigRequest request)
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (request.AutoTimeEnabled.HasValue)
            _currentGame.AutoTimeEnabled = request.AutoTimeEnabled.Value;
        
        if (request.AutoTimeIntervalSeconds.HasValue)
            _currentGame.AutoTimeIntervalSeconds = Math.Clamp(request.AutoTimeIntervalSeconds.Value, 1, 60);
        
        if (request.AutoTimeIncrementMinutes.HasValue)
            _currentGame.AutoTimeIncrementMinutes = Math.Clamp(request.AutoTimeIncrementMinutes.Value, 5, 360);
        
        if (request.PauseOnCouncil.HasValue)
            _currentGame.PauseOnCouncil = request.PauseOnCouncil.Value;
        
        if (request.PauseOnDeath.HasValue)
            _currentGame.PauseOnDeath = request.PauseOnDeath.Value;
        
        if (request.PauseOnPlayerAction.HasValue)
            _currentGame.PauseOnPlayerAction = request.PauseOnPlayerAction.Value;

        _currentGame.LastAutoAdvance = DateTime.UtcNow;
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("auto-time/toggle")]
    public ActionResult<object> ToggleAutoTime()
    {
        if (_currentGame == null)
            return NotFound("No active game");

        _currentGame.AutoTimeEnabled = !_currentGame.AutoTimeEnabled;
        _currentGame.LastAutoAdvance = DateTime.UtcNow;
        SyncSharedState();
        return Ok(new { 
            autoTimeEnabled = _currentGame.AutoTimeEnabled,
            message = _currentGame.AutoTimeEnabled ? "Auto-time enabled" : "Auto-time paused"
        });
    }

    [HttpGet("auto-time/should-advance")]
    public ActionResult<object> ShouldAutoAdvance()
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (!_currentGame.AutoTimeEnabled || _currentGame.Status != GameStatus.InProgress)
            return Ok(new { shouldAdvance = false, reason = "Auto-time disabled or game over" });

        // Check pause conditions
        if (_currentGame.PauseOnCouncil && _currentGame.CurrentPhase == GamePhase.VillageCouncil)
            return Ok(new { shouldAdvance = false, reason = "Paused on council" });

        var secondsSinceLastAdvance = (DateTime.UtcNow - _currentGame.LastAutoAdvance).TotalSeconds;
        var shouldAdvance = secondsSinceLastAdvance >= _currentGame.AutoTimeIntervalSeconds;

        return Ok(new { 
            shouldAdvance,
            secondsSinceLastAdvance,
            intervalSeconds = _currentGame.AutoTimeIntervalSeconds,
            incrementMinutes = _currentGame.AutoTimeIncrementMinutes
        });
    }

    [HttpPost("auto-time/advance")]
    public async Task<ActionResult<object>> AutoAdvanceTime()
    {
        if (_currentGame == null)
            return NotFound("No active game");

        if (!_currentGame.AutoTimeEnabled)
            return BadRequest("Auto-time is not enabled");

        var previousAliveCount = _currentGame.NPCs.Count(n => n.Status == NPCStatus.Alive);
        
        await HandleTimeAdvancement(_currentGame, _currentGame.AutoTimeIncrementMinutes);
        _currentGame.LastAutoAdvance = DateTime.UtcNow;

        // Check if someone died and pause if configured
        var currentAliveCount = _currentGame.NPCs.Count(n => n.Status == NPCStatus.Alive);
        if (_currentGame.PauseOnDeath && currentAliveCount < previousAliveCount)
        {
            _currentGame.AutoTimeEnabled = false;
            _currentGame.PlayerNotifications.Add($"⏸️ Auto-time paused: {previousAliveCount - currentAliveCount} death(s) detected");
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

    [HttpPost("council/player-statement")]
    public ActionResult<object> MakePlayerCouncilStatement([FromBody] PlayerCouncilStatementRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.Player == null) return BadRequest("No active player");
        if (_currentGame.CurrentPhase != GamePhase.VillageCouncil)
            return BadRequest("Council statements can only be made during Village Council");
        if (_currentGame.ActiveCouncil == null) return BadRequest("No active council");

        if (string.IsNullOrWhiteSpace(request.Statement))
            return BadRequest("Statement is required");

        // Add player's statement to council log
        _currentGame.ActiveCouncil.Statements.Add(new CouncilStatement
        {
            NpcId = _currentGame.Player.Id,
            Statement = request.Statement.Trim(),
            Timestamp = DateTime.UtcNow
        });

        _currentGame.RecentEvents.Add($"Council Day {_currentGame.CurrentDay}: {_currentGame.Player.Name} — {request.Statement.Trim()}");

        // NPCs analyze the player's statement and update their opinions
        _npcDecisions.AnalyzeStatement(_currentGame, _currentGame.Player.Id, request.Statement.Trim());

        SyncSharedState();
        return Ok(new { 
            success = true,
            statement = request.Statement.Trim(),
            gameState = ToClientGameState(_currentGame)
        });
    }

    [HttpGet("council/dialogue-options")]
    public ActionResult<object> GetCouncilDialogueOptions()
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.Player == null) return BadRequest("No active player");

        // Get the same ambiguous dialogue options that NPCs use
        var options = DialoguePools.PassiveResponses.Take(10).ToList();

        // Add some council-specific options
        var councilOptions = new List<string>
        {
            "We need to stay calm and think this through carefully.",
            "Someone here knows more than they're letting on.",
            "The evidence points to suspicious activity last night.",
            "I trust some of you more than others here.",
            "We can't let fear drive our decisions.",
            "There's been too much blood already.",
            "I've noticed some strange behavior around the village.",
            "We need to work together if we want to survive.",
            "Not everyone here has the village's best interests at heart.",
            "I'm watching all of you carefully."
        };

        return Ok(new
        {
            options = options.Concat(councilOptions).OrderBy(_ => Guid.NewGuid()).Take(12).ToList()
        });
    }

    [HttpPost("council/player-action")]
    public async Task<ActionResult<object>> CouncilPlayerAction([FromBody] CouncilPlayerActionRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.CurrentPhase != GamePhase.VillageCouncil)
            return BadRequest("Council actions are only available during Village Council");
        if (_currentGame.Player == null) return BadRequest("No active player");
        if (!await ConsumeAction(_currentGame, _currentGame.Player))
            return BadRequest($"Maximum {MAX_ACTIONS_PER_PHASE} actions allowed per phase.");

        string actionResult = "Council action completed.";

        if (!string.IsNullOrWhiteSpace(request.TargetNpcId) && !string.IsNullOrWhiteSpace(request.Reason))
        {
            var target = FindPerson(_currentGame, request.TargetNpcId);
            _councilService.ProcessAccusation(_currentGame, "player", request.TargetNpcId, request.Reason.Trim());
            _currentGame.ActiveCouncil?.Accusations.Add(new Accusation
            {
                SourceNpcId = "player",
                TargetNpcId = request.TargetNpcId,
                Reason = request.Reason.Trim()
            });
            actionResult = $"You formally accused {target?.Name ?? "them"} of: {request.Reason}";
        }

        SyncSharedState();
        return Ok(new {
            gameState = ToClientGameState(_currentGame),
            actionResult = actionResult
        });
    }

    [HttpPost("council/start-vote")]
    public ActionResult<object> StartCouncilVote()
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.ActiveCouncil == null) return BadRequest("No active council");
        _councilService.StartVoting(_currentGame, _currentGame.ActiveCouncil);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("council/vote")]
    public ActionResult<object> CouncilVote([FromBody] TargetNpcRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.Player == null) return BadRequest("No active player");
        if (_currentGame.ActiveCouncil == null) return BadRequest("No active council");
        var target = FindPerson(_currentGame, request.TargetNpcId);
        if (target == null || target.Status != NPCStatus.Alive) return BadRequest("Vote target is unavailable");

        _councilService.ProcessVote(_currentGame, _currentGame.Player.Id, request.TargetNpcId);
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("council/resolve-vote")]
    public ActionResult<object> ResolveCouncilVote()
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.ActiveCouncil == null) return BadRequest("No active council");
        _councilService.StartVoting(_currentGame, _currentGame.ActiveCouncil);
        var outcome = _councilService.ResolveCouncil(_currentGame, _currentGame.ActiveCouncil);
        if (!string.IsNullOrEmpty(outcome.ExecutedNpcId))
        {
            var executed = FindPerson(_currentGame, outcome.ExecutedNpcId);
            if (executed != null)
                _currentGame.RecentEvents.Add($"Council Day {_currentGame.CurrentDay}: {executed.Name} was burned for retribution. Revealed role: {outcome.RevealedRole}.");
        }

        if (string.IsNullOrEmpty(_currentGame.PendingPranksterRevealNpcId))
            _currentGame.ActiveCouncil = null;

        SyncSharedState();
        return Ok(ToClientGameState(_currentGame) );
    }

    [HttpPost("council/alibi")]
    public ActionResult<object> CouncilAlibi([FromBody] CouncilAlibiRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        
        // Handle remain silent choice
        if (request.PlayerChoice == "remain-silent")
        {
            var target = request.TargetNpcId == "player" ? _currentGame.Player : 
                        (string.IsNullOrEmpty(request.TargetNpcId) ? null : FindPerson(_currentGame, request.TargetNpcId));
            if (target == null) return BadRequest("Target not found");
            
            var silentResponse = target.Id == _currentGame.Player?.Id
                ? "I choose to remain silent."
                : $"{target.Name} refuses to respond and remains silent.";
            
            // Slight suspicion increase for remaining silent
            foreach (var observer in _currentGame.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != target.Id))
            {
                observer.Suspicion[target.Id] = Math.Min(100, observer.Suspicion.GetValueOrDefault(target.Id, 0) + 3);
            }
            
            SyncSharedState();
            return Ok(new { speakerId = target.Id, speaker = target.Name, text = silentResponse, gameState = ToClientGameState(_currentGame) });
        }
        
        var alibiTarget = string.IsNullOrWhiteSpace(request.TargetNpcId)
            ? _currentGame.Player
            : FindPerson(_currentGame, request.TargetNpcId);
        if (alibiTarget == null) return BadRequest("No alibi speaker found");

        var line = alibiTarget.Id == _currentGame.Player?.Id && !string.IsNullOrWhiteSpace(request.PlayerChoice)
            ? request.PlayerChoice.Trim()
            : _npcDecisions.GenerateAlibiLine(alibiTarget, _currentGame, request.AccusationReason ?? string.Empty);

        foreach (var observer in _currentGame.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != alibiTarget.Id))
        {
            var reduction = alibiTarget.Id == _currentGame.Player?.Id ? 6 : 4;
            
            // Enhanced logic for player role claims
            if (alibiTarget.Id == _currentGame.Player?.Id && !string.IsNullOrWhiteSpace(request.PlayerChoice))
            {
                var choice = request.PlayerChoice.ToLower();
                var actualRole = alibiTarget.Role.ToString().ToLower();
                
                if (choice.Contains(actualRole))
                {
                    reduction = 12; // Truthful claim is effective
                }
                else if (choice.Contains("doctor") || choice.Contains("priest") || choice.Contains("scholar"))
                {
                    // Believable lies for non-evil roles, or deceptive for evil roles
                    var isEvil = alibiTarget.Alignment is Alignment.Evil or Alignment.EvilNeutral;
                    reduction = isEvil ? 8 : 5;
                }
                else if (choice.Contains("villager") || choice.Contains("farmer"))
                {
                    reduction = 10; // Simple roles are less suspicious
                }
            }
            
            observer.Suspicion[alibiTarget.Id] = Math.Max(0, observer.Suspicion.GetValueOrDefault(alibiTarget.Id, 0) - reduction);
        }

        SyncSharedState();
        return Ok(new { speakerId = alibiTarget.Id, speaker = alibiTarget.Name, text = line, gameState = ToClientGameState(_currentGame) });
    }

    [HttpPost("council/remain-silent")]
    public ActionResult<object> CouncilRemainSilent([FromBody] RemainSilentRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        
        var target = FindPerson(_currentGame, request.TargetNpcId);
        if (target == null) return BadRequest("Target not found");
        
        var silentResponse = target.Id == _currentGame.Player?.Id
            ? "I choose to remain silent."
            : $"{target.Name} refuses to respond and remains silent.";
        
        // Slight suspicion increase for remaining silent
        foreach (var observer in _currentGame.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != target.Id))
        {
            observer.Suspicion[target.Id] = Math.Min(100, observer.Suspicion.GetValueOrDefault(target.Id, 0) + 3);
        }
        
        SyncSharedState();
        return Ok(new { speakerId = target.Id, speaker = target.Name, text = silentResponse, gameState = ToClientGameState(_currentGame) });
    }

    [HttpPost("council/prankster-reveal")]
    public ActionResult<object> PranksterReveal([FromBody] PranksterRevealRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.Player?.Role != RoleType.Prankster || _currentGame.Player.Status != NPCStatus.Alive)
            return BadRequest("Only a living Prankster can alter a role reveal");
        if (_currentGame.Player.PranksterRoleChangesUsed >= 2) return BadRequest("Prankster role reveal changes are already spent");
        if (_currentGame.Player.PhaseActionCount >= 2) return BadRequest("The Prankster needs an unused council action to alter the reveal");
        if (string.IsNullOrWhiteSpace(_currentGame.PendingPranksterRevealNpcId)) return BadRequest("No pending retribution reveal");

        var burned = FindPerson(_currentGame, _currentGame.PendingPranksterRevealNpcId);
        if (burned == null) return BadRequest("Burned target is unavailable");
        if (!Enum.TryParse<RoleType>(request.FakeRole, ignoreCase: true, out var fakeRole) || fakeRole == burned.Role)
            return BadRequest("Choose a different visible role");

        burned.RevealedRole = fakeRole;
        burned.RoleRevealTampered = true;
        _currentGame.Player.PhaseActionCount++;
        _currentGame.Player.PranksterRoleChangesUsed++;
        _currentGame.PendingPranksterRevealNpcId = null;
        if (_currentGame.ActiveCouncil != null)
        {
            _currentGame.ActiveCouncil.RevealedRole = fakeRole;
            _currentGame.ActiveCouncil.RoleRevealTampered = true;
            _currentGame.ActiveCouncil = null;
        }

        _currentGame.RecentEvents.Add($"Council Day {_currentGame.CurrentDay}: the burned role reveal was altered to {fakeRole}.");
        SyncSharedState();
        return Ok(ToClientGameState(_currentGame));
    }

    [HttpPost("council/npc-reaction")]
    public async Task<ActionResult<object>> CouncilNpcReaction([FromBody] CouncilReactionRequest request)
    {
        if (_currentGame == null) return NotFound("No active game");
        if (_currentGame.CurrentPhase != GamePhase.VillageCouncil)
            return BadRequest("Council reactions are only available during Village Council");

        var speakers = _currentGame.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != request.TargetNpcId)
            .ToList();
        if (!speakers.Any()) return BadRequest("No NPCs available to respond");

        var speaker = speakers[new Random().Next(speakers.Count)];
        if (!await ConsumeAction(_currentGame, speaker))
            return BadRequest("NPC has no actions remaining this phase");

        var line = _npcDecisions.GenerateCouncilReaction(
            speaker,
            _currentGame,
            _currentGame.ActiveCouncil?.RecentDiscourse ?? new List<CouncilStatement>());

        if (string.IsNullOrEmpty(line))
            return Ok(new { success = false, message = "NPC had nothing to say" });

        // Add to council statements so it's persistent and analyzed
        if (_currentGame.ActiveCouncil != null)
        {
            var stmt = new CouncilStatement
            {
                NpcId = speaker.Id,
                Statement = line,
                Timestamp = DateTime.UtcNow,
                TargetNpcId = request.TargetNpcId
            };
            _currentGame.ActiveCouncil.Statements.Add(stmt);
            
            // Track in social context
            _currentGame.ActiveCouncil.RecentDiscourse.Add(stmt);
            if (_currentGame.ActiveCouncil.RecentDiscourse.Count > 5)
                _currentGame.ActiveCouncil.RecentDiscourse.RemoveAt(0);
            
            // Analyze the reaction
            _npcDecisions.AnalyzeStatement(_currentGame, speaker.Id, line);
        }

        SyncSharedState();
        return Ok(new
        {
            speakerId = speaker.Id,
            speaker = speaker.Name,
            text = line,
            gameState = ToClientGameState(_currentGame)
        });
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
            Player = game.Player == null ? null : ToClientNpc(game.Player),
            Items = game.Items,
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
            game.RecentEvents,
            PlayerNotifications = game.PlayerNotifications.ToList(), // Send a copy
            ActiveCouncil = game.ActiveCouncil == null ? null : new
            {
                game.ActiveCouncil.Day,
                game.ActiveCouncil.VotingPhase,
                game.ActiveCouncil.Resolved,
                game.ActiveCouncil.Votes,
                game.ActiveCouncil.BurnedNpcId,
                game.ActiveCouncil.RevealedRole,
                game.ActiveCouncil.RoleRevealTampered,
                Statements = game.ActiveCouncil.Statements,
                Accusations = game.ActiveCouncil.Accusations
            },
            game.PendingPranksterRevealNpcId,
            game.CreatedAt,
            game.LastUpdated,
            // Automation settings
            AutoTime = new
            {
                game.AutoTimeEnabled,
                game.AutoTimeIntervalSeconds,
                game.AutoTimeIncrementMinutes,
                game.PauseOnCouncil,
                game.PauseOnDeath,
                game.PauseOnPlayerAction,
                game.LastAutoAdvance
            }
        };
    }

    private static object ToClientNpc(NPC npc)
    {
        return new
        {
            npc.Id,
            npc.Name,
            npc.Role,
            npc.Alignment,
            npc.HouseId,
            npc.Status,
            npc.Trust,
            npc.Suspicion,
            npc.Fear,
            npc.KnownFacts,
            npc.Rumors,
            npc.Goals,
            npc.IsGoalCompleted,
            npc.Inventory,
            HeldItems = CurrentItemsFor(npc, _currentGame).ToList(),
            npc.DailySchedule,
            npc.NightActions,
            npc.BehaviorFlags,
            npc.IsCursed,
            npc.IsIll,
            npc.IllnessSuppressedUntilDay,
            npc.CurseSourceItemId,
            npc.RevealedRole,
            npc.RoleRevealTampered,
            npc.PranksterRoleChangesUsed,
            npc.Health,
            npc.Hunger,
            npc.PhaseActionCount,
            npc.CurrentLocation
        };
    }

    private static IEnumerable<GameItem> CurrentItemsFor(NPC npc, GameState? game) =>
        game?.Items.Where(i => i.CurrentHolderId == npc.Id) ?? Enumerable.Empty<GameItem>();

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
            RoleType.Thief or RoleType.Voyeur or RoleType.Vagabond or RoleType.Prankster => Alignment.EvilNeutral,
            RoleType.Shopkeeper => Alignment.FixedNeutral,
            _ => Alignment.Neutral
        };
    }

    private string ApplyRoleAction(GameState game, NPC actor, string action, NPC? target, string? targetId = null)
    {
        var actionKey = action.Replace(" ", string.Empty);
        var location = actor.CurrentLocation;
        var resultMessage = $"{action} completed.";

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
            
            // Do NOT notify the player as if they found it; they are the one who left it.
            // NPCs or the player later can find it through investigation.
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

            if (target.Id == "player")
            {
                if (trust < 0) game.PlayerNotifications.Add($"📉 Your trust with {actor.Name} has decreased.");
                if (suspicion > 0) game.PlayerNotifications.Add($"🔍 {actor.Name} is becoming more suspicious of you!");
                if (fear > 0) game.PlayerNotifications.Add($"😱 {actor.Name} has intimidated you.");
                if (health < 0) game.PlayerNotifications.Add($"🤕 {actor.Name} has harmed you! Health: {target.Health}%");
                if (health > 0) game.PlayerNotifications.Add($"❤️ {actor.Name} has helped you. Health: {target.Health}%");
            }
            else if (actor.Id == "player")
            {
                // Result messages will be shown in the modal, but we can still use notifications for non-modal updates
                if (trust < 0) game.PlayerNotifications.Add($"📉 {target.Name} trusts you less now.");
                if (trust > 0) game.PlayerNotifications.Add($"📈 {target.Name} trusts you more!");
            }
        }

        void RemoveCurse(NPC cursed)
        {
            cursed.IsCursed = false;
            cursed.IsIll = false;
            cursed.IllnessSuppressedUntilDay = 0;
            cursed.CurseSourceItemId = null;
            if (cursed.Id == "player")
                game.PlayerNotifications.Add("✨ Your curse has been lifted!");
        }

        void SuppressIllness(NPC patient)
        {
            patient.IsIll = false;
            patient.IllnessSuppressedUntilDay = Math.Max(patient.IllnessSuppressedUntilDay, game.CurrentDay + 2);
            patient.Health = Math.Min(100, patient.Health + 25);
            if (patient.Id == "player")
                game.PlayerNotifications.Add("💊 Your illness has been suppressed. You feel better.");
        }

        void ApplyCurse(NPC cursed)
        {
            cursed.IsCursed = true;
            cursed.IsIll = cursed.IllnessSuppressedUntilDay < game.CurrentDay;
            cursed.Fear[actor.Id] = Math.Clamp(cursed.Fear.GetValueOrDefault(actor.Id, 0) + 25, 0, 100);
            cursed.Health = Math.Max(1, cursed.Health - 15);
            game.VillageCorruption = Math.Min(100, game.VillageCorruption + 8);
            AddEvidence(EvidenceType.RitualMarkings, 60, cursed.Id);

            var sourceItem = game.Items.FirstOrDefault(i =>
                i.OwnerNpcId == cursed.Id &&
                i.CurrentHolderId == actor.Id &&
                !i.IsEvilOwned);

            if (sourceItem != null)
            {
                cursed.CurseSourceItemId = sourceItem.Id;
                cursed.Status = NPCStatus.Dead;
                var msg = $"Night {game.CurrentDay}: {cursed.Name}'s {sourceItem.Name} carried the witch curse back to its owner.";
                game.RecentEvents.Add(msg);
                if (cursed.Id == "player")
                    game.PlayerNotifications.Add($"💀 Your {sourceItem.Name} carried a curse back to you! You have perished.");
            }
            else
            {
                game.RecentEvents.Add($"Night {game.CurrentDay}: {cursed.Name} was cursed and fell ill.");
                if (cursed.Id == "player")
                    game.PlayerNotifications.Add("🧪 You have been cursed and fallen ill!");
            }
        }

        switch (actionKey)
        {
            case "InvestigateHouse": 
                AddEvidence(EvidenceType.ResearchNotes, 25, target?.Id); 
                AdjustTarget(suspicion: 4); 
                resultMessage = $"You investigated {target?.Name ?? "the house"} and noted several peculiarities.";
                break;
            case "InterrogateNPC": 
                AdjustTarget(trust: -3, suspicion: 6, fear: 3); 
                resultMessage = $"You interrogated {target?.Name ?? "the person"}. They seemed rattled but were they truthful?";
                break;
            case "AnalyzeEvidence": 
                game.VillageFear = Math.Max(0, game.VillageFear - 3); 
                AddEvidence(EvidenceType.ResearchNotes, 15); 
                resultMessage = "You spent time cross-referencing clues. Some patterns are beginning to emerge.";
                break;
            case "CompareAlibis": 
                AdjustTarget(suspicion: 10); 
                AddEvidence(EvidenceType.ResearchNotes, 20, target?.Id); 
                resultMessage = $"Comparing {target?.Name ?? "their"} alibi against the village timeline reveals inconsistencies.";
                break;
            case "HealNPC":
                if (target != null && target.IsIll) SuppressIllness(target);
                AdjustTarget(trust: 12, health: 30);
                AddEvidence(EvidenceType.MedicalResidue, 20, target?.Id);
                resultMessage = $"You provided medical treatment to {target?.Name ?? "the patient"}.";
                break;
            case "DiagnoseInjury": 
                AddEvidence(EvidenceType.MedicalResidue, 15, target?.Id); 
                AdjustTarget(trust: 5); 
                resultMessage = $"Your examination of {target?.Name ?? "the patient"} provides a clearer picture of their health.";
                break;
            case "TreatPanic": 
                AdjustTarget(trust: 8, fear: -20); 
                AdjustWorld(fear: -5); 
                resultMessage = "Your calm demeanor and medical expertise have quieted the rising panic.";
                break;
            case "BlessHouse": 
                AddEvidence(EvidenceType.HolyMarkings, 25, target?.Id); 
                AdjustWorld(corruption: -4, fear: -3); 
                resultMessage = $"You blessed {target?.Name ?? "the home"}, driving back the encroaching shadows.";
                break;
            case "CalmVillagers": 
                AdjustWorld(fear: -10); 
                resultMessage = "Your sermons and presence have brought a measure of peace to the village center.";
                break;
            case "ConductRitual":
                if (target != null && target.IsCursed) RemoveCurse(target);
                AddEvidence(EvidenceType.IncenseSmoke, 35, target?.Id);
                AdjustWorld(corruption: -6);
                resultMessage = "The ancient ritual was conducted successfully. The spiritual weight has lifted.";
                break;
            case "RemoveCurse":
                if (target != null)
                {
                    RemoveCurse(target);
                    target.Trust[actor.Id] = Math.Min(100, target.Trust.GetValueOrDefault(actor.Id, 50) + 12);
                    game.RecentEvents.Add($"Day {game.CurrentDay}: {actor.Name} removed the curse from {target.Name}.");
                    resultMessage = $"You successfully removed the curse from {target.Name}. They look relieved.";
                }
                AddEvidence(EvidenceType.HolyMarkings, 35, target?.Id);
                AdjustWorld(corruption: -8);
                break;
            case "PublicAccusation": 
                AdjustTarget(suspicion: 18, fear: 5); 
                AdjustWorld(fear: 4); 
                resultMessage = $"You publicly accused {target?.Name ?? "them"}! The village is now watching them closely.";
                break;
            case "ReviewStatements": 
                AddEvidence(EvidenceType.ResearchNotes, 15); 
                resultMessage = "You reviewed the council's record. You found several contradictions in previous statements.";
                break;
            case "DemandTestimony": 
                AdjustTarget(suspicion: 8, fear: 8, trust: -5); 
                AdjustWorld(fear: 3); 
                resultMessage = $"You demanded a formal testimony from {target?.Name ?? "them"}. The pressure is mounting.";
                break;
            case "BuyIngredients": 
                AddEvidence(EvidenceType.RarePurchases, 35); 
                AdjustWorld(economy: 2); 
                resultMessage = "You acquired the rare ingredients needed for your work. They were expensive.";
                break;
            case "SpreadFear": 
                AdjustWorld(fear: 12); 
                AddEvidence(EvidenceType.RarePurchases, 20); 
                resultMessage = "The whispers you started have grown into a palpable dread. The village is on edge.";
                break;
            case "PlantRumors": 
                if (target != null) 
                {
                    game.Rumors.Add(new Rumor { Id = Guid.NewGuid().ToString(), SourceNpcId = actor.Id, TargetNpcId = target.Id, Context = "linked to unsettling village events", Truthfulness = 25, SpreadRate = 65, CreatedAt = DateTime.UtcNow, KnownBy = new() { actor.Id } }); 
                    resultMessage = $"You have successfully planted a damaging rumor about {target.Name}.";
                }
                break;
            case "CurseNPC": 
                if (target != null) 
                {
                    ApplyCurse(target);
                    resultMessage = $"You have placed a dark curse upon {target.Name}. They will suffer the consequences.";
                }
                break;
            case "HideInShadows": 
                actor.BehaviorFlags.Add("Hidden"); 
                AdjustTarget(suspicion: -8); 
                resultMessage = "You have merged with the shadows. You are much harder to track now.";
                break;
            case "ObserveNPCs": 
                var fact = $"Observed {target?.Name ?? "someone"}'s schedule";
                actor.KnownFacts.Add(fact); 
                AddEvidence(EvidenceType.Footprints, 10, target?.Id); 
                resultMessage = $"You spent hours watching {target?.Name ?? "them"}. You've learned: {fact}.";
                break;
            case "SellMeat": 
                var meatCount = actor.Inventory.Count(i => string.Equals(i, "meat", StringComparison.OrdinalIgnoreCase));
                actor.Inventory.RemoveAll(i => string.Equals(i, "meat", StringComparison.OrdinalIgnoreCase));
                if (meatCount > 0)
                    actor.Inventory.AddRange(Enumerable.Repeat("coin", meatCount));
                AddEvidence(EvidenceType.TransactionRecords, 20); 
                AdjustWorld(food: meatCount * 2, economy: meatCount * 2); 
                resultMessage = $"You sold {meatCount} meat to the shopkeeper. You feel the weight of coins in your pocket.";
                break;
            case "CleanTools": 
                AddEvidence(EvidenceType.MedicalResidue, 15); 
                AdjustTarget(suspicion: -6); 
                resultMessage = "You meticulously cleaned your equipment. Any traces of your recent work are gone.";
                break;
            case "TradeSupplies": 
                AdjustWorld(economy: 4, food: 2); 
                AddEvidence(EvidenceType.TransactionRecords, 20); 
                resultMessage = "The trade was successful. The village's supply chain is slightly more stable.";
                break;
            case "ManifestBriefly": 
                AdjustWorld(fear: 18, corruption: 4); 
                AddEvidence(EvidenceType.ColdSpots, 45); 
                resultMessage = "You manifested your true form for a fleeting moment. The air around you turned to ice.";
                break;
            case "SowCrops": 
                AdjustWorld(food: 8); 
                AddEvidence(EvidenceType.DamagedCrops, 10); 
                resultMessage = "You spent the day working the fields. Future harvests look promising.";
                break;
            case "HarvestCrops": 
                actor.Inventory.AddRange(Enumerable.Repeat("crop", 5)); 
                AdjustWorld(food: 10, economy: 2); 
                resultMessage = "You harvested a bountiful yield of crops. This will keep the village fed.";
                break;
            case "SellProduce": 
                var cropCount = actor.Inventory.Count(i => string.Equals(i, "crop", StringComparison.OrdinalIgnoreCase));
                actor.Inventory.RemoveAll(i => string.Equals(i, "crop", StringComparison.OrdinalIgnoreCase));
                if (cropCount > 0)
                    actor.Inventory.AddRange(Enumerable.Repeat("coin", cropCount));
                AddEvidence(EvidenceType.TransactionRecords, 15); 
                AdjustWorld(food: cropCount * 2, economy: cropCount * 3); 
                resultMessage = $"You sold {cropCount} crops to the shopkeeper. Your hard work has paid off.";
                break;
            case "FertilizeLand": 
                AdjustWorld(food: 6); 
                AddEvidence(EvidenceType.ChemicalResidue, 20); 
                resultMessage = "The soil is now rich with nutrients. The crops will grow stronger.";
                break;
            case "BrewPotions": 
                actor.Inventory.Add("potion"); 
                AddEvidence(EvidenceType.ChemicalResidue, 35); 
                resultMessage = "You carefully brewed a potent potion. Its contents glow with a faint, unnatural light.";
                break;
            case "GivePotion":
                var potionIndex = actor.Inventory.FindIndex(i => string.Equals(i, "potion", StringComparison.OrdinalIgnoreCase));
                if (target != null && potionIndex >= 0)
                {
                    actor.Inventory.RemoveAt(potionIndex);
                    target.Health = Math.Min(100, target.Health + 30);
                    target.Trust[actor.Id] = Math.Min(100, target.Trust.GetValueOrDefault(actor.Id, 50) + 10);
                    game.RecentEvents.Add($"Day {game.CurrentDay}: {actor.Name} gave {target.Name} a potion.");
                    resultMessage = $"You gave the potion to {target.Name}. They seem more vigorous.";
                }
                AddEvidence(EvidenceType.ChemicalResidue, 25, target?.Id);
                break;
            case "SellRemedies": 
                var potionCount = actor.Inventory.Count(i => string.Equals(i, "potion", StringComparison.OrdinalIgnoreCase));
                actor.Inventory.RemoveAll(i => string.Equals(i, "potion", StringComparison.OrdinalIgnoreCase));
                if (potionCount > 0)
                    actor.Inventory.AddRange(Enumerable.Repeat("coin", potionCount * 2));
                if (target != null)
                {
                    if (target.IsIll)
                    {
                        target.Health = Math.Min(100, target.Health + 20);
                    }
                    AdjustTarget(trust: 10, health: 10);
                }
                AddEvidence(EvidenceType.TransactionRecords, 15);
                AdjustWorld(economy: potionCount * 3); 
                resultMessage = $"You sold {potionCount} remedies. Your skills are in high demand.";
                break;
            case "HuntAnimals": 
                actor.Inventory.AddRange(Enumerable.Repeat("meat", 2)); 
                AdjustWorld(food: 5); 
                AddEvidence(EvidenceType.AnimalCarcass, 30); 
                resultMessage = "The hunt was successful. You tracked and brought down enough game for several meals.";
                break;
            case "IdentifyTraces":
                var traceEvid = game.Evidence.FirstOrDefault(e => e.Id == targetId);
                if (traceEvid != null)
                {
                    var owner = game.NPCs.FirstOrDefault(n => n.Id == traceEvid.CreatedBy);
                    var ownerName = owner?.Name ?? "Unknown";
                    var factIden = $"Identified that the {traceEvid.Type} at {traceEvid.Location} belongs to {ownerName}.";
                    actor.KnownFacts.Add(factIden);
                    resultMessage = $"Your tracking skills revealed the truth: {factIden}";
                }
                break;
            case "AnalyzeRecords": 
                AddEvidence(EvidenceType.ResearchNotes, 20); 
                resultMessage = "Hours spent in the village archives have revealed hidden connections between families.";
                break;
            case "StudyPatterns": 
                actor.KnownFacts.Add("Studied village behavior patterns"); 
                AddEvidence(EvidenceType.ResearchNotes, 15); 
                resultMessage = "You've identified a distinct rhythm to the village's daily life—and those who break it.";
                break;
            case "DecodeSymbols": 
                AdjustWorld(corruption: -5); 
                AddEvidence(EvidenceType.ResearchNotes, 25); 
                resultMessage = "The cryptic marks left on the old stones have been deciphered. Their dark influence is weakened.";
                break;
            case "ScoutHouses": 
                AddEvidence(EvidenceType.Footprints, 30, target?.Id); 
                AdjustTarget(suspicion: 8); 
                resultMessage = $"You scouted {target?.Name ?? "the area"} from a distance, noting all exits and guard rotations.";
                break;
            case "TradeStolenGoods": 
                var stolenCount = actor.Inventory.Count(i =>
                    string.Equals(i, "coin", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(i, "scrap", StringComparison.OrdinalIgnoreCase));
                actor.Inventory.RemoveAll(i =>
                    string.Equals(i, "coin", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(i, "scrap", StringComparison.OrdinalIgnoreCase));
                if (stolenCount > 0)
                    actor.Inventory.AddRange(Enumerable.Repeat("coin", stolenCount));
                AddEvidence(EvidenceType.TransactionRecords, 35); 
                AdjustWorld(economy: -stolenCount, food: stolenCount); 
                resultMessage = $"You traded your ill-gotten gains for clean coins. No questions were asked.";
                break;
            case "ListenToRumors": 
                actor.KnownFacts.Add("Collected rumors from the village"); 
                resultMessage = "You spent the day in the tavern and markets, absorbing the village's whispers.";
                break;
            case "SellInformation": 
                if (actor.KnownFacts.Count == 0) 
                {
                    resultMessage = "You have no information of value to sell currently.";
                    break;
                }
                var factsToSell = Math.Max(1, actor.KnownFacts.Count / 2);
                actor.KnownFacts.RemoveRange(0, factsToSell);
                actor.Inventory.AddRange(Enumerable.Repeat("coin", factsToSell));
                AddEvidence(EvidenceType.VisitorLogs, 20); 
                AdjustWorld(economy: 2);
                resultMessage = $"You sold your secrets for {factsToSell} coins. Knowledge is indeed power.";
                break;
            case "BegResources": 
                actor.Inventory.Add("crop"); 
                AdjustTarget(trust: -2, suspicion: 4); 
                resultMessage = "Your pleas for help were answered, but the villagers look at you with pity and suspicion.";
                break;
            case "TradeRumors": 
                if (target != null) 
                {
                    game.Rumors.Add(new Rumor { Id = Guid.NewGuid().ToString(), SourceNpcId = actor.Id, TargetNpcId = target.Id, Context = "seen during odd hours", Truthfulness = 45, SpreadRate = 55, CreatedAt = DateTime.UtcNow, KnownBy = new() { actor.Id } }); 
                    resultMessage = $"You exchanged secrets with {target.Name}. The web of rumors grows thicker.";
                }
                break;
            case "SearchScrap": 
                actor.Inventory.Add("scrap"); 
                AddEvidence(EvidenceType.DisturbedDirt, 15); 
                resultMessage = "You scoured the village outskirts for anything useful. You found some salvageable scrap.";
                break;
            case "TradeResources": 
                AdjustWorld(economy: 6, food: 2); 
                AddEvidence(EvidenceType.TransactionRecords, 10); 
                resultMessage = "The bulk trade was profitable for both you and the village stores.";
                break;
            case "SellPersonalItems":
                var sellableItems = game.Items.Where(i => i.CurrentHolderId == actor.Id).ToList();
                foreach (var item in sellableItems)
                {
                    actor.Inventory.Remove(item.Name.ToLowerInvariant());
                    actor.Inventory.AddRange(Enumerable.Repeat("coin", item.Value));
                    game.Items.Remove(item);
                }
                AddEvidence(EvidenceType.TransactionRecords, 25);
                AdjustWorld(economy: sellableItems.Sum(i => i.Value));
                resultMessage = $"You sold {sellableItems.Count} personal items. Your backpack is lighter, but your purse is heavy.";
                break;
            case "UseMagnifyingGlass":
            case "UseListeningConch":
            case "UseFamilyLedger":
            case "UseSilverCharm":
                UseUtilityItem(game, actor, actionKey, target);
                resultMessage = $"You used a utility item to gain an advantage.";
                break;
            case "SellClues": 
                if (game.Evidence.Count == 0) 
                {
                    resultMessage = "There are no clues to sell at the moment.";
                    break;
                }
                var evidenceToSell = Math.Max(1, game.Evidence.Count / 3);
                game.Evidence.RemoveRange(0, evidenceToSell);
                actor.Inventory.AddRange(Enumerable.Repeat("coin", evidenceToSell));
                AddEvidence(EvidenceType.VisitorLogs, 20); 
                AdjustWorld(economy: 3);
                resultMessage = $"You sold {evidenceToSell} clues. Information is the village's most valuable currency.";
                break;
            
            case "LeaveVillage":
                if (actor.Id == "player")
                {
                    game.Status = GameStatus.NeutralWin;
                    game.WinMessage = "You have managed to escape the village, leaving the darkness behind.";
                    resultMessage = "You have successfully escaped the village.";
                }
                break;
            case "JoinGoodFaction":
                _progressionService.HandleFactionShift(game, actor.Id, true);
                resultMessage = "You have formally aligned yourself with those seeking to protect the village.";
                break;
            case "JoinEvilFaction":
                _progressionService.HandleFactionShift(game, actor.Id, false);
                resultMessage = "You have embraced the darkness and aligned yourself with the village's predators.";
                break;
                
            default: 
                actor.KnownFacts.Add($"Prepared action: {action}"); 
                resultMessage = $"You prepared for: {action}.";
                break;
        }

        return resultMessage;
    }


    private static NPC? FindPerson(GameState game, string personId) =>
        game.NPCs.FirstOrDefault(n => n.Id == personId)
        ?? (game.Player?.Id == personId ? game.Player : null);

    private static bool IsEvilAligned(NPC npc) =>
        npc.Alignment is Alignment.Evil or Alignment.EvilNeutral;

    private static bool IsEvilAttackAction(string action)
    {
        var key = action.Replace(" ", string.Empty);
        return key is "CurseNPC" or "KillNPC" or "AmbushNPC" or "StalkTarget" or "TerrorizeNPC" or "HauntArea" or "CurseGround";
    }

    private void UseUtilityItem(GameState game, NPC actor, string actionKey, NPC? target)
    {
        var itemName = actionKey switch
        {
            "UseMagnifyingGlass" => "Magnifying Glass",
            "UseListeningConch" => "Listening Conch",
            "UseFamilyLedger" => "Family Ledger",
            "UseSilverCharm" => "Silver Charm",
            _ => string.Empty
        };

        var item = game.Items.FirstOrDefault(i =>
            i.Name == itemName &&
            i.CurrentHolderId == actor.Id &&
            (!i.UsableByOwnerOnly || i.OwnerNpcId == actor.Id));

        if (item == null)
        {
            actor.KnownFacts.Add($"{itemName} could not be used because it is not owned by {actor.Name}.");
            return;
        }

        switch (actionKey)
        {
            case "UseMagnifyingGlass":
            {
                var note = game.RecentEvents
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .OrderBy(_ => Guid.NewGuid())
                    .FirstOrDefault() ?? "No clear activity could be recovered from past events.";
                actor.KnownFacts.Add($"Note from the Magnifying Glass: {note}");
                game.RecentEvents.Add($"Day {game.CurrentDay}: {actor.Name} used a Magnifying Glass to study old activity.");
                break;
            }
            case "UseListeningConch":
            {
                var watched = target ?? game.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != actor.Id).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                if (watched != null)
                    actor.KnownFacts.Add($"Listening Conch: {watched.Name} was heard near {watched.CurrentLocation}.");
                break;
            }
            case "UseFamilyLedger":
            {
                var owner = game.NPCs.FirstOrDefault(n => n.Id == item.OwnerNpcId);
                actor.KnownFacts.Add($"Family Ledger: {owner?.Name ?? "Unknown"} is tied to {owner?.HouseId ?? item.HouseId}.");
                break;
            }
            case "UseSilverCharm":
            {
                var evilNames = game.NPCs
                    .Where(n => n.Status == NPCStatus.Alive && n.Alignment == Alignment.Evil)
                    .Select(n => n.Name)
                    .ToList();
                actor.KnownFacts.Add(evilNames.Any()
                    ? $"Silver Charm: evil stirs around {string.Join(", ", evilNames.Take(2))}."
                    : "Silver Charm: no strong evil presence answered.");
                break;
            }
        }

        if (item.IsConsumedOnUse)
            game.Items.Remove(item);
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
            
            var npc = new NPC
            {
                Id = $"npc_{i + 1:D3}",
                Name = shuffledNames[i],
                Role = role,
                Alignment = alignment,
                HouseId = $"house_{i + 1:D2}",
                Status = NPCStatus.Alive,
                CurrentLocation = "VillageCenter"
            };
            _npcDecisions.InitializeNpcGoals(npc, game);
            game.NPCs.Add(npc);
        }
        
        // Add Shopkeeper
        var shopkeeper = new NPC
        {
            Id = "npc_shopkeeper",
            Name = "Tobias Reed",
            Role = RoleType.Shopkeeper,
            Alignment = Alignment.FixedNeutral,
            HouseId = "house_shop",
            Status = NPCStatus.Alive,
            CurrentLocation = "ShopkeeperHouse"
        };
        _npcDecisions.InitializeNpcGoals(shopkeeper, game);
        game.NPCs.Add(shopkeeper);
        
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
        _npcDecisions.InitializeNpcGoals(game.Player, game);
        DistributeStartingItems(game, random);
        ShareEvilRoleAwareness(game);
        
        return game;
    }

    private static void DistributeStartingItems(GameState game, Random random)
    {
        var itemTemplates = new[]
        {
            ("Magnifying Glass", "UseMagnifyingGlass", "Reads a random note from past person activity during day actions.", 4, false),
            ("Listening Conch", "UseListeningConch", "Lets the owner hear a clue about a person's recent location.", 3, false),
            ("Family Ledger", "UseFamilyLedger", "Confirms an owner's house tie and can be sold to the shopkeeper.", 3, false),
            ("Silver Charm", "UseSilverCharm", "Whispers whether evil is present in the village.", 5, false),
            ("Antique Key", "None", "A personal item useful for ownership curses or resale.", 2, false),
            ("Prayer Beads", "None", "A devotional item useful for ownership curses or resale.", 2, false),
            ("Black Candle", "None", "An evil-owned ritual token. Witch item curses do not rebound through it.", 3, true)
        };

        var people = game.NPCs.Concat(game.Player == null ? Enumerable.Empty<NPC>() : new[] { game.Player }).ToList();
        foreach (var person in people)
        {
            var template = itemTemplates[random.Next(itemTemplates.Length)];
            var evilOwned = person.Alignment == Alignment.Evil || template.Item5;
            var item = new GameItem
            {
                Id = $"item_{Guid.NewGuid():N}",
                Name = template.Item1,
                UtilityAction = template.Item2,
                Description = template.Item3,
                Value = template.Item4,
                OwnerNpcId = person.Id,
                CurrentHolderId = person.Id,
                HouseId = person.HouseId,
                IsEvilOwned = evilOwned,
                UsableByOwnerOnly = true,
                IsConsumedOnUse = false
            };

            game.Items.Add(item);
            person.Inventory.Add(item.Name.ToLowerInvariant());
        }
    }

    private static void ShareEvilRoleAwareness(GameState game)
    {
        var evils = game.NPCs
            .Concat(game.Player == null ? Enumerable.Empty<NPC>() : new[] { game.Player })
            .Where(n => n.Alignment == Alignment.Evil)
            .ToList();

        foreach (var evil in evils)
        {
            foreach (var ally in evils.Where(e => e.Id != evil.Id))
            {
                evil.KnownFacts.Add($"{ally.Name} is the {ally.Role} and serves evil.");
            }
        }
    }
}

public class CouncilPlayerActionRequest
{
    public string? TargetNpcId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ActionType { get; set; } = "accusation";
}

public class CouncilReactionRequest
{
    public string Context { get; set; } = "announcement";
    public string? TargetNpcId { get; set; }
}

public class CouncilAlibiRequest
{
    public string? TargetNpcId { get; set; }
    public string? AccusationReason { get; set; }
    public string? PlayerChoice { get; set; }
}

public class RemainSilentRequest
{
    public string TargetNpcId { get; set; } = string.Empty;
}

public class PranksterRevealRequest
{
    public string FakeRole { get; set; } = string.Empty;
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

public class PlayerCouncilStatementRequest
{
    public string Statement { get; set; } = string.Empty;
}

public class NewGameRequest
{
    public bool AutoTimeEnabled { get; set; } = false;
    public int AutoTimeIntervalSeconds { get; set; } = 5;
    public int AutoTimeIncrementMinutes { get; set; } = 30;
    public bool PauseOnCouncil { get; set; } = true;
    public bool PauseOnDeath { get; set; } = true;
    public bool PauseOnPlayerAction { get; set; } = false;
}

public class AutoTimeConfigRequest
{
    public bool? AutoTimeEnabled { get; set; }
    public int? AutoTimeIntervalSeconds { get; set; }
    public int? AutoTimeIncrementMinutes { get; set; }
    public bool? PauseOnCouncil { get; set; }
    public bool? PauseOnDeath { get; set; }
    public bool? PauseOnPlayerAction { get; set; }
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
