using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class CouncilService : ICouncilService
{
    private readonly Random _random = new();
    private readonly ISuspicionCalculator _suspicionCalculator;

    public CouncilService(ISuspicionCalculator suspicionCalculator)
    {
        _suspicionCalculator = suspicionCalculator;
    }

    public async Task<CouncilSession> StartCouncilSession(GameState gameState)
    {
        var session = new CouncilSession
        {
            Day = gameState.CurrentDay,
            VotingPhase = false
        };

        // Generate NPC statements based on their knowledge and suspicions
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();

        foreach (var npc in aliveNpcs)
        {
            var statement = GenerateStatement(npc, gameState);
            if (!string.IsNullOrEmpty(statement))
            {
                session.Statements.Add(new CouncilStatement
                {
                    NpcId = npc.Id,
                    Statement = statement,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Generate accusations based on suspicion
            var mostSuspicious = npc.Suspicion
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();

            if (mostSuspicious.Value > 60 && _random.Next(100) < 50)
            {
                var target = gameState.NPCs.FirstOrDefault(n => n.Id == mostSuspicious.Key);
                if (target != null && target.Status == NPCStatus.Alive)
                {
                    var reason = GenerateAccusationReason(npc, target, gameState);
                    session.Accusations.Add(new Accusation
                    {
                        SourceNpcId = npc.Id,
                        TargetNpcId = target.Id,
                        Reason = reason
                    });
                }
            }
        }

        await Task.CompletedTask;
        return session;
    }

    public void ProcessAccusation(GameState gameState, string sourceNpcId, string targetNpcId, string reason)
    {
        var source = gameState.NPCs.FirstOrDefault(n => n.Id == sourceNpcId);
        var target = gameState.NPCs.FirstOrDefault(n => n.Id == targetNpcId);

        if (source == null || target == null) return;

        // Update suspicion for all NPCs
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            if (npc.Id == targetNpcId) continue;

            // NPCs who trust the accuser will increase suspicion of target
            var trustInAccuser = npc.Trust.GetValueOrDefault(sourceNpcId, 50);
            var suspicionIncrease = (trustInAccuser / 10) + _random.Next(-5, 10);

            if (!npc.Suspicion.ContainsKey(targetNpcId))
                npc.Suspicion[targetNpcId] = 0;

            npc.Suspicion[targetNpcId] = Math.Clamp(
                npc.Suspicion[targetNpcId] + suspicionIncrease, 
                0, 
                100
            );
        }

        // Target's trust in accuser decreases
        if (!target.Trust.ContainsKey(sourceNpcId))
            target.Trust[sourceNpcId] = 50;
        target.Trust[sourceNpcId] = Math.Max(0, target.Trust[sourceNpcId] - 20);
    }

    public void ProcessVote(GameState gameState, string voterNpcId, string targetNpcId)
    {
        var currentCouncil = gameState.CouncilHistory.LastOrDefault();
        if (currentCouncil == null) return;

        currentCouncil.Votes.Add(new Vote
        {
            VoterNpcId = voterNpcId,
            TargetNpcId = targetNpcId
        });
    }

    public CouncilOutcome ResolveCouncil(GameState gameState, CouncilSession session)
    {
        var outcome = new CouncilOutcome();

        // Get current council record
        var councilRecord = new CouncilRecord
        {
            Day = gameState.CurrentDay,
            Accusations = session.Accusations,
            PublicSuspicion = _suspicionCalculator.GetPublicSuspicionRankings(gameState)
        };

        // Simulate voting based on suspicion levels
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();
        var votes = new Dictionary<string, int>();

        foreach (var npc in aliveNpcs)
        {
            // Each NPC votes for their most suspected target
            var mostSuspected = npc.Suspicion
                .Where(kvp => aliveNpcs.Any(n => n.Id == kvp.Key))
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();

            if (mostSuspected.Value > 40)
            {
                if (!votes.ContainsKey(mostSuspected.Key))
                    votes[mostSuspected.Key] = 0;
                votes[mostSuspected.Key]++;

                councilRecord.Votes.Add(new Vote
                {
                    VoterNpcId = npc.Id,
                    TargetNpcId = mostSuspected.Key
                });
            }
        }

        // Determine if anyone should be executed (requires majority)
        var totalVoters = aliveNpcs.Count;
        var majorityThreshold = totalVoters / 2 + 1;

        var topVoted = votes.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
        if (topVoted.Value >= majorityThreshold)
        {
            var executedNpc = gameState.NPCs.FirstOrDefault(n => n.Id == topVoted.Key);
            if (executedNpc != null)
            {
                executedNpc.Status = NPCStatus.Dead;
                outcome.ExecutedNpcId = executedNpc.Id;
            }
        }

        // Calculate suspicion changes
        outcome.SuspicionChanges = _suspicionCalculator.GetPublicSuspicionRankings(gameState);

        // Identify new alliances (NPCs who voted together)
        var voteGroups = councilRecord.Votes
            .GroupBy(v => v.TargetNpcId)
            .Where(g => g.Count() >= 2);

        foreach (var group in voteGroups)
        {
            var voters = group.Select(v => v.VoterNpcId).ToList();
            for (int i = 0; i < voters.Count - 1; i++)
            {
                for (int j = i + 1; j < voters.Count; j++)
                {
                    var npc1 = gameState.NPCs.FirstOrDefault(n => n.Id == voters[i]);
                    var npc2 = gameState.NPCs.FirstOrDefault(n => n.Id == voters[j]);

                    if (npc1 != null && npc2 != null)
                    {
                        // Increase trust between voters
                        if (!npc1.Trust.ContainsKey(npc2.Id))
                            npc1.Trust[npc2.Id] = 50;
                        if (!npc2.Trust.ContainsKey(npc1.Id))
                            npc2.Trust[npc1.Id] = 50;

                        npc1.Trust[npc2.Id] = Math.Min(100, npc1.Trust[npc2.Id] + 10);
                        npc2.Trust[npc1.Id] = Math.Min(100, npc2.Trust[npc1.Id] + 10);

                        outcome.NewAlliances.Add($"{npc1.Name} and {npc2.Name}");
                    }
                }
            }
        }

        gameState.CouncilHistory.Add(councilRecord);
        return outcome;
    }

    private string GenerateStatement(NPC npc, GameState gameState)
    {
        var statements = new List<string>();

        // Comment on deaths
        var recentDeaths = gameState.NPCs.Count(n => n.Status == NPCStatus.Dead);
        if (recentDeaths > 0 && _random.Next(100) < 60)
        {
            var deathStatements = new[]
            {
                $"We've lost {recentDeaths} people. This madness has to stop!",
                $"{recentDeaths} dead already. Who will be next?",
                "Another death. We need to find who's responsible.",
                "How many more must die before we act?",
                "The killer is among us. We must be vigilant."
            };
            statements.Add(deathStatements[_random.Next(deathStatements.Length)]);
        }

        // Comment on evidence - ambiguous observations
        var recentEvidence = gameState.Evidence
            .Where(e => e.CreatedAt > DateTime.UtcNow.AddHours(-12))
            .ToList();

        if (recentEvidence.Any() && _random.Next(100) < 50)
        {
            var evidence = recentEvidence[_random.Next(recentEvidence.Count)];
            var evidenceStatements = new[]
            {
                $"I found {evidence.Type} near {evidence.Location}. Someone was there.",
                $"There's {evidence.Type} at {evidence.Location}. We should investigate.",
                $"I noticed {evidence.Type} by {evidence.Location} this morning.",
                $"Has anyone else seen the {evidence.Type} near {evidence.Location}?",
                $"The {evidence.Type} at {evidence.Location} wasn't there yesterday."
            };
            statements.Add(evidenceStatements[_random.Next(evidenceStatements.Length)]);
        }

        // Share rumors - without revealing who saw what
        var knownRumors = npc.Rumors.Take(2);
        foreach (var rumor in knownRumors)
        {
            if (_random.Next(100) < 40)
            {
                var target = gameState.NPCs.FirstOrDefault(n => n.Id == rumor.TargetNpcId);
                if (target != null)
                {
                    var rumorStatements = new[]
                    {
                        $"I heard {target.Name} was {rumor.Context}.",
                        $"Someone told me {target.Name} was {rumor.Context}.",
                        $"Word is that {target.Name} was {rumor.Context}.",
                        $"People are saying {target.Name} was {rumor.Context}.",
                        $"There are rumors about {target.Name} - {rumor.Context}."
                    };
                    statements.Add(rumorStatements[_random.Next(rumorStatements.Length)]);
                }
            }
        }

        // Observations about other NPCs - ambiguous, could be any role
        var otherNpcs = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != npc.Id)
            .ToList();

        if (otherNpcs.Any() && _random.Next(100) < 35)
        {
            var observed = otherNpcs[_random.Next(otherNpcs.Count)];
            var observationStatements = new[]
            {
                $"I saw {observed.Name} outside last night.",
                $"{observed.Name} was near {observed.CurrentLocation} after dark.",
                $"I noticed {observed.Name} acting strangely yesterday.",
                $"{observed.Name} wasn't home when I checked.",
                $"I heard footsteps near {observed.Name}'s house.",
                $"{observed.Name} has been avoiding me lately.",
                $"I saw {observed.Name} talking to someone in secret.",
                $"{observed.Name} was carrying something suspicious.",
                $"I found {observed.Name} near the scene this morning."
            };
            statements.Add(observationStatements[_random.Next(observationStatements.Length)]);
        }

        // General suspicion statements
        if (_random.Next(100) < 25)
        {
            var generalStatements = new[]
            {
                "Someone here knows more than they're saying.",
                "The killer is sitting among us right now.",
                "We need to be more careful about who we trust.",
                "I don't feel safe anymore.",
                "Someone is lying. I can feel it.",
                "We should watch each other more closely.",
                "There's a pattern to these deaths.",
                "The evidence points to someone in this room.",
                "We're running out of time to find the truth."
            };
            statements.Add(generalStatements[_random.Next(generalStatements.Length)]);
        }

        return statements.Any() 
            ? statements[_random.Next(statements.Count)] 
            : string.Empty;
    }

    private string GenerateAccusationReason(NPC accuser, NPC target, GameState gameState)
    {
        var reasons = new List<string>();

        // Check for evidence near target
        var evidenceNearTarget = gameState.Evidence
            .Where(e => e.Location == target.CurrentLocation)
            .ToList();

        if (evidenceNearTarget.Any())
        {
            var evidenceReasons = new[]
            {
                $"Found near suspicious evidence at {target.CurrentLocation}",
                $"Was seen at {target.CurrentLocation} where evidence was discovered",
                $"Too many coincidences linking them to {target.CurrentLocation}",
                $"Evidence at {target.CurrentLocation} points in their direction"
            };
            reasons.Add(evidenceReasons[_random.Next(evidenceReasons.Length)]);
        }

        // Check for rumors
        var rumorsAboutTarget = gameState.Rumors
            .Where(r => r.TargetNpcId == target.Id && r.KnownBy.Contains(accuser.Id))
            .ToList();

        if (rumorsAboutTarget.Any())
        {
            var rumorReasons = new[]
            {
                "Multiple reports of suspicious behavior",
                "Too many people have seen them acting strange",
                "The rumors about them keep piling up",
                "Everyone's talking about their suspicious activities",
                "I've heard too many concerning things about them"
            };
            reasons.Add(rumorReasons[_random.Next(rumorReasons.Length)]);
        }

        // Behavioral observations - ambiguous, could apply to any role
        var behavioralReasons = new[]
        {
            "Always wandering around at night",
            "Avoiding eye contact and acting nervous",
            "Their story keeps changing",
            "They were outside when they claimed to be home",
            "Too defensive when questioned",
            "Acting suspiciously during council meetings",
            "Seen near multiple crime scenes",
            "Can't account for their whereabouts",
            "Others have noticed their strange behavior",
            "They know too much about the deaths",
            "Always has an excuse ready",
            "Trying too hard to deflect suspicion",
            "Their alibis don't check out",
            "Caught in multiple lies"
        };

        if (_random.Next(100) < 60)
        {
            reasons.Add(behavioralReasons[_random.Next(behavioralReasons.Length)]);
        }

        // Default reason if nothing else
        if (!reasons.Any())
        {
            var defaultReasons = new[]
            {
                "Something about them doesn't feel right",
                "My instincts tell me they're involved",
                "Process of elimination points to them",
                "They're the most suspicious person here",
                "I have a bad feeling about them"
            };
            reasons.Add(defaultReasons[_random.Next(defaultReasons.Length)]);
        }

        return reasons[_random.Next(reasons.Count)];
    }
}
