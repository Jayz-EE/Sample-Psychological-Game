# 🏚️ Village of Ashes - START HERE

## Quick Start (30 seconds)

```bash
# 1. Start the server
./start-game.sh

# 2. Open browser
# Go to: http://localhost:5000

# 3. Start playing
# Click "New Game" button
```

---

## ✅ Verify It's Working

You should see:
- ✅ Title shows "Village of Ashes - **v2.0**"
- ✅ "New Game" button at top
- ✅ "Villagers" panel with NPCs
- ✅ "Player Actions" panel with dropdowns
- ✅ "Village Discussions" panel
- ✅ "Investigation Summary" panel

**If you see v1.1 or older:** Press `Ctrl+F5` to hard refresh

---

## 🎮 First 5 Minutes

### 1. Start a New Game (10 seconds)
- Click "New Game" button
- Check "Your Role" panel to see your role
- Note the NPC names in "Villagers" panel

### 2. Try Talking to an NPC (30 seconds)
- Go to "Player Actions" panel
- Select an NPC from "Talk to NPCs" dropdown
- Click "Start Conversation"
- Read their dialogue
- Choose a response option

### 3. Investigate Someone (30 seconds)
- Select an NPC from "Investigate" dropdown
- Click "Investigate Behavior"
- Read the behavior analysis
- Check suspicious findings

### 4. Spread a Rumor (30 seconds)
- Select a target from "Spread Rumor" dropdown
- Type: "I saw them acting suspicious"
- Click "Spread Rumor"
- Check "Rumors" panel to see it appear

### 5. Advance Time (1 minute)
- Click "Advance 1 Hour" to reach 7:00 AM
- Watch "Village Discussions" panel
- See NPCs talking during council
- Check "Investigation Summary" for data

---

## 📚 Documentation

### Quick Reference
- **QUICK_REFERENCE.md** - All commands and tips (5 min read)

### Detailed Guides
- **NEW_FEATURES_V2.md** - Complete feature documentation (15 min read)
- **V2_UPDATE_SUMMARY.md** - What's new in v2.0 (10 min read)

### Troubleshooting
- **TROUBLESHOOTING.md** - Fix common issues
- **BROWSER_CACHE_FIX.md** - Fix display issues

### Original Docs
- **QUICKSTART.md** - Original quick start guide
- **README.md** - Full project documentation
- **prototype_md_social_horror_village_simulation.md** - Game design

---

## 🎯 Your Goal

### Check Your Role
Look at "Your Role" panel to see your objective:

- **Detective** 🔍 - Investigate and identify the killer
- **Doctor** 💉 - Heal and protect villagers  
- **Butcher** 🔪 - Eliminate villagers without being caught
- **Vagabond** 🎒 - Survive 5 nights and escape
- **Farmer** 🌾 - Maintain food supply for 7 days

### Win Conditions
- **Good (Detective/Doctor):** Eliminate the Butcher
- **Evil (Butcher):** Outnumber good roles
- **Neutral (Vagabond/Farmer):** Complete your specific objective

---

## 💡 Pro Tips

### Early Game
✅ Talk to all NPCs to build relationships
✅ Investigate to learn behavior patterns
✅ Watch who dies first
❌ Don't spread rumors yet

### Mid Game
✅ Focus investigations on suspicious NPCs
✅ Build alliances with trusted NPCs
✅ Use strategic rumors
✅ Check Investigation Summary regularly

### Late Game
✅ Confirm your suspicions
✅ Rally your allies
✅ Push for decisive action
✅ Complete your role objective

---

## 🆘 Problems?

### Server Won't Start
```bash
# Kill any existing processes
pkill -9 dotnet

# Try again
./start-game.sh
```

### Wrong Version Showing
```
Press Ctrl+F5 (Windows/Linux)
or Cmd+Shift+R (Mac)
```

### Actions Not Working
```
1. Check browser console (F12)
2. Verify server is running
3. Try hard refresh (Ctrl+F5)
```

### Still Stuck?
```bash
# Run verification
./verify-fix.sh

# Check troubleshooting guide
cat TROUBLESHOOTING.md
```

---

## 🎮 Game Controls

### Time
- **Advance 1 Hour** - Move forward 60 minutes
- **Advance 6 Hours** - Quick skip
- **Refresh** - Update display

### Actions
- **Talk** - Start conversation with NPC
- **Investigate** - Analyze NPC behavior
- **Spread Rumor** - Create gossip

### Panels
- **Villagers** - All NPCs and status
- **Evidence** - Physical clues
- **Rumors** - Circulating gossip
- **Your Role** - Your character info
- **Village Discussions** - Council talks
- **Player Actions** - Interactive controls
- **Investigation Summary** - Data overview
- **Event Log** - Game events

---

## 🎯 What Makes This Game Unique

### Hidden Information
- NPCs have secret roles
- You don't know who's good or evil
- Must deduce through investigation

### Social Dynamics
- Trust and suspicion systems
- Rumors spread through network
- Alliances and betrayals

### Night Simulation
- Backend executes role actions
- Killings, investigations, healing
- Evidence and rumors generated

### Emergent Storytelling
- Every game is different
- NPCs react to events
- Unpredictable outcomes

---

## 🚀 Ready to Play?

```bash
./start-game.sh
```

Then open: **http://localhost:5000**

**Good luck, and may you survive the Village of Ashes! 🏚️💀**

---

## 📞 Quick Links

- **Game Design:** `prototype_md_social_horror_village_simulation.md`
- **Quick Reference:** `QUICK_REFERENCE.md`
- **New Features:** `NEW_FEATURES_V2.md`
- **Troubleshooting:** `TROUBLESHOOTING.md`

---

**Version:** 2.0  
**Status:** ✅ Fully Interactive  
**Last Updated:** 2026-05-26
