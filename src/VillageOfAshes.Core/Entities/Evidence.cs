using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Entities;

public class Evidence
{
    public string Id { get; set; } = string.Empty;
    public EvidenceType Type { get; set; }
    public string Location { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public int Visibility { get; set; } // 0-100
    public int DecayTime { get; set; } // Days until disappears
    public RoleType? LinkedRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
