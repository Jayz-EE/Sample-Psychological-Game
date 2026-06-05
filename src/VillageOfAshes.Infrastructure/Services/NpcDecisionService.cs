using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Roles;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class NpcDecisionService : INpcDecisionService
{
    private readonly Random _random = new();
    private readonly ISuspicionCalculator _suspicionCalculator;

    public NpcDecisionService(ISuspicionCalculator suspicionCalculator)
    {
        _suspicionCalculator = suspicionCalculator;
    }

    public NPC? ChooseTarget(GameState game, NPC actor, NpcTargetIntent intent)
    {
        var candidates = game.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != actor.Id)
            .Where(n => n.Role != RoleType.Shopkeeper || game.ShopkeeperProtectionDays <= 0)
            .ToList();

        if (game.Player?.Status == NPCStatus.Alive && game.Player.Id != actor.Id)
        {
            if (intent is NpcTargetIntent.Attack or NpcTargetIntent.Accuse or NpcTargetIntent.Frame or NpcTargetIntent.Spy)
                candidates.Add(game.Player);
            else if (intent is NpcTargetIntent.Protect or NpcTargetIntent.Befriend)
                candidates.Add(game.Player);
        }

        if (intent == NpcTargetIntent.Attack && IsEvil(actor))
        {
            candidates = candidates
                .Where(n => !IsEvil(n))
                .ToList();
        }

        if (intent is NpcTargetIntent.Accuse or NpcTargetIntent.Frame)
        {
            candidates = candidates
                .Where(n => actor.Role != RoleType.Prankster || n.Suspicion.GetValueOrDefault(actor.Id, 0) < 80)
                .ToList();
        }

        if (!candidates.Any()) return null;

        var weighted = candidates
            .Select(c => (Npc: c, Weight: Math.Max(1, ScoreTarget(actor, c, game, intent))))
            .ToList();

        var total = weighted.Sum(w => w.Weight);
        var roll = _random.Next(total);
        var cumulative = 0;

        foreach (var entry in weighted)
        {
            cumulative += entry.Weight;
            if (roll < cumulative) return entry.Npc;
        }

        return weighted[^1].Npc;
    }

    public DialogueContext ResolveDialogueContext(NPC npc, GameState game, string observerId = "player")
    {
        var suspicion = npc.Suspicion.GetValueOrDefault(observerId, 0);
        var trust = npc.Trust.GetValueOrDefault(observerId, 0);
        var fear = npc.Fear.GetValueOrDefault(observerId, 0);
        var roll = _random.Next(100);

        // Evil NPCs are better at masking their intent if they are not too suspicious
        if (IsEvil(npc) && suspicion < 45 && trust < 55)
        {
            if (roll < 30) return DialogueContext.Rumor;
            if (roll < 60) return DialogueContext.Neutral;
        }

        // Fuzzy thresholds to make it less predictable
        if (suspicion > 60 && roll < (suspicion - 10)) return DialogueContext.Aggressive;
        if (suspicion > 40 && roll < (suspicion + 10)) return DialogueContext.Suspicious;
        
        if (fear > 50 && roll < (fear + 15)) return DialogueContext.Fearful;
        if (game.VillageFear > 65 && roll < 40) return DialogueContext.Fearful;

        if (trust > 60 && roll < (trust - 10)) return DialogueContext.Trusting;
        if (trust > 40 && roll < (trust / 2)) return DialogueContext.Trusting;

        if (game.Rumors.Any(r => r.KnownBy.Contains(npc.Id) && r.TargetNpcId == observerId) && roll < 70)
            return DialogueContext.Rumor;

        // Small chance for NPCs to be randomly suspicious or trusting regardless of stats
        if (roll < 5) return DialogueContext.Suspicious;
        if (roll > 95) return DialogueContext.Trusting;

        return DialogueContext.Neutral;
    }

    public void RefreshNpcSuspicion(GameState game, NPC observer, NPC target)
    {
        var calculated = _suspicionCalculator.CalculateSuspicion(observer, target, game);
        observer.Suspicion[target.Id] = calculated;
    }

    public void RefreshAllNpcSuspicions(GameState game)
    {
        var alive = game.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();
        if (game.Player?.Status == NPCStatus.Alive)
            alive.Add(game.Player);

        foreach (var observer in alive)
        {
            foreach (var target in alive.Where(t => t.Id != observer.Id))
            {
                if (!observer.Suspicion.ContainsKey(target.Id))
                    observer.Suspicion[target.Id] = 0;
                RefreshNpcSuspicion(game, observer, target);
            }
        }
    }

    public string GenerateCouncilReaction(NPC npc, GameState game, string trigger, string? targetNpcId = null)
    {
        var target = targetNpcId == null
            ? null
            : game.NPCs.FirstOrDefault(n => n.Id == targetNpcId)
              ?? (game.Player?.Id == targetNpcId ? game.Player : null);

        var targetName = target?.Name ?? "them";
        var topSuspect = GetTopSuspect(npc, game);
        var topName = topSuspect?.Name ?? "someone";

        if (IsEvil(npc) && target != null)
        {
            var trustInTarget = npc.Trust.GetValueOrDefault(target.Id, 50);
            if (trustInTarget > 55 && trigger is "accusation" or "suspicion")
                return $"I won't point fingers at {targetName} without proof. This feels rushed.";
        }

        if (npc.Role == RoleType.Detective)
        {
            return trigger switch
            {
                "accusation" => $"If {targetName} is involved, we need timestamps and witnesses before the record is set.",
                "suspicion" => $"Suspicion on {targetName} is noted. I'll cross-check it against last night's evidence.",
                _ => $"Let's keep this orderly. Every claim about {targetName} goes into the record."
            };
        }

        if (npc.Role == RoleType.Priest)
        {
            return trigger switch
            {
                "accusation" => $"Accusing {targetName} is a grave step. May we hear them in peace before judgment spreads.",
                "suspicion" => $"Doubt weighs on {targetName}, but fear must not become cruelty.",
                _ => "The village needs calm voices today, not more panic."
            };
        }

        if (npc.Role == RoleType.Prosecutor)
        {
            return trigger switch
            {
                "accusation" => $"{targetName} must answer this charge publicly. No more evasions.",
                "suspicion" => $"The pattern points toward {targetName}. I want testimony now.",
                _ => "Someone here is lying. The council will drag it out of them."
            };
        }

        if (npc.Role == RoleType.Doctor)
        {
            return trigger switch
            {
                "accusation" => $"Before we condemn {targetName}, remember they may still need care, not a mob.",
                "suspicion" => $"I've seen {targetName} under stress, but stress is not guilt.",
                _ => "If violence returns tonight, I may not have enough supplies for everyone."
            };
        }

        if (target != null)
        {
            var suspicionOfTarget = npc.Suspicion.GetValueOrDefault(target.Id, 0);
            if (suspicionOfTarget > 60)
            {
                return trigger switch
                {
                    "accusation" => $"I knew something was wrong with {targetName}. This accusation fits what I've seen.",
                    "suspicion" => $"I've had my eye on {targetName} for days. This doesn't surprise me.",
                    _ => $"{targetName} still hasn't explained their movements. That bothers me."
                };
            }

            if (npc.Trust.GetValueOrDefault(target.Id, 50) > 60)
            {
                return trigger switch
                {
                    "accusation" => $"I don't believe {targetName} would do this. We're being led astray.",
                    "suspicion" => $"{targetName} has always been fair to me. I need more than whispers.",
                    _ => $"If we're turning on {targetName}, the real culprit is smiling in silence."
                };
            }
        }

        if (topSuspect != null && trigger != "announcement" && _random.Next(100) < 45)
        {
            return trigger switch
            {
                "accusation" => $"If we're accusing anyone, why not ask where {topName} was last night?",
                "suspicion" => $"Between {targetName} and {topName}, the signs point more toward {topName}.",
                _ => $"Someone should speak about {topName} before we fixate on the wrong person."
            };
        }

        var generic = trigger switch
        {
            "accusation" => new[]
            {
                $"{targetName} should answer that directly.",
                $"That is a serious charge against {targetName}. We need witnesses.",
                $"If {targetName} has an alibi, now is the time."
            },
            "suspicion" => new[]
            {
                $"I noticed something strange about {targetName} too.",
                $"Suspicion is not proof, but it is not nothing.",
                $"{targetName}, explain yourself before this spreads."
            },
            _ => new[]
            {
                "That should be written into the record.",
                "Words are easy. I want proof before I trust any claim.",
                "At least someone is saying it aloud."
            }
        };

        return generic[_random.Next(generic.Length)];
    }

    public string? GenerateDynamicDialogueLine(NPC npc, GameState game, DialogueContext context)
    {
        var fact = npc.KnownFacts.LastOrDefault();
        var topSuspect = GetTopSuspect(npc, game);
        var deaths = game.NPCs.Count(n => n.Status == NPCStatus.Dead);
        
        // Natural openers based on mood rather than role
        var moodTone = context switch
        {
            DialogueContext.Suspicious => "Look, I don't want any trouble, but ",
            DialogueContext.Trusting => "I feel like I can talk to you. ",
            DialogueContext.Fearful => "Did you hear that? This place... ",
            DialogueContext.Aggressive => "You've got a lot of nerve coming here. ",
            DialogueContext.Rumor => "I probably shouldn't repeat this, but ",
            _ => "The air is heavy today. "
        };

        // Add some variety based on NPC role/personality even if not explicitly role-claiming
        if (_random.Next(100) < 30)
        {
            var roleOpener = npc.Role switch
            {
                RoleType.Doctor => "I've been checking on the neighbors. ",
                RoleType.Farmer => "The soil feels cold today. ",
                RoleType.Shopkeeper => "People have been buying a lot of supplies lately. ",
                RoleType.Priest => "I've been praying for the village. ",
                RoleType.Detective => "I'm trying to make sense of the latest reports. ",
                _ => moodTone
            };
            
            if (roleOpener != moodTone)
                return roleOpener + (context is DialogueContext.Fearful ? "I'm worried about what's coming." : "It's all very unsettling.");
        }

        // If highly suspicious of the player, they might hint or lie about their role
        var playerSuspicion = npc.Suspicion.GetValueOrDefault("player", 0);
        if (playerSuspicion > 50 && _random.Next(100) < 40)
        {
            return GenerateRoleClaim(npc, game, isAccusation: false);
        }

        // Check for evidence
        var nearbyEvidence = game.Evidence
            .Where(e => e.Location == npc.CurrentLocation && e.CreatedAt > DateTime.UtcNow.AddHours(-12))
            .ToList();

        if (nearbyEvidence.Any() && _random.Next(100) < 65)
        {
            var evidence = nearbyEvidence[_random.Next(nearbyEvidence.Count)];
            return $"{moodTone}I found {evidence.Type} at {evidence.Location}. It seems out of place, doesn't it?";
        }

        if (fact != null && _random.Next(100) < 75)
        {
            return context switch
            {
                DialogueContext.Suspicious => $"{moodTone}I know about {fact} — and it makes me wonder what else is being hidden.",
                DialogueContext.Trusting => $"{moodTone}I'll tell you this: {fact}. We need to stay ahead of whoever is doing this.",
                DialogueContext.Fearful => $"{moodTone}{fact}. If even that isn't safe, what is?",
                DialogueContext.Rumor => $"The word on the street is {fact}. I'm starting to believe it.",
                DialogueContext.Aggressive => $"{fact}. Don't you dare lie to me about it.",
                _ => $"{moodTone}Have you heard? {fact}."
            };
        }

        if (topSuspect != null && (context is DialogueContext.Suspicious or DialogueContext.Rumor or DialogueContext.Aggressive))
        {
            var suspicionLevel = npc.Suspicion.GetValueOrDefault(topSuspect.Id, 0);
            if (suspicionLevel > 70)
            {
                return $"{moodTone}{topSuspect.Name} is practically screaming their guilt. Look at how they've been acting near {topSuspect.CurrentLocation}.";
            }
            
            return context switch
            {
                DialogueContext.Suspicious => $"I don't like how {topSuspect.Name} has been acting near {topSuspect.CurrentLocation}.",
                DialogueContext.Rumor => $"Word is {topSuspect.Name} was out after dark again. Near {topSuspect.CurrentLocation}, I believe.",
                DialogueContext.Aggressive => $"If someone here is guilty, look at {topSuspect.Name} first. They have no alibi for last night.",
                _ => null
            };
        }

        // Fallback to more natural neutral dialogue instead of null
        return _random.Next(3) switch
        {
            0 => "I don't know who to trust anymore.",
            1 => "Every day feels shorter than the last.",
            _ => "We just need to survive until morning."
        };
    }

    private string GenerateRoleClaim(NPC npc, GameState game, bool isAccusation)
    {
        var isEvil = IsEvil(npc);
        var strategyRoll = _random.Next(100);
        
        // Evil NPCs usually lie to claim a Good/Neutral role
        if (isEvil)
        {
            var fakeRoles = new[] { RoleType.Doctor, RoleType.Farmer, RoleType.Shopkeeper, RoleType.Scholar, RoleType.Priest };
            var fakeRole = fakeRoles[_random.Next(fakeRoles.Length)];
            
            return strategyRoll switch
            {
                < 40 => GetRoleHint(fakeRole), // Hint at fake role
                < 80 => GetOutrightClaim(fakeRole), // Outright claim fake role
                _ => "I'm just a simple villager trying to survive, like everyone else." // Vague
            };
        }

        // Good NPCs might hint, outright claim, or be vague
        return strategyRoll switch
        {
            < 30 => GetRoleHint(npc.Role), // Hint
            < 60 => GetOutrightClaim(npc.Role), // Outright
            _ => "My role in this village is my own business, but I'm no killer." // Vague but honest
        };
    }

    private string GetRoleHint(RoleType role) => role switch
    {
        RoleType.Detective => "I have a knack for finding what people want to stay hidden.",
        RoleType.Doctor => "I've spent more time stitching wounds than making them lately.",
        RoleType.Priest => "I've heard many confessions, and I know the weight of a guilty soul.",
        RoleType.Farmer => "My hands are dirty from the soil, not from blood.",
        RoleType.Shopkeeper => "I see everyone who passes through the market. I notice things.",
        RoleType.Hunter => "I know how to track prey, but I'm not the one hunting people.",
        RoleType.Scholar => "The old records tell us this has happened before. I'm trying to find the pattern.",
        RoleType.Butcher => "I know my way around a blade, but only for the cattle.",
        RoleType.Alchemist => "I deal in spirits and solutions, not in ending lives.",
        _ => "I have my place in this village, and I fulfill my duties."
    };

    private string GetOutrightClaim(RoleType role) => role switch
    {
        RoleType.Detective => "I am the one investigating these crimes. You should be helping me, not accusing me.",
        RoleType.Doctor => "I am the village doctor. My hands are for healing, not for murder.",
        RoleType.Priest => "As your priest, I find your suspicion offensive. I seek only the light.",
        RoleType.Farmer => "I'm just a farmer. I have no reason to hurt anyone.",
        RoleType.Shopkeeper => "I run the shop. Why would I kill my own customers?",
        RoleType.Scholar => "I am a scholar. I seek knowledge, not blood.",
        RoleType.Butcher => "I'm the butcher. I work hard all day, I have no energy for this madness at night.",
        _ => $"I am the {role}. That should be enough for you."
    };

    public string GenerateAlibiLine(NPC npc, GameState game, string accusationReason)
    {
        var latestFact = npc.KnownFacts.LastOrDefault();
        var location = string.IsNullOrWhiteSpace(npc.CurrentLocation) ? "my usual route" : npc.CurrentLocation;

        if (_random.Next(100) < 60)
        {
            return GenerateRoleClaim(npc, game, isAccusation: true) + $" Besides, I was at {location} last night.";
        }

        if (npc.Role == RoleType.Prankster && _random.Next(100) < 45)
            return $"That's a charming theory, but I was making myself visible near {location}. Ask anyone who enjoys being confused.";

        if (IsEvil(npc) && _random.Next(100) < 55)
            return latestFact != null
                ? $"I can explain that. {latestFact}, and it kept me nowhere near what you're claiming."
                : $"I was at {location}, where half the village could have seen me if they were paying attention.";

        if (npc.Role == RoleType.Doctor)
            return $"I was treating people, not hunting them. Check who needed care around {location}.";

        if (npc.Role == RoleType.Priest)
            return $"My work was public enough: prayers, marks, and frightened people at {location}.";

        if (latestFact != null)
            return $"My alibi is simple: {latestFact}. That does not match the accusation.";

        return $"I was near {location}. Suspicion is not proof, and I will not confess to a story with missing hours.";
    }

    public string? ChooseDayAction(NPC npc, GameState game)
    {
        if (npc.Role == RoleType.Doctor && game.NPCs.Any(n => n.Status == NPCStatus.Alive && n.IsIll))
            return "HealNPC";

        if (npc.Role == RoleType.Priest && game.NPCs.Any(n => n.Status == NPCStatus.Alive && n.IsCursed))
            return "RemoveCurse";

        if (npc.Role == RoleType.Alchemist &&
            npc.Inventory.Any(i => string.Equals(i, "potion", StringComparison.OrdinalIgnoreCase)) &&
            game.NPCs.Any(n => n.Status == NPCStatus.Alive && n.Health < 75))
            return "GivePotion";

        var usableItem = game.Items
            .Where(i => i.CurrentHolderId == npc.Id &&
                        (!i.UsableByOwnerOnly || i.OwnerNpcId == npc.Id) &&
                        !string.IsNullOrWhiteSpace(i.UtilityAction) &&
                        i.UtilityAction != "None")
            .OrderByDescending(i => i.Value)
            .FirstOrDefault();

        if (usableItem != null && _random.Next(100) < 35)
            return usableItem.UtilityAction;

        return null;
    }

    public void InitializeNpcGoals(NPC npc, GameState game)
    {
        npc.Goals.Clear();
        var roleDef = RoleDefinitions.Roles.GetValueOrDefault(npc.Role);

        if (roleDef?.WinCondition != null)
            npc.Goals.Add(roleDef.WinCondition);

        npc.Goals.Add(npc.Alignment switch
        {
            Alignment.Good => "Protect the village and expose the killer",
            Alignment.Evil => "Survive council scrutiny while advancing the darkness",
            Alignment.GoodNeutral => "Keep the village stable and survive",
            Alignment.EvilNeutral => "Profit from chaos without becoming the scapegoat",
            Alignment.FixedNeutral => "Keep trade flowing and learn everyone's secrets",
            _ => "Survive the week and read the village correctly"
        });

        if (npc.Role == RoleType.Detective)
            npc.Goals.Add("Gather contradictions before the next council");
        else if (npc.Role == RoleType.Witch || npc.Role == RoleType.Crawler)
            npc.Goals.Add("Redirect suspicion toward a trusted villager");
        else if (npc.Role == RoleType.Voyeur)
            npc.Goals.Add("Trade secrets at the shop without revealing sources");
        else if (npc.Role == RoleType.Prankster)
            npc.Goals.Add("Confuse public certainty and tamper with role reveals twice at most");
    }

    public void AnalyzeStatement(GameState game, string speakerId, string statement)
    {
        // All NPCs analyze the statement made publicly (e.g., at council)
        var speaker = game.NPCs.FirstOrDefault(n => n.Id == speakerId) 
                      ?? (game.Player?.Id == speakerId ? game.Player : null);
        
        if (speaker == null) return;

        var aliveNpcs = game.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != speakerId).ToList();

        foreach (var npc in aliveNpcs)
        {
            // Determine how the NPC interprets the statement based on their role and alignment
            var trustChange = 0;
            var suspicionChange = 0;

            // Analyze statement tone (simplified keyword matching)
            var statement_lower = statement.ToLower();
            var isCautious = statement_lower.Contains("careful") || statement_lower.Contains("wait") || 
                            statement_lower.Contains("evidence") || statement_lower.Contains("calm") ||
                            statement_lower.Contains("orderly");
            var isAccusatory = statement_lower.Contains("suspicious") || statement_lower.Contains("guilty") || 
                              statement_lower.Contains("liar") || statement_lower.Contains("evil") ||
                              statement_lower.Contains("knows more") || statement_lower.Contains("witness");
            var isDefensive = statement_lower.Contains("nothing") || statement_lower.Contains("innocent") || 
                             statement_lower.Contains("silence") || statement_lower.Contains("watching") ||
                             statement_lower.Contains("alibi") || statement_lower.Contains("claim");
            var isCooperative = statement_lower.Contains("together") || statement_lower.Contains("help") || 
                               statement_lower.Contains("cooperate") || statement_lower.Contains("trust") ||
                               statement_lower.Contains("work together");

            // NPCs with different alignments interpret statements differently
            if (npc.Alignment == Alignment.Good || npc.Alignment == Alignment.GoodNeutral)
            {
                // Good NPCs appreciate cooperation and caution
                if (isCooperative) trustChange += _random.Next(3, 8);
                if (isCautious) trustChange += _random.Next(2, 5);
                if (isAccusatory) suspicionChange += _random.Next(-3, 2); // Might be seen as paranoid
                if (isDefensive) suspicionChange += _random.Next(1, 5); // Defensiveness raises suspicion
            }
            else if (npc.Alignment == Alignment.Evil || npc.Alignment == Alignment.EvilNeutral)
            {
                // Evil NPCs are wary of accusatory and cautious people
                if (isAccusatory) suspicionChange += _random.Next(3, 7); // Threats are noted
                if (isCautious) suspicionChange += _random.Next(2, 5); // Careful observers are dangerous
                if (isCooperative) trustChange += _random.Next(1, 3); // Can be manipulated
                if (isDefensive) trustChange += _random.Next(2, 4); // Defensiveness might mean they're vulnerable
            }

            // Role-specific analysis
            if (npc.Role == RoleType.Detective)
            {
                // Detectives pay more attention to defensive or evasive statements
                if (isDefensive) suspicionChange += _random.Next(3, 6);
                if (isCautious) trustChange += _random.Next(2, 4);
            }
            else if (npc.Role == RoleType.Butcher)
            {
                // Butcher looks for weak targets
                if (isDefensive || isCautious) trustChange -= _random.Next(2, 5);
            }

            // Update trust and suspicion
            if (!npc.Trust.ContainsKey(speakerId))
                npc.Trust[speakerId] = 50;
            if (!npc.Suspicion.ContainsKey(speakerId))
                npc.Suspicion[speakerId] = 0;

            npc.Trust[speakerId] = Math.Clamp(npc.Trust[speakerId] + trustChange, 0, 100);
            npc.Suspicion[speakerId] = Math.Clamp(npc.Suspicion[speakerId] + suspicionChange, 0, 100);

            // NPCs might respond to the statement (small chance)
            if (_random.Next(100) < 15 && game.ActiveCouncil != null && game.ActiveCouncil.Statements.Count < 30)
            {
                var responseStatement = GenerateDynamicDialogueLine(npc, game, DialogueContext.CouncilResponse);
                
                if (!string.IsNullOrEmpty(responseStatement))
                {
                    game.ActiveCouncil.Statements.Add(new CouncilStatement
                    {
                        NpcId = npc.Id,
                        Statement = responseStatement,
                        Timestamp = DateTime.UtcNow
                    });

                    game.RecentEvents.Add($"Council Day {game.CurrentDay}: {npc.Name} — {responseStatement}");

                    // Other NPCs analyze this response too (careful with recursion, but limited by chance and count)
                    AnalyzeStatement(game, npc.Id, responseStatement);
                }
            }
        }
    }

    public string? GenerateCouncilReaction(NPC npc, GameState game, List<CouncilStatement> recentDiscourse)
    {
        var last = recentDiscourse.LastOrDefault();
        if (last == null) return null;

        var tone = GetRoleTone(npc.Role);
        var speaker = game.NPCs.FirstOrDefault(n => n.Id == last.NpcId) ?? (game.Player?.Id == last.NpcId ? game.Player : null);
        var statement = last.Statement.ToLower();

        // 1. Direct Rebuttal if accused
        if (last.TargetNpcId == npc.Id && (statement.Contains("found") || statement.Contains("suspicious") || statement.Contains("killer")))
        {
            return $"{tone} {GenerateAlibiLine(npc, game, last.Statement)}";
        }

        // 2. Support Ally / Friend
        if (speaker != null && npc.Trust.GetValueOrDefault(speaker.Id, 50) > 75 && _random.Next(100) < 40)
        {
            return $"{tone} I've known {speaker.Name} for a long time. They're not the kind of person to do what's being suggested.";
        }

        // 3. React to Fear/Panic
        if (statement.Contains("killing") || statement.Contains("dead") || statement.Contains("monster"))
        {
            return npc.Alignment switch
            {
                Alignment.Good => $"{tone} We must not let panic divide us. That is exactly what the evil among us wants.",
                Alignment.Evil => $"{tone} The village is rotting. Maybe it's time we all admitted how dangerous it has become.",
                _ => $"{tone} Fear is spread as easily as disease. We should focus on what we actually saw."
            };
        }

        // 4. Doubt Accuser
        if (last.TargetNpcId != null && npc.Trust.GetValueOrDefault(last.NpcId, 50) < 40 && _random.Next(100) < 30)
        {
            return $"{tone} Why are you so quick to point fingers, {speaker?.Name ?? "friend"}? Perhaps you're trying to hide your own tracks.";
        }

        return null;
    }

    private int ScoreTarget(NPC actor, NPC target, GameState game, NpcTargetIntent intent)
    {
        var suspicion = actor.Suspicion.GetValueOrDefault(target.Id, 0);
        var trust = actor.Trust.GetValueOrDefault(target.Id, 50);
        var fear = actor.Fear.GetValueOrDefault(target.Id, 0);

        return intent switch
        {
            NpcTargetIntent.Attack => ScoreAttack(actor, target, trust, suspicion),
            NpcTargetIntent.Protect => ScoreProtect(actor, target, trust, suspicion, fear),
            NpcTargetIntent.Accuse => Math.Max(1, suspicion + (IsEvil(actor) ? 20 - trust / 2 : 0)),
            NpcTargetIntent.Frame => Math.Max(1, trust + 30 - suspicion),
            NpcTargetIntent.Spy => Math.Max(1, suspicion + 15),
            NpcTargetIntent.Befriend => Math.Max(1, 100 - suspicion + trust / 2),
            _ => 10
        };
    }

    private static int ScoreAttack(NPC actor, NPC target, int trust, int suspicion)
    {
        var weight = 20 + trust / 3;
        if (IsEvil(actor) && !IsEvil(target))
            weight += 25;
        if (target.Id == "player")
            weight += 10;
        weight -= suspicion / 4;
        return Math.Max(5, weight);
    }

    private static int ScoreProtect(NPC actor, NPC target, int trust, int suspicion, int fear)
    {
        var weight = trust / 2 + fear / 3;
        if (IsGood(actor) && !IsEvil(target))
            weight += 20;
        if (suspicion > 70)
            weight += 15;
        if (target.Health < 60)
            weight += 20;
        return Math.Max(5, weight);
    }

    private NPC? GetTopSuspect(NPC observer, GameState game)
    {
        var alive = game.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != observer.Id).ToList();
        return alive
            .OrderByDescending(n => observer.Suspicion.GetValueOrDefault(n.Id, 0))
            .FirstOrDefault();
    }

    private static bool IsEvil(NPC npc) =>
        npc.Alignment is Alignment.Evil or Alignment.EvilNeutral;

    private static bool IsGood(NPC npc) =>
        npc.Alignment is Alignment.Good or Alignment.GoodNeutral or Alignment.FixedNeutral;

    private static string GetRoleTone(RoleType role) => role switch
    {
        RoleType.Detective => "As I see it,",
        RoleType.Priest => "In faith and fear,",
        RoleType.Doctor => "From what I've treated,",
        RoleType.Prosecutor => "Mark my words,",
        RoleType.Witch => "The air feels wrong,",
        RoleType.Butcher => "Between you and me,",
        RoleType.Scholar => "According to the records,",
        RoleType.Hunter => "Out in the wild,",
        RoleType.Farmer => "Working the fields,",
        RoleType.Shopkeeper => "Through the market,",
        RoleType.Thief => "Off the record,",
        RoleType.Voyeur => "I overheard that",
        RoleType.Vagabond => "No one listens, but",
        RoleType.Prankster => "For the record, or off it,",
        _ => "Honestly,"
    };
}
