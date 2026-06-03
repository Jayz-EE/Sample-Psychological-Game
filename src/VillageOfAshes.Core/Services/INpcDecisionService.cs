using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Services;

public enum NpcTargetIntent
{
    Attack,
    Protect,
    Accuse,
    Spy,
    Befriend,
    Frame
}

public interface INpcDecisionService
{
    NPC? ChooseTarget(GameState game, NPC actor, NpcTargetIntent intent);
    DialogueContext ResolveDialogueContext(NPC npc, GameState game, string observerId = "player");
    void RefreshNpcSuspicion(GameState game, NPC observer, NPC target);
    void RefreshAllNpcSuspicions(GameState game);
    string GenerateCouncilReaction(NPC npc, GameState game, string trigger, string? targetNpcId = null);
    string GenerateAlibiLine(NPC npc, GameState game, string accusationReason);
    string? ChooseDayAction(NPC npc, GameState game);
    string? GenerateDynamicDialogueLine(NPC npc, GameState game, DialogueContext context);
    void InitializeNpcGoals(NPC npc, GameState game);
    void AnalyzeStatement(GameState game, string speakerId, string statement);
}
