# UI Display Quick Reference

## Action Visibility Rules

### Hunter Role
| Action | Condition | Location |
|--------|-----------|----------|
| HuntAnimals | Always available | Actions Tab (Day Phase) |
| IdentifyTraces | Only when traces found | Actions Tab (Day Phase) |
| SellMeat | Always available | Shopkeeper Modal |

**Trace Types Detected:**
- Footprints (Type 1)
- Claw Marks (Type 12)
- Drag Marks (Type 20)
- Disturbed Dirt (Type 24)

### Shopkeeper Interactions

#### Visiting the Shop
- **Button Visible:** Only when Shopkeeper is alive
- **Button Hidden:** When Shopkeeper is dead (shows death notice)
- **Action Cost:** 1 action point consumed when LEAVING the shop
- **Available During:** Day Phase (8 AM - 6 PM)

#### In the Shop Modal
**All Players Can:**
- Buy Crops
- Buy Meat
- Buy Intelligence (Detective only)
- Sell Crops (if you have them)
- Sell Meat (if you have them)

**Role-Specific Selling:**
- **Thief:** Trade Stolen Goods
- **Voyeur:** Sell Information
- **Alchemist:** Sell Remedies

**Shopkeeper Role Actions (Shopkeeper only):**
- Trade Resources
- Sell Clues
- Spread Rumors

### Phase-Based Action Availability

#### Council Phase (7:00 AM - 8:00 AM)
**Available:**
- Talk to NPCs privately
- Make announcements (Council Tab)
- Make accusations (Council Tab)
- Raise suspicions (Council Tab)
- Start private conversations (Council Tab)

**Not Available:**
- Day role actions
- Night role actions
- Shopkeeper visits

#### Day Phase (8:00 AM - 6:00 PM)
**Available:**
- All day role actions
- Visit Shopkeeper
- Investigate NPCs
- Spread rumors
- Neutral strategic actions (if neutral alignment)

**Not Available:**
- Night role actions
- Council forum actions

#### Night Phase (9:00 PM - 6:00 AM)
**Available:**
- Night role actions (limited)
- Time advancement

**Not Available:**
- Day role actions
- Shopkeeper visits
- Council actions

#### Other Phases (Morning Discovery, Evening)
**Available:**
- Time advancement
- View information

**Not Available:**
- Most role actions
- Shopkeeper visits

### Role-Specific Action Locations

| Role | Day Actions Location | Selling Location |
|------|---------------------|------------------|
| Detective | Actions Tab | N/A |
| Doctor | Actions Tab | N/A |
| Butcher | Actions Tab (CleanTools) | Shop Modal (SellMeat) |
| Vagabond | Actions Tab | N/A |
| Farmer | Actions Tab (Sow/Harvest/Fertilize) | Shop Modal (SellProduce) |
| Shopkeeper | Shop Modal ONLY | Shop Modal |
| Priest | Actions Tab | N/A |
| Prosecutor | Actions Tab | N/A |
| Witch | Actions Tab | N/A |
| Crawler | Actions Tab | N/A |
| Headless | Actions Tab | N/A |
| Alchemist | Actions Tab (BrewPotions) | Shop Modal (SellRemedies) |
| Hunter | Actions Tab (Hunt/Identify) | Shop Modal (SellMeat) |
| Scholar | Actions Tab | N/A |
| Thief | Actions Tab (ScoutHouses) | Shop Modal (TradeStolenGoods) |
| Voyeur | Actions Tab (ListenToRumors) | Shop Modal (SellInformation) |

### Visual Indicators

#### Hunter Trace Indicator
```
✓ 3 trace(s) found - you can identify them!  [Green background]
```
or
```
✗ No traces found yet - tracking unavailable  [Red background]
```

#### Shopkeeper Status
```
🏪 Visit Shopkeeper  [Button visible when alive]
```
or
```
💀 The Shopkeeper is no longer available  [Red notice when dead]
```

#### Shop Warning
```
⚠️ Action point will be consumed when you leave the shop
```

### Action Consumption Rules

| Action Type | When Consumed |
|-------------|---------------|
| Role Actions | Immediately when performed |
| Investigate NPC | Immediately when performed |
| Spread Rumor | Immediately when performed |
| Shop Visit | When LEAVING the shop |
| Shop Transactions | Free (no additional cost) |
| Council Actions | Immediately when performed |
| Private Conversations | Immediately when started |

### Neutral Alignment Strategic Actions

**Available During:** Day Phase only  
**Visible To:** Neutral, Good Neutral, Evil Neutral alignments

**Actions:**
- Leave Village (escape the village)
- Join Good Faction (align with good)
- Join Evil Faction (align with evil)

### Common UI States

#### No Actions Available
```
No role actions available. Visit the Shopkeeper to trade goods.
```

#### Shopkeeper Role (No Day Actions)
```
Visit the Shopkeeper to perform your role actions.
```

#### Wrong Phase
```
Limited actions available during this phase. Advance time to reach an active phase.
```

#### Action Limit Reached
```
Maximum 2 actions allowed per phase.
```

## Tips for Players

1. **Hunter Players:** Check the trace indicator before trying to identify traces
2. **All Players:** Visit the shopkeeper during Day Phase to buy/sell goods
3. **Shopkeeper Players:** All your role actions are in the shop modal
4. **Selling Players:** Go to the shop to sell your goods (crops, meat, etc.)
5. **Phase Awareness:** Check the current phase to know what actions are available
6. **Action Management:** Remember that visiting the shop costs 1 action when you leave
7. **Strategic Timing:** Plan your shop visits carefully to maximize action efficiency
