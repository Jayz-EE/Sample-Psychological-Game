using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Entities;

public class NPC
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public RoleType Role { get; set; }
    public Alignment Alignment { get; set; }
    public string HouseId { get; set; } = string.Empty;
    public NPCStatus Status { get; set; } = NPCStatus.Alive;
    
    // Social dynamics
    public Dictionary<string, int> Trust { get; set; } = new();
    public Dictionary<string, int> Suspicion { get; set; } = new();
    public Dictionary<string, int> Fear { get; set; } = new();
    
    // Knowledge and memory
    public List<string> KnownFacts { get; set; } = new();
    public List<Rumor> Rumors { get; set; } = new();
    public List<string> Goals { get; set; } = new();
    public bool IsGoalCompleted { get; set; } = false;
    
    // Inventory and actions
    public List<string> Inventory { get; set; } = new();
    public List<ScheduleEntry> DailySchedule { get; set; } = new();
    public List<string> NightActions { get; set; } = new();
    public List<string> BehaviorFlags { get; set; } = new();
    public bool IsCursed { get; set; } = false;
    public bool IsIll { get; set; } = false;
    public int IllnessSuppressedUntilDay { get; set; } = 0;
    public string? CurseSourceItemId { get; set; }
    public RoleType? RevealedRole { get; set; }
    public bool RoleRevealTampered { get; set; }
    public int PranksterRoleChangesUsed { get; set; } = 0;
    
    // Stats
    public int Health { get; set; } = 100;
    public int Hunger { get; set; } = 0;
    public int PhaseActionCount { get; set; } = 0;
    public string CurrentLocation { get; set; } = string.Empty;
    public int IsDisabledUntilDay { get; set; } = 0;
    public string? TrappedByNpcId { get; set; }
}

public class ScheduleEntry
{
    public TimeSpan Time { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
