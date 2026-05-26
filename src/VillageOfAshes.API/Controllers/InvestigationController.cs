using Microsoft.AspNetCore.Mvc;
using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestigationController : ControllerBase
{
    private readonly IObservationService _observationService;
    private readonly IBehaviorAnalysisService _behaviorAnalysisService;
    
    // Shared game state (should be replaced with proper state management)
    private static GameState? _sharedGameState;

    public InvestigationController(
        IObservationService observationService,
        IBehaviorAnalysisService behaviorAnalysisService)
    {
        _observationService = observationService;
        _behaviorAnalysisService = behaviorAnalysisService;
    }
    
    internal static void SetGameState(GameState gameState)
    {
        _sharedGameState = gameState;
    }

    /// <summary>
    /// Get all observations about a specific NPC
    /// </summary>
    [HttpGet("observations/about/{npcId}")]
    public ActionResult<List<Observation>> GetObservationsAbout(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var observations = _observationService.GetObservationsAboutTarget(_sharedGameState, npcId);
        return Ok(observations);
    }

    /// <summary>
    /// Get all observations made by a specific NPC
    /// </summary>
    [HttpGet("observations/by/{npcId}")]
    public ActionResult<List<Observation>> GetObservationsBy(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var observations = _observationService.GetObservationsByObserver(_sharedGameState, npcId);
        return Ok(observations);
    }

    /// <summary>
    /// Analyze behavior pattern of an NPC
    /// </summary>
    [HttpGet("behavior/{npcId}")]
    public ActionResult<BehaviorPattern> GetBehaviorPattern(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var pattern = _behaviorAnalysisService.GetBehaviorPattern(_sharedGameState, npcId);
        return Ok(pattern);
    }

    /// <summary>
    /// Get suspicious behavior analysis for an NPC
    /// </summary>
    [HttpGet("suspicious/{npcId}")]
    public ActionResult<List<string>> GetSuspiciousBehavior(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var findings = _behaviorAnalysisService.AnalyzeSuspiciousBehavior(_sharedGameState, npcId);
        return Ok(findings);
    }

    /// <summary>
    /// Predict role based on behavior pattern
    /// </summary>
    [HttpGet("predict-role/{npcId}")]
    public ActionResult<Dictionary<string, int>> PredictRole(string npcId)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var pattern = _behaviorAnalysisService.GetBehaviorPattern(_sharedGameState, npcId);
        var predictions = _behaviorAnalysisService.PredictRoleFromBehavior(pattern);
        
        return Ok(predictions);
    }

    /// <summary>
    /// Compare behavior patterns between two NPCs
    /// </summary>
    [HttpGet("compare/{npcId1}/{npcId2}")]
    public ActionResult<ComparisonResult> CompareBehavior(string npcId1, string npcId2)
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var pattern1 = _behaviorAnalysisService.GetBehaviorPattern(_sharedGameState, npcId1);
        var pattern2 = _behaviorAnalysisService.GetBehaviorPattern(_sharedGameState, npcId2);
        
        var similarity = _behaviorAnalysisService.CalculateBehaviorSimilarity(pattern1, pattern2);
        
        var npc1 = _sharedGameState.NPCs.FirstOrDefault(n => n.Id == npcId1);
        var npc2 = _sharedGameState.NPCs.FirstOrDefault(n => n.Id == npcId2);

        return Ok(new ComparisonResult
        {
            Npc1Name = npc1?.Name ?? "Unknown",
            Npc2Name = npc2?.Name ?? "Unknown",
            SimilarityScore = similarity,
            Interpretation = GetSimilarityInterpretation(similarity)
        });
    }

    /// <summary>
    /// Get investigation summary for all NPCs
    /// </summary>
    [HttpGet("summary")]
    public ActionResult<InvestigationSummary> GetInvestigationSummary()
    {
        if (_sharedGameState == null)
            return NotFound("No active game");

        var summary = new InvestigationSummary
        {
            TotalObservations = _sharedGameState.NPCs
                .Sum(n => n.KnownFacts.Count),
            TotalRumors = _sharedGameState.Rumors.Count,
            TotalEvidence = _sharedGameState.Evidence.Count,
            MostSuspiciousNpcs = GetMostSuspiciousNpcs(),
            MostTrustedNpcs = GetMostTrustedNpcs()
        };

        return Ok(summary);
    }

    private List<SuspicionRanking> GetMostSuspiciousNpcs()
    {
        if (_sharedGameState == null) return new List<SuspicionRanking>();

        var rankings = new List<SuspicionRanking>();

        foreach (var npc in _sharedGameState.NPCs.Where(n => n.Status == Core.Enums.NPCStatus.Alive))
        {
            var totalSuspicion = 0;
            var count = 0;

            foreach (var otherNpc in _sharedGameState.NPCs.Where(n => n.Status == Core.Enums.NPCStatus.Alive && n.Id != npc.Id))
            {
                if (otherNpc.Suspicion.TryGetValue(npc.Id, out var suspicion))
                {
                    totalSuspicion += suspicion;
                    count++;
                }
            }

            var avgSuspicion = count > 0 ? totalSuspicion / count : 0;
            
            rankings.Add(new SuspicionRanking
            {
                NpcId = npc.Id,
                NpcName = npc.Name,
                AverageSuspicion = avgSuspicion
            });
        }

        return rankings
            .Where(r => r.AverageSuspicion > 0)
            .OrderByDescending(r => r.AverageSuspicion)
            .Take(5)
            .ToList();
    }

    private List<TrustRanking> GetMostTrustedNpcs()
    {
        if (_sharedGameState == null) return new List<TrustRanking>();

        var rankings = new List<TrustRanking>();

        foreach (var npc in _sharedGameState.NPCs.Where(n => n.Status == Core.Enums.NPCStatus.Alive))
        {
            var totalTrust = 0;
            var count = 0;

            foreach (var otherNpc in _sharedGameState.NPCs.Where(n => n.Status == Core.Enums.NPCStatus.Alive && n.Id != npc.Id))
            {
                if (otherNpc.Trust.TryGetValue(npc.Id, out var trust))
                {
                    totalTrust += trust;
                    count++;
                }
            }

            var avgTrust = count > 0 ? totalTrust / count : 0;
            
            rankings.Add(new TrustRanking
            {
                NpcId = npc.Id,
                NpcName = npc.Name,
                AverageTrust = avgTrust
            });
        }

        return rankings
            .Where(r => r.AverageTrust > 0)
            .OrderByDescending(r => r.AverageTrust)
            .Take(5)
            .ToList();
    }

    private string GetSimilarityInterpretation(int similarity)
    {
        return similarity switch
        {
            >= 80 => "Very similar behavior patterns - possibly same role type",
            >= 60 => "Similar behavior - may have overlapping activities",
            >= 40 => "Some similarities - occasional overlap",
            >= 20 => "Different behavior patterns",
            _ => "Completely different behavior"
        };
    }
}

public class ComparisonResult
{
    public string Npc1Name { get; set; } = string.Empty;
    public string Npc2Name { get; set; } = string.Empty;
    public int SimilarityScore { get; set; }
    public string Interpretation { get; set; } = string.Empty;
}

public class InvestigationSummary
{
    public int TotalObservations { get; set; }
    public int TotalRumors { get; set; }
    public int TotalEvidence { get; set; }
    public List<SuspicionRanking> MostSuspiciousNpcs { get; set; } = new();
    public List<TrustRanking> MostTrustedNpcs { get; set; } = new();
}

public class SuspicionRanking
{
    public string NpcId { get; set; } = string.Empty;
    public string NpcName { get; set; } = string.Empty;
    public int AverageSuspicion { get; set; }
}

public class TrustRanking
{
    public string NpcId { get; set; } = string.Empty;
    public string NpcName { get; set; } = string.Empty;
    public int AverageTrust { get; set; }
}
