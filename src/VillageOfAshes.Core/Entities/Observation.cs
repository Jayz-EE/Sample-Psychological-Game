namespace VillageOfAshes.Core.Entities;

/// <summary>
/// Represents an observation made by an NPC about another NPC or event
/// Used to track what NPCs have witnessed without revealing their roles
/// </summary>
public class Observation
{
    public string Id { get; set; } = string.Empty;
    public string ObserverId { get; set; } = string.Empty; // Who saw it
    public string TargetId { get; set; } = string.Empty; // Who/what was observed
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Ambiguous description
    public DateTime Timestamp { get; set; }
    public int Reliability { get; set; } // 0-100, how reliable is this observation
    public bool Shared { get; set; } // Has this been shared with others
    public List<string> SharedWith { get; set; } = new(); // Who knows about this
}

/// <summary>
/// Tracks relationships between NPCs for alliance and betrayal mechanics
/// </summary>
public class Relationship
{
    public string NpcId1 { get; set; } = string.Empty;
    public string NpcId2 { get; set; } = string.Empty;
    public int TrustLevel { get; set; } = 50; // 0-100
    public int SuspicionLevel { get; set; } = 0; // 0-100
    public int FearLevel { get; set; } = 0; // 0-100
    public bool IsAllied { get; set; }
    public DateTime LastInteraction { get; set; }
    public List<string> SharedSecrets { get; set; } = new(); // Information shared between them
}

/// <summary>
/// Represents a secret that NPCs can discover and share
/// </summary>
public class Secret
{
    public string Id { get; set; } = string.Empty;
    public string AboutNpcId { get; set; } = string.Empty; // Who the secret is about
    public string Content { get; set; } = string.Empty; // The secret itself
    public int Severity { get; set; } = 0; // 0-100, how damaging is this
    public bool RoleRevealing { get; set; } // Does this hint at their role
    public List<string> KnownBy { get; set; } = new(); // Who knows this secret
    public DateTime DiscoveredAt { get; set; }
}

/// <summary>
/// Tracks behavioral patterns that can be analyzed
/// </summary>
public class BehaviorPattern
{
    public string NpcId { get; set; } = string.Empty;
    public Dictionary<string, int> NightLocations { get; set; } = new(); // Location -> frequency
    public Dictionary<string, int> DayActivities { get; set; } = new(); // Activity -> frequency
    public int TimesSeenAtNight { get; set; }
    public int TimesAvoidedCouncil { get; set; }
    public int TimesDefensive { get; set; }
    public int TimesHelpful { get; set; }
    public List<string> SuspiciousActions { get; set; } = new();
}
