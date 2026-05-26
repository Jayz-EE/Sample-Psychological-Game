# UI Action Flow Diagram

## Hunter Role Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    HUNTER ROLE ACTIONS                       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │  Check Evidence  │
                    │   for Traces     │
                    └─────────────────┘
                              │
                ┌─────────────┴─────────────┐
                ▼                           ▼
        ┌──────────────┐            ┌──────────────┐
        │ Traces Found │            │ No Traces    │
        │   (Count: 3) │            │    Found     │
        └──────────────┘            └──────────────┘
                │                           │
                ▼                           ▼
    ┌────────────────────┐      ┌────────────────────┐
    │ ✓ Show Actions:    │      │ ✗ Show Actions:    │
    │ - Hunt Animals     │      │ - Hunt Animals     │
    │ - Identify Traces  │      │                    │
    │   [Dropdown: 3]    │      │ Message: No traces │
    └────────────────────┘      └────────────────────┘
                │
                ▼
    ┌────────────────────┐
    │ Select Trace       │
    │ - Footprints at X  │
    │ - Claw Marks at Y  │
    │ - Drag Marks at Z  │
    └────────────────────┘
                │
                ▼
    ┌────────────────────┐
    │ Identify Owner     │
    │ (Consume Action)   │
    └────────────────────┘
```

---

## Shopkeeper Visit Flow

```
┌─────────────────────────────────────────────────────────────┐
│                   SHOPKEEPER VISIT FLOW                      │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Check Shopkeeper│
                    │     Status      │
                    └─────────────────┘
                              │
                ┌─────────────┴─────────────┐
                ▼                           ▼
        ┌──────────────┐            ┌──────────────┐
        │ Shopkeeper   │            │ Shopkeeper   │
        │    Alive     │            │     Dead     │
        └──────────────┘            └──────────────┘
                │                           │
                ▼                           ▼
    ┌────────────────────┐      ┌────────────────────┐
    │ Show Button:       │      │ Show Notice:       │
    │ [Visit Shopkeeper] │      │ 💀 Shopkeeper      │
    │                    │      │    Unavailable     │
    └────────────────────┘      └────────────────────┘
                │                           │
                ▼                           ▼
    ┌────────────────────┐           [End Flow]
    │ Click Button       │
    │ (No action cost)   │
    └────────────────────┘
                │
                ▼
    ┌────────────────────────────────────────┐
    │         SHOPKEEPER MODAL               │
    ├────────────────────────────────────────┤
    │ Buy Section    │ Sell Section          │
    │ - Buy Crop     │ - Sell Crops (5)      │
    │ - Buy Meat     │ - Sell Meat (2)       │
    │ - Buy Info     │ - Role-Specific Sells │
    │                │                        │
    │ Shopkeeper Role Actions (if Shopkeeper)│
    │ - Trade Resources                      │
    │ - Sell Clues                          │
    │ - Spread Rumors                       │
    │                                        │
    │ ⚠️ Action consumed on exit            │
    └────────────────────────────────────────┘
                │
                ▼
    ┌────────────────────┐
    │ Perform Multiple   │
    │ Transactions       │
    │ (All Free)         │
    └────────────────────┘
                │
                ▼
    ┌────────────────────┐
    │ Close Modal        │
    │ (Consume 1 Action) │
    └────────────────────┘
```

---

## Phase-Based Action Flow

```
┌─────────────────────────────────────────────────────────────┐
│                  PHASE-BASED ACTION FLOW                     │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Check Current   │
                    │     Phase       │
                    └─────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ Council Phase│    │  Day Phase   │    │ Night Phase  │
│  (7-8 AM)    │    │ (8AM-6PM)    │    │ (9PM-6AM)    │
└──────────────┘    └──────────────┘    └──────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ Show:        │    │ Show:        │    │ Show:        │
│ - Talk to    │    │ - Role Day   │    │ - Role Night │
│   NPCs       │    │   Actions    │    │   Actions    │
│ - Announce   │    │ - Visit Shop │    │ - Limited    │
│ - Accuse     │    │ - Investigate│    │   Actions    │
│ - Suspicion  │    │ - Rumors     │    │              │
│ - Private    │    │ - Strategic  │    │              │
│   Talk       │    │   (Neutral)  │    │              │
└──────────────┘    └──────────────┘    └──────────────┘
        │                     │                     │
        └─────────────────────┼─────────────────────┘
                              ▼
                    ┌─────────────────┐
                    │ Hide Irrelevant │
                    │    Actions      │
                    └─────────────────┘
```

---

## Selling Action Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    SELLING ACTION FLOW                       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Player Has      │
                    │ Items to Sell   │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Visit Shopkeeper│
                    │ (Day Phase)     │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Open Shop Modal │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Check Inventory │
                    └─────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ Has Crops    │    │  Has Meat    │    │ Has Special  │
│   (Farmer)   │    │(Butcher/Hunt)│    │    Items     │
└──────────────┘    └──────────────┘    └──────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ Show:        │    │ Show:        │    │ Show:        │
│ [Sell Crops] │    │ [Sell Meat]  │    │ [Sell Item]  │
│ Button       │    │ Button       │    │ Button       │
└──────────────┘    └──────────────┘    └──────────────┘
        │                     │                     │
        └─────────────────────┼─────────────────────┘
                              ▼
                    ┌─────────────────┐
                    │ Click Sell      │
                    │ (Free Action)   │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Remove Items    │
                    │ Update Economy  │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Continue or     │
                    │ Close Shop      │
                    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Close Modal     │
                    │ (1 Action Cost) │
                    └─────────────────┘
```

---

## Role-Specific Action Location Flow

```
┌─────────────────────────────────────────────────────────────┐
│              ROLE-SPECIFIC ACTION LOCATIONS                  │
└─────────────────────────────────────────────────────────────┘

FARMER:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Sow Crops              │ - Sell Produce               │
│ - Harvest Crops          │                              │
│ - Fertilize Land         │                              │
└──────────────────────────┴──────────────────────────────┘

HUNTER:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Hunt Animals           │ - Sell Meat                  │
│ - Identify Traces*       │                              │
│   (*if traces found)     │                              │
└──────────────────────────┴──────────────────────────────┘

BUTCHER:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Clean Tools            │ - Sell Meat                  │
└──────────────────────────┴──────────────────────────────┘

SHOPKEEPER:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ [Visit Shopkeeper]       │ - Trade Resources            │
│                          │ - Sell Clues                 │
│                          │ - Spread Rumors              │
└──────────────────────────┴──────────────────────────────┘

ALCHEMIST:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Brew Potions           │ - Sell Remedies              │
└──────────────────────────┴──────────────────────────────┘

THIEF:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Scout Houses           │ - Trade Stolen Goods         │
└──────────────────────────┴──────────────────────────────┘

VOYEUR:
┌──────────────────────────────────────────────────────────┐
│ Actions Tab (Day)        │ Shopkeeper Modal             │
├──────────────────────────┼──────────────────────────────┤
│ - Listen to Rumors       │ - Sell Information           │
└──────────────────────────┴──────────────────────────────┘
```

---

## Action Consumption Timeline

```
┌─────────────────────────────────────────────────────────────┐
│              ACTION CONSUMPTION TIMELINE                     │
└─────────────────────────────────────────────────────────────┘

BEFORE FIX:
Time    Action                          Actions Remaining
─────────────────────────────────────────────────────────
8:00    Start Day Phase                 2/2
8:30    Visit Shopkeeper                1/2  ← Consumed
9:00    Buy Crop                        0/2  ← Consumed
9:00    Sell Meat                       -1/2 ← ERROR!
        Close Shop                      -1/2

AFTER FIX:
Time    Action                          Actions Remaining
─────────────────────────────────────────────────────────
8:00    Start Day Phase                 2/2
8:30    Visit Shopkeeper                2/2  ← Not consumed
8:30    Buy Crop                        2/2  ← Free
8:30    Sell Meat                       2/2  ← Free
8:30    Buy Meat                        2/2  ← Free
8:30    Sell Crops                      2/2  ← Free
9:00    Close Shop                      1/2  ← Consumed here
9:30    Investigate NPC                 0/2  ← Consumed
```

---

## Decision Tree: Can I Perform This Action?

```
                    ┌─────────────────┐
                    │ Want to Perform │
                    │     Action      │
                    └─────────────────┘
                            │
                            ▼
                    ┌─────────────────┐
                    │ Is it a selling │
                    │    action?      │
                    └─────────────────┘
                            │
                    ┌───────┴───────┐
                    ▼               ▼
                ┌──────┐        ┌──────┐
                │ YES  │        │  NO  │
                └──────┘        └──────┘
                    │               │
                    ▼               ▼
        ┌────────────────┐  ┌────────────────┐
        │ Go to Shop     │  │ Check Phase    │
        │ Modal          │  └────────────────┘
        └────────────────┘          │
                                    ▼
                        ┌────────────────────┐
                        │ Is action valid    │
                        │ for current phase? │
                        └────────────────────┘
                                    │
                            ┌───────┴───────┐
                            ▼               ▼
                        ┌──────┐        ┌──────┐
                        │ YES  │        │  NO  │
                        └──────┘        └──────┘
                            │               │
                            ▼               ▼
                ┌────────────────┐  ┌────────────────┐
                │ Check if       │  │ Action Hidden  │
                │ conditions met │  │ or Disabled    │
                └────────────────┘  └────────────────┘
                            │
                    ┌───────┴───────┐
                    ▼               ▼
                ┌──────┐        ┌──────┐
                │ YES  │        │  NO  │
                └──────┘        └──────┘
                    │               │
                    ▼               ▼
        ┌────────────────┐  ┌────────────────┐
        │ Show Action    │  │ Hide Action    │
        │ Button         │  │ or Show Notice │
        └────────────────┘  └────────────────┘
```

This comprehensive flow diagram shows how the UI now intelligently displays actions based on multiple conditions, creating a much more intuitive user experience.
