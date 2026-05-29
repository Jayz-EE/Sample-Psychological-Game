using Microsoft.AspNetCore.Mvc;
using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DialogueController : ControllerBase
{
    private readonly IDialogueService _dialogueService;
    private readonly INpcDecisionService _npcDecisions;
    
    // This should be replaced with proper state management (database, cache, etc.)
    private static GameState? _sharedGameState;
    private static readonly Dictionary<string, DialogueExchange> _activeExchanges = new();

    public DialogueController(IDialogueService dialogueService, INpcDecisionService npcDecisions)
    {
        _dialogueService = dialogueService;
        _npcDecisions = npcDecisions;
    }
    
    // Helper method to sync game state between controllers
    internal static void SetGameState(GameState gameState)
    {
        if (_sharedGameState?.Id != gameState.Id)
            _activeExchanges.Clear();

        _sharedGameState = gameState;
    }

    [HttpGet("npc/{npcId}")]
    public ActionResult<DialogueExchange> GetDialogueOptions(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");
            
        var npc = _sharedGameState.NPCs.FirstOrDefault(n => n.Id == npcId);
        if (npc == null)
            return NotFound("NPC not found");
            
        // Determine context based on NPC's state
        var context = DetermineDialogueContext(npc, _sharedGameState);
        var exchange = _dialogueService.GenerateDialogue(npc, _sharedGameState, context);
        _activeExchanges[npc.Id] = exchange;
        
        return Ok(exchange);
    }

    [HttpPost("respond")]
    public ActionResult<object> RespondToDialogue([FromBody] DialogueResponse response)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");
            
        var npc = _sharedGameState.NPCs.FirstOrDefault(n => n.Id == response.NpcId);
        if (npc == null)
            return NotFound("NPC not found");
            
        if (!_activeExchanges.TryGetValue(response.NpcId, out var exchange))
            return BadRequest("No active dialogue exchange for this NPC");

        var option = exchange.Options.FirstOrDefault(o => o.Id == response.SelectedOptionId);
        if (option == null)
            return BadRequest("Dialogue option not found");
        
        _dialogueService.ApplyDialogueEffects(_sharedGameState, response.NpcId, option);
        _activeExchanges.Remove(response.NpcId);
        
        return Ok(new {
            npcResponse = option.NpcResponse,
            effects = option.Effects
        });
    }
    
    private DialogueContext DetermineDialogueContext(NPC npc, GameState gameState) =>
        _npcDecisions.ResolveDialogueContext(npc, gameState, "player");
}

public class DialogueResponse
{
    public string NpcId { get; set; } = string.Empty;
    public string SelectedOptionId { get; set; } = string.Empty;
}
