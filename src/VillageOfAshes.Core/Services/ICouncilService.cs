using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Services;

public interface ICouncilService
{
    Task<CouncilSession> StartCouncilSession(GameState gameState);
    void ProcessAccusation(GameState gameState, string sourceNpcId, string targetNpcId, string reason);
    void ProcessVote(GameState gameState, string voterNpcId, string targetNpcId);
    void StartVoting(GameState gameState, CouncilSession session);
    CouncilOutcome ResolveCouncil(GameState gameState, CouncilSession session);
}

public class CouncilSession
{
    public int Day { get; set; }
    public List<CouncilStatement> Statements { get; set; } = new();
    public List<Accusation> Accusations { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
    public bool VotingPhase { get; set; }
    public bool Resolved { get; set; }
    public string? BurnedNpcId { get; set; }
    public RoleType? RevealedRole { get; set; }
    public bool RoleRevealTampered { get; set; }
    
    // Social Context: Tracks the flow of conversation for smarter NPC reactions
    public List<CouncilStatement> RecentDiscourse { get; set; } = new();
}

public class CouncilStatement
{
    public string NpcId { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? TargetNpcId { get; set; } // Who is this statement directed at?
}


public class CouncilOutcome
{
    public string? ExecutedNpcId { get; set; }
    public RoleType? RevealedRole { get; set; }
    public bool RoleRevealTampered { get; set; }
    public Dictionary<string, int> SuspicionChanges { get; set; } = new();
    public List<string> NewAlliances { get; set; } = new();
}
