using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;
using VillageOfAshes.Core.Dialogue;

namespace VillageOfAshes.Infrastructure.Services;

public class CouncilService : ICouncilService
{
    private readonly Random _random = new();
    private readonly ISuspicionCalculator _suspicionCalculator;
    private readonly INpcDecisionService _npcDecisions;

    public CouncilService(ISuspicionCalculator suspicionCalculator, INpcDecisionService npcDecisions)
    {
        _suspicionCalculator = suspicionCalculator;
        _npcDecisions = npcDecisions;
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

            // Generate accusations based on calculated suspicion and role intent
            var target = _npcDecisions.ChooseTarget(gameState, npc, NpcTargetIntent.Accuse);
            var suspicionLevel = target == null ? 0 : npc.Suspicion.GetValueOrDefault(target.Id, 0);

            if (target != null && target.Status == NPCStatus.Alive && suspicionLevel > 45 && _random.Next(100) < 55)
            {
                var reason = GenerateAccusationReason(npc, target, gameState);
                var response = _npcDecisions.GenerateAlibiLine(target, gameState, reason);
                session.Accusations.Add(new Accusation
                {
                    SourceNpcId = npc.Id,
                    TargetNpcId = target.Id,
                    Reason = reason,
                    Response = response
                });
                ProcessAccusation(gameState, npc.Id, target.Id, reason);
            }
        }

        await Task.CompletedTask;
        return session;
    }

    public void ProcessAccusation(GameState gameState, string sourceNpcId, string targetNpcId, string reason)
    {
        var source = FindPerson(gameState, sourceNpcId);
        var target = FindPerson(gameState, targetNpcId);

        if (source == null || target == null) return;

        // Update suspicion for all NPCs
        foreach (var npc in GetAlivePeople(gameState))
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
        var currentCouncil = gameState.ActiveCouncil;
        if (currentCouncil == null || currentCouncil.Resolved) return;

        currentCouncil.VotingPhase = true;
        currentCouncil.Votes.RemoveAll(v => v.VoterNpcId == voterNpcId);
        currentCouncil.Votes.Add(new Vote
        {
            VoterNpcId = voterNpcId,
            TargetNpcId = targetNpcId
        });
    }

    public void StartVoting(GameState gameState, CouncilSession session)
    {
        if (session.Resolved) return;

        session.VotingPhase = true;
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            if (session.Votes.Any(v => v.VoterNpcId == npc.Id)) continue;

            var voteTarget = ChooseVoteTarget(gameState, npc);
            if (voteTarget != null)
                ProcessVote(gameState, npc.Id, voteTarget.Id);
        }
    }

    public CouncilOutcome ResolveCouncil(GameState gameState, CouncilSession session)
    {
        var outcome = new CouncilOutcome();
        if (session.Resolved)
        {
            outcome.ExecutedNpcId = session.BurnedNpcId;
            outcome.RevealedRole = session.RevealedRole;
            outcome.RoleRevealTampered = session.RoleRevealTampered;
            outcome.SuspicionChanges = _suspicionCalculator.GetPublicSuspicionRankings(gameState);
            return outcome;
        }

        // Get current council record
        var councilRecord = new CouncilRecord
        {
            Day = gameState.CurrentDay,
            Accusations = session.Accusations,
            PublicSuspicion = _suspicionCalculator.GetPublicSuspicionRankings(gameState)
        };

        if (!session.VotingPhase)
            StartVoting(gameState, session);

        var aliveNpcs = GetAlivePeople(gameState).ToList();
        var votes = new Dictionary<string, int>();
        foreach (var vote in session.Votes.Where(v => aliveNpcs.Any(n => n.Id == v.TargetNpcId)))
        {
            votes[vote.TargetNpcId] = votes.GetValueOrDefault(vote.TargetNpcId, 0) + 1;
            councilRecord.Votes.Add(vote);
        }

        // Determine if anyone should be executed (requires majority)
        var totalVoters = aliveNpcs.Count;
        var majorityThreshold = totalVoters / 2 + 1;

        var topVoted = votes.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
        if (topVoted.Value >= majorityThreshold)
        {
            var executedNpc = FindPerson(gameState, topVoted.Key);
            if (executedNpc != null)
            {
                executedNpc.Status = NPCStatus.Dead;
                executedNpc.RevealedRole = ResolveRevealedRole(gameState, executedNpc, out var tampered);
                executedNpc.RoleRevealTampered = tampered;
                councilRecord.BurnedNpcId = executedNpc.Id;
                councilRecord.RevealedRole = executedNpc.RevealedRole;
                councilRecord.RoleRevealTampered = tampered;
                outcome.ExecutedNpcId = executedNpc.Id;
                outcome.RevealedRole = executedNpc.RevealedRole;
                outcome.RoleRevealTampered = tampered;
                session.BurnedNpcId = executedNpc.Id;
                session.RevealedRole = executedNpc.RevealedRole;
                session.RoleRevealTampered = tampered;
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
        session.Resolved = true;
        return outcome;
    }

    private NPC? ChooseVoteTarget(GameState gameState, NPC voter)
    {
        var alive = GetAlivePeople(gameState).Where(n => n.Id != voter.Id).ToList();
        var accusedIds = gameState.ActiveCouncil?.Accusations.Select(a => a.TargetNpcId).ToHashSet() ?? new HashSet<string>();
        var ranked = alive
            .Select(target =>
            {
                var suspicion = voter.Suspicion.GetValueOrDefault(target.Id, 0);
                var accusedBonus = accusedIds.Contains(target.Id) ? 25 : 0;
                var evilFrameBonus = voter.Alignment is Alignment.Evil or Alignment.EvilNeutral && target.Alignment != Alignment.Evil ? 15 : 0;
                return (Target: target, Score: suspicion + accusedBonus + evilFrameBonus - voter.Trust.GetValueOrDefault(target.Id, 50) / 5);
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        return ranked.Score >= 25 ? ranked.Target : null;
    }

    private RoleType ResolveRevealedRole(GameState gameState, NPC executedNpc, out bool tampered)
    {
        tampered = false;
        var pranksters = GetAlivePeople(gameState)
            .Where(n => n.Role == RoleType.Prankster && n.PranksterRoleChangesUsed < 2)
            .ToList();

        var playerPrankster = gameState.Player?.Role == RoleType.Prankster &&
                              gameState.Player.Status == NPCStatus.Alive &&
                              gameState.Player.PranksterRoleChangesUsed < 2;

        if (playerPrankster)
            gameState.PendingPranksterRevealNpcId = executedNpc.Id;

        var npcPrankster = pranksters
            .Where(n => n.Id != gameState.Player?.Id && n.PhaseActionCount < 2)
            .OrderBy(_ => _random.Next())
            .FirstOrDefault();

        if (npcPrankster != null && _random.Next(100) < 45)
        {
            npcPrankster.PhaseActionCount = Math.Min(2, npcPrankster.PhaseActionCount + 1);
            npcPrankster.PranksterRoleChangesUsed++;
            tampered = true;
            return RandomFalseRole(executedNpc.Role);
        }

        return executedNpc.Role;
    }

    private RoleType RandomFalseRole(RoleType actualRole)
    {
        var roles = Enum.GetValues<RoleType>().Where(r => r != actualRole).ToList();
        return roles[_random.Next(roles.Count)];
    }

    private static NPC? FindPerson(GameState gameState, string personId) =>
        gameState.NPCs.FirstOrDefault(n => n.Id == personId)
        ?? (gameState.Player?.Id == personId ? gameState.Player : null);

    private static IEnumerable<NPC> GetAlivePeople(GameState gameState)
    {
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
            yield return npc;
        if (gameState.Player?.Status == NPCStatus.Alive)
            yield return gameState.Player;
    }

    private string GenerateStatement(NPC npc, GameState gameState)
    {
        if (_random.Next(100) < 30)
            return string.Empty;

        return DialoguePools.PassiveResponses[_random.Next(DialoguePools.PassiveResponses.Count)];
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
