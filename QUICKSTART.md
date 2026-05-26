# 🚀 Quick Start Guide

## Installation & Setup

### 1. Verify .NET Installation
```bash
dotnet --version
# Should show 10.x.x or higher
```

### 2. Build the Project
```bash
# From the Village directory
dotnet build
```

### 3. Run the Game

**Option A: Using the startup script (Recommended)**
```bash
# From the Village directory
./start-game.sh
```

**Option B: Manual start**
```bash
cd src/VillageOfAshes.API
dotnet run --urls "http://localhost:5000"
```

**Note:** The game must run on port 5000 to match the frontend configuration.

### 4. Open in Browser
Navigate to: `http://localhost:5000`

## 🎮 How to Play

### Starting a Game

1. Click **"New Game"** button
2. The game initializes with:
   - 5 NPCs with random roles
   - 1 Shopkeeper (fixed)
   - You (the player) with a random role
   - Starting time: 6:00 AM, Day 1

### Understanding the Interface

#### Top Controls
- **New Game**: Start fresh
- **Advance 1 Hour**: Move time forward 60 minutes
- **Advance 6 Hours**: Quick time skip
- **Refresh**: Update display

#### Game Info Cards
- **Day**: Current day number
- **Time**: Current in-game time
- **Phase**: Current game phase
- **Alive**: Living NPCs / Total NPCs

#### Main Panels

**👥 Villagers Panel**
- Shows all NPCs and their status
- Green cards = Alive
- Faded cards = Dead 💀
- Displays: Name, Role, Alignment, Location, Health, Hunger

**🔍 Evidence Panel**
- Lists all discovered evidence
- Shows: Type, Location, Visibility %, Decay time

**💬 Rumors Panel**
- Displays circulating rumors
- Shows: Source, Target, Context, Truthfulness

**📜 Your Role Panel**
- Your character information
- Role and alignment
- Current objective

**📰 Event Log**
- Real-time game events
- System messages
- Important notifications

### Game Phases

#### 🌙 Night Simulation (9:00 PM - 5:00 AM)
- Backend executes role actions
- Butcher may kill
- Detective tracks movements
- Doctor protects someone
- Evidence is generated

#### 🌅 Morning Discovery (6:00 AM - 7:00 AM)
- Discover what happened overnight
- Find evidence
- Notice missing villagers

#### 🏛️ Village Council (7:00 AM - 8:00 AM)
- NPCs discuss events
- Accusations are made
- Voting may occur
- Alliances form

#### ☀️ Day Actions (8:00 AM - 6:00 PM)
- Role-specific actions
- Investigation
- Trading
- Healing

#### 🌆 Evening (6:00 PM - 9:00 PM)
- Limited movement
- Preparation for night

### Strategy Tips

#### As Detective (Good)
- Track suspicious NPCs at night
- Examine evidence carefully
- Build trust with other good roles
- Identify the Butcher before it's too late

#### As Doctor (Good)
- Protect high-value targets
- Keep track of injuries
- Build alliances
- Don't reveal your role too early

#### As Butcher (Evil)
- Kill strategically
- Avoid leaving obvious evidence
- Blend in during council
- Frame others for your crimes

#### As Vagabond (Neutral)
- Stay under the radar
- Gather information
- Survive 5 nights
- Plan your escape

#### As Farmer (Neutral)
- Maintain food supply
- Build trust through usefulness
- Protect your crops
- Survive 7 days

### Reading the Game State

#### NPC Suspicion
- NPCs track suspicion of each other
- High suspicion leads to accusations
- Evidence increases suspicion
- Rumors spread suspicion

#### Trust Levels
- NPCs build trust through interactions
- High trust = alliances
- Low trust = conflict
- Trust affects voting

#### Evidence Interpretation
- **Blood**: Likely from Butcher
- **Footprints**: Someone was there
- **Broken Lock**: Forced entry
- **Damaged Crops**: Farmer's livelihood threatened

### Win Conditions

#### Good Faction Wins
- Eliminate the Butcher
- Keep village stable
- Protect innocents

#### Evil Faction Wins
- Butcher outnumbers good roles
- Village collapses into chaos

#### Neutral Wins
- **Vagabond**: Survive 5 nights, escape
- **Farmer**: Maintain food for 7 days

## 🎯 Example Playthrough

### Day 1 Morning (6:00 AM)
```
1. Click "New Game"
2. Check your role in "Your Role" panel
3. Note all NPC names and roles (you can see them for testing)
4. Click "Advance 1 Hour" to reach Council time
```

### Day 1 Council (7:00 AM)
```
1. Watch for accusations in event log
2. Note which NPCs are suspicious of each other
3. Check rumors panel for gossip
```

### Day 1 Day Phase (8:00 AM)
```
1. Advance time through the day
2. Watch for evidence generation
3. Monitor NPC movements
```

### Day 1 Night (9:00 PM)
```
1. Advance into night phase
2. Night simulation executes automatically
3. Check event log for "killed during the night"
4. Evidence is generated
```

### Day 2 Morning (6:00 AM)
```
1. Check Villagers panel for deaths
2. Review new evidence
3. New rumors may have spread
4. Suspicion levels updated
```

## 🔧 Troubleshooting

### Game Won't Start - "Address already in use"
**Problem:** Port 5000 (or 5151) is already occupied by another process.

**Solution:**
```bash
# Find the process using the port
lsof -i :5000

# Kill the process (replace PID with the actual process ID)
kill -9 PID

# Or use the startup script which handles this automatically
./start-game.sh
```

### Port Mismatch Error
**Problem:** Server runs on port 5151 but frontend expects port 5000.

**Solution:** Always start the server with the correct port:
```bash
dotnet run --urls "http://localhost:5000"
```

### Port Already in Use
```bash
# Run on different port
dotnet run --urls "http://localhost:5001"
# Update API_BASE in index.html to match
```

### CORS Errors
- Make sure API is running
- Check browser console for errors
- Verify API_BASE URL in index.html

### Game Not Loading
- Check that API is running
- Open browser dev tools (F12)
- Look for JavaScript errors
- Verify API endpoints are accessible

### No NPCs Showing
- Click "Refresh" button
- Check browser console
- Restart API server

## 📊 Testing Scenarios

### Test Night Simulation
```
1. Start new game
2. Note alive count
3. Advance to 9:00 PM (night phase)
4. Advance through night to 6:00 AM
5. Check for deaths and evidence
```

### Test Council System
```
1. Play through to Day 2
2. Ensure at least one death occurred
3. Advance to 7:00 AM council
4. Watch for accusations in event log
```

### Test Evidence Generation
```
1. Advance through multiple nights
2. Check Evidence panel
3. Note evidence types and locations
4. Watch decay timers
```

## 🎨 Customization

### Modify Game Speed
In `index.html`, change advance time amounts:
```javascript
<button onclick="advanceTime(30)">30 Minutes</button>
<button onclick="advanceTime(120)">2 Hours</button>
```

### Change Starting Conditions
In `GameController.cs`, modify `InitializeGame()`:
```csharp
// Change number of NPCs
var shuffledNames = allNames.OrderBy(_ => random.Next()).Take(8).ToList();

// Change starting time
CurrentTime = new TimeSpan(9, 0, 0), // Start at 9 AM
```

### Adjust Simulation Rates
In `NightSimulationService.cs`:
```csharp
// Change kill probability
if (_random.Next(100) < 50) // Was 70, now 50%
    roleActions.Add("KillNPC");
```

## 📚 Next Steps

1. **Play through a full game** - Experience all phases
2. **Try different roles** - Restart to get different assignments
3. **Watch NPC behavior** - Observe suspicion and trust dynamics
4. **Experiment with time** - See how different phases affect gameplay
5. **Read the code** - Understand the simulation mechanics

## 🆘 Need Help?

- Check `README.md` for detailed documentation
- Review `prototype_md_social_horror_village_simulation.md` for game design
- Examine API endpoints at `http://localhost:5000/swagger`

---

**Enjoy the psychological horror! 🏚️💀**
