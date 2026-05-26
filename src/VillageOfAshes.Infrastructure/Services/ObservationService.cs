using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class ObservationService : IObservationService
{
    private readonly Random _random = new();

    public List<Observation> GenerateNightObservations(GameState gameState)
    {
        var observations = new List<Observation>();
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();

        // NPCs who are awake at night might observe others
        foreach (var observer in aliveNpcs)
        {
            // Chance to observe based on role and random factor
            if (_random.Next(100) < 40) // 40% chance to observe something
            {
                // Find potential targets in same or nearby locations
                var potentialTargets = aliveNpcs
                    .Where(n => n.Id != observer.Id && n.CurrentLocation == observer.CurrentLocation)
                    .ToList();

                if (potentialTargets.Any())
                {
                    var target = potentialTargets[_random.Next(potentialTargets.Count)];
                    
                    var observation = new Observation
                    {
                        Id = Guid.NewGuid().ToString(),
                        ObserverId = observer.Id,
                        TargetId = target.Id,
                        Location = target.CurrentLocation,
                        Description = GenerateAmbiguousDescription(target, observer),
                        Timestamp = DateTime.UtcNow,
                        Reliability = CalculateReliability(observer),
                        Shared = false,
                        SharedWith = new List<string>()
                    };

                    observations.Add(observation);
                    
                    // Add to observer's known facts
                    observer.KnownFacts.Add($"Saw {target.Name} at {target.CurrentLocation} during the night");
                }
            }
        }

        return observations;
    }

    public List<Observation> GetObservationsByObserver(GameState gameState, string observerId)
    {
        return gameState.NPCs
            .Where(n => n.Id == observerId)
            .SelectMany(n => n.KnownFacts)
            .Select(fact => new Observation
            {
                ObserverId = observerId,
                Description = fact
            })
            .ToList();
    }

    public List<Observation> GetObservationsAboutTarget(GameState gameState, string targetId)
    {
        var observations = new List<Observation>();
        
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            var relevantFacts = npc.KnownFacts
                .Where(fact => fact.Contains(gameState.NPCs.FirstOrDefault(n => n.Id == targetId)?.Name ?? ""))
                .ToList();

            observations.AddRange(relevantFacts.Select(fact => new Observation
            {
                ObserverId = npc.Id,
                TargetId = targetId,
                Description = fact
            }));
        }

        return observations;
    }

    public void ShareObservation(GameState gameState, Observation observation, string shareWithNpcId)
    {
        var recipient = gameState.NPCs.FirstOrDefault(n => n.Id == shareWithNpcId);
        if (recipient == null) return;

        observation.SharedWith.Add(shareWithNpcId);
        observation.Shared = true;

        // Recipient now knows this information
        recipient.KnownFacts.Add($"Heard that {observation.Description}");

        // Might generate a rumor
        if (_random.Next(100) < 30)
        {
            var target = gameState.NPCs.FirstOrDefault(n => n.Id == observation.TargetId);
            if (target != null)
            {
                var rumor = new Rumor
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceNpcId = shareWithNpcId,
                    TargetNpcId = observation.TargetId,
                    Context = observation.Description,
                    Truthfulness = observation.Reliability,
                    SpreadRate = 50,
                    CreatedAt = DateTime.UtcNow,
                    KnownBy = new List<string> { shareWithNpcId }
                };
                gameState.Rumors.Add(rumor);
            }
        }
    }

    public int CalculateObservationSuspicion(Observation observation, GameState gameState)
    {
        var suspicion = 0;

        // Base suspicion for being seen at night
        suspicion += 10;

        // Higher suspicion if near evidence
        var evidenceNearby = gameState.Evidence
            .Any(e => e.Location == observation.Location && 
                     e.CreatedAt > observation.Timestamp.AddHours(-2));
        
        if (evidenceNearby)
            suspicion += 20;

        // Reliability affects suspicion
        suspicion = (int)(suspicion * (observation.Reliability / 100.0));

        return Math.Clamp(suspicion, 0, 50);
    }

    private string GenerateAmbiguousDescription(NPC target, NPC observer)
    {
        var descriptions = new[]
        {
            $"{target.Name} moving around suspiciously",
            $"{target.Name} near a house",
            $"{target.Name} carrying something",
            $"{target.Name} acting nervous",
            $"{target.Name} looking around cautiously",
            $"{target.Name} in an unusual location",
            $"{target.Name} meeting with someone",
            $"{target.Name} leaving in a hurry",
            $"{target.Name} behaving oddly"
        };

        return descriptions[_random.Next(descriptions.Length)];
    }

    private int CalculateReliability(NPC observer)
    {
        // Base reliability
        var reliability = 60;

        // Detective has higher reliability
        if (observer.Role == RoleType.Detective)
            reliability += 20;

        // Random factor
        reliability += _random.Next(-10, 10);

        return Math.Clamp(reliability, 30, 95);
    }
}
