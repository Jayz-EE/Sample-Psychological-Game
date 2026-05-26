using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface ISuspicionCalculator
{
    int CalculateSuspicion(NPC observer, NPC target, GameState gameState);
    void UpdateSuspicionFromEvidence(GameState gameState, Evidence evidence);
    void UpdateSuspicionFromRumor(GameState gameState, Rumor rumor);
    Dictionary<string, int> GetPublicSuspicionRankings(GameState gameState);
}

public class SuspicionFactors
{
    public int BaseSuspicion { get; set; }
    public int WitnessEvidence { get; set; }
    public int RumorWeight { get; set; }
    public int ContradictionWeight { get; set; }
    public int RoleBias { get; set; }
    public int RngModifier { get; set; }
}
