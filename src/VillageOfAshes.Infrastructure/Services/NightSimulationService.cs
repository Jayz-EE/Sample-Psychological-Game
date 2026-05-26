using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class NightSimulationService : INightSimulationService
{
    private readonly Random _random = new();
    private readonly IObservationService _observationService;
    private readonly IBehaviorAnalysisService _behaviorAnalysisService;
    private readonly ISuspicionCalculator _suspicionCalculator;

    public NightSimulationService(
        IObservationService observationService,
        IBehaviorAnalysisService behaviorAnalysisService,
        ISuspicionCalculator suspicionCalculator)
    {
        _observationService = observationService;
        _behaviorAnalysisService = behaviorAnalysisService;
        _suspicionCalculator = suspicionCalculator;
    }

    public async Task<NightSimulationResult> ExecuteNightPhase(GameState gameState)
    {
        var result = new NightSimulationResult();
        
        // 1. Assign role actions
        var roleActions = AssignRoleActions(gameState);
        
        // 2. Execute movements
        ExecuteMovements(gameState, result);
        
        // 3. Execute role-specific actions
        await ExecuteRoleActions(gameState, roleActions, result);
        
        // 4. Generate encounters
        GenerateEncounters(gameState, result);
        
        // 5. Generate witness observations from the resolved movements/actions
        GenerateObservations(gameState, result);
        
        // 6. Spawn evidence
        SpawnEvidence(gameState, result);
        
        // 7. Generate rumors
        GenerateRumors(gameState, result);
        
        // 8. Apply public suspicion changes from newly generated information
        ApplyInformationEffects(gameState, result);
        
        // 9. Update NPC states
        UpdateNPCStates(gameState);
        
        return result;
    }

    private Dictionary<string, List<string>> AssignRoleActions(GameState gameState)
    {
        var actions = new Dictionary<string, List<string>>();
        
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            var roleActions = new List<string>();
            
            switch (npc.Role)
            {
                case RoleType.Detective:
                    if (_random.Next(100) < 60)
                        roleActions.Add("TrackMovement");
                    break;
                    
                case RoleType.Doctor:
                    if (_random.Next(100) < 50)
                        roleActions.Add("ProtectNPC");
                    break;

                case RoleType.Priest:
                    roleActions.Add(_random.Next(100) < 50 ? "NightPrayer" : "PurifyArea");
                    break;

                case RoleType.Prosecutor:
                    if (_random.Next(100) < 45)
                        roleActions.Add("CollectRecords");
                    break;
                    
                case RoleType.Butcher:
                    if (_random.Next(100) < 70)
                        roleActions.Add("KillNPC");
                    if (_random.Next(100) < 35 && roleActions.Count < 2)
                        roleActions.Add("HarvestFlesh");
                    break;

                case RoleType.Witch:
                    roleActions.Add(_random.Next(100) < 50 ? "CurseNPC" : "PlantFalseEvidence");
                    break;

                case RoleType.Crawler:
                    roleActions.Add(_random.Next(100) < 55 ? "StalkTarget" : "CrawlThroughVillage");
                    if (_random.Next(100) < 35 && roleActions.Count < 2)
                        roleActions.Add("AmbushNPC");
                    break;

                case RoleType.Headless:
                    roleActions.Add(_random.Next(100) < 50 ? "HauntArea" : "TerrorizeNPC");
                    break;
                    
                case RoleType.Vagabond:
                    roleActions.Add(_random.Next(100) < 50 ? "SleepOutdoors" : "WanderVillage");
                    break;
                    
                case RoleType.Farmer:
                    roleActions.Add(_random.Next(100) < 70 ? "ProtectCrops" : "HideSupplies");
                    break;

                case RoleType.Alchemist:
                    roleActions.Add(_random.Next(100) < 50 ? "Experiment" : "DistillElixir");
                    break;

                case RoleType.Hunter:
                    roleActions.Add(_random.Next(100) < 50 ? "PatrolForest" : "SetTraps");
                    break;

                case RoleType.Scholar:
                    roleActions.Add(_random.Next(100) < 50 ? "SecretObservation" : "HiddenResearch");
                    break;

                case RoleType.Thief:
                    roleActions.Add(_random.Next(100) < 50 ? "StealResources" : "SneakIntoHouses");
                    break;

                case RoleType.Voyeur:
                    roleActions.Add(_random.Next(100) < 50 ? "SpyOnNPC" : "ObserveMeetings");
                    break;

                case RoleType.Shopkeeper:
                    roleActions.Add(_random.Next(100) < 60 ? "RecordVisitors" : "LockShop");
                    break;
            }
            
            // Ensure max 2 actions
            if (roleActions.Count > 2)
                roleActions = roleActions.Take(2).ToList();

            npc.PhaseActionCount = roleActions.Count;
            actions[npc.Id] = roleActions;
        }
        
        return actions;
    }

    private void ExecuteMovements(GameState gameState, NightSimulationResult result)
    {
        var locations = new[] { "VillageCenter", "Forest", "Church", "Farmland", "AbandonedShed" };
        
        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            // Some NPCs move at night
            if (_random.Next(100) < 40)
            {
                var newLocation = locations[_random.Next(locations.Length)];
                result.NPCMovements[npc.Id] = newLocation;
                npc.CurrentLocation = newLocation;
                _behaviorAnalysisService.RecordBehavior(gameState, npc.Id, "moved at night", newLocation);
            }
        }
    }

    private async Task ExecuteRoleActions(GameState gameState, Dictionary<string, List<string>> roleActions, NightSimulationResult result)
    {
        foreach (var (npcId, actions) in roleActions)
        {
            var npc = gameState.NPCs.FirstOrDefault(n => n.Id == npcId);
            if (npc == null) continue;
            
            foreach (var action in actions)
            {
                switch (action)
                {
                    case "KillNPC":
                        ExecuteKillAction(gameState, npc, result);
                        break;
                        
                    case "ProtectNPC":
                        ExecuteProtectAction(gameState, npc, result);
                        break;
                        
                    case "TrackMovement":
                    case "SecretObservation":
                    case "SpyOnNPC":
                    case "ObserveMeetings":
                    case "CollectRecords":
                    case "RecordVisitors":
                        ExecuteTrackAction(gameState, npc, result);
                        break;

                    case "NightPrayer":
                        gameState.VillageFear = Math.Max(0, gameState.VillageFear - 8);
                        result.Events.Add("Night prayers calmed the village");
                        AddEvidence(result, EvidenceType.HolyMarkings, npc.CurrentLocation, npc.Id, 25);
                        break;

                    case "PurifyArea":
                        gameState.VillageCorruption = Math.Max(0, gameState.VillageCorruption - 8);
                        result.Events.Add("A corrupted area was purified");
                        AddEvidence(result, EvidenceType.IncenseSmoke, npc.CurrentLocation, npc.Id, 35);
                        break;

                    case "CurseNPC":
                        ExecuteCurseAction(gameState, npc, result);
                        break;

                    case "PlantFalseEvidence":
                        AddEvidence(result, EvidenceType.RitualMarkings, npc.CurrentLocation, npc.Id, 55);
                        gameState.VillageCorruption = Math.Min(100, gameState.VillageCorruption + 5);
                        result.Events.Add("False evidence was planted during the night");
                        break;

                    case "StalkTarget":
                        ExecuteFearAction(gameState, npc, result, EvidenceType.ClawMarks, "Someone was stalked in the dark");
                        break;

                    case "AmbushNPC":
                        ExecuteKillAction(gameState, npc, result);
                        AddEvidence(result, EvidenceType.ClawMarks, npc.CurrentLocation, npc.Id, 70);
                        break;

                    case "CrawlThroughVillage":
                        gameState.VillageFear = Math.Min(100, gameState.VillageFear + 12);
                        AddEvidence(result, EvidenceType.DisturbedDirt, npc.CurrentLocation, npc.Id, 45);
                        result.Events.Add("Something crawled through the village");
                        break;

                    case "HauntArea":
                        gameState.VillageFear = Math.Min(100, gameState.VillageFear + 10);
                        gameState.VillageCorruption = Math.Min(100, gameState.VillageCorruption + 5);
                        AddEvidence(result, EvidenceType.ColdSpots, npc.CurrentLocation, npc.Id, 45);
                        break;

                    case "TerrorizeNPC":
                        ExecuteFearAction(gameState, npc, result, EvidenceType.ColdSpots, "Someone was terrorized at night");
                        break;

                    case "SleepOutdoors":
                    case "WanderVillage":
                    case "SneakAround":
                        _behaviorAnalysisService.RecordBehavior(gameState, npc.Id, "seen outside at night", npc.CurrentLocation);
                        AddEvidence(result, EvidenceType.Footprints, npc.CurrentLocation, npc.Id, 35);
                        break;

                    case "ProtectCrops":
                        gameState.FoodSupply = Math.Min(200, gameState.FoodSupply + 4);
                        AddEvidence(result, EvidenceType.Footprints, "Farmland", npc.Id, 20);
                        break;

                    case "HideSupplies":
                        gameState.FoodSupply = Math.Max(0, gameState.FoodSupply - 2);
                        AddEvidence(result, EvidenceType.BuriedSacks, "Farmland", npc.Id, 35);
                        break;

                    case "Experiment":
                    case "DistillElixir":
                        AddEvidence(result, EvidenceType.ChemicalResidue, npc.CurrentLocation, npc.Id, 40);
                        if (_random.Next(100) < 35) gameState.VillageFear = Math.Min(100, gameState.VillageFear + 5);
                        break;

                    case "PatrolForest":
                    case "SetTraps":
                    case "FollowSounds":
                        AddEvidence(result, EvidenceType.Footprints, "Forest", npc.Id, 30);
                        _behaviorAnalysisService.RecordBehavior(gameState, npc.Id, "patrolled at night", "Forest");
                        break;

                    case "StealResources":
                    case "SneakIntoHouses":
                    case "PickpocketNPC":
                        gameState.EconomyStability = Math.Max(0, gameState.EconomyStability - 5);
                        AddEvidence(result, EvidenceType.BrokenLock, npc.CurrentLocation, npc.Id, 50);
                        break;

                    case "LockShop":
                        gameState.EconomyStability = Math.Min(100, gameState.EconomyStability + 2);
                        break;
                }
            }
        }
        
        await Task.CompletedTask;
    }


    private void ExecuteCurseAction(GameState gameState, NPC actor, NightSimulationResult result)
    {
        var target = GetRandomTarget(gameState, actor);
        if (target == null) return;

        target.Fear[actor.Id] = Math.Clamp(target.Fear.GetValueOrDefault(actor.Id, 0) + 25, 0, 100);
        target.Health = Math.Max(1, target.Health - 10);
        gameState.VillageCorruption = Math.Min(100, gameState.VillageCorruption + 8);
        AddEvidence(result, EvidenceType.RitualMarkings, target.CurrentLocation, actor.Id, 60);
        result.Events.Add($"{target.Name} was weakened by a curse");
    }

    private void ExecuteFearAction(GameState gameState, NPC actor, NightSimulationResult result, EvidenceType trace, string eventText)
    {
        var target = GetRandomTarget(gameState, actor);
        if (target == null) return;

        target.Fear[actor.Id] = Math.Clamp(target.Fear.GetValueOrDefault(actor.Id, 0) + 20, 0, 100);
        gameState.VillageFear = Math.Min(100, gameState.VillageFear + 8);
        AddEvidence(result, trace, target.CurrentLocation, actor.Id, 50);
        result.Events.Add(eventText);
    }

    private NPC? GetRandomTarget(GameState gameState, NPC actor)
    {
        var potentialTargets = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != actor.Id)
            .Where(n => n.Role != RoleType.Shopkeeper || gameState.ShopkeeperProtectionDays <= 0)
            .ToList();

        return potentialTargets.Any() ? potentialTargets[_random.Next(potentialTargets.Count)] : null;
    }

    private static void AddEvidence(NightSimulationResult result, EvidenceType type, string location, string createdBy, int visibility)
    {
        result.GeneratedEvidence.Add(new Evidence
        {
            Id = Guid.NewGuid().ToString(),
            Type = type,
            Location = location,
            CreatedBy = createdBy,
            Visibility = visibility,
            DecayTime = 2,
            CreatedAt = DateTime.UtcNow
        });
    }

    private void ExecuteKillAction(GameState gameState, NPC butcher, NightSimulationResult result)
    {
        var potentialTargets = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != butcher.Id)
            .Where(n => n.Role != RoleType.Shopkeeper || gameState.ShopkeeperProtectionDays <= 0)
            .ToList();
            
        if (potentialTargets.Any())
        {
            var target = potentialTargets[_random.Next(potentialTargets.Count)];
            
            // Check if target is protected
            var protectors = gameState.NPCs.Where(n => 
                n.Role == RoleType.Doctor && 
                n.Status == NPCStatus.Alive &&
                n.NightActions.Contains($"Protect:{target.Id}")).ToList();
                
            if (!protectors.Any())
            {
                target.Status = NPCStatus.Dead;
                result.Deaths.Add(target.Id);
                result.Events.Add($"{target.Name} was killed during the night");
                _behaviorAnalysisService.RecordBehavior(gameState, butcher.Id, "violent action at night", butcher.CurrentLocation);
                
                // Generate blood evidence
                var evidence = new Evidence
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = EvidenceType.Blood,
                    Location = target.CurrentLocation,
                    CreatedBy = butcher.Id,
                    Visibility = 80,
                    DecayTime = 2,
                    LinkedRole = RoleType.Butcher,
                    CreatedAt = DateTime.UtcNow
                };
                result.GeneratedEvidence.Add(evidence);
            }
            else
            {
                result.Events.Add($"{target.Name} was protected during the night");
            }
        }
    }

    private void ExecuteProtectAction(GameState gameState, NPC doctor, NightSimulationResult result)
    {
        var potentialTargets = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != doctor.Id)
            .ToList();
            
        if (potentialTargets.Any())
        {
            var target = potentialTargets[_random.Next(potentialTargets.Count)];
            doctor.NightActions.Add($"Protect:{target.Id}");
            _behaviorAnalysisService.RecordBehavior(gameState, doctor.Id, "helped someone at night", doctor.CurrentLocation);
            result.Events.Add($"{doctor.Name} protected someone during the night");
        }
    }

    private void ExecuteTrackAction(GameState gameState, NPC detective, NightSimulationResult result)
    {
        var potentialTargets = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive && n.Id != detective.Id)
            .ToList();
            
        if (potentialTargets.Any())
        {
            var target = potentialTargets[_random.Next(potentialTargets.Count)];
            detective.KnownFacts.Add($"{target.Name} was at {target.CurrentLocation} during night {gameState.CurrentDay}");
            _behaviorAnalysisService.RecordBehavior(gameState, detective.Id, "tracked movement at night", detective.CurrentLocation);
            result.Events.Add($"{detective.Name} tracked someone's movements");
        }
    }

    private void GenerateEncounters(GameState gameState, NightSimulationResult result)
    {
        var locationGroups = gameState.NPCs
            .Where(n => n.Status == NPCStatus.Alive)
            .GroupBy(n => n.CurrentLocation)
            .Where(g => g.Count() > 1);
            
        foreach (var group in locationGroups)
        {
            var npcs = group.ToList();
            result.Events.Add($"Multiple people were seen near {group.Key}");
            
            // Generate footprint evidence
            var evidence = new Evidence
            {
                Id = Guid.NewGuid().ToString(),
                Type = EvidenceType.Footprints,
                Location = group.Key,
                CreatedBy = npcs[0].Id,
                Visibility = 60,
                DecayTime = 1,
                CreatedAt = DateTime.UtcNow
            };
            result.GeneratedEvidence.Add(evidence);
        }
    }


    private void GenerateObservations(GameState gameState, NightSimulationResult result)
    {
        var observations = _observationService.GenerateNightObservations(gameState);
        foreach (var observation in observations)
        {
            var observer = gameState.NPCs.FirstOrDefault(n => n.Id == observation.ObserverId);
            var target = gameState.NPCs.FirstOrDefault(n => n.Id == observation.TargetId);
            if (observer == null || target == null) continue;

            var suspicion = _observationService.CalculateObservationSuspicion(observation, gameState);
            if (!observer.Suspicion.ContainsKey(target.Id))
                observer.Suspicion[target.Id] = 0;

            observer.Suspicion[target.Id] = Math.Clamp(observer.Suspicion[target.Id] + suspicion, 0, 100);
            _behaviorAnalysisService.RecordBehavior(gameState, target.Id, "seen at night", target.CurrentLocation);
        }
    }

    private void ApplyInformationEffects(GameState gameState, NightSimulationResult result)
    {
        foreach (var evidence in result.GeneratedEvidence)
        {
            _suspicionCalculator.UpdateSuspicionFromEvidence(gameState, evidence);
        }

        foreach (var rumor in result.GeneratedRumors)
        {
            _suspicionCalculator.UpdateSuspicionFromRumor(gameState, rumor);
        }
    }

    private void SpawnEvidence(GameState gameState, NightSimulationResult result)
    {
        // Additional random evidence spawning
        if (_random.Next(100) < 30)
        {
            var locations = new[] { "VillageCenter", "Forest", "Church", "Farmland" };
            var evidence = new Evidence
            {
                Id = Guid.NewGuid().ToString(),
                Type = (EvidenceType)_random.Next(Enum.GetValues<EvidenceType>().Length),
                Location = locations[_random.Next(locations.Length)],
                Visibility = _random.Next(40, 90),
                DecayTime = _random.Next(1, 3),
                CreatedAt = DateTime.UtcNow
            };
            result.GeneratedEvidence.Add(evidence);
        }
    }

    private void GenerateRumors(GameState gameState, NightSimulationResult result)
    {
        var aliveNpcs = gameState.NPCs.Where(n => n.Status == NPCStatus.Alive).ToList();
        
        if (aliveNpcs.Count >= 2 && _random.Next(100) < 50)
        {
            var source = aliveNpcs[_random.Next(aliveNpcs.Count)];
            var target = aliveNpcs.Where(n => n.Id != source.Id).ToList()[_random.Next(aliveNpcs.Count - 1)];
            
            // Ambiguous contexts that don't reveal roles
            var contexts = new[] 
            { 
                "Seen near the forest at night",
                "Acting suspiciously",
                "Heard strange noises from their direction",
                "Wandering outside after dark",
                "Seen near someone's house",
                "Found near the crime scene",
                "Acting nervous and avoiding people",
                "Carrying something in the dark",
                "Whispering with someone secretly",
                "Leaving their house at odd hours",
                "Seen with blood on their clothes",
                "Acting differently lately",
                "Avoiding the council meetings",
                "Lying about their whereabouts",
                "Seen arguing with the victim",
                "Found in a restricted area",
                "Behaving erratically",
                "Seen running from somewhere",
                "Hiding something",
                "Making suspicious trades"
            };
            
            var rumor = new Rumor
            {
                Id = Guid.NewGuid().ToString(),
                SourceNpcId = source.Id,
                TargetNpcId = target.Id,
                Truthfulness = _random.Next(30, 80),
                SpreadRate = _random.Next(20, 60),
                Context = contexts[_random.Next(contexts.Length)],
                CreatedAt = DateTime.UtcNow,
                KnownBy = new List<string> { source.Id }
            };
            result.GeneratedRumors.Add(rumor);
        }
    }

    private void UpdateNPCStates(GameState gameState)
    {
        var aliveCount = gameState.NPCs.Count(n => n.Status == NPCStatus.Alive);
        gameState.FoodSupply = Math.Max(0, gameState.FoodSupply - aliveCount);
        if (gameState.FoodSupply == 0)
            gameState.VillageFear = Math.Min(100, gameState.VillageFear + 10);

        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            npc.Hunger = gameState.FoodSupply > 0 ? Math.Min(100, npc.Hunger + 5) : Math.Min(100, npc.Hunger + 20);
            if (npc.Hunger >= 90) npc.Health = Math.Max(1, npc.Health - 10);
            npc.NightActions.Clear();
        }

        var shopkeeper = gameState.NPCs.FirstOrDefault(n => n.Role == RoleType.Shopkeeper);
        gameState.ShopkeeperAlive = shopkeeper?.Status == NPCStatus.Alive;
        if (!gameState.ShopkeeperAlive)
        {
            gameState.EconomyStability = Math.Max(0, gameState.EconomyStability - 15);
            gameState.BlackMarketActive = true;
            gameState.VillageFear = Math.Min(100, gameState.VillageFear + 8);
        }
        else if (gameState.ShopkeeperProtectionDays > 0)
        {
            gameState.ShopkeeperProtectionDays--;
        }
    }
}
