using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class SuspicionCalculator : ISuspicionCalculator
{
    private readonly Random _random = new();

    public int CalculateSuspicion(NPC observer, NPC target, GameState gameState)
    {
        var factors = new SuspicionFactors
        {
            BaseSuspicion = observer.Suspicion.GetValueOrDefault(target.Id, 0),
            WitnessEvidence = CalculateWitnessEvidence(observer, target, gameState),
            RumorWeight = CalculateRumorWeight(observer, target, gameState),
            ContradictionWeight = CalculateContradictions(observer, target),
            RoleBias = CalculateRoleBias(target),
            RngModifier = _random.Next(-10, 10)
        };

        var totalSuspicion = factors.BaseSuspicion 
            + factors.WitnessEvidence 
            + factors.RumorWeight 
            + factors.ContradictionWeight 
            + factors.RoleBias 
            + factors.RngModifier;

        return Math.Clamp(totalSuspicion, 0, 100);
    }

    private int CalculateWitnessEvidence(NPC observer, NPC target, GameState gameState)
    {
        var suspicion = 0;
        
        // Check if observer witnessed target near evidence
        var evidenceNearTarget = gameState.Evidence
            .Where(e => e.Location == target.CurrentLocation)
            .ToList();
            
        suspicion += evidenceNearTarget.Count * 15;
        
        return Math.Min(suspicion, 40);
    }

    private int CalculateRumorWeight(NPC observer, NPC target, GameState gameState)
    {
        var rumors = gameState.Rumors
            .Where(r => r.TargetNpcId == target.Id && r.KnownBy.Contains(observer.Id))
            .ToList();
            
        var weight = 0;
        foreach (var rumor in rumors)
        {
            weight += (rumor.Truthfulness / 10) + (rumor.SpreadRate / 20);
        }
        
        return Math.Min(weight, 30);
    }

    private int CalculateContradictions(NPC observer, NPC target)
    {
        // Check for contradictory statements in known facts
        var contradictions = 0;
        
        // Simplified: check if target has conflicting location claims
        var locationClaims = target.KnownFacts
            .Where(f => f.Contains("was at"))
            .ToList();
            
        if (locationClaims.Count > 2)
        {
            contradictions = 20;
        }
        
        return contradictions;
    }

    private int CalculateRoleBias(NPC target)
    {
        return target.Role switch
        {
            RoleType.Vagabond => 25, // Always suspicious
            RoleType.Prankster => 15,
            RoleType.Butcher => 10,
            RoleType.Detective => -5, // Less suspicious
            _ => 0
        };
    }

    public void UpdateSuspicionFromEvidence(GameState gameState, Evidence evidence)
    {
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            // NPCs near evidence location become suspicious
            var nearbyNpcs = gameState.NPCs
                .Where(n => n.CurrentLocation == evidence.Location && n.Status == NPCStatus.Alive)
                .ToList();
                
            foreach (var suspect in nearbyNpcs)
            {
                if (npc.Id == suspect.Id)
                    continue;

                if (!npc.Suspicion.ContainsKey(suspect.Id))
                    npc.Suspicion[suspect.Id] = 0;
                    
                npc.Suspicion[suspect.Id] = Math.Clamp(npc.Suspicion[suspect.Id] + evidence.Visibility / 10, 0, 100);
            }
        }
    }

    public void UpdateSuspicionFromRumor(GameState gameState, Rumor rumor)
    {
        foreach (var npcId in rumor.KnownBy)
        {
            var npc = gameState.NPCs.FirstOrDefault(n => n.Id == npcId);
            if (npc == null) continue;
            
            if (npc.Id == rumor.TargetNpcId)
                continue;

            if (!npc.Suspicion.ContainsKey(rumor.TargetNpcId))
                npc.Suspicion[rumor.TargetNpcId] = 0;
                
            npc.Suspicion[rumor.TargetNpcId] = Math.Clamp(npc.Suspicion[rumor.TargetNpcId] + rumor.Truthfulness / 10, 0, 100);
        }
    }

    public Dictionary<string, int> GetPublicSuspicionRankings(GameState gameState)
    {
        var rankings = new Dictionary<string, int>();
        
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            var totalSuspicion = 0;
            var count = 0;
            
            foreach (var otherNpc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != npc.Id))
            {
                if (otherNpc.Suspicion.TryGetValue(npc.Id, out var suspicion))
                {
                    totalSuspicion += suspicion;
                    count++;
                }
            }
            
            rankings[npc.Id] = count > 0 ? totalSuspicion / count : 0;
        }
        
        return rankings.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
