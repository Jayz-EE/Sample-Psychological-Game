using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface INightSimulationService
{
    Task<NightSimulationResult> ExecuteNightPhase(GameState gameState);
}

public class NightSimulationResult
{
    public List<string> Events { get; set; } = new();
    public List<Evidence> GeneratedEvidence { get; set; } = new();
    public List<Rumor> GeneratedRumors { get; set; } = new();
    public List<string> Deaths { get; set; } = new();
    public Dictionary<string, string> NPCMovements { get; set; } = new();
}
