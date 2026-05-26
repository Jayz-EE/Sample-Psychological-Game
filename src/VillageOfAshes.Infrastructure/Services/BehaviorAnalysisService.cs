using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class BehaviorAnalysisService : IBehaviorAnalysisService
{
    private readonly Dictionary<string, BehaviorPattern> _patterns = new();

    public void RecordBehavior(GameState gameState, string npcId, string action, string location)
    {
        if (!_patterns.ContainsKey(npcId))
        {
            _patterns[npcId] = new BehaviorPattern { NpcId = npcId };
        }

        var pattern = _patterns[npcId];

        // Record location frequency
        if (!pattern.NightLocations.ContainsKey(location))
            pattern.NightLocations[location] = 0;
        pattern.NightLocations[location]++;

        // Record activity frequency
        if (!pattern.DayActivities.ContainsKey(action))
            pattern.DayActivities[action] = 0;
        pattern.DayActivities[action]++;

        // Track specific behaviors
        if (action.Contains("night") || action.Contains("dark"))
            pattern.TimesSeenAtNight++;

        if (action.Contains("avoid") || action.Contains("skip"))
            pattern.TimesAvoidedCouncil++;

        if (action.Contains("defensive") || action.Contains("deny"))
            pattern.TimesDefensive++;

        if (action.Contains("help") || action.Contains("assist"))
            pattern.TimesHelpful++;

        // Identify suspicious actions
        var suspiciousKeywords = new[] { "blood", "weapon", "sneak", "hide", "flee", "threaten" };
        if (suspiciousKeywords.Any(keyword => action.ToLower().Contains(keyword)))
        {
            pattern.SuspiciousActions.Add($"{action} at {location}");
        }
    }

    public BehaviorPattern GetBehaviorPattern(GameState gameState, string npcId)
    {
        return _patterns.GetValueOrDefault(npcId, new BehaviorPattern { NpcId = npcId });
    }

    public List<string> AnalyzeSuspiciousBehavior(GameState gameState, string npcId)
    {
        var suspiciousFindings = new List<string>();
        var pattern = GetBehaviorPattern(gameState, npcId);
        var npc = gameState.NPCs.FirstOrDefault(n => n.Id == npcId);
        if (npc == null) return suspiciousFindings;

        // Frequent night activity
        if (pattern.TimesSeenAtNight > 3)
        {
            suspiciousFindings.Add($"{npc.Name} has been seen outside at night {pattern.TimesSeenAtNight} times");
        }

        // Avoiding council
        if (pattern.TimesAvoidedCouncil > 2)
        {
            suspiciousFindings.Add($"{npc.Name} has avoided council meetings {pattern.TimesAvoidedCouncil} times");
        }

        // Overly defensive
        if (pattern.TimesDefensive > 4)
        {
            suspiciousFindings.Add($"{npc.Name} has been defensive {pattern.TimesDefensive} times when questioned");
        }

        // Visiting same location repeatedly
        var mostVisited = pattern.NightLocations
            .OrderByDescending(kvp => kvp.Value)
            .FirstOrDefault();
        
        if (mostVisited.Value > 3)
        {
            suspiciousFindings.Add($"{npc.Name} has visited {mostVisited.Key} {mostVisited.Value} times at night");
        }

        // Suspicious actions
        if (pattern.SuspiciousActions.Any())
        {
            suspiciousFindings.Add($"{npc.Name} has performed {pattern.SuspiciousActions.Count} suspicious actions");
        }

        return suspiciousFindings;
    }

    public int CalculateBehaviorSimilarity(BehaviorPattern pattern1, BehaviorPattern pattern2)
    {
        var similarity = 0;
        var totalComparisons = 0;

        // Compare night locations
        var allLocations = pattern1.NightLocations.Keys
            .Union(pattern2.NightLocations.Keys)
            .ToList();

        foreach (var location in allLocations)
        {
            var freq1 = pattern1.NightLocations.GetValueOrDefault(location, 0);
            var freq2 = pattern2.NightLocations.GetValueOrDefault(location, 0);
            
            if (freq1 > 0 && freq2 > 0)
                similarity += Math.Min(freq1, freq2);
            
            totalComparisons++;
        }

        // Compare behavioral metrics
        similarity += Math.Abs(pattern1.TimesSeenAtNight - pattern2.TimesSeenAtNight) < 2 ? 10 : 0;
        similarity += Math.Abs(pattern1.TimesDefensive - pattern2.TimesDefensive) < 2 ? 10 : 0;
        similarity += Math.Abs(pattern1.TimesHelpful - pattern2.TimesHelpful) < 2 ? 10 : 0;

        return Math.Clamp(similarity, 0, 100);
    }

    public Dictionary<string, int> PredictRoleFromBehavior(BehaviorPattern pattern)
    {
        var predictions = new Dictionary<string, int>
        {
            { "Detective", 0 },
            { "Doctor", 0 },
            { "Butcher", 0 },
            { "Vagabond", 0 },
            { "Farmer", 0 }
        };

        // Detective indicators
        if (pattern.TimesSeenAtNight > 2)
            predictions["Detective"] += 20;
        if (pattern.SuspiciousActions.Count < 2)
            predictions["Detective"] += 15;

        // Doctor indicators
        if (pattern.TimesHelpful > 3)
            predictions["Doctor"] += 25;
        if (pattern.NightLocations.Count > 2)
            predictions["Doctor"] += 15;

        // Butcher indicators
        if (pattern.SuspiciousActions.Count > 2)
            predictions["Butcher"] += 30;
        if (pattern.TimesDefensive > 3)
            predictions["Butcher"] += 20;
        if (pattern.TimesAvoidedCouncil > 1)
            predictions["Butcher"] += 15;

        // Vagabond indicators
        if (pattern.TimesSeenAtNight > 4)
            predictions["Vagabond"] += 25;
        if (pattern.NightLocations.Count > 3)
            predictions["Vagabond"] += 20;
        if (pattern.TimesAvoidedCouncil > 0)
            predictions["Vagabond"] += 10;

        // Farmer indicators
        var farmlandVisits = pattern.NightLocations.GetValueOrDefault("Farmland", 0);
        if (farmlandVisits > 2)
            predictions["Farmer"] += 30;
        if (pattern.TimesHelpful > 2)
            predictions["Farmer"] += 15;

        // Normalize to 0-100
        var total = predictions.Values.Sum();
        if (total > 0)
        {
            predictions = predictions.ToDictionary(
                kvp => kvp.Key,
                kvp => (int)((kvp.Value / (double)total) * 100)
            );
        }

        return predictions;
    }
}
