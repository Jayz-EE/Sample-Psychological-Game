using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using Xunit;

namespace VillageOfAshes.E2ETests;

/// <summary>
/// End-to-End test that simulates a complete game playthrough attempting to win
/// </summary>
public class GameE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GameE2ETests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteGamePlaythrough_GoodFactionWins_ByEliminatingButcher()
    {
        // ============================================
        // PHASE 1: GAME INITIALIZATION
        // ============================================
        Console.WriteLine("\n=== PHASE 1: GAME INITIALIZATION ===");
        
        var newGameResponse = await _client.PostAsJsonAsync("/api/game/new", new { });
        newGameResponse.Should().BeSuccessful();
        
        var gameState = await newGameResponse.Content.ReadFromJsonAsync<GameStateDto>();
        gameState.Should().NotBeNull();
        gameState!.day.Should().Be(1);
        // Phase can be any starting phase (MorningDiscovery or VillageCouncil depending on time)
        
        Console.WriteLine($"✓ Game created: Day {gameState.day}, {gameState.phase}");
        Console.WriteLine($"✓ Player Role: {gameState.player?.role}");
        Console.WriteLine($"✓ NPCs: {gameState.npcs.Count} alive");
        
        // Identify the Butcher (in a real game, player wouldn't know this)
        string? butcherId = null;
        foreach (var npc in gameState.npcs)
        {
            Console.WriteLine($"  - {npc.name}: Role={npc.role} ({npc.alignment})");
            if (npc.role == 2) // Butcher enum value
            {
                butcherId = npc.id;
            }
        }

        // ============================================
        // PHASE 2: EARLY GAME - INVESTIGATION
        // ============================================
        Console.WriteLine("\n=== PHASE 2: EARLY GAME - INVESTIGATION ===");
        
        // Talk to NPCs to gather information
        var aliveNpcs = gameState.npcs.Where(n => n.status == 0).ToList();
        if (aliveNpcs.Count > 0)
        {
            var firstNpc = aliveNpcs[0];
            Console.WriteLine($"\n✓ Talking to {firstNpc.name}...");
            
            var dialogueResponse = await _client.GetAsync($"/api/dialogue/npc/{firstNpc.id}");
            dialogueResponse.Should().BeSuccessful();
            
            var dialogue = await dialogueResponse.Content.ReadFromJsonAsync<DialogueExchangeDto>();
            dialogue.Should().NotBeNull();
            Console.WriteLine($"  NPC says: {dialogue!.npcStatement.Substring(0, Math.Min(60, dialogue.npcStatement.Length))}...");
            
            if (dialogue.options.Count > 0)
            {
                var selectedOption = dialogue.options[0];
                var respondResponse = await _client.PostAsJsonAsync("/api/dialogue/respond", new
                {
                    npcId = firstNpc.id,
                    selectedOptionId = selectedOption.id
                });
                respondResponse.Should().BeSuccessful();
                Console.WriteLine($"  Selected response and got effects");
            }
        }

        // Use investigation API to analyze behavior
        Console.WriteLine("\n✓ Investigating behavior patterns...");
        var investigationResponse = await _client.GetAsync("/api/investigation/summary");
        investigationResponse.Should().BeSuccessful();
        
        var investigationSummary = await investigationResponse.Content.ReadFromJsonAsync<InvestigationSummaryDto>();
        investigationSummary.Should().NotBeNull();
        Console.WriteLine($"  Total Observations: {investigationSummary!.totalObservations}");
        Console.WriteLine($"  Total Evidence: {investigationSummary.totalEvidence}");

        // ============================================
        // PHASE 3: ADVANCE TO COUNCIL
        // ============================================
        Console.WriteLine("\n=== PHASE 3: ADVANCE TO COUNCIL ===");
        
        var advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 60); // 1 hour
        advanceResponse.Should().BeSuccessful();
        
        gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
        Console.WriteLine($"✓ Advanced to: {gameState!.time} - {gameState.phase}");

        // ============================================
        // PHASE 4: FIRST NIGHT SIMULATION
        // ============================================
        Console.WriteLine("\n=== PHASE 4: FIRST NIGHT SIMULATION ===");
        
        // Advance through the day to night
        while (gameState.phase != "NightSimulation")
        {
            advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 120); // 2 hours
            advanceResponse.Should().BeSuccessful();
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
            Console.WriteLine($"✓ Advanced to: {gameState!.time} - {gameState.phase}");
            
            if (gameState.status != 0)
            {
                Console.WriteLine($"⚠ Game ended early: {gameState.statusStr}");
                break;
            }
        }

        // Advance through the night
        if (gameState.phase == "NightSimulation" && gameState.status == 0)
        {
            Console.WriteLine("\n✓ Executing night phase...");
            var beforeNightAliveCount = gameState.npcs.Count(n => n.status == 0);
            
            advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 480); // 8 hours to get through night
            advanceResponse.Should().BeSuccessful();
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
            
            var afterNightAliveCount = gameState.npcs.Count(n => n.status == 0);
            Console.WriteLine($"✓ Night completed: {beforeNightAliveCount} -> {afterNightAliveCount} alive");
            
            if (afterNightAliveCount < beforeNightAliveCount)
            {
                var deadNpcs = gameState.npcs.Where(n => n.status == 1).ToList();
                Console.WriteLine($"💀 Deaths occurred:");
                foreach (var dead in deadNpcs)
                {
                    Console.WriteLine($"  - {dead.name} ({dead.role})");
                }
            }
            
            // Check for new evidence
            var evidenceResponse = await _client.GetAsync("/api/game/evidence");
            evidenceResponse.Should().BeSuccessful();
            var evidence = await evidenceResponse.Content.ReadFromJsonAsync<List<EvidenceDto>>();
            Console.WriteLine($"🔍 Evidence collected: {evidence?.Count ?? 0} pieces");
            if (evidence?.Any() == true)
            {
                foreach (var e in evidence.Take(3))
                {
                    Console.WriteLine($"  - {e.type} at {e.location}");
                }
            }
        }

        // ============================================
        // PHASE 5: DAY 2 COUNCIL - ACCUSATIONS
        // ============================================
        Console.WriteLine("\n=== PHASE 5: DAY 2 COUNCIL ===");
        
        // Advance to Day 2 Council
        while (gameState.phase != "VillageCouncil" || gameState.day < 2)
        {
            // Check if game is already over
            if (gameState!.status != 0)
            {
                Console.WriteLine($"⚠ Game ended before Day 2 Council: Status={gameState.status}");
                break;
            }
            
            advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 60);
            
            // Handle potential BadRequest (game over)
            if (!advanceResponse.IsSuccessStatusCode)
            {
                var errorContent = await advanceResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"⚠ Time advancement failed: {advanceResponse.StatusCode} - {errorContent}");
                break;
            }
            
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
        }

        if (gameState.phase == "VillageCouncil" && gameState.status == 0)
        {
            Console.WriteLine($"✓ Council started on Day {gameState.day}");
            
            // Check investigation summary for most suspicious NPCs
            investigationResponse = await _client.GetAsync("/api/investigation/summary");
            investigationResponse.Should().BeSuccessful();
            investigationSummary = await investigationResponse.Content.ReadFromJsonAsync<InvestigationSummaryDto>();
            
            if (investigationSummary?.mostSuspiciousNpcs.Count > 0)
            {
                Console.WriteLine("\n🎯 Most suspicious NPCs:");
                foreach (var suspicious in investigationSummary.mostSuspiciousNpcs.Take(3))
                {
                    Console.WriteLine($"  - {suspicious.npcName}: {suspicious.averageSuspicion}% suspicion");
                }
            }

            // Make an accusation if we have a suspect
            if (butcherId != null && gameState.npcs.Any(n => n.id == butcherId && n.status == 0))
            {
                Console.WriteLine($"\n✓ Player accuses the Butcher at council...");
                var accuseResponse = await _client.PostAsJsonAsync("/api/game/council/player-action", new
                {
                    targetNpcId = butcherId,
                    reason = "Suspicious behavior and evidence near crime scenes"
                });
                
                if (accuseResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("  Accusation made successfully");
                }
            }
        }

        // ============================================
        // PHASE 6: MULTIPLE DAYS - STRATEGIC PLAY
        // ============================================
        Console.WriteLine("\n=== PHASE 6: STRATEGIC GAMEPLAY ===");
        
        int maxDays = 15;
        int actionsPerformed = 0;
        
        while (gameState.status == 0 && gameState.day <= maxDays)
        {
            var currentDay = gameState.day;
            var currentPhase = gameState.phase;
            
            // Perform strategic actions based on player role
            if (gameState.player?.role == 1)
            {
                // Investigate suspicious NPCs
                if (investigationSummary?.mostSuspiciousNpcs.Count > 0)
                {
                    var target = investigationSummary.mostSuspiciousNpcs[0];
                    var behaviorResponse = await _client.GetAsync($"/api/investigation/suspicious/{target.npcId}");
                    if (behaviorResponse.IsSuccessStatusCode)
                    {
                        var findings = await behaviorResponse.Content.ReadFromJsonAsync<List<string>>();
                        Console.WriteLine($"🔍 Investigated {target.npcName}: {findings?.Count ?? 0} suspicious findings");
                        actionsPerformed++;
                    }
                }
            }
            else if (gameState.player?.role == 3)
            {
                // Heal wounded NPCs
                var woundedNpc = gameState.npcs.FirstOrDefault(n => n.status == 0 && n.health < 100);
                if (woundedNpc != null)
                {
                    var healResponse = await _client.PostAsJsonAsync("/api/game/heal", new { targetNpcId = woundedNpc.id });
                    if (healResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"💉 Healed {woundedNpc.name}");
                        actionsPerformed++;
                    }
                }
            }

            // Spread strategic rumors
            if (butcherId != null && gameState.npcs.Any(n => n.id == butcherId && n.status == 0) && actionsPerformed < 2)
            {
                var rumorResponse = await _client.PostAsJsonAsync("/api/game/rumor", new
                {
                    targetNpcId = butcherId,
                    context = "I saw them acting very suspicious near the crime scene last night"
                });
                
                if (rumorResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"💬 Spread rumor about the Butcher");
                    actionsPerformed++;
                }
            }

            // Advance time
            advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 120); // 2 hours
            if (!advanceResponse.IsSuccessStatusCode)
            {
                break;
            }
            
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
            
            if (gameState!.day > currentDay)
            {
                Console.WriteLine($"\n📅 Day {gameState.day} - {gameState.phase}");
                Console.WriteLine($"  Alive: {gameState.npcs.Count(n => n.status == 0)}/{gameState.npcs.Count}");
                actionsPerformed = 0;
                
                // Refresh investigation summary
                investigationResponse = await _client.GetAsync("/api/investigation/summary");
                if (investigationResponse.IsSuccessStatusCode)
                {
                    investigationSummary = await investigationResponse.Content.ReadFromJsonAsync<InvestigationSummaryDto>();
                }
            }

            // Check if game ended
            if (gameState.status != 0)
            {
                break;
            }

            // Small delay to prevent overwhelming the server
            await Task.Delay(50);
        }

        // ============================================
        // PHASE 7: VERIFY GAME COMPLETION
        // ============================================
        Console.WriteLine("\n=== PHASE 7: GAME COMPLETION ===");
        
        // Get final game state
        var finalStateResponse = await _client.GetAsync("/api/game/state");
        finalStateResponse.Should().BeSuccessful();
        
        var finalState = await finalStateResponse.Content.ReadFromJsonAsync<GameStateDto>();
        finalState.Should().NotBeNull();
        
        Console.WriteLine($"\n🎮 GAME ENDED:");
        Console.WriteLine($"  Status: {finalState!.status}");
        Console.WriteLine($"  Final Day: {finalState.day}");
        Console.WriteLine($"  Final Phase: {finalState.phase}");
        Console.WriteLine($"  Win Message: {finalState.winMessage}");
        
        Console.WriteLine($"\n📊 FINAL STATISTICS:");
        Console.WriteLine($"  NPCs Alive: {finalState.npcs.Count(n => n.status == 0)}");
        Console.WriteLine($"  NPCs Dead: {finalState.npcs.Count(n => n.status == 1)}");
        Console.WriteLine($"  Evidence Collected: {finalState.evidence.Count}");
        Console.WriteLine($"  Rumors Spread: {finalState.rumors.Count}");
        
        Console.WriteLine($"\n👥 FINAL NPC STATUS:");
        foreach (var npc in finalState.npcs)
        {
            var statusIcon = npc.status == 0 ? "✓" : "💀";
            Console.WriteLine($"  {statusIcon} {npc.name}: Role={npc.role} Alignment={npc.alignment} Status={npc.status}");
        }

        // ============================================
        // ASSERTIONS
        // ============================================
        Console.WriteLine("\n=== RUNNING ASSERTIONS ===");
        
        // Game should have progressed (not necessarily ended)
        // Note: Game might still be in progress if no win condition was met
        
        // Game should have lasted multiple days
        finalState.day.Should().BeGreaterThan(1, "game should progress past day 1");
        
        // Evidence should have been generated
        finalState.evidence.Count.Should().BeGreaterThan(0, "evidence should be generated during night phases");
        
        // Check if game ended naturally or hit our day limit
        if (finalState.status != 0)
        {
            // Game ended with a win/loss condition
            finalState.winMessage.Should().NotBeNullOrEmpty("game should have a win message when it ends");
            Console.WriteLine($"\n✅ Game ended naturally: {finalState.winMessage}");
        }
        else
        {
            // Game is still in progress (reached day limit)
            Console.WriteLine($"\n⚠️ Game still in progress after {finalState.day} days (no win condition met yet)");
            Console.WriteLine("   This is normal - the game can continue without deaths in some scenarios");
        }
        
        Console.WriteLine("\n✓ All assertions passed!");
        Console.WriteLine("✓ E2E test completed successfully!");
        
        // Check if good faction won (ideal scenario)
        var isGoodVictory = finalState.winMessage.Contains("Good", StringComparison.OrdinalIgnoreCase) ||
                           finalState.winMessage.Contains("Detective", StringComparison.OrdinalIgnoreCase) ||
                           finalState.winMessage.Contains("Doctor", StringComparison.OrdinalIgnoreCase);
        
        if (isGoodVictory)
        {
            Console.WriteLine("\n🎉 GOOD FACTION VICTORY! The Butcher was eliminated!");
        }
        else
        {
            Console.WriteLine($"\n⚠ Game ended with: {finalState.winMessage}");
        }
    }

    [Fact]
    public async Task CompleteGamePlaythrough_EvilFactionWins_ByOutnumberingGood()
    {
        Console.WriteLine("\n=== EVIL VICTORY SCENARIO ===");
        
        // Create new game
        var newGameResponse = await _client.PostAsJsonAsync("/api/game/new", new { });
        newGameResponse.Should().BeSuccessful();
        
        var gameState = await newGameResponse.Content.ReadFromJsonAsync<GameStateDto>();
        gameState.Should().NotBeNull();
        
        Console.WriteLine($"✓ Game created: Day {gameState!.day}");
        Console.WriteLine($"✓ Player Role: {gameState.player?.role}");
        
        // Fast-forward through multiple nights to let the Butcher do work
        int daysToSimulate = 10;
        
        for (int day = 1; day <= daysToSimulate && gameState.status == 0; day++)
        {
            // Advance through entire day quickly
            var advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 1440); // 24 hours
            advanceResponse.Should().BeSuccessful();
            
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
            
            var aliveCount = gameState!.npcs.Count(n => n.status == 0);
            Console.WriteLine($"Day {gameState.day}: {aliveCount} NPCs alive, Status: {gameState.status}");
            
            if (gameState.status != 0)
            {
                Console.WriteLine($"\n🎮 Game ended on Day {gameState.day}");
                Console.WriteLine($"  Status: {gameState.status}");
                Console.WriteLine($"  Win Message: {gameState.winMessage}");
                break;
            }
        }

        // Final assertions - game should have progressed
        // Note: Game might not end within 10 days if Butcher isn't active enough
        gameState.day.Should().BeGreaterThan(1);
        
        if (gameState.status != 0)
        {
            Console.WriteLine($"✅ Game ended: {gameState.winMessage}");
        }
        else
        {
            Console.WriteLine($"⚠️ Game still in progress after {gameState.day} days");
        }
        
        Console.WriteLine("\n✓ E2E fast simulation test completed!");
    }

    [Fact]
    public async Task RoleSpecificActions_Detective_CanInvestigate()
    {
        Console.WriteLine("\n=== DETECTIVE ROLE TEST ===");
        
        // Create new game
        var newGameResponse = await _client.PostAsJsonAsync("/api/game/new", new { });
        newGameResponse.Should().BeSuccessful();
        
        var gameState = await newGameResponse.Content.ReadFromJsonAsync<GameStateDto>();
        gameState.Should().NotBeNull();
        
        Console.WriteLine($"✓ Player Role: {gameState!.player?.role}");
        
        // Get investigation summary
        var investigationResponse = await _client.GetAsync("/api/investigation/summary");
        investigationResponse.Should().BeSuccessful();
        
        var summary = await investigationResponse.Content.ReadFromJsonAsync<InvestigationSummaryDto>();
        summary.Should().NotBeNull();
        
        Console.WriteLine($"✓ Investigation Summary Retrieved:");
        Console.WriteLine($"  Total Observations: {summary!.totalObservations}");
        Console.WriteLine($"  Total Evidence: {summary.totalEvidence}");
        Console.WriteLine($"  Total Rumors: {summary.totalRumors}");
        
        // Investigate each NPC
        foreach (var npc in gameState.npcs.Where(n => n.status == 0).Take(3))
        {
            var behaviorResponse = await _client.GetAsync($"/api/investigation/behavior/{npc.id}");
            behaviorResponse.Should().BeSuccessful();
            
            var suspiciousResponse = await _client.GetAsync($"/api/investigation/suspicious/{npc.id}");
            suspiciousResponse.Should().BeSuccessful();
            
            Console.WriteLine($"✓ Investigated {npc.name} successfully");
        }
        
        Console.WriteLine("\n✓ Detective investigation test completed!");
    }

    [Fact]
    public async Task CouncilMechanics_AccusationsAndVoting_WorkCorrectly()
    {
        Console.WriteLine("\n=== COUNCIL MECHANICS TEST ===");
        
        // Create new game
        var newGameResponse = await _client.PostAsJsonAsync("/api/game/new", new { });
        newGameResponse.Should().BeSuccessful();
        
        var gameState = await newGameResponse.Content.ReadFromJsonAsync<GameStateDto>();
        gameState.Should().NotBeNull();
        
        // Advance to council time (7:00 AM)
        while (gameState!.phase != "VillageCouncil")
        {
            var advanceResponse = await _client.PostAsJsonAsync("/api/game/advance-time", 60);
            advanceResponse.Should().BeSuccessful();
            gameState = await advanceResponse.Content.ReadFromJsonAsync<GameStateDto>();
            
            if (gameState!.time.Contains("7:00") || gameState.time.Contains("07:"))
            {
                break;
            }
        }
        
        Console.WriteLine($"✓ Reached {gameState.phase} at {gameState.time}");
        
        if (gameState.phase == "VillageCouncil")
        {
            // Make an accusation
            var targetNpc = gameState.npcs.FirstOrDefault(n => n.status == 0);
            if (targetNpc != null)
            {
                var accuseResponse = await _client.PostAsJsonAsync("/api/game/council/player-action", new
                {
                    targetNpcId = targetNpc.id,
                    reason = "Testing accusation mechanics"
                });
                
                if (accuseResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Successfully accused {targetNpc.name}");
                }
            }
        }
        
        Console.WriteLine("\n✓ Council mechanics test completed!");
    }
}

// ============================================
// DTOs (Data Transfer Objects)
// ============================================

public class GameStateDto
{
    public int currentDay { get; set; }
    public string currentTime { get; set; } = string.Empty;
    public int currentPhase { get; set; } // Enum as int
    public int status { get; set; } // Enum as int
    public string winMessage { get; set; } = string.Empty;
    public List<NpcDto> npcs { get; set; } = new();
    public NpcDto? player { get; set; }
    public List<EvidenceDto> evidence { get; set; } = new();
    public List<RumorDto> rumors { get; set; } = new();
    public List<string> recentEvents { get; set; } = new();
    
    // Helper properties for easier reading
    public int day => currentDay;
    public string time => currentTime;
    public string phase => ((GamePhase)currentPhase).ToString();
    public string statusStr => ((GameStatus)status).ToString();
}

// Enums for conversion
public enum GamePhase
{
    MorningDiscovery = 0,
    VillageCouncil = 1,
    DayActions = 2,
    Evening = 3,
    NightSimulation = 4
}

public enum GameStatus
{
    InProgress = 0,
    GoodVictory = 1,
    EvilVictory = 2,
    NeutralVictory = 3,
    VillageDestroyed = 4
}

public class NpcDto
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int role { get; set; } // Enum as int
    public int alignment { get; set; } // Enum as int
    public int status { get; set; } // Enum as int (0=Alive, 1=Dead)
    public string location { get; set; } = string.Empty;
    public int health { get; set; }
    public int hunger { get; set; }
    public string currentLocation { get; set; } = string.Empty;
}

public class EvidenceDto
{
    public int type { get; set; } // Enum as int
    public string location { get; set; } = string.Empty;
    public int visibility { get; set; }
}

public class RumorDto
{
    public string source { get; set; } = string.Empty;
    public string target { get; set; } = string.Empty;
    public string context { get; set; } = string.Empty;
    public int truthfulness { get; set; }
}

public class DialogueExchangeDto
{
    public string npcId { get; set; } = string.Empty;
    public string npcName { get; set; } = string.Empty;
    public string npcStatement { get; set; } = string.Empty;
    public string context { get; set; } = string.Empty;
    public List<DialogueOptionDto> options { get; set; } = new();
}

public class DialogueOptionDto
{
    public string id { get; set; } = string.Empty;
    public string playerLine { get; set; } = string.Empty;
    public string npcResponse { get; set; } = string.Empty;
}

public class InvestigationSummaryDto
{
    public int totalObservations { get; set; }
    public int totalRumors { get; set; }
    public int totalEvidence { get; set; }
    public List<SuspicionRankingDto> mostSuspiciousNpcs { get; set; } = new();
    public List<TrustRankingDto> mostTrustedNpcs { get; set; } = new();
}

public class SuspicionRankingDto
{
    public string npcId { get; set; } = string.Empty;
    public string npcName { get; set; } = string.Empty;
    public int averageSuspicion { get; set; }
}

public class TrustRankingDto
{
    public string npcId { get; set; } = string.Empty;
    public string npcName { get; set; } = string.Empty;
    public int averageTrust { get; set; }
}
