using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Entities;

public class Dialogue
{
    public string Id { get; set; } = string.Empty;
    public DialogueContext Context { get; set; }
    public RoleType? SpeakerRole { get; set; }
    public string Emotion { get; set; } = string.Empty;
    public List<string> Conditions { get; set; } = new();
    public List<string> Lines { get; set; } = new();
    public DialogueEffects Effects { get; set; } = new();
}

public class DialogueEffects
{
    public int Trust { get; set; }
    public int Suspicion { get; set; }
    public int Fear { get; set; }
    public bool SpreadRumor { get; set; }
}

public class DialogueOption
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string NpcResponse { get; set; } = string.Empty;
    public DialogueEffects Effects { get; set; } = new();
    public List<string> Conditions { get; set; } = new();
}

public class DialogueExchange
{
    public string Id { get; set; } = string.Empty;
    public string NpcId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public List<DialogueOption> Options { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
