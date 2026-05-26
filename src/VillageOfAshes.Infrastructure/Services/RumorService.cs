using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class RumorService : IRumorService
{
    private readonly Random _random = new();

    public Rumor GenerateRumor(string sourceNpcId, string targetNpcId, string context, int truthfulness)
    {
        return new Rumor
        {
            Id = Guid.NewGuid().ToString(),
            SourceNpcId = sourceNpcId,
            TargetNpcId = targetNpcId,
            Context = context,
            Truthfulness = Math.Clamp(truthfulness, 0, 100),
            SpreadRate = _random.Next(20, 70),
            CreatedAt = DateTime.UtcNow,
            KnownBy = new List<string> { sourceNpcId }
        };
    }

    public void SpreadRumor(GameState gameState, Rumor rumor)
    {
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();

        foreach (var npc in aliveNpcs)
        {
            // Skip if NPC already knows the rumor
            if (rumor.KnownBy.Contains(npc.Id))
                continue;

            // Calculate spread chance based on spread rate and social connections
            var spreadChance = rumor.SpreadRate;

            // Increase chance if NPC trusts the source
            var sourceNpc = gameState.NPCs.FirstOrDefault(n => n.Id == rumor.SourceNpcId);
            if (sourceNpc != null && npc.Trust.GetValueOrDefault(rumor.SourceNpcId, 0) > 50)
            {
                spreadChance += 20;
            }

            // Decrease chance if NPC is isolated
            if (npc.BehaviorFlags.Contains("Isolated"))
            {
                spreadChance -= 30;
            }

            if (_random.Next(100) < spreadChance)
            {
                rumor.KnownBy.Add(npc.Id);
                npc.Rumors.Add(rumor);

                // Update suspicion based on rumor
                if (!npc.Suspicion.ContainsKey(rumor.TargetNpcId))
                    npc.Suspicion[rumor.TargetNpcId] = 0;

                npc.Suspicion[rumor.TargetNpcId] += rumor.Truthfulness / 10;
            }
        }
    }

    public List<Rumor> GetRumorsAbout(GameState gameState, string targetNpcId)
    {
        return gameState.Rumors
            .Where(r => r.TargetNpcId == targetNpcId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public List<Rumor> GetRumorsKnownBy(GameState gameState, string npcId)
    {
        return gameState.Rumors
            .Where(r => r.KnownBy.Contains(npcId))
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }
}
