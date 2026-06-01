# 🕐 Automated Time Progression Feature

## Overview

The **Auto-Time** feature allows the game to progress automatically without requiring manual time advancement clicks. This creates a smoother, more immersive gameplay experience where you can focus on investigation and decision-making while the village simulation runs in the background.

---

## 🎮 How It Works

### Automatic Time Advancement

When enabled, the game automatically advances time at configurable intervals:

```
Real-World Time → Game Time Advancement
Every 5 seconds → Advance 30 game minutes (default)
```

This means the game progresses continuously, simulating the passage of time in the village without player intervention.

### Intelligent Pause System

The auto-time system includes smart pause conditions to prevent missing important events:

1. **Pause on Council** - Stops during Village Council meetings so you can participate
2. **Pause on Death** - Halts when someone dies so you can investigate
3. **Pause on Player Action** - Optional pause when you take an action

---

## ⚙️ Configuration Options

### When Starting a New Game

Click **"New Game"** to open the configuration modal with these settings:

#### **Enable Auto-Time**
- ✅ Checked: Game progresses automatically
- ❌ Unchecked: Manual time advancement only (classic mode)

#### **Real-Time Interval (seconds)**
- **Range:** 1-60 seconds
- **Default:** 5 seconds
- **Description:** How often the game checks and advances time in real-world seconds
- **Examples:**
  - `3 seconds` = Very fast progression
  - `5 seconds` = Balanced (recommended)
  - `10 seconds` = Slower, more deliberate pace

#### **Game Time Increment (minutes)**
- **Range:** 5-360 minutes
- **Default:** 30 minutes
- **Description:** How many in-game minutes to advance each tick
- **Examples:**
  - `15 minutes` = Granular control, slower progression
  - `30 minutes` = Balanced (recommended)
  - `60 minutes` = Fast progression, 1 hour per tick
  - `180 minutes` = Very fast, 3 hours per tick

#### **Pause Conditions**

**Pause on Council** (Recommended: ✅)
- Automatically pauses during Village Council phase (7:00 AM - 8:00 AM)
- Allows you to read NPC statements, make accusations, and vote
- Resume by clicking the **"▶️ Auto-Time"** button

**Pause on Death** (Recommended: ✅)
- Automatically pauses when an NPC dies
- Gives you time to investigate the death and examine new evidence
- Shows notification: "⏸️ Auto-time paused: 1 death(s) detected"
- Resume manually after investigation

**Pause on Player Action** (Optional: ❌)
- Pauses whenever you perform any action
- Useful if you want full control over time progression
- Not recommended for smooth gameplay

---

## 🎛️ In-Game Controls

### Auto-Time Toggle Button

Located in the game controls bar:

**▶️ Auto-Time** (Red button)
- Auto-time is currently **OFF**
- Click to enable automatic progression

**⏸️ Pause Auto** (Green button)
- Auto-time is currently **ON**
- Click to pause automatic progression

### Manual Time Advancement

Even with auto-time enabled, you can still use manual controls:
- **Advance 1 Hour** - Jump forward 60 minutes
- **Advance 6 Hours** - Quick skip 6 hours

These work alongside auto-time for flexible control.

---

## 📊 Configuration Examples

### Example 1: Relaxed Observation Mode
```
✅ Auto-Time Enabled
⏱️ Interval: 10 seconds
⏰ Increment: 30 minutes
✅ Pause on Council
✅ Pause on Death
❌ Pause on Player Action

Result: Game advances every 10 seconds by 30 minutes.
        Pauses for important events.
        Smooth, relaxed pace for observation.
```

### Example 2: Fast-Paced Action Mode
```
✅ Auto-Time Enabled
⏱️ Interval: 3 seconds
⏰ Increment: 60 minutes
✅ Pause on Council
✅ Pause on Death
❌ Pause on Player Action

Result: Game advances every 3 seconds by 1 hour.
        Rapid progression through days.
        Good for experienced players.
```

### Example 3: Cinematic Story Mode
```
✅ Auto-Time Enabled
⏱️ Interval: 5 seconds
⏰ Increment: 15 minutes
✅ Pause on Council
✅ Pause on Death
✅ Pause on Player Action

Result: Game advances every 5 seconds by 15 minutes.
        Pauses frequently for player input.
        Cinematic, story-focused experience.
```

### Example 4: Classic Manual Mode
```
❌ Auto-Time Disabled
⏱️ Interval: N/A
⏰ Increment: N/A
❌ All pause conditions

Result: No automatic progression.
        Full manual control via buttons.
        Original gameplay experience.
```

---

## 🎯 Recommended Settings by Playstyle

### 🔍 **Detective/Investigation Focus**
```
Interval: 5 seconds
Increment: 30 minutes
Pause on Council: ✅
Pause on Death: ✅
Pause on Player Action: ❌
```
**Why:** Balanced pace with pauses for investigation opportunities.

### ⚡ **Fast Simulation Observer**
```
Interval: 3 seconds
Increment: 60 minutes
Pause on Council: ✅
Pause on Death: ✅
Pause on Player Action: ❌
```
**Why:** Quickly see how the village evolves over multiple days.

### 🎬 **Story-Driven Experience**
```
Interval: 7 seconds
Increment: 20 minutes
Pause on Council: ✅
Pause on Death: ✅
Pause on Player Action: ✅
```
**Why:** Slower pace with frequent pauses for immersive storytelling.

### 🎮 **Classic Manual Control**
```
Auto-Time: ❌ Disabled
```
**Why:** Traditional gameplay with full manual time control.

---

## 🔄 Typical Gameplay Flow with Auto-Time

### Day 1 Morning (Auto-Time Enabled)

```
6:00 AM - Game starts
  ↓ (5 seconds pass)
6:30 AM - Auto-advance
  ↓ (5 seconds pass)
7:00 AM - Village Council begins
  ⏸️ AUTO-PAUSE (Pause on Council)
  
  [You read NPC statements, make accusations, vote]
  
  Click "▶️ Auto-Time" to resume
  ↓
8:00 AM - Council ends, Day Actions begin
  ↓ (5 seconds pass)
8:30 AM - Auto-advance
  ↓ (continues automatically)
```

### Night Phase (Death Occurs)

```
9:00 PM - Night Simulation begins
  ↓ (auto-advances through night)
5:00 AM - Night ends
  ↓
6:00 AM - Morning Discovery
  💀 NPC Death Detected
  ⏸️ AUTO-PAUSE (Pause on Death)
  
  Notification: "⏸️ Auto-time paused: 1 death(s) detected"
  
  [You investigate evidence, examine body, review events]
  
  Click "▶️ Auto-Time" to resume
```

---

## 🛠️ Technical Details

### Backend Implementation

**New API Endpoints:**

1. **POST** `/api/game/new` (Enhanced)
   - Accepts `NewGameRequest` with auto-time configuration
   - Initializes game with automation settings

2. **POST** `/api/game/auto-time/configure`
   - Update auto-time settings during gameplay
   - Body: `AutoTimeConfigRequest`

3. **POST** `/api/game/auto-time/toggle`
   - Toggle auto-time on/off
   - Returns current state

4. **GET** `/api/game/auto-time/should-advance`
   - Check if enough time has passed for auto-advance
   - Returns `shouldAdvance` boolean

5. **POST** `/api/game/auto-time/advance`
   - Execute automatic time advancement
   - Checks pause conditions
   - Returns updated game state

### Frontend Implementation

**JavaScript Polling System:**
```javascript
// Polls every 1 second to check if auto-advance should occur
setInterval(async () => {
    const response = await fetch('/api/game/auto-time/should-advance');
    const result = await response.json();
    
    if (result.shouldAdvance) {
        await autoAdvanceTime();
    }
}, 1000);
```

**State Management:**
- Auto-time state stored in `GameState.AutoTime` object
- Persists across page refreshes (while server is running)
- Button state updates automatically

---

## 📝 GameState Properties

New properties added to `GameState`:

```csharp
public bool AutoTimeEnabled { get; set; } = false;
public int AutoTimeIntervalSeconds { get; set; } = 5;
public int AutoTimeIncrementMinutes { get; set; } = 30;
public DateTime LastAutoAdvance { get; set; } = DateTime.UtcNow;
public bool PauseOnCouncil { get; set; } = true;
public bool PauseOnDeath { get; set; } = true;
public bool PauseOnPlayerAction { get; set; } = false;
```

---

## 🎨 UI/UX Features

### Visual Indicators

**Auto-Time Button States:**
- 🔴 **Red "▶️ Auto-Time"** - Auto-time is OFF
- 🟢 **Green "⏸️ Pause Auto"** - Auto-time is ON

**Notifications:**
- "▶️ Auto-time enabled" - When activated
- "⏸️ Auto-time paused" - When deactivated
- "⏸️ Auto-time paused: X death(s) detected" - When death triggers pause

### Configuration Modal

- Clean, dark-themed modal matching game aesthetic
- Organized sections for clarity
- Helpful tooltips explaining each setting
- Validation (min/max values enforced)

---

## 🚀 Benefits

### For Players

1. **Smoother Gameplay** - No constant clicking to advance time
2. **Focus on Strategy** - Spend time investigating, not managing time
3. **Immersive Experience** - Village feels alive and autonomous
4. **Flexible Control** - Pause anytime, manual override available
5. **Smart Pauses** - Never miss important events

### For Game Design

1. **Emergent Storytelling** - Events unfold naturally
2. **Reduced Tedium** - Less micromanagement
3. **Better Pacing** - Configurable speed for different playstyles
4. **Accessibility** - Easier for new players to follow

---

## ⚠️ Important Notes

### Server Restart Behavior

- Auto-time settings are **NOT** persisted to database (current implementation)
- Settings reset when server restarts
- Game state is in-memory only

### Performance Considerations

- Polling occurs every 1 second (lightweight check)
- Actual time advancement only when interval threshold met
- Minimal performance impact

### Browser Tab Behavior

- Auto-time continues even if tab is in background
- Polling may slow down in inactive tabs (browser behavior)
- Refresh page to sync state if tab was inactive

---

## 🔮 Future Enhancements

### Planned Features

1. **Persistent Settings** - Save auto-time preferences per player
2. **Speed Presets** - Quick buttons for "Slow/Normal/Fast"
3. **Phase-Specific Speeds** - Different speeds for different phases
4. **Event Notifications** - Browser notifications for important events
5. **Replay Mode** - Watch a game unfold at high speed
6. **Custom Pause Conditions** - Pause on specific events (e.g., evidence found)

### Advanced Options (Future)

```
- Pause on Evidence Discovery
- Pause on Rumor Spread
- Pause on Alliance Formation
- Pause on Resource Depletion
- Variable Speed (slow during day, fast at night)
- Scheduled Pauses (pause at specific times)
```

---

## 📖 Usage Examples

### Example Session 1: First-Time Player

```
1. Click "New Game"
2. Keep default settings:
   - Auto-Time: ✅ Enabled
   - Interval: 5 seconds
   - Increment: 30 minutes
   - Pause on Council: ✅
   - Pause on Death: ✅
3. Click "🎮 Start Game"
4. Watch the village come to life
5. When council starts, game pauses automatically
6. Read statements, make decisions
7. Click "▶️ Auto-Time" to continue
8. When death occurs, game pauses
9. Investigate evidence
10. Resume when ready
```

### Example Session 2: Experienced Player

```
1. Click "New Game"
2. Configure for fast pace:
   - Auto-Time: ✅ Enabled
   - Interval: 3 seconds
   - Increment: 60 minutes
   - Pause on Council: ✅
   - Pause on Death: ❌ (disabled)
3. Click "🎮 Start Game"
4. Game progresses rapidly
5. Only pauses for councils
6. Manually pause with "⏸️ Pause Auto" if needed
7. Investigate on your own schedule
```

### Example Session 3: Manual Control Purist

```
1. Click "New Game"
2. Disable auto-time:
   - Auto-Time: ❌ Disabled
3. Click "🎮 Start Game"
4. Use "Advance 1 Hour" and "Advance 6 Hours" buttons
5. Full manual control over time progression
6. Classic gameplay experience
```

---

## 🎓 Tips & Best Practices

### Getting Started

1. **Start with Defaults** - The default settings (5s interval, 30min increment) are well-balanced
2. **Enable Pause on Council** - You don't want to miss voting opportunities
3. **Enable Pause on Death** - Critical for investigation gameplay
4. **Disable Pause on Player Action** - Unless you want very granular control

### Optimization

1. **Adjust Speed Based on Phase** - Manually speed up boring phases
2. **Use Manual Override** - Don't be afraid to manually advance during slow periods
3. **Pause for Investigation** - Hit pause when you need time to think
4. **Resume Quickly** - Don't leave game paused too long or you'll lose immersion

### Troubleshooting

**Auto-time not advancing?**
- Check if game is paused (button shows "▶️ Auto-Time")
- Verify game status is "In Progress" (not game over)
- Check if pause condition is active (council, death)

**Game advancing too fast?**
- Increase interval seconds (e.g., 5 → 10)
- Decrease increment minutes (e.g., 60 → 30)
- Enable more pause conditions

**Game advancing too slow?**
- Decrease interval seconds (e.g., 5 → 3)
- Increase increment minutes (e.g., 30 → 60)
- Use manual "Advance 6 Hours" for big jumps

---

## 🏆 Conclusion

The Auto-Time feature transforms Village of Ashes from a turn-based simulation into a living, breathing world. Configure it to match your playstyle, and enjoy a smoother, more immersive psychological horror experience.

**Happy investigating! 🔍💀**

---

**Version:** 3.0  
**Last Updated:** June 2026  
**Feature Status:** ✅ Fully Implemented
