using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;
using VillageOfAshes.Core.Dialogue;

namespace VillageOfAshes.Infrastructure.Services;

public class DialogueService : IDialogueService
{
    private readonly List<Dialogue> _dialogueDatabase;
    private readonly INpcDecisionService _npcDecisions;

    public DialogueService(INpcDecisionService npcDecisions)
    {
        _npcDecisions = npcDecisions;
        _dialogueDatabase = InitializeDialogues();
    }

    public DialogueExchange GenerateDialogue(NPC npc, GameState gameState, DialogueContext context)
    {
        var isCouncil = gameState.CurrentPhase == GamePhase.VillageCouncil;
        
        var availableDialogues = GetAvailableDialogues(npc, gameState)
            .Where(d => d.Context == context)
            .ToList();

        var dialogue = availableDialogues.Any() 
            ? availableDialogues[new Random().Next(availableDialogues.Count)]
            : GetDefaultDialogue(context);

        var question = isCouncil 
            ? DialoguePools.PassiveResponses[new Random().Next(DialoguePools.PassiveResponses.Count)]
            : _npcDecisions.GenerateDynamicDialogueLine(npc, gameState, context) ?? dialogue.Lines.FirstOrDefault() ?? "...";
            
        var options = GenerateOptions(npc, gameState, context);

        return new DialogueExchange
        {
            Id = Guid.NewGuid().ToString(),
            NpcId = npc.Id,
            Question = question,
            Options = options,
            Timestamp = DateTime.UtcNow
        };
    }

    public void ApplyDialogueEffects(GameState gameState, string npcId, DialogueOption selectedOption)
    {
        var npc = gameState.NPCs.FirstOrDefault(n => n.Id == npcId);
        if (npc == null) return;

        var effects = selectedOption.Effects;

        // Update trust
        if (!npc.Trust.ContainsKey("player"))
            npc.Trust["player"] = 50;
        npc.Trust["player"] = Math.Clamp(npc.Trust["player"] + effects.Trust, 0, 100);

        // Update suspicion
        if (!npc.Suspicion.ContainsKey("player"))
            npc.Suspicion["player"] = 0;
        npc.Suspicion["player"] = Math.Clamp(npc.Suspicion["player"] + effects.Suspicion, 0, 100);

        // Update fear
        if (!npc.Fear.ContainsKey("player"))
            npc.Fear["player"] = 0;
        npc.Fear["player"] = Math.Clamp(npc.Fear["player"] + effects.Fear, 0, 100);

        // Spread rumor if applicable
        if (effects.SpreadRumor)
        {
            var rumor = new Rumor
            {
                Id = Guid.NewGuid().ToString(),
                SourceNpcId = npcId,
                TargetNpcId = "player",
                Context = selectedOption.Text,
                Truthfulness = 50,
                SpreadRate = 40,
                CreatedAt = DateTime.UtcNow,
                KnownBy = new List<string> { npcId }
            };
            gameState.Rumors.Add(rumor);
        }

        // If in Council Phase, propagate to Forum
        if (gameState.CurrentPhase == GamePhase.VillageCouncil && gameState.ActiveCouncil != null)
        {
            // Add Player's choice
            gameState.ActiveCouncil.Statements.Add(new CouncilStatement
            {
                NpcId = "player",
                Statement = selectedOption.Text,
                Timestamp = DateTime.UtcNow
            });
            _npcDecisions.AnalyzeStatement(gameState, "player", selectedOption.Text);

            // Add NPC's response
            gameState.ActiveCouncil.Statements.Add(new CouncilStatement
            {
                NpcId = npcId,
                Statement = selectedOption.NpcResponse,
                Timestamp = DateTime.UtcNow
            });
            _npcDecisions.AnalyzeStatement(gameState, npcId, selectedOption.NpcResponse);
        }

        gameState.ConversationLogs.Add(new ConversationLog
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Participants = new List<string> { npcId, "player" },
            Context = selectedOption.Text,
            Dialogue = new List<DialogueLine>
            {
                new() { Speaker = npc.Name, Line = selectedOption.NpcResponse },
                new() { Speaker = "Player", Line = selectedOption.Text }
            },
            Effects = selectedOption.Effects
        });
        if (gameState.ConversationLogs.Count > 40)
            gameState.ConversationLogs = gameState.ConversationLogs.TakeLast(40).ToList();
    }

    public List<Dialogue> GetAvailableDialogues(NPC npc, GameState gameState)
    {
        // Remove role-specific filtering - all dialogues available to all NPCs
        return _dialogueDatabase
            .Where(d => EvaluateConditions(d.Conditions, npc, gameState))
            .ToList();
    }

    public bool EvaluateConditions(List<string> conditions, NPC npc, GameState gameState)
    {
        foreach (var condition in conditions)
        {
            if (condition.Contains("player_suspicion >"))
            {
                var threshold = int.Parse(condition.Split('>')[1].Trim());
                var suspicion = npc.Suspicion.GetValueOrDefault("player", 0);
                if (suspicion <= threshold) return false;
            }
            else if (condition.Contains("player_trust >"))
            {
                var threshold = int.Parse(condition.Split('>')[1].Trim());
                var trust = npc.Trust.GetValueOrDefault("player", 0);
                if (trust <= threshold) return false;
            }
            else if (condition.Contains("deaths >"))
            {
                var threshold = int.Parse(condition.Split('>')[1].Trim());
                var deaths = gameState.NPCs.Count(n => n.Status == NPCStatus.Dead);
                if (deaths <= threshold) return false;
            }
        }
        return true;
    }

    private List<DialogueOption> GenerateOptions(NPC npc, GameState gameState, DialogueContext context)
    {
        var options = new List<DialogueOption>();
        var isCouncil = gameState.CurrentPhase == GamePhase.VillageCouncil;
        var random = new Random();

        // Add options based on NPC's known facts - Interactive Information Handling
        foreach (var fact in npc.KnownFacts.Take(2))
        {
            options.Add(new DialogueOption
            {
                Id = $"fact_{Guid.NewGuid().ToString().Substring(0, 4)}",
                Text = $"What can you tell me about: {fact}?",
                NpcResponse = isCouncil 
                    ? $"That is already in the record. {fact}"
                    : $"I've already said what I know about that. {fact}. Does it mean something more to you?",
                Effects = new DialogueEffects { Trust = 5, Suspicion = -2 }
            });
        }

        // Add options based on rumors the NPC knows
        foreach (var rumor in npc.Rumors.Take(1))
        {
            var target = gameState.NPCs.FirstOrDefault(n => n.Id == rumor.TargetNpcId);
            if (target != null)
            {
                options.Add(new DialogueOption
                {
                    Id = $"rumor_{Guid.NewGuid().ToString().Substring(0, 4)}",
                    Text = $"I heard something about {target.Name} being {rumor.Context}. Is it true?",
                    NpcResponse = isCouncil 
                        ? $"Rumors have no place in the formal record, but... {rumor.Context} is a pattern I've noted too."
                        : $"Rumors are dangerous, but they often have a seed of truth. {rumor.Context} is what I heard too. Why are you asking?",
                    Effects = new DialogueEffects { Trust = 3, Suspicion = 5, SpreadRumor = true }
                });
            }
        }

        // New Interactive Feature: Share Player's Knowledge
        var player = gameState.Player;
        if (player != null && player.KnownFacts.Any() && context == DialogueContext.Trusting)
        {
            var playerFact = player.KnownFacts.Last();
            options.Add(new DialogueOption
            {
                Id = "share_fact",
                Text = $"I know something you don't: {playerFact}",
                NpcResponse = $"You're sharing this with me? {playerFact}... that changes how I see the village. Thank you.",
                Effects = new DialogueEffects { Trust = 15, Suspicion = -10 }
            });
        }

        switch (context)
        {
            case DialogueContext.Suspicious:
                options.Add(new DialogueOption
                {
                    Id = "sus_accuse",
                    Text = "I think you're the one behind these murders. What do you have to say for yourself?",
                    NpcResponse = isCouncil 
                        ? $"A public accusation! {_npcDecisions.GenerateAlibiLine(npc, gameState, "Public confrontation")}"
                        : _npcDecisions.GenerateAlibiLine(npc, gameState, "Direct confrontation"),
                    Effects = new DialogueEffects { Trust = -25, Suspicion = 30, Fear = 10 }
                });

                // Player Role Claims
                if (!isCouncil)
                {
                    if (player != null)
                    {
                        // Truthful claim (if applicable)
                        options.Add(new DialogueOption
                        {
                            Id = "claim_truth",
                            Text = $"I am the {player.Role}. My work in this village is transparent.",
                            NpcResponse = $"The {player.Role}, you say? If you're telling the truth, we should be allies. If not...",
                            Effects = new DialogueEffects { Trust = 10, Suspicion = -10 }
                        });

                        // Lying/Defensive claims
                        options.Add(new DialogueOption
                        {
                            Id = "claim_lie_doctor",
                            Text = "I am a Doctor. I've been helping the sick, not causing harm.",
                            NpcResponse = player.Role == RoleType.Doctor ? "I see your hands are steady. I believe you." : "A doctor? I haven't seen you near the infirmary once.",
                            Effects = player.Role == RoleType.Doctor ? new DialogueEffects { Trust = 15, Suspicion = -15 } : new DialogueEffects { Trust = -5, Suspicion = 5 }
                        });
                    }
                }

                options.Add(new DialogueOption
                {
                    Id = "sus_1",
                    Text = "I stayed inside my house all night.",
                    NpcResponse = isCouncil ? "A common claim. Let the record show you were at home." : "I hope that's true. For your sake. Many people say they were home when they weren't.",
                    Effects = new DialogueEffects { Trust = 5, Suspicion = -10 }
                });
                break;

            case DialogueContext.Fearful:
                options.Add(new DialogueOption
                {
                    Id = "fear_1",
                    Text = "We'll get through this together. Stay calm.",
                    NpcResponse = isCouncil ? "Calm is what we need. Thank you." : "I want to believe you. I really do. But the village feels like it's dying.",
                    Effects = new DialogueEffects { Trust = 15, Fear = -10 }
                });
                options.Add(new DialogueOption
                {
                    Id = "fear_2",
                    Text = "You should be afraid. We all should.",
                    NpcResponse = isCouncil ? "Fear is not helpful here. Speak only facts." : "You're right... there's nowhere safe left. I can see it in your eyes too.",
                    Effects = new DialogueEffects { Fear = 15, Trust = -5 }
                });
                break;

            case DialogueContext.Trusting:
                options.Add(new DialogueOption
                {
                    Id = "trust_1",
                    Text = "Thank you for trusting me. What did you see?",
                    NpcResponse = isCouncil ? "I'll tell the council what I saw. I saw someone near the well last night." : "I saw someone near the well. They were carrying something heavy... it looked like a body, or maybe just a large sack.",
                    Effects = new DialogueEffects { Trust = 15 }
                });
                options.Add(new DialogueOption
                {
                    Id = "trust_3",
                    Text = "Tell me everything you know.",
                    NpcResponse = isCouncil ? "The formal record already contains my statements." : BuildTrustedReveal(npc, gameState),
                    Effects = new DialogueEffects { Trust = 10, Suspicion = 3 }
                });
                break;

            case DialogueContext.Aggressive:
                options.Add(new DialogueOption
                {
                    Id = "agg_1",
                    Text = "Are you threatening me?",
                    NpcResponse = isCouncil ? "This is a council of law, not threats. Order!" : "Take it however you want. Just stay out of my way, or you'll regret it.",
                    Effects = new DialogueEffects { Suspicion = 15, Trust = -10, Fear = 5 }
                });
                break;

            default: // Neutral
                options.Add(new DialogueOption
                {
                    Id = "neutral_1",
                    Text = "How are you holding up?",
                    NpcResponse = isCouncil ? "Surviving. Like the rest of the village." : "Surviving. That's all any of us can do right now. Every day feels shorter than the last.",
                    Effects = new DialogueEffects { Trust = 5 }
                });
                options.Add(new DialogueOption
                {
                    Id = "neutral_2",
                    Text = "Have you seen anything unusual lately?",
                    NpcResponse = isCouncil ? "Everything is unusual. Speak specifically." : "Everything feels unusual. The air, the shadows... it's all wrong. I keep seeing things out of the corner of my eye.",
                    Effects = new DialogueEffects { Suspicion = 3, Trust = 3 }
                });
                break;
        }

        return options;
    }


    private static string BuildTrustedReveal(NPC npc, GameState gameState)
    {
        var suspect = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != npc.Id)
            .OrderByDescending(n => npc.Suspicion.GetValueOrDefault(n.Id, 0))
            .FirstOrDefault();

        if (suspect != null && npc.Suspicion.GetValueOrDefault(suspect.Id, 0) > 35)
            return $"Between us, {suspect.Name} has been near {suspect.CurrentLocation} at the wrong hours.";

        var fact = npc.KnownFacts.LastOrDefault();
        if (!string.IsNullOrWhiteSpace(fact))
            return $"I'll share what I know: {fact}";

        return "I keep seeing the same faces in the wrong places after dark.";
    }

    private Dialogue GetDefaultDialogue(DialogueContext context)
    {
        var fallbacks = context switch
        {
            DialogueContext.Suspicious => new[] { "I'm watching you.", "You're acting very strange lately.", "I don't have time for your games." },
            DialogueContext.Trusting => new[] { "It's good to see a friendly face.", "I hope we can both get out of this alive.", "Stay safe out there." },
            DialogueContext.Fearful => new[] { "Did you hear that? I'm sure someone's out there.", "I just want this night to end.", "I don't feel safe anywhere anymore." },
            DialogueContext.Aggressive => new[] { "Get away from me.", "I don't want to talk to you.", "Watch your back." },
            DialogueContext.Rumor => new[] { "People are talking, you know.", "Secrets never stay buried for long.", "I heard something... but maybe I shouldn't say." },
            _ => new[] { "Surviving. That's all any of us can do.", "It's a heavy day, isn't it?", "Keep your head down and stay out of trouble." }
        };

        return new Dialogue
        {
            Id = "default_" + context.ToString().ToLower(),
            Context = context,
            Lines = new List<string> { fallbacks[new Random().Next(fallbacks.Length)] },
            Effects = new DialogueEffects()
        };
    }

    private List<Dialogue> InitializeDialogues()
    {
        return new List<Dialogue>
        {
            // NEUTRAL CONTEXT - Casual conversations
            new Dialogue
            {
                Id = "dlg_neutral_01",
                Context = DialogueContext.Neutral,
                Emotion = "Casual",
                Lines = new List<string> { "Good morning. Another day in this cursed village." },
                Effects = new DialogueEffects()
            },
            new Dialogue
            {
                Id = "dlg_neutral_02",
                Context = DialogueContext.Neutral,
                Emotion = "Tired",
                Lines = new List<string> { "I barely slept last night. Did you hear those strange noises?" },
                Effects = new DialogueEffects { Fear = 5 }
            },
            new Dialogue
            {
                Id = "dlg_neutral_06",
                Context = DialogueContext.Neutral,
                Emotion = "Wary",
                Lines = new List<string> { "The shadows are getting longer. We should all be more careful." },
                Effects = new DialogueEffects { Suspicion = 2 }
            },
            new Dialogue
            {
                Id = "dlg_neutral_07",
                Context = DialogueContext.Neutral,
                Emotion = "Stressed",
                Lines = new List<string> { "Everyone is looking at everyone else like they're the killer. It's exhausting." },
                Effects = new DialogueEffects { Trust = -2 }
            },
            
            // SUSPICIOUS CONTEXT - Observations without revealing roles
            new Dialogue
            {
                Id = "dlg_suspicious_09",
                Context = DialogueContext.Suspicious,
                Emotion = "Curious",
                Lines = new List<string> { "I've seen you around quite a bit lately. What exactly is your business here?" },
                Effects = new DialogueEffects { Suspicion = 5 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_10",
                Context = DialogueContext.Suspicious,
                Emotion = "Wary",
                Lines = new List<string> { "People are saying you've been asking a lot of questions. Some people don't like that." },
                Effects = new DialogueEffects { Suspicion = 7 }
            },
            
            // FEARFUL CONTEXT - Panic and anxiety
            new Dialogue
            {
                Id = "dlg_fearful_07",
                Context = DialogueContext.Fearful,
                Emotion = "Anxious",
                Lines = new List<string> { "Do you think the council can really protect us? It feels like we're just waiting for the end." },
                Effects = new DialogueEffects { Fear = 10 }
            },
            
            // TRUSTING CONTEXT - Sharing information
            new Dialogue
            {
                Id = "dlg_trusting_06",
                Context = DialogueContext.Trusting,
                Emotion = "Friendly",
                Lines = new List<string> { "It's good to talk to someone who doesn't seem to have a hidden agenda. At least, I hope you don't." },
                Effects = new DialogueEffects { Trust = 5 }
            },
            
            // AGGRESSIVE CONTEXT - Hostility
            new Dialogue
            {
                Id = "dlg_aggressive_05",
                Context = DialogueContext.Aggressive,
                Emotion = "Dismissive",
                Lines = new List<string> { "I have nothing to say to you. Leave me be." },
                Effects = new DialogueEffects { Trust = -5 }
            },
            new Dialogue
            {
                Id = "dlg_neutral_03",
                Context = DialogueContext.Neutral,
                Emotion = "Concerned",
                Lines = new List<string> { "How are you holding up? These are difficult times." },
                Effects = new DialogueEffects { Trust = 3 }
            },
            new Dialogue
            {
                Id = "dlg_neutral_04",
                Context = DialogueContext.Neutral,
                Emotion = "Weary",
                Lines = new List<string> { "I can't remember the last time I felt safe here." },
                Effects = new DialogueEffects { Fear = 3 }
            },
            new Dialogue
            {
                Id = "dlg_neutral_05",
                Context = DialogueContext.Neutral,
                Emotion = "Reflective",
                Lines = new List<string> { "This village used to be peaceful. What happened to us?" },
                Effects = new DialogueEffects()
            },
            
            // SUSPICIOUS CONTEXT - Observations without revealing roles
            new Dialogue
            {
                Id = "dlg_suspicious_01",
                Context = DialogueContext.Suspicious,
                Emotion = "Questioning",
                Conditions = new List<string> { "player_suspicion > 40" },
                Lines = new List<string> { "Where were you last night? I didn't see you at home." },
                Effects = new DialogueEffects { Suspicion = 10 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_02",
                Context = DialogueContext.Suspicious,
                Emotion = "Accusatory",
                Conditions = new List<string> { "player_suspicion > 50" },
                Lines = new List<string> { "I saw you near the forest after dark. What were you doing there?" },
                Effects = new DialogueEffects { Suspicion = 15 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_03",
                Context = DialogueContext.Suspicious,
                Emotion = "Doubtful",
                Conditions = new List<string> { "player_suspicion > 30" },
                Lines = new List<string> { "Your story doesn't add up. You said you were home, but someone saw you outside." },
                Effects = new DialogueEffects { Suspicion = 12, Trust = -5 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_04",
                Context = DialogueContext.Suspicious,
                Emotion = "Observant",
                Lines = new List<string> { "I noticed you've been avoiding the council meetings. Why is that?" },
                Effects = new DialogueEffects { Suspicion = 8 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_05",
                Context = DialogueContext.Suspicious,
                Emotion = "Wary",
                Conditions = new List<string> { "player_suspicion > 45" },
                Lines = new List<string> { "You've been acting strange lately. Is there something you're not telling us?" },
                Effects = new DialogueEffects { Suspicion = 10 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_06",
                Context = DialogueContext.Suspicious,
                Emotion = "Confrontational",
                Conditions = new List<string> { "player_suspicion > 55" },
                Lines = new List<string> { "I found blood near your house. Care to explain?" },
                Effects = new DialogueEffects { Suspicion = 18, Fear = 5 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_07",
                Context = DialogueContext.Suspicious,
                Emotion = "Investigative",
                Lines = new List<string> { "Several people mentioned seeing you wandering at night. What's going on?" },
                Effects = new DialogueEffects { Suspicion = 12 }
            },
            new Dialogue
            {
                Id = "dlg_suspicious_08",
                Context = DialogueContext.Suspicious,
                Emotion = "Defensive",
                Conditions = new List<string> { "player_suspicion > 60" },
                Lines = new List<string> { "Don't think I haven't noticed. You're always around when something bad happens." },
                Effects = new DialogueEffects { Suspicion = 20, Trust = -10 }
            },
            
            // FEARFUL CONTEXT - Panic and anxiety
            new Dialogue
            {
                Id = "dlg_fearful_01",
                Context = DialogueContext.Fearful,
                Emotion = "Panicked",
                Conditions = new List<string> { "deaths > 0" },
                Lines = new List<string> { "Someone is killing us! We need to do something before we're all dead!" },
                Effects = new DialogueEffects { Fear = 15 }
            },
            new Dialogue
            {
                Id = "dlg_fearful_02",
                Context = DialogueContext.Fearful,
                Emotion = "Terrified",
                Conditions = new List<string> { "deaths > 1" },
                Lines = new List<string> { "I can't sleep anymore. Every sound makes me jump. Who's next?" },
                Effects = new DialogueEffects { Fear = 18 }
            },
            new Dialogue
            {
                Id = "dlg_fearful_03",
                Context = DialogueContext.Fearful,
                Emotion = "Desperate",
                Lines = new List<string> { "We should leave this village. Nothing good will come from staying here." },
                Effects = new DialogueEffects { Fear = 12 }
            },
            new Dialogue
            {
                Id = "dlg_fearful_04",
                Context = DialogueContext.Fearful,
                Emotion = "Paranoid",
                Conditions = new List<string> { "deaths > 0" },
                Lines = new List<string> { "I don't trust anyone anymore. How do we know who the killer is?" },
                Effects = new DialogueEffects { Fear = 10, Suspicion = 5 }
            },
            new Dialogue
            {
                Id = "dlg_fearful_05",
                Context = DialogueContext.Fearful,
                Emotion = "Anxious",
                Lines = new List<string> { "I heard screaming last night. Did you hear it too?" },
                Effects = new DialogueEffects { Fear = 8 }
            },
            new Dialogue
            {
                Id = "dlg_fearful_06",
                Context = DialogueContext.Fearful,
                Emotion = "Shaken",
                Conditions = new List<string> { "deaths > 2" },
                Lines = new List<string> { "Three people dead already. When will this nightmare end?" },
                Effects = new DialogueEffects { Fear = 20 }
            },
            
            // TRUSTING CONTEXT - Sharing information
            new Dialogue
            {
                Id = "dlg_trusting_01",
                Context = DialogueContext.Trusting,
                Emotion = "Confiding",
                Conditions = new List<string> { "player_trust > 60" },
                Lines = new List<string> { "I saw something last night. I think I know who might be involved." },
                Effects = new DialogueEffects { Trust = 10 }
            },
            new Dialogue
            {
                Id = "dlg_trusting_02",
                Context = DialogueContext.Trusting,
                Emotion = "Relieved",
                Conditions = new List<string> { "player_trust > 55" },
                Lines = new List<string> { "I'm glad I can talk to you. I don't know who else to trust." },
                Effects = new DialogueEffects { Trust = 12 }
            },
            new Dialogue
            {
                Id = "dlg_trusting_03",
                Context = DialogueContext.Trusting,
                Emotion = "Grateful",
                Conditions = new List<string> { "player_trust > 50" },
                Lines = new List<string> { "Thank you for being honest with me. We need to stick together." },
                Effects = new DialogueEffects { Trust = 8 }
            },
            new Dialogue
            {
                Id = "dlg_trusting_04",
                Context = DialogueContext.Trusting,
                Emotion = "Conspiratorial",
                Conditions = new List<string> { "player_trust > 65" },
                Lines = new List<string> { "Between you and me, I think I know who we should be watching." },
                Effects = new DialogueEffects { Trust = 15, Suspicion = 5 }
            },
            new Dialogue
            {
                Id = "dlg_trusting_05",
                Context = DialogueContext.Trusting,
                Emotion = "Supportive",
                Conditions = new List<string> { "player_trust > 70" },
                Lines = new List<string> { "Whatever happens, I've got your back. We'll figure this out together." },
                Effects = new DialogueEffects { Trust = 18 }
            },
            
            // AGGRESSIVE CONTEXT - Hostility without revealing role
            new Dialogue
            {
                Id = "dlg_aggressive_01",
                Context = DialogueContext.Aggressive,
                Emotion = "Threatening",
                Conditions = new List<string> { "player_suspicion > 70" },
                Lines = new List<string> { "You better watch yourself. Accidents happen around here." },
                Effects = new DialogueEffects { Fear = 15, Suspicion = 15 }
            },
            new Dialogue
            {
                Id = "dlg_aggressive_02",
                Context = DialogueContext.Aggressive,
                Emotion = "Hostile",
                Conditions = new List<string> { "player_suspicion > 65" },
                Lines = new List<string> { "Stay away from me. I know what you've been doing." },
                Effects = new DialogueEffects { Trust = -15, Suspicion = 12 }
            },
            new Dialogue
            {
                Id = "dlg_aggressive_03",
                Context = DialogueContext.Aggressive,
                Emotion = "Angry",
                Conditions = new List<string> { "player_suspicion > 60" },
                Lines = new List<string> { "Don't lie to me! I saw you with my own eyes!" },
                Effects = new DialogueEffects { Suspicion = 18, Trust = -10 }
            },
            new Dialogue
            {
                Id = "dlg_aggressive_04",
                Context = DialogueContext.Aggressive,
                Emotion = "Confrontational",
                Lines = new List<string> { "You think you're clever, but I'm watching you." },
                Effects = new DialogueEffects { Suspicion = 10, Fear = 8 }
            },
            
            // RUMOR CONTEXT - Spreading information
            new Dialogue
            {
                Id = "dlg_rumor_01",
                Context = DialogueContext.Rumor,
                Emotion = "Gossipy",
                Lines = new List<string> { "I heard from someone that there was blood found near the church." },
                Effects = new DialogueEffects { SpreadRumor = true }
            },
            new Dialogue
            {
                Id = "dlg_rumor_02",
                Context = DialogueContext.Rumor,
                Emotion = "Informative",
                Lines = new List<string> { "People are saying they saw someone sneaking around last night." },
                Effects = new DialogueEffects { Suspicion = 5, SpreadRumor = true }
            },
            new Dialogue
            {
                Id = "dlg_rumor_03",
                Context = DialogueContext.Rumor,
                Emotion = "Uncertain",
                Lines = new List<string> { "I'm not sure if it's true, but I heard someone was seen near the victim's house." },
                Effects = new DialogueEffects { SpreadRumor = true }
            },
            new Dialogue
            {
                Id = "dlg_rumor_04",
                Context = DialogueContext.Rumor,
                Emotion = "Speculative",
                Lines = new List<string> { "Word is going around that someone's been stealing food at night." },
                Effects = new DialogueEffects { SpreadRumor = true }
            },
            new Dialogue
            {
                Id = "dlg_rumor_05",
                Context = DialogueContext.Rumor,
                Emotion = "Concerned",
                Lines = new List<string> { "Everyone's talking about the footprints found near the forest." },
                Effects = new DialogueEffects { Fear = 5, SpreadRumor = true }
            }
        };
    }
}
