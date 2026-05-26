using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface IGameProgressionService
{
    void CheckWinConditions(GameState gameState);
    void UpdateFactionAlignments(GameState gameState);
    void HandleFactionShift(GameState gameState, string npcId, bool toGood);
}
