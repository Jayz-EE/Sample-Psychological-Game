using VillageOfAshes.Core.Entities;
using VillageOfAshes.Core.Enums;
using VillageOfAshes.Core.Roles;
using VillageOfAshes.Core.Services;

namespace VillageOfAshes.Infrastructure.Services;

public class DaySimulationService : IDaySimulationService
{
    private readonly Random _random = new();
    private readonly INpcDecisionService _decisions;
    private readonly IRumorService _rumorService;
    private readonly IBehaviorAnalysisService _behaviorAnalysis;

    public DaySimulationService(
        INpcDecisionService decisions,
        IRumorService rumorService,
        IBehaviorAnalysisService behaviorAnalysis)
    {
        _decisions = decisions;
        _rumorService = rumorService;
        _behaviorAnalysis = behaviorAnalysis;
    }

    public DaySimulationResult ExecuteDayPhase(GameState gameState)
    {
        var result = new DaySimulationResult();
        _decisions.RefreshAllNpcSuspicions(gameState);

        foreach (var npc in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive))
        {
            if (npc.Role == RoleType.Shopkeeper) continue;
            if (_random.Next(100) > 65) continue;

            var action = ChooseDayBehavior(npc, gameState);
            ApplyDayBehavior(gameState, npc, action, result);
        }

        return result;
    }

    private string ChooseDayBehavior(NPC npc, GameState gameState)
    {
        var decisionAction = _decisions.ChooseDayAction(npc, gameState);
        if (!string.IsNullOrWhiteSpace(decisionAction))
            return decisionAction;

        if (RoleDefinitions.Roles.TryGetValue(npc.Role, out var roleDef) && roleDef.DayActions.Count > 0)
        {
            return roleDef.DayActions[_random.Next(roleDef.DayActions.Count)];
        }

        return npc.Alignment switch
        {
            Alignment.Evil => _random.Next(100) < 50 ? "PlantRumors" : "HideInShadows",
            Alignment.Good => "AnalyzeEvidence",
            _ => "ListenToRumors"
        };
    }

    private void ApplyDayBehavior(GameState gameState, NPC npc, string action, DaySimulationResult result)
    {
        var actionKey = action.Replace(" ", string.Empty);
        _behaviorAnalysis.RecordBehavior(gameState, npc.Id, $"day:{actionKey}", npc.CurrentLocation);

        switch (actionKey)
        {
            case "InvestigateHouse":
            case "AnalyzeEvidence":
            case "CompareAlibis":
            case "AnalyzeRecords":
            case "StudyPatterns":
            {
                var suspect = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Spy);
                if (suspect != null)
                {
                    npc.KnownFacts.Add($"Day {gameState.CurrentDay}: {suspect.Name} was active near {suspect.CurrentLocation}.");
                    result.Events.Add($"{npc.Name} investigated {suspect.Name}'s movements.");
                    if (suspect.Id == "player")
                        gameState.PlayerNotifications.Add($"👁️ You have a feeling that {npc.Name} is investigating you.");
                }
                break;
            }
            case "HealNPC":
            case "DiagnoseInjury":
            case "TreatPanic":
            {
                var patient = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Protect);
                if (patient != null)
                {
                    if (patient.IsIll)
                    {
                        patient.IsIll = false;
                        patient.IllnessSuppressedUntilDay = Math.Max(patient.IllnessSuppressedUntilDay, gameState.CurrentDay + 2);
                        if (patient.Id == "player")
                            gameState.PlayerNotifications.Add($"💊 {npc.Name} has treated your illness. You feel better.");
                    }
                    patient.Health = Math.Min(100, patient.Health + 15);
                    npc.Trust[patient.Id] = Math.Min(100, npc.Trust.GetValueOrDefault(patient.Id, 50) + 8);
                    patient.Trust[npc.Id] = Math.Min(100, patient.Trust.GetValueOrDefault(npc.Id, 50) + 8);
                    result.Events.Add($"{npc.Name} tended to {patient.Name} during the day.");
                    if (patient.Id == "player")
                        gameState.PlayerNotifications.Add($"❤️ {npc.Name} has tended to your wounds. Health: {patient.Health}%");
                }
                break;
            }
            case "BlessHouse":
            case "CalmVillagers":
            case "ConductRitual":
                gameState.VillageFear = Math.Max(0, gameState.VillageFear - 4);
                result.Events.Add($"{npc.Name} calmed tensions in the village.");
                break;
            case "RemoveCurse":
            {
                var cursed = gameState.NPCs
                    .Concat(gameState.Player == null ? Enumerable.Empty<NPC>() : new[] { gameState.Player })
                    .Where(n => n.Status == NPCStatus.Alive && n.IsCursed)
                    .OrderBy(_ => _random.Next())
                    .FirstOrDefault();
                if (cursed != null)
                {
                    cursed.IsCursed = false;
                    cursed.IsIll = false;
                    cursed.IllnessSuppressedUntilDay = 0;
                    cursed.CurseSourceItemId = null;
                    result.Events.Add($"{npc.Name} removed a curse from {cursed.Name}.");
                    if (cursed.Id == "player")
                        gameState.PlayerNotifications.Add($"✨ {npc.Name} has lifted your curse!");
                }
                break;
            }
            case "PublicAccusation":
            case "DemandTestimony":
            {
                var accused = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Accuse);
                if (accused != null)
                {
                    foreach (var witness in gameState.NPCs.Where(n => n.Status == NPCStatus.Alive && n.Id != accused.Id))
                    {
                        witness.Suspicion[accused.Id] = Math.Min(100, witness.Suspicion.GetValueOrDefault(accused.Id, 0) + 8);
                    }
                    result.Events.Add($"{npc.Name} publicly pressured {accused.Name} at the square.");
                    if (accused.Id == "player")
                        gameState.PlayerNotifications.Add($"📢 {npc.Name} is publicly accusing you in the square! Your suspicion is rising.");
                }
                break;
            }
            case "PlantRumors":
            case "SpreadFear":
            {
                var victim = _decisions.ChooseTarget(gameState, npc, IsEvil(npc) ? NpcTargetIntent.Frame : NpcTargetIntent.Accuse);
                if (victim != null)
                {
                    var rumor = _rumorService.GenerateRumor(
                        npc.Id,
                        victim.Id,
                        "seen acting strangely during daylight hours",
                        IsEvil(npc) ? 35 : 55);
                    gameState.Rumors.Add(rumor);
                    result.GeneratedRumors.Add(rumor);
                    result.Events.Add($"{npc.Name} spread word about {victim.Name}.");
                    if (victim.Id == "player")
                        gameState.PlayerNotifications.Add($"👂 You heard that {npc.Name} is spreading rumors about you.");
                }
                break;
            }
            case "HarvestCrops":
            case "SowCrops":
                npc.Inventory.AddRange(Enumerable.Repeat("crop", _random.Next(2, 5)));
                gameState.FoodSupply = Math.Min(200, gameState.FoodSupply + 4);
                result.Events.Add($"{npc.Name} worked the fields.");
                break;
            case "HuntAnimals":
                npc.Inventory.AddRange(Enumerable.Repeat("meat", 2));
                result.Events.Add($"{npc.Name} returned from the forest with game.");
                break;
            case "BrewPotions":
                npc.Inventory.Add("potion");
                result.Events.Add($"{npc.Name} brewed remedies.");
                break;
            case "GivePotion":
            {
                var patient = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Protect);
                var potionIndex = npc.Inventory.FindIndex(i => string.Equals(i, "potion", StringComparison.OrdinalIgnoreCase));
                if (patient != null && potionIndex >= 0)
                {
                    npc.Inventory.RemoveAt(potionIndex);
                    patient.Health = Math.Min(100, patient.Health + 25);
                    result.Events.Add($"{npc.Name} gave {patient.Name} a potion.");
                    if (patient.Id == "player")
                        gameState.PlayerNotifications.Add($"🧪 {npc.Name} gave you a potion. Health: {patient.Health}%");
                }
                break;
            }
            case "ListenToRumors":
            case "TradeRumors":
                npc.KnownFacts.Add($"Day {gameState.CurrentDay}: village gossip is intensifying.");
                result.Events.Add($"{npc.Name} traded rumors in the square.");
                break;
            case "ScoutHouses":
            case "HideInShadows":
            case "ObserveNPCs":
            {
                var watched = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Spy);
                if (watched != null)
                {
                    npc.KnownFacts.Add($"Day {gameState.CurrentDay}: watched {watched.Name} near {watched.CurrentLocation}.");
                    result.Events.Add($"{npc.Name} kept watch on {watched.Name}.");
                    if (watched.Id == "player")
                        gameState.PlayerNotifications.Add($"👁️ You noticed {npc.Name} watching you from the shadows.");
                }
                break;
            }
            case "BegResources":
                npc.Inventory.Add("crop");
                result.Events.Add($"{npc.Name} begged for food near the market.");
                break;
            case "SearchScrap":
                npc.Inventory.Add("scrap");
                break;
            case "UseMagnifyingGlass":
            case "UseListeningConch":
            case "UseFamilyLedger":
            case "UseSilverCharm":
                UseNpcUtilityItem(gameState, npc, actionKey, result);
                break;
            default:
                npc.KnownFacts.Add($"Day {gameState.CurrentDay}: prepared for {action}.");
                break;
        }
    }

    private void UseNpcUtilityItem(GameState gameState, NPC npc, string actionKey, DaySimulationResult result)
    {
        var item = gameState.Items.FirstOrDefault(i =>
            i.CurrentHolderId == npc.Id &&
            i.UtilityAction == actionKey &&
            (!i.UsableByOwnerOnly || i.OwnerNpcId == npc.Id));
        if (item == null) return;

        switch (actionKey)
        {
            case "UseMagnifyingGlass":
                var note = gameState.RecentEvents.OrderBy(_ => _random.Next()).FirstOrDefault()
                    ?? "No clear past activity could be recovered.";
                npc.KnownFacts.Add($"Magnifying Glass note: {note}");
                break;
            case "UseListeningConch":
                var target = _decisions.ChooseTarget(gameState, npc, NpcTargetIntent.Spy);
                if (target != null)
                    npc.KnownFacts.Add($"Listening Conch: {target.Name} was heard near {target.CurrentLocation}.");
                break;
            case "UseFamilyLedger":
                var owner = gameState.NPCs.FirstOrDefault(n => n.Id == item.OwnerNpcId);
                npc.KnownFacts.Add($"Family Ledger: {owner?.Name ?? "Unknown"} is tied to {owner?.HouseId ?? item.HouseId}.");
                break;
            case "UseSilverCharm":
                var evil = gameState.NPCs.FirstOrDefault(n => n.Status == NPCStatus.Alive && n.Alignment == Alignment.Evil);
                npc.KnownFacts.Add(evil == null ? "Silver Charm: no evil presence answered." : $"Silver Charm: evil stirs near {evil.CurrentLocation}.");
                break;
        }

        result.Events.Add($"{npc.Name} used {item.Name}.");
    }

    private static bool IsEvil(NPC npc) =>
        npc.Alignment is Alignment.Evil or Alignment.EvilNeutral;
}
