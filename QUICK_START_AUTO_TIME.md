# ⚡ Quick Start: Auto-Time Feature

## 🎮 Get Started in 30 Seconds

### Step 1: Start a New Game
Click the **"New Game"** button in the game controls.

### Step 2: Configure Auto-Time
A modal will appear with these options:

```
┌─────────────────────────────────────────────┐
│  ⚙️ New Game Configuration                  │
├─────────────────────────────────────────────┤
│                                             │
│  🕐 Automated Time Progression              │
│                                             │
│  ✅ Enable Auto-Time                        │
│     (game progresses automatically)         │
│                                             │
│  Real-time interval: [5] seconds            │
│  Game time increment: [30] minutes          │
│                                             │
│  Pause Conditions:                          │
│  ✅ Pause during Village Council            │
│  ✅ Pause when someone dies                 │
│  ☐ Pause when player takes action           │
│                                             │
│         [🎮 Start Game]                     │
└─────────────────────────────────────────────┘
```

### Step 3: Play!
- Game advances automatically every 5 seconds by 30 minutes
- Pauses during councils and deaths
- Click **"⏸️ Pause Auto"** to pause anytime
- Click **"▶️ Auto-Time"** to resume

---

## 🎯 Recommended Presets

### 🔍 Detective Mode (Recommended for First-Time Players)
```
✅ Auto-Time Enabled
⏱️ Interval: 5 seconds
⏰ Increment: 30 minutes
✅ Pause on Council
✅ Pause on Death
```
**Best for:** Investigation, learning the game

### ⚡ Fast Observer Mode
```
✅ Auto-Time Enabled
⏱️ Interval: 3 seconds
⏰ Increment: 60 minutes
✅ Pause on Council
✅ Pause on Death
```
**Best for:** Watching village dynamics, experienced players

### 🎬 Cinematic Mode
```
✅ Auto-Time Enabled
⏱️ Interval: 7 seconds
⏰ Increment: 20 minutes
✅ Pause on Council
✅ Pause on Death
✅ Pause on Player Action
```
**Best for:** Story-focused, immersive experience

### 🎮 Classic Manual Mode
```
❌ Auto-Time Disabled
```
**Best for:** Full manual control, traditional gameplay

---

## 🎛️ Controls

### In-Game Buttons

**▶️ Auto-Time** (Red Button)
- Auto-time is OFF
- Click to enable automatic progression

**⏸️ Pause Auto** (Green Button)
- Auto-time is ON
- Click to pause automatic progression

**Advance 1 Hour**
- Manually jump forward 60 minutes
- Works even with auto-time enabled

**Advance 6 Hours**
- Manually jump forward 6 hours
- Quick skip for boring periods

---

## 💡 Quick Tips

### ✅ DO:
- Start with default settings (5s interval, 30min increment)
- Enable "Pause on Council" to participate in voting
- Enable "Pause on Death" to investigate murders
- Use manual advance for quick skips
- Pause when you need time to think

### ❌ DON'T:
- Set interval too low (<3 seconds) - too fast
- Set interval too high (>10 seconds) - too slow
- Disable all pause conditions - you'll miss events
- Forget to resume after pausing

---

## 🔄 Typical Gameplay Flow

```
6:00 AM - Game Starts
  ↓ (auto-advances every 5 seconds)
6:30 AM
  ↓
7:00 AM - Village Council
  ⏸️ AUTO-PAUSE
  
  [Read statements, vote, participate]
  
  Click "▶️ Auto-Time" to resume
  ↓
8:00 AM - Day Actions
  ↓ (auto-advances)
9:00 PM - Night Simulation
  ↓ (auto-advances through night)
6:00 AM - Morning Discovery
  💀 Death Detected
  ⏸️ AUTO-PAUSE
  
  Notification: "⏸️ Auto-time paused: 1 death(s) detected"
  
  [Investigate evidence, examine clues]
  
  Click "▶️ Auto-Time" to resume
```

---

## 🎨 Visual Guide

### Configuration Modal
```
┌───────────────────────────────────────────────────┐
│  ⚙️ New Game Configuration                   [×]  │
├───────────────────────────────────────────────────┤
│                                                   │
│  🕐 Automated Time Progression                    │
│  ─────────────────────────────────────────────    │
│                                                   │
│  ☑ Enable Auto-Time                              │
│     (game progresses automatically)               │
│                                                   │
│     Real-time interval (seconds):                 │
│     ┌─────┐                                       │
│     │  5  │ ← How often to advance               │
│     └─────┘                                       │
│                                                   │
│     Game time increment (minutes):                │
│     ┌─────┐                                       │
│     │ 30  │ ← How many game minutes per tick     │
│     └─────┘                                       │
│                                                   │
│     Pause Conditions:                             │
│     ☑ Pause during Village Council                │
│     ☑ Pause when someone dies                     │
│     ☐ Pause when player takes action              │
│                                                   │
│                                                   │
│              ┌──────────────────┐                 │
│              │  🎮 Start Game   │                 │
│              └──────────────────┘                 │
│                                                   │
└───────────────────────────────────────────────────┘
```

### Game Controls Bar
```
┌─────────────────────────────────────────────────────────┐
│  [New Game] [Advance 1 Hour] [Advance 6 Hours]          │
│  [⏸️ Pause Auto] [Refresh]                              │
│     ↑                                                   │
│     Green = Auto-time ON                                │
│     Red = Auto-time OFF                                 │
└─────────────────────────────────────────────────────────┘
```

---

## 🆘 Troubleshooting

### Problem: Game not advancing automatically

**Solution:**
1. Check if auto-time button is green (⏸️ Pause Auto)
2. If red (▶️ Auto-Time), click it to enable
3. Check if game is paused due to council or death
4. Verify game status is "In Progress" (not game over)

### Problem: Game advancing too fast

**Solution:**
1. Click "⏸️ Pause Auto" to stop
2. Click "New Game" to reconfigure
3. Increase interval (e.g., 5 → 10 seconds)
4. Decrease increment (e.g., 60 → 30 minutes)

### Problem: Game advancing too slow

**Solution:**
1. Click "⏸️ Pause Auto" to stop
2. Click "New Game" to reconfigure
3. Decrease interval (e.g., 5 → 3 seconds)
4. Increase increment (e.g., 30 → 60 minutes)
5. Or use "Advance 6 Hours" for quick skips

### Problem: Missed important event

**Solution:**
1. Enable "Pause on Death" in configuration
2. Enable "Pause on Council" in configuration
3. Check event log for what happened
4. Use "Refresh" to update display

---

## 📊 Speed Comparison

| Interval | Increment | Real Time → Game Time | Days per Hour |
|----------|-----------|----------------------|---------------|
| 3s | 60min | 3s → 1 hour | ~20 days |
| 5s | 30min | 5s → 30 min | ~6 days |
| 5s | 60min | 5s → 1 hour | ~12 days |
| 10s | 30min | 10s → 30 min | ~3 days |
| 10s | 60min | 10s → 1 hour | ~6 days |

**Example:** With 5s interval and 30min increment:
- 1 real minute = 12 advances = 6 game hours
- 10 real minutes = 120 advances = 60 game hours = 2.5 game days

---

## 🎓 Pro Tips

### Tip 1: Use Manual Override
Even with auto-time enabled, you can manually advance:
- Click "Advance 1 Hour" to skip ahead
- Click "Advance 6 Hours" to jump through boring periods
- Manual advances work alongside auto-time

### Tip 2: Pause for Investigation
When you find interesting evidence:
1. Click "⏸️ Pause Auto"
2. Investigate thoroughly
3. Click "▶️ Auto-Time" when ready

### Tip 3: Adjust Speed Mid-Game
You can't change settings mid-game, but you can:
- Toggle auto-time on/off
- Use manual advances to speed up
- Pause to slow down

### Tip 4: Watch the Event Log
The event log shows what's happening:
- Deaths
- Evidence found
- Council decisions
- Auto-time status changes

### Tip 5: Experiment with Settings
Try different configurations:
- Start with defaults
- Adjust based on your preference
- Find your perfect pace

---

## 🎯 Common Use Cases

### Use Case 1: First Playthrough
```
Goal: Learn the game mechanics
Settings: 5s interval, 30min increment, all pauses enabled
Strategy: Let game guide you with automatic pauses
```

### Use Case 2: Investigation Focus
```
Goal: Solve the mystery
Settings: 5s interval, 30min increment, pause on death
Strategy: Pause when deaths occur, investigate thoroughly
```

### Use Case 3: Observation Mode
```
Goal: Watch village dynamics
Settings: 3s interval, 60min increment, pause on council only
Strategy: Fast progression, only pause for councils
```

### Use Case 4: Story Experience
```
Goal: Immersive narrative
Settings: 7s interval, 20min increment, all pauses enabled
Strategy: Slow pace with frequent pauses for decisions
```

---

## 📱 One-Page Cheat Sheet

```
╔═══════════════════════════════════════════════════════╗
║           AUTO-TIME QUICK REFERENCE                   ║
╠═══════════════════════════════════════════════════════╣
║                                                       ║
║  🎮 START NEW GAME                                    ║
║     1. Click "New Game"                               ║
║     2. Configure settings                             ║
║     3. Click "Start Game"                             ║
║                                                       ║
║  ⚙️ RECOMMENDED SETTINGS                              ║
║     ✅ Auto-Time: Enabled                             ║
║     ⏱️ Interval: 5 seconds                            ║
║     ⏰ Increment: 30 minutes                          ║
║     ✅ Pause on Council                               ║
║     ✅ Pause on Death                                 ║
║                                                       ║
║  🎛️ CONTROLS                                          ║
║     ▶️ Auto-Time (Red) = OFF, click to enable        ║
║     ⏸️ Pause Auto (Green) = ON, click to pause       ║
║     Advance 1 Hour = Manual 60min jump                ║
║     Advance 6 Hours = Manual 6hr jump                 ║
║                                                       ║
║  💡 TIPS                                              ║
║     • Start with defaults                             ║
║     • Enable pause conditions                         ║
║     • Use manual advance for skips                    ║
║     • Pause when investigating                        ║
║     • Resume when ready                               ║
║                                                       ║
║  🆘 TROUBLESHOOTING                                   ║
║     Not advancing? → Check button is green            ║
║     Too fast? → Increase interval or decrease incr    ║
║     Too slow? → Decrease interval or increase incr    ║
║     Missed event? → Enable pause conditions           ║
║                                                       ║
╚═══════════════════════════════════════════════════════╝
```

---

## 🚀 Ready to Play!

You're all set! Here's what to do:

1. **Click "New Game"**
2. **Keep default settings** (or customize)
3. **Click "🎮 Start Game"**
4. **Watch the village come alive!**

The game will progress automatically, pausing for important events. Focus on investigation and strategy while the village evolves around you.

**Enjoy the horror! 🏚️💀**

---

**For detailed information:**
- User Guide: [AUTO_TIME_FEATURE.md](AUTO_TIME_FEATURE.md)
- Developer Guide: [AUTO_TIME_IMPLEMENTATION.md](AUTO_TIME_IMPLEMENTATION.md)
- Implementation Summary: [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
