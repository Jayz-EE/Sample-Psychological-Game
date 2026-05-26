using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class TimeManager : ITimeManager
{
    public GamePhase GetCurrentPhase(TimeSpan currentTime)
    {
        var hour = currentTime.Hours;
        
        if (hour >= 21 || hour < 5)
            return GamePhase.NightSimulation;
        if (hour >= 5 && hour < 7)
            return GamePhase.MorningDiscovery;
        if (hour >= 7 && hour < 8)
            return GamePhase.VillageCouncil;
        if (hour >= 8 && hour < 18)
            return GamePhase.DayActions;
        if (hour >= 18 && hour < 21)
            return GamePhase.Evening;
            
        return GamePhase.DayActions;
    }

    public TimeSpan AdvanceTime(TimeSpan currentTime, int minutes)
    {
        var totalMinutes = ((int)currentTime.TotalMinutes + minutes) % (24 * 60);
        if (totalMinutes < 0)
            totalMinutes += 24 * 60;

        return TimeSpan.FromMinutes(totalMinutes);
    }

    public bool ShouldTransitionPhase(TimeSpan currentTime, GamePhase currentPhase)
    {
        var actualPhase = GetCurrentPhase(currentTime);
        return actualPhase != currentPhase;
    }

    public GamePhase GetNextPhase(GamePhase currentPhase)
    {
        return currentPhase switch
        {
            GamePhase.NightSimulation => GamePhase.MorningDiscovery,
            GamePhase.MorningDiscovery => GamePhase.VillageCouncil,
            GamePhase.VillageCouncil => GamePhase.DayActions,
            GamePhase.DayActions => GamePhase.Evening,
            GamePhase.Evening => GamePhase.NightSimulation,
            _ => GamePhase.MorningDiscovery
        };
    }
}
