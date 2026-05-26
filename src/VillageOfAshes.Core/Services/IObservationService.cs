using VillageOfAshes.Core.Entities;

namespace VillageOfAshes.Core.Services;

/// <summary>
/// Service for managing NPC observations and witness accounts
/// </summary>
public interface IObservationService
{
    /// <summary>
    /// Generate observations during night phase based on NPC movements
    /// </summary>
    List<Observation> GenerateNightObservations(GameState gameState);
    
    /// <summary>
    /// Get all observations made by a specific NPC
    /// </summary>
    List<Observation> GetObservationsByObserver(GameState gameState, string observerId);
    
    /// <summary>
    /// Get all observations about a specific NPC
    /// </summary>
    List<Observation> GetObservationsAboutTarget(GameState gameState, string targetId);
    
    /// <summary>
    /// Share an observation with another NPC (gossip)
    /// </summary>
    void ShareObservation(GameState gameState, Observation observation, string shareWithNpcId);
    
    /// <summary>
    /// Calculate how suspicious an observation makes the target appear
    /// </summary>
    int CalculateObservationSuspicion(Observation observation, GameState gameState);
}

/// <summary>
/// Service for managing relationships and alliances between NPCs
/// </summary>
public interface IRelationshipService
{
    /// <summary>
    /// Get or create relationship between two NPCs
    /// </summary>
    Relationship GetRelationship(GameState gameState, string npcId1, string npcId2);
    
    /// <summary>
    /// Update relationship based on interaction
    /// </summary>
    void UpdateRelationship(GameState gameState, string npcId1, string npcId2, int trustChange, int suspicionChange);
    
    /// <summary>
    /// Form an alliance between two NPCs
    /// </summary>
    void FormAlliance(GameState gameState, string npcId1, string npcId2);
    
    /// <summary>
    /// Break an alliance
    /// </summary>
    void BreakAlliance(GameState gameState, string npcId1, string npcId2);
    
    /// <summary>
    /// Get all allies of an NPC
    /// </summary>
    List<NPC> GetAllies(GameState gameState, string npcId);
    
    /// <summary>
    /// Check if two NPCs are allied
    /// </summary>
    bool AreAllied(GameState gameState, string npcId1, string npcId2);
}

/// <summary>
/// Service for managing secrets and information discovery
/// </summary>
public interface ISecretService
{
    /// <summary>
    /// Generate secrets about NPCs based on their actions
    /// </summary>
    List<Secret> GenerateSecrets(GameState gameState);
    
    /// <summary>
    /// NPC discovers a secret
    /// </summary>
    void DiscoverSecret(GameState gameState, string discovererNpcId, Secret secret);
    
    /// <summary>
    /// Share a secret with another NPC
    /// </summary>
    void ShareSecret(GameState gameState, Secret secret, string shareWithNpcId);
    
    /// <summary>
    /// Get all secrets known by an NPC
    /// </summary>
    List<Secret> GetSecretsKnownBy(GameState gameState, string npcId);
    
    /// <summary>
    /// Get all secrets about a specific NPC
    /// </summary>
    List<Secret> GetSecretsAbout(GameState gameState, string targetNpcId);
}

/// <summary>
/// Service for tracking and analyzing NPC behavior patterns
/// </summary>
public interface IBehaviorAnalysisService
{
    /// <summary>
    /// Update behavior pattern based on NPC action
    /// </summary>
    void RecordBehavior(GameState gameState, string npcId, string action, string location);
    
    /// <summary>
    /// Get behavior pattern for an NPC
    /// </summary>
    BehaviorPattern GetBehaviorPattern(GameState gameState, string npcId);
    
    /// <summary>
    /// Analyze patterns to identify suspicious behavior
    /// </summary>
    List<string> AnalyzeSuspiciousBehavior(GameState gameState, string npcId);
    
    /// <summary>
    /// Compare behavior patterns between NPCs
    /// </summary>
    int CalculateBehaviorSimilarity(BehaviorPattern pattern1, BehaviorPattern pattern2);
    
    /// <summary>
    /// Predict likely role based on behavior pattern
    /// </summary>
    Dictionary<string, int> PredictRoleFromBehavior(BehaviorPattern pattern);
}
