# Village of Ashes v3.0 - Quick Start

## 🚀 Start Playing (30 seconds)

```bash
./start-game.sh
# Open: http://localhost:5000
# Click: "New Game"
```

**Hard refresh if you don't see v3.0:** `Ctrl+F5`

---

## 🎮 Phase-Based Actions

### Before Game Starts
❌ No actions available
→ Click "New Game" first

### Council Phase (7-8 AM)
✅ Talk to NPCs
→ Select NPC → Start Conversation

### Day Phase (8 AM-6 PM)
✅ Visit Shopkeeper (always available)
✅ Role-specific actions
✅ Investigate NPCs
✅ Spread rumors

### Night Phase (9 PM-6 AM)
✅ Set night targets (role-specific)
→ Detective: Track
→ Doctor: Protect
→ Butcher: Kill

---

## 🏪 Shopkeeper Quick Guide

### How to Visit
1. Wait for Day Phase (8 AM-6 PM)
2. Click "🏪 Visit Shopkeeper"
3. Buy/sell in modal

### What to Buy
- **🌾 Crops** - Prevents starvation
- **🥩 Meat** - Alternative food
- **📜 Info** - Detective only (costs 1 crop)

### What to Sell
- **🥩 Meat** - Butcher (anonymous)
- **📜 Info** - Vagabond

---

## 🎯 Role Quick Actions

### Farmer 🌾
**Day:** Harvest Crops (+5 crops)
**Strategy:** Harvest daily, feed yourself

### Butcher 🔪
**Day:** Sell Meat (anonymous)
**Night:** Set Kill Target
**Strategy:** Kill → Gain 3 meat → Sell excess

### Detective 🔍
**Day:** Track NPCs, Buy Info
**Night:** Set Track Target
**Strategy:** Investigate → Buy info → Find Butcher

### Doctor 💉
**Day:** Heal NPCs (+30 health)
**Night:** Set Protection Target
**Strategy:** Heal injured → Protect at night

### Vagabond 📜
**Day:** Sell Information
**Strategy:** Gather intel → Sell → Survive 5 nights

---

## ⚠️ Survival Rules

### Food Consumption
- **Every night:** 1 food consumed (crop or meat)
- **No food:** -20 health
- **0 health:** Death

### First Day Survival
```
If Farmer:
→ Harvest 5 crops immediately

If Not Farmer:
→ Advance to Day Phase
→ Visit Shopkeeper
→ Buy 1 crop
→ Survive first night
```

---

## 📊 Resource Display

Check "Your Role" panel:
- 🌾 **Crops** - Your crop count
- 🥩 **Meat** - Your meat count
- 🎒 **Items** - Your item count

⚠️ Warning shows: "Crops/Meat consumed nightly"

---

## 🔄 Daily Routine

### Morning (6-7 AM)
→ Check for deaths
→ Advance to Council

### Council (7-8 AM)
→ Talk to NPCs
→ Build alliances

### Day (8 AM-6 PM)
→ Use role actions
→ Visit shopkeeper
→ Manage resources

### Evening (6-9 PM)
→ Prepare for night
→ Advance to Night

### Night (9 PM-6 AM)
→ Set night targets
→ Advance to morning
→ Food consumed automatically

---

## 💡 Pro Tips

✅ **Always maintain food supply**
✅ **Farmer is crucial for village**
✅ **Butcher is self-sustaining**
✅ **Detective should buy info**
✅ **Check resources regularly**
❌ **Don't let food reach 0**
❌ **Don't ignore health warnings**

---

## 🆘 Quick Fixes

### Actions Not Showing
→ Start a new game first
→ Check current phase
→ Advance time to active phase

### Shopkeeper Won't Open
→ Must be Day Phase (8 AM-6 PM)
→ Check time in game info

### Resources Not Updating
→ Check "Your Role" panel
→ Resources update after transactions

### Wrong Version
→ Hard refresh: Ctrl+F5
→ Should see "v3.0" in title

---

## 📚 Full Documentation

- **V3_FEATURES.md** - Complete feature guide
- **V3_SUMMARY.md** - Implementation details
- **QUICK_REFERENCE.md** - General game guide

---

**Version:** 3.0  
**Ready to play!** 🏚️💀🎮
