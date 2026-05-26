using VillageOfAshes.Core.Enums;

namespace VillageOfAshes.Core.Roles;

public class RoleDefinition
{
    public RoleType Type { get; set; }
    public Alignment Alignment { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> DayActions { get; set; } = new();
    public List<string> NightActions { get; set; } = new();
    public List<string> PassiveAbilities { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public string? WinCondition { get; set; }
}

public static class RoleDefinitions
{
    public static readonly Dictionary<RoleType, RoleDefinition> Roles = new()
    {
        {
            RoleType.Detective, new RoleDefinition
            {
                Type = RoleType.Detective,
                Alignment = Alignment.Good,
                Name = "Detective",
                Description = "Investigation and contradiction analysis",
                DayActions = new() { "InvestigateHouse", "InterrogateNPC", "AnalyzeEvidence", "CompareAlibis" },
                NightActions = new() { "Stakeout", "TrackTarget", "SecretSurveillance" },
                PassiveAbilities = new() { "HigherClueAccuracy", "ContradictionAnalysis" },
                Weaknesses = new() { "FrequentlySeenInvestigating", "FalseAccusationRisk" }
            }
        },
        {
            RoleType.Doctor, new RoleDefinition
            {
                Type = RoleType.Doctor,
                Alignment = Alignment.Good,
                Name = "Doctor",
                Description = "Protection and injury analysis",
                DayActions = new() { "HealNPC", "DiagnoseInjury", "TreatPanic" },
                NightActions = new() { "ProtectNPC", "EmergencyTreatment", "SecretRecovery" },
                PassiveAbilities = new() { "DetectInjurySources", "FearReduction" },
                Weaknesses = new() { "MedicineShortages", "BloodTraceRisk" }
            }
        },
        {
            RoleType.Priest, new RoleDefinition
            {
                Type = RoleType.Priest,
                Alignment = Alignment.Good,
                Name = "Priest",
                Description = "Spiritual protection and fear control",
                DayActions = new() { "BlessHouse", "CalmVillagers", "ConductRitual" },
                NightActions = new() { "PurifyArea", "SenseEvilPresence", "NightPrayer" },
                PassiveAbilities = new() { "CorruptionDetection", "FearControl" },
                Weaknesses = new() { "TargetedByEvil", "FailedRitualDistrust" }
            }
        },
        {
            RoleType.Prosecutor, new RoleDefinition
            {
                Type = RoleType.Prosecutor,
                Alignment = Alignment.Good,
                Name = "Prosecutor",
                Description = "Public accusation and council influence",
                DayActions = new() { "PublicAccusation", "ReviewStatements", "DemandTestimony" },
                NightActions = new() { "SecretInvestigation", "CollectRecords" },
                PassiveAbilities = new() { "CouncilInfluence", "LieDetection" },
                Weaknesses = new() { "FalseAccusationPanic" }
            }
        },
        {
            RoleType.Witch, new RoleDefinition
            {
                Type = RoleType.Witch,
                Alignment = Alignment.Evil,
                Name = "Witch",
                Description = "Manipulation, curses, misinformation",
                DayActions = new() { "BuyIngredients", "SpreadFear", "PlantRumors" },
                NightActions = new() { "CurseNPC", "PlantFalseEvidence", "PerformRitual" },
                PassiveAbilities = new() { "Misinformation", "Corruption" },
                Weaknesses = new() { "RitualTracesDetectable" }
            }
        },
        {
            RoleType.Crawler, new RoleDefinition
            {
                Type = RoleType.Crawler,
                Alignment = Alignment.Evil,
                Name = "Crawler",
                Description = "Stealth predator and fear generator",
                DayActions = new() { "HideInShadows", "ObserveNPCs" },
                NightActions = new() { "StalkTarget", "AmbushNPC", "CrawlThroughVillage" },
                PassiveAbilities = new() { "Stealth", "FearGeneration" },
                Weaknesses = new() { "AnimalisticTraces" }
            }
        },
        {
            RoleType.Butcher, new RoleDefinition
            {
                Type = RoleType.Butcher,
                Alignment = Alignment.Evil,
                Name = "Butcher",
                Description = "Physical killer with economic disguise",
                DayActions = new() { "SellMeat", "CleanTools", "TradeSupplies" },
                NightActions = new() { "KillNPC", "DisposeBody", "HarvestFlesh" },
                PassiveAbilities = new() { "EconomicCover", "StrongAttacks" },
                Weaknesses = new() { "BloodEvidenceRisk", "MeatPatternSuspicion" }
            }
        },
        {
            RoleType.Headless, new RoleDefinition
            {
                Type = RoleType.Headless,
                Alignment = Alignment.Evil,
                Name = "Headless",
                Description = "Supernatural terror and fear corruption",
                DayActions = new() { "ManifestBriefly" },
                NightActions = new() { "HauntArea", "TerrorizeNPC", "CurseGround" },
                PassiveAbilities = new() { "FearCorruption", "ScheduleDisruption" },
                Weaknesses = new() { "SupernaturalTraces" }
            }
        },
        {
            RoleType.Farmer, new RoleDefinition
            {
                Type = RoleType.Farmer,
                Alignment = Alignment.GoodNeutral,
                Name = "Farmer",
                Description = "Food production and village stability",
                DayActions = new() { "SowCrops", "HarvestCrops", "SellProduce", "FertilizeLand" },
                NightActions = new() { "GuardCrops", "HideSupplies" },
                PassiveAbilities = new() { "FoodGeneration" },
                Weaknesses = new() { "CropDestructionPanic" },
                WinCondition = "Maintain food production and survive"
            }
        },
        {
            RoleType.Alchemist, new RoleDefinition
            {
                Type = RoleType.Alchemist,
                Alignment = Alignment.GoodNeutral,
                Name = "Alchemist",
                Description = "Potion crafting and chemical manipulation",
                DayActions = new() { "BrewPotions", "BuyIngredients", "SellRemedies" },
                NightActions = new() { "Experiment", "PoisonResources", "DistillElixir" },
                PassiveAbilities = new() { "PotionUtility", "RandomEffects" },
                Weaknesses = new() { "ChemicalSuspicion" },
                WinCondition = "Complete research objectives and survive"
            }
        },
        {
            RoleType.Hunter, new RoleDefinition
            {
                Type = RoleType.Hunter,
                Alignment = Alignment.GoodNeutral,
                Name = "Hunter",
                Description = "Tracking and wilderness survival",
                DayActions = new() { "HuntAnimals", "TrackFootprints", "SellMeat" },
                NightActions = new() { "PatrolForest", "SetTraps", "FollowSounds" },
                PassiveAbilities = new() { "WildernessTracking", "MeatGeneration" },
                Weaknesses = new() { "TrapSuspicion" },
                WinCondition = "Protect self or village and escape alive"
            }
        },
        {
            RoleType.Scholar, new RoleDefinition
            {
                Type = RoleType.Scholar,
                Alignment = Alignment.GoodNeutral,
                Name = "Scholar",
                Description = "Knowledge and behavioral analysis",
                DayActions = new() { "AnalyzeRecords", "StudyPatterns", "DecodeSymbols" },
                NightActions = new() { "SecretObservation", "HiddenResearch" },
                PassiveAbilities = new() { "PatternPrediction", "RitualDecoding" },
                Weaknesses = new() { "NightStudySuspicion" },
                WinCondition = "Discover hidden truth and survive"
            }
        },
        {
            RoleType.Thief, new RoleDefinition
            {
                Type = RoleType.Thief,
                Alignment = Alignment.EvilNeutral,
                Name = "Thief",
                Description = "Resource theft and economic disruption",
                DayActions = new() { "ScoutHouses", "TradeStolenGoods" },
                NightActions = new() { "StealResources", "SneakIntoHouses", "PickpocketNPC" },
                PassiveAbilities = new() { "ResourceTheft", "InfoTheft" },
                Weaknesses = new() { "BrokenLockSuspicion" },
                WinCondition = "Accumulate wealth and escape alive"
            }
        },
        {
            RoleType.Voyeur, new RoleDefinition
            {
                Type = RoleType.Voyeur,
                Alignment = Alignment.EvilNeutral,
                Name = "Voyeur",
                Description = "Information gathering and blackmail",
                DayActions = new() { "ListenToRumors", "SellInformation" },
                NightActions = new() { "SpyOnNPC", "ObserveMeetings" },
                PassiveAbilities = new() { "SecretGathering", "Blackmail" },
                Weaknesses = new() { "EavesdroppingSuspicion" },
                WinCondition = "Gather secrets and manipulate factions"
            }
        },
        {
            RoleType.Vagabond, new RoleDefinition
            {
                Type = RoleType.Vagabond,
                Alignment = Alignment.EvilNeutral,
                Name = "Vagabond",
                Description = "Outcast survivalist and social scapegoat",
                DayActions = new() { "BegResources", "TradeRumors", "SearchScrap" },
                NightActions = new() { "SleepOutdoors", "WanderVillage", "SneakAround" },
                PassiveAbilities = new() { "OutdoorsSurvival", "HiddenClues" },
                Weaknesses = new() { "AutomaticSuspicion" },
                WinCondition = "Survive 5 nights and escape village safely"
            }
        },
        {
            RoleType.Shopkeeper, new RoleDefinition
            {
                Type = RoleType.Shopkeeper,
                Alignment = Alignment.FixedNeutral,
                Name = "Shopkeeper",
                Description = "Village economic and information center",
                DayActions = new() { "TradeResources", "SellClues", "SpreadRumors" },
                NightActions = new() { "LockShop", "RecordVisitors" },
                PassiveAbilities = new() { "ProtectedByTalisman", "InformationHub" },
                Weaknesses = new() { "DeathCollapsesEconomy" },
                WinCondition = "Maintain village economy"
            }
        }
    };
}
