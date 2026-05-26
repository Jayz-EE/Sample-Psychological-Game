using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface IRumorService
{
    Rumor GenerateRumor(string sourceNpcId, string targetNpcId, string context, int truthfulness);
    void SpreadRumor(GameState gameState, Rumor rumor);
    List<Rumor> GetRumorsAbout(GameState gameState, string targetNpcId);
    List<Rumor> GetRumorsKnownBy(GameState gameState, string npcId);
}
