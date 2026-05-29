using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface IDaySimulationService
{
    DaySimulationResult ExecuteDayPhase(GameState gameState);
}

public class DaySimulationResult
{
    public List<string> Events { get; set; } = new();
    public List<Rumor> GeneratedRumors { get; set; } = new();
}
