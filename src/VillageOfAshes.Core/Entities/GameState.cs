using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Core.Entities;

public class GameState
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int CurrentDay { get; set; } = 1;
    public TimeSpan CurrentTime { get; set; } = new TimeSpan(6, 0, 0); // Start at 6:00 AM
    public GamePhase CurrentPhase { get; set; } = GamePhase.MorningDiscovery;
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public string WinMessage { get; set; } = string.Empty;
    
    public List<NPC> NPCs { get; set; } = new();
    public NPC? Player { get; set; }
    public List<Evidence> Evidence { get; set; } = new();
    public List<Rumor> Rumors { get; set; } = new();
    public List<GameItem> Items { get; set; } = new();
    public List<CouncilRecord> CouncilHistory { get; set; } = new();
    public List<ConversationLog> ConversationLogs { get; set; } = new();
    public List<string> RecentEvents { get; set; } = new();
    public List<string> PlayerNotifications { get; set; } = new();
    public CouncilSession? ActiveCouncil { get; set; }
    public string? PendingPranksterRevealNpcId { get; set; }
    
    public Dictionary<string, int> VillageResources { get; set; } = new();
    public int VillageFear { get; set; } = 10;
    public int VillageCorruption { get; set; } = 0;
    public int FoodSupply { get; set; } = 30;
    public int EconomyStability { get; set; } = 80;
    public bool BlackMarketActive { get; set; } = false;
    public bool ShopkeeperAlive { get; set; } = true;
    public int ShopkeeperProtectionDays { get; set; } = 7;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    // Automated Time Progression Settings
    public bool AutoTimeEnabled { get; set; } = false;
    public int AutoTimeIntervalSeconds { get; set; } = 5; // Real-world seconds between auto-advances
    public int AutoTimeIncrementMinutes { get; set; } = 30; // Game minutes to advance each tick
    public DateTime LastAutoAdvance { get; set; } = DateTime.UtcNow;
    public bool PauseOnCouncil { get; set; } = true; // Pause automation during council
    public bool PauseOnDeath { get; set; } = true; // Pause when someone dies
    public bool PauseOnPlayerAction { get; set; } = false; // Pause when player takes action
}

public class GameItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UtilityAction { get; set; } = string.Empty;
    public string OwnerNpcId { get; set; } = string.Empty;
    public string CurrentHolderId { get; set; } = string.Empty;
    public string HouseId { get; set; } = string.Empty;
    public int Value { get; set; } = 2;
    public bool IsEvilOwned { get; set; }
    public bool UsableByOwnerOnly { get; set; } = true;
    public bool IsConsumedOnUse { get; set; }
}

public class CouncilRecord
{
    public int Day { get; set; }
    public List<Accusation> Accusations { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
    public Dictionary<string, int> PublicSuspicion { get; set; } = new();
    public string? BurnedNpcId { get; set; }
    public RoleType? RevealedRole { get; set; }
    public bool RoleRevealTampered { get; set; }
}

public class Accusation
{
    public string SourceNpcId { get; set; } = string.Empty;
    public string TargetNpcId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
}

public class Vote
{
    public string VoterNpcId { get; set; } = string.Empty;
    public string TargetNpcId { get; set; } = string.Empty;
}

public class ConversationLog
{
    public string Id { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public List<string> Participants { get; set; } = new();
    public string Context { get; set; } = string.Empty;
    public List<DialogueLine> Dialogue { get; set; } = new();
    public DialogueEffects Effects { get; set; } = new();
}

public class DialogueLine
{
    public string Speaker { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
}
