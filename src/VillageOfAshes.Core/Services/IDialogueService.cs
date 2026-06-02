using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Services;

public interface IDialogueService
{
    DialogueExchange GenerateDialogue(NPC npc, GameState gameState, DialogueContext context);
    void ApplyDialogueEffects(GameState gameState, string npcId, DialogueOption selectedOption);
    List<Entities.Dialogue> GetAvailableDialogues(NPC npc, GameState gameState);
    bool EvaluateConditions(List<string> conditions, NPC npc, GameState gameState);
}
