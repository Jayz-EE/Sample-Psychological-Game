using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Services;

public interface ITimeManager
{
    GamePhase GetCurrentPhase(TimeSpan currentTime);
    TimeSpan AdvanceTime(TimeSpan currentTime, int minutes);
    bool ShouldTransitionPhase(TimeSpan currentTime, GamePhase currentPhase);
    GamePhase GetNextPhase(GamePhase currentPhase);
}
