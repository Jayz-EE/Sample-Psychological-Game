# Village of Ashes - Quick Reference Guide

## 🎮 Game Controls

### Time Management
- **Advance 1 Hour** - Move forward 60 minutes
- **Advance 6 Hours** - Quick skip 6 hours
- **Refresh** - Update display without advancing time

### Player Actions
- **Talk to NPC** - Start conversation, choose responses
- **Investigate NPC** - Analyze behavior and find suspicious activity
- **Spread Rumor** - Create gossip about a target

---

## ⏰ Time Phases

| Time | Phase | What Happens |
|------|-------|--------------|
| 6:00 AM - 7:00 AM | Morning Discovery | Find evidence, discover deaths |
| 7:00 AM - 8:00 AM | Village Council | NPCs discuss, accuse, vote |
| 8:00 AM - 6:00 PM | Day Actions | Role actions, investigations, trading |
| 6:00 PM - 9:00 PM | Evening | Limited movement, preparation |
| 9:00 PM - 6:00 AM | Night Simulation | Backend executes role actions |

---

## 👥 Roles

### Good Faction
- **Detective** 🔍 - Investigate and identify the killer
- **Doctor** 💉 - Heal and protect villagers

### Evil Faction
- **Butcher** 🔪 - Eliminate villagers without being caught

### Neutral Faction
- **Vagabond** 🎒 - Survive 5 nights and escape
- **Farmer** 🌾 - Maintain food supply for 7 days
- **Shopkeeper** 🏪 - Maintain village economy (NPC only)

---

## 🎯 Win Conditions

### Good Wins
- Eliminate the Butcher
- Keep village stable

### Evil Wins
- Butcher outnumbers good roles
- Village collapses

### Neutral Wins
- **Vagabond:** Survive 5 nights, then escape
- **Farmer:** Maintain food for 7 days

---

## 📊 UI Panels

### Villagers Panel
- All NPCs and their status
- Health, hunger, location
- Role and alignment (visible for testing)
- 💀 = Dead

### Evidence Panel
- Physical clues found
- Location and visibility
- Decay time remaining

### Rumors Panel
- Circulating gossip
- Source and target
- Truthfulness percentage
- Who knows the rumor

### Your Role Panel
- Your character info
- Role and alignment
- Current objective

### Village Discussions Panel
- Council conversations (7-8 AM)
- NPC dialogue
- Your conversation interface

### Player Actions Panel
- Talk to NPCs
- Investigate behavior
- Spread rumors

### Investigation Summary Panel
- Total evidence/rumors/observations
- Most suspicious NPCs (red bars)
- Most trusted NPCs (green bars)

### Event Log
- Real-time game events
- System messages
- Action results

---

## 🎲 Strategy Tips

### Early Game (Day 1-2)
✅ Talk to all NPCs to build relationships
✅ Investigate to establish baselines
✅ Observe who dies first
❌ Don't spread rumors yet

### Mid Game (Day 3-5)
✅ Focus investigations on suspicious NPCs
✅ Build alliances with trusted NPCs
✅ Use strategic rumors
✅ Watch suspicion levels

### Late Game (Day 6+)
✅ Confirm your suspicions
✅ Rally your allies
✅ Push for decisive action
✅ Complete your role objective

---

## 🔍 Investigation Guide

### Suspicion Levels
- **80-100%** - Highly suspected, likely accused
- **60-79%** - Moderately suspected
- **40-59%** - Some suspicion
- **0-39%** - Low suspicion

### Trust Levels
- **80-100%** - Strong ally
- **60-79%** - Trusted
- **40-59%** - Neutral
- **0-39%** - Distrusted

### Behavior Patterns
- **Activity Level** - How active at night
- **Consistency** - Pattern regularity
- **Social Interaction** - How much they talk

---

## 💬 Dialogue Effects

### Response Types
- **Friendly** - Builds trust (+5)
- **Suspicious** - Increases suspicion (+5)
- **Neutral** - No effect

### When to Use
- **Build Trust:** Early game, potential allies
- **Increase Suspicion:** Testing reactions, framing
- **Neutral:** When unsure or avoiding attention

---

## 📢 Rumor Strategy

### Effective Rumors
✅ "I saw them near the murder scene"
✅ "They've been acting suspicious"
✅ "I heard they're planning something"

### Ineffective Rumors
❌ Too vague: "They're weird"
❌ Too specific: "They killed John at 2:13 AM"
❌ Easily disproven: "They were at the shop" (when they weren't)

### Rumor Tactics
- **Frame:** Spread false rumors as Butcher
- **Test:** Spread rumors to see reactions
- **Defend:** Counter rumors about you
- **Guide:** Direct suspicion as Detective

---

## 🚨 Common Mistakes

### ❌ Don't Do This
- Spread too many rumors early (draws attention)
- Investigate the same NPC repeatedly (suspicious)
- Ignore the investigation summary (miss patterns)
- Talk to everyone every day (unrealistic)
- Forget your role objective (lose condition)

### ✅ Do This Instead
- Pace your actions naturally
- Investigate different NPCs
- Check summary regularly
- Be selective with conversations
- Focus on your win condition

---

## 🔧 Keyboard Shortcuts

- **F5** - Refresh page
- **Ctrl+F5** - Hard refresh (clear cache)
- **F12** - Open developer tools

---

## 📱 Quick Actions

### Start New Game
1. Click "New Game"
2. Check your role in "Your Role" panel
3. Note NPC names and roles

### Investigate Someone
1. Select NPC from "Investigate" dropdown
2. Click "Investigate Behavior"
3. Read results in Village Discussions panel

### Have a Conversation
1. Select NPC from "Talk to NPCs" dropdown
2. Click "Start Conversation"
3. Choose a response option
4. Watch relationship change

### Spread a Rumor
1. Select target from "Spread Rumor" dropdown
2. Type rumor text
3. Click "Spread Rumor"
4. Watch it appear in Rumors panel

### Advance to Council
1. Check current time
2. Click "Advance 1 Hour" until 7:00 AM
3. Watch Village Discussions panel
4. See NPCs converse

---

## 🎯 Role-Specific Quick Tips

### Detective 🔍
- Investigate everyone systematically
- Build trust with Doctor
- Spread accurate rumors
- Watch for inconsistent behavior

### Doctor 💉
- Talk to build alliances
- Investigate quietly
- Protect key NPCs (future feature)
- Stay neutral in discussions

### Butcher 🔪
- Spread false rumors early
- Build trust to appear innocent
- Investigate targets before killing
- Deflect suspicion in council

### Vagabond 🎒
- Minimal interactions
- Investigate threats only
- Avoid all rumors
- Survive 5 nights

### Farmer 🌾
- Build community trust
- Investigate threats
- Defend if targeted
- Maintain resources

---

## 🆘 Emergency Commands

### Server Issues
```bash
# Kill and restart
pkill -9 dotnet
./start-game.sh
```

### Browser Issues
```
1. Hard refresh: Ctrl+F5
2. Clear cache
3. Try incognito mode
4. Check console (F12)
```

### Verify Fix
```bash
./verify-fix.sh
```

---

## 📞 Getting Help

1. Check `TROUBLESHOOTING.md`
2. Check `NEW_FEATURES_V2.md`
3. Check browser console (F12)
4. Check server terminal for errors

---

**Version:** 2.0  
**Last Updated:** 2026-05-26  
**Status:** ✅ Fully Interactive
