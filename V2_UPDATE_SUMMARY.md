# Village of Ashes v2.0 - Update Summary

## ✅ What Was Added

You requested interactive features that were missing from the UI. I've now implemented:

### 1. ✅ Player Actions
- **Talk to NPCs** - Interactive conversations with dialogue choices
- **Investigate NPCs** - Analyze behavior patterns and find suspicious activity
- **Spread Rumors** - Create and circulate gossip about targets

### 2. ✅ Village Discussions
- **Council Phase Conversations** - Simulated NPC discussions during council time (7-8 AM)
- **Dynamic Content** - Discussions based on game events (deaths, evidence, etc.)
- **Contextual Dialogue** - NPCs talk about relevant events

### 3. ✅ Investigation System
- **Behavior Analysis** - Activity level, consistency, social interaction
- **Suspicious Findings** - Detailed list of unusual behaviors
- **Suspicion Rankings** - Visual bars showing most suspected NPCs
- **Trust Rankings** - Visual bars showing most trusted NPCs
- **Summary Statistics** - Total evidence, rumors, and observations

### 4. ✅ Interactive Dialogue
- **Conversation Interface** - Clean dialogue box with NPC responses
- **Multiple Choices** - 3 response options per conversation
- **Relationship Effects** - Choices affect trust and suspicion
- **Real-time Feedback** - See effects in event log

---

## 🎨 UI Enhancements

### New Panels Added
1. **Village Discussions Panel** - Shows council conversations and dialogue
2. **Player Actions Panel** - Interactive controls for all player actions
3. **Investigation Summary Panel** - Comprehensive data visualization

### Visual Improvements
- Dropdown selects for NPC selection
- Text input for rumor creation
- Conversation boxes with styled dialogue
- Investigation cards with detailed results
- Suspicion/trust bars with gradient fills
- Discussion items with speaker names
- Responsive dialogue options

### CSS Additions
- `.action-select` - Styled dropdowns
- `.action-input` - Styled text inputs
- `.conversation-box` - Dialogue container
- `.dialogue-option` - Interactive response buttons
- `.discussion-item` - Council conversation styling
- `.investigation-card` - Investigation results display
- `.suspicion-bar` / `.trust-bar` - Visual meters
- `.bar-fill` - Animated progress bars

---

## 🔧 Technical Implementation

### New JavaScript Functions
```javascript
updateNPCSelects()           // Populate all NPC dropdowns
talkToNPC()                  // Initiate conversation
showDialogueBox()            // Display dialogue interface
selectDialogueOption()       // Handle response selection
investigateNPC()             // Perform investigation
showInvestigationResults()   // Display findings
spreadRumor()                // Create and add rumor
updateInvestigationSummary() // Load and display summary
updateVillageDiscussions()   // Generate council talks
```

### API Endpoints Used
```
GET  /api/dialogue/npc/{npcId}              - Get dialogue options
POST /api/dialogue/respond                   - Submit dialogue choice
GET  /api/investigation/behavior/{npcId}     - Get behavior pattern
GET  /api/investigation/suspicious/{npcId}   - Get suspicious findings
GET  /api/investigation/summary              - Get investigation overview
```

### Data Flow
1. **User Action** → Button click
2. **API Call** → Fetch data from backend
3. **Data Processing** → Format and prepare display
4. **UI Update** → Show results in appropriate panel
5. **Event Log** → Record action in event log

---

## 📊 Features Comparison

### Before (v1.1)
- ❌ No player actions
- ❌ No conversations
- ❌ No investigations
- ❌ No village discussions
- ❌ No rumor spreading
- ✅ Basic display only
- ✅ Time advancement
- ✅ View NPCs, evidence, rumors

### After (v2.0)
- ✅ Full player actions
- ✅ Interactive conversations
- ✅ Detailed investigations
- ✅ Village discussions
- ✅ Rumor spreading
- ✅ Enhanced display
- ✅ Time advancement
- ✅ View NPCs, evidence, rumors
- ✅ Investigation summary
- ✅ Suspicion/trust visualization

---

## 🎮 How to Use New Features

### 1. Talk to NPCs
```
1. Select NPC from "Talk to NPCs" dropdown
2. Click "Start Conversation"
3. Read NPC's dialogue
4. Choose one of 3 response options
5. See effect in event log
```

### 2. Investigate NPCs
```
1. Select NPC from "Investigate" dropdown
2. Click "Investigate Behavior"
3. View behavior pattern analysis
4. Read suspicious findings list
5. Use info to guide decisions
```

### 3. Spread Rumors
```
1. Select target from "Spread Rumor" dropdown
2. Type rumor text in input field
3. Click "Spread Rumor"
4. See rumor appear in Rumors panel
5. Watch it affect suspicion levels
```

### 4. View Village Discussions
```
1. Advance time to 7:00 AM (Council phase)
2. Check "Village Discussions" panel
3. Read NPC conversations
4. See what they're discussing
5. Use info to plan actions
```

### 5. Check Investigation Summary
```
1. Scroll to "Investigation Summary" panel
2. View total evidence/rumors/observations
3. Check "Most Suspicious" rankings
4. Check "Most Trusted" rankings
5. Use data to identify threats/allies
```

---

## 🚀 Getting Started

### Step 1: Start Server
```bash
./start-game.sh
```

### Step 2: Open Browser
```
http://localhost:5000
```

### Step 3: Verify Version
- Check title shows "v2.0"
- If not, hard refresh (Ctrl+F5)

### Step 4: Start Game
- Click "New Game"
- Check your role in "Your Role" panel

### Step 5: Try Features
1. Select an NPC and click "Start Conversation"
2. Select an NPC and click "Investigate Behavior"
3. Type a rumor and click "Spread Rumor"
4. Advance to 7:00 AM to see village discussions
5. Check Investigation Summary panel

---

## 📁 Files Created/Modified

### Modified
1. `src/VillageOfAshes.API/wwwroot/index.html` - Added all new features

### Created
1. `NEW_FEATURES_V2.md` - Comprehensive feature documentation
2. `QUICK_REFERENCE.md` - Quick reference guide
3. `V2_UPDATE_SUMMARY.md` - This file

---

## 🎯 What You Can Now Do

### Social Interaction
✅ Have conversations with any NPC
✅ Choose dialogue responses
✅ Build trust or increase suspicion
✅ See relationship changes

### Investigation
✅ Analyze NPC behavior patterns
✅ Find suspicious activities
✅ View suspicion rankings
✅ View trust rankings
✅ Track evidence and rumors

### Manipulation
✅ Spread rumors about NPCs
✅ Frame innocent villagers
✅ Deflect suspicion
✅ Influence social dynamics

### Observation
✅ Watch council discussions
✅ See NPC conversations
✅ Monitor accusations
✅ Track village sentiment

---

## 🎮 Gameplay Now Includes

### Detective Role
- Investigate all NPCs systematically
- Build trust with allies
- Spread accurate rumors to guide suspicion
- Use investigation summary to identify Butcher

### Doctor Role
- Talk to build alliances
- Investigate threats quietly
- Monitor trust levels
- Protect key NPCs (conceptually)

### Butcher Role
- Spread false rumors to frame innocents
- Build trust to appear innocent
- Investigate targets before killing
- Manipulate council discussions

### Vagabond Role
- Minimal interactions to stay hidden
- Investigate only immediate threats
- Avoid spreading rumors
- Survive 5 nights

### Farmer Role
- Build community trust
- Investigate threats to farm
- Spread defensive rumors if targeted
- Maintain resources for 7 days

---

## 🔄 Game Loop Now

1. **Morning (6-7 AM)** - Check for deaths, new evidence
2. **Council (7-8 AM)** - Watch village discussions, see accusations
3. **Day (8 AM-6 PM)** - Talk to NPCs, investigate, spread rumors
4. **Evening (6-9 PM)** - Final preparations
5. **Night (9 PM-6 AM)** - Backend simulation executes
6. **Repeat** - Continue until win/lose condition

---

## 📈 Next Steps

### Immediate
1. Play through a full game
2. Try all three action types
3. Watch village discussions during council
4. Check investigation summary regularly

### Strategic
1. Develop your role strategy
2. Build alliances through conversation
3. Use investigations to identify threats
4. Spread rumors strategically

### Advanced
1. Combine actions for complex strategies
2. Time actions with game phases
3. Use investigation data to predict roles
4. Manipulate social network through rumors

---

## 🐛 Known Limitations

### Current Limitations
- Dialogue options are simplified (3 generic choices)
- Village discussions are simulated (not real NPC AI)
- Rumor spreading doesn't immediately affect NPC behavior
- Investigation data may be limited early game

### Future Enhancements (v3.0)
- Role-specific actions (Detective track, Doctor heal)
- Evidence examination interface
- NPC relationship graph visualization
- Council voting system
- Inventory and item system
- Location-based movement
- Time-consuming actions

---

## ✅ Verification

### Server Status
```bash
# Check server is running
curl http://localhost:5000/

# Verify v2.0 is served
curl -s http://localhost:5000/ | grep "v2.0"

# Run verification script
./verify-fix.sh
```

### Browser Check
1. Open http://localhost:5000
2. Title should show "Village of Ashes - v2.0"
3. Header should show "v2.0"
4. Player Actions panel should be visible
5. Village Discussions panel should be visible
6. Investigation Summary panel should be visible

---

## 🎉 Summary

**Version 2.0 is now live!**

The game now includes:
- ✅ Full player interaction system
- ✅ Conversation mechanics
- ✅ Investigation features
- ✅ Village discussions
- ✅ Rumor spreading
- ✅ Comprehensive data visualization

**The game is now fully playable with rich social mechanics!**

---

**Remember:** Hard refresh (Ctrl+F5) if you don't see v2.0!

**Enjoy the enhanced Village of Ashes experience! 🏚️💀🎮**
