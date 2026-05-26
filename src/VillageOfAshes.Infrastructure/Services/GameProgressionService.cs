using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class GameProgressionService : IGameProgressionService
{
    public void CheckWinConditions(GameState gameState)
    {
        if (gameState.Status != GameStatus.InProgress) return;

        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();
        var goodNpcs = aliveNpcs.Where(n => IsGood(n.Alignment)).ToList();
        var evilNpcs = aliveNpcs.Where(n => IsEvil(n.Alignment)).ToList();

        // Check if player is alive
        if (gameState.Player != null && gameState.Player.Status != NPCStatus.Alive)
        {
            gameState.Status = GameStatus.GameOver;
            gameState.WinMessage = "You have perished. The village's story ends here for you.";
            return;
        }

        // Evil Win: All Good NPCs are dead
        if (goodNpcs.Count == 0 && (gameState.Player == null || !IsGood(gameState.Player.Alignment)))
        {
            gameState.Status = GameStatus.EvilWin;
            gameState.WinMessage = "The darkness has fully consumed the village. No light remains.";
            return;
        }

        // Good Win: All Evil NPCs are dead
        if (evilNpcs.Count == 0 && (gameState.Player == null || !IsEvil(gameState.Player.Alignment)))
        {
            gameState.Status = GameStatus.GoodWin;
            gameState.WinMessage = "The evil has been eradicated. The village can finally begin to heal.";
            return;
        }
        
        // Special case: If player is the only one left and is Evil/Good
        if (aliveNpcs.Count == 0 && gameState.Player != null)
        {
             if (IsEvil(gameState.Player.Alignment))
             {
                 gameState.Status = GameStatus.EvilWin;
                 gameState.WinMessage = "You are the last shadow remaining. The village is yours.";
             }
             else if (IsGood(gameState.Player.Alignment))
             {
                 gameState.Status = GameStatus.GoodWin;
                 gameState.WinMessage = "You have survived the nightmare. The village is safe, but at a terrible cost.";
             }
        }
    }

    public void UpdateFactionAlignments(GameState gameState)
    {
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();
        
        foreach (var npc in aliveNpcs)
        {
            if (!IsNeutral(npc.Alignment)) continue;
            if (npc.Role == RoleType.Shopkeeper) continue; // Shopkeeper is fixed

            // Check trust for player if player is non-neutral
            if (gameState.Player != null && !IsNeutral(gameState.Player.Alignment))
            {
                var playerTrust = npc.Trust.GetValueOrDefault("player", 50);
                if (playerTrust > 80)
                {
                    HandleFactionShift(gameState, npc.Id, IsGood(gameState.Player.Alignment));
                    continue;
                }
            }

            // Check trust for other NPCs
            foreach (var other in aliveNpcs)
            {
                if (other.Id == npc.Id || IsNeutral(other.Alignment)) continue;

                var trust = npc.Trust.GetValueOrDefault(other.Id, 50);
                if (trust > 85)
                {
                    HandleFactionShift(gameState, npc.Id, IsGood(other.Alignment));
                    break;
                }
            }
        }
    }

    public void HandleFactionShift(GameState gameState, string npcId, bool toGood)
    {
        var npc = npcId == "player" ? gameState.Player : gameState.NPCs.FirstOrDefault(n => n.Id == npcId);
        if (npc == null) return;

        // Only neutrals can shift
        if (!IsNeutral(npc.Alignment)) return;

        if (toGood)
        {
            npc.Alignment = Alignment.Good;
        }
        else
        {
            npc.Alignment = Alignment.Evil;
        }
        
        // Check win conditions again after faction shift
        CheckWinConditions(gameState);
    }

    private bool IsGood(Alignment alignment) => alignment == Alignment.Good || alignment == Alignment.GoodNeutral;
    private bool IsEvil(Alignment alignment) => alignment == Alignment.Evil || alignment == Alignment.EvilNeutral;
    private bool IsNeutral(Alignment alignment) => alignment == Alignment.Neutral || alignment == Alignment.GoodNeutral || alignment == Alignment.EvilNeutral;
}
