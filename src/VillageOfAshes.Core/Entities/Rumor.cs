namespace VillageOfAshes.Core.Entities;

public class Rumor
{
    public string Id { get; set; } = string.Empty;
    public string SourceNpcId { get; set; } = string.Empty;
    public string TargetNpcId { get; set; } = string.Empty;
    public int Truthfulness { get; set; } // 0-100
    public int SpreadRate { get; set; } // 0-100
    public string Context { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> KnownBy { get; set; } = new(); // NPC IDs who know this rumor
}
