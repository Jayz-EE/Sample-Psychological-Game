using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

public interface ICouncilService
{
    Task<CouncilSession> StartCouncilSession(GameState gameState);
    void ProcessAccusation(GameState gameState, string sourceNpcId, string targetNpcId, string reason);
    void ProcessVote(GameState gameState, string voterNpcId, string targetNpcId);
    CouncilOutcome ResolveCouncil(GameState gameState, CouncilSession session);
}

public class CouncilSession
{
    public int Day { get; set; }
    public List<CouncilStatement> Statements { get; set; } = new();
    public List<Accusation> Accusations { get; set; } = new();
    public bool VotingPhase { get; set; }
}

public class CouncilStatement
{
    public string NpcId { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class CouncilOutcome
{
    public string? ExecutedNpcId { get; set; }
    public Dictionary<string, int> SuspicionChanges { get; set; } = new();
    public List<string> NewAlliances { get; set; } = new();
}
