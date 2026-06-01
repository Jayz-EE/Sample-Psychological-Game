# 🔧 Auto-Time Implementation Guide

## Developer Reference

This document provides technical details for developers working with the Auto-Time feature.

---

## 📁 Files Modified

### Backend (C#)

1. **`VillageOfAshes.Core/Entities/GameState.cs`**
   - Added auto-time configuration properties
   - Added `LastAutoAdvance` timestamp tracking

2. **`VillageOfAshes.API/Controllers/GameController.cs`**
   - Enhanced `CreateNewGame()` to accept configuration
   - Added `ConfigureAutoTime()` endpoint
   - Added `ToggleAutoTime()` endpoint
   - Added `ShouldAutoAdvance()` endpoint
   - Added `AutoAdvanceTime()` endpoint
   - Updated `ToClientGameState()` to include auto-time settings
   - Added request models: `NewGameRequest`, `AutoTimeConfigRequest`

### Frontend (HTML/JavaScript)

3. **`VillageOfAshes.API/wwwroot/index.html`**
   - Added configuration modal UI
   - Added auto-time toggle button
   - Added JavaScript polling system
   - Added auto-time management functions
   - Updated `updateUI()` to sync button state

---

## 🔌 API Endpoints

### 1. Create New Game with Configuration

**Endpoint:** `POST /api/game/new`

**Request Body:**
```json
{
  "autoTimeEnabled": true,
  "autoTimeIntervalSeconds": 5,
  "autoTimeIncrementMinutes": 30,
  "pauseOnCouncil": true,
  "pauseOnDeath": true,
  "pauseOnPlayerAction": false
}
```

**Response:**
```json
{
  "id": "game-id",
  "currentDay": 1,
  "currentTime": "06:00:00",
  "autoTime": {
    "autoTimeEnabled": true,
    "autoTimeIntervalSeconds": 5,
    "autoTimeIncrementMinutes": 30,
    "pauseOnCouncil": true,
    "pauseOnDeath": true,
    "pauseOnPlayerAction": false,
    "lastAutoAdvance": "2026-06-01T10:00:00Z"
  },
  // ... rest of game state
}
```

### 2. Configure Auto-Time Settings

**Endpoint:** `POST /api/game/auto-time/configure`

**Request Body:**
```json
{
  "autoTimeEnabled": true,
  "autoTimeIntervalSeconds": 10,
  "autoTimeIncrementMinutes": 60,
  "pauseOnCouncil": false,
  "pauseOnDeath": true,
  "pauseOnPlayerAction": false
}
```

**Notes:**
- All fields are optional
- Only provided fields will be updated
- Values are clamped to valid ranges

### 3. Toggle Auto-Time

**Endpoint:** `POST /api/game/auto-time/toggle`

**Request Body:** None

**Response:**
```json
{
  "autoTimeEnabled": true,
  "message": "Auto-time enabled"
}
```

### 4. Check Should Advance

**Endpoint:** `GET /api/game/auto-time/should-advance`

**Response:**
```json
{
  "shouldAdvance": true,
  "secondsSinceLastAdvance": 5.2,
  "intervalSeconds": 5,
  "incrementMinutes": 30
}
```

**Logic:**
```csharp
if (!AutoTimeEnabled || Status != InProgress)
    return false;

if (PauseOnCouncil && CurrentPhase == VillageCouncil)
    return false;

var elapsed = (DateTime.UtcNow - LastAutoAdvance).TotalSeconds;
return elapsed >= AutoTimeIntervalSeconds;
```

### 5. Execute Auto-Advance

**Endpoint:** `POST /api/game/auto-time/advance`

**Request Body:** None

**Response:** Full game state (same as `/api/game/state`)

**Logic:**
```csharp
1. Check if auto-time is enabled
2. Record previous alive count
3. Advance time by AutoTimeIncrementMinutes
4. Update LastAutoAdvance timestamp
5. Check if death occurred
6. If PauseOnDeath && death occurred:
   - Set AutoTimeEnabled = false
   - Add notification
7. Return updated game state
```

---

## 💾 Data Models

### GameState Properties

```csharp
public class GameState
{
    // ... existing properties ...
    
    // Auto-Time Configuration
    public bool AutoTimeEnabled { get; set; } = false;
    public int AutoTimeIntervalSeconds { get; set; } = 5;
    public int AutoTimeIncrementMinutes { get; set; } = 30;
    public DateTime LastAutoAdvance { get; set; } = DateTime.UtcNow;
    public bool PauseOnCouncil { get; set; } = true;
    public bool PauseOnDeath { get; set; } = true;
    public bool PauseOnPlayerAction { get; set; } = false;
}
```

### Request Models

```csharp
public class NewGameRequest
{
    public bool AutoTimeEnabled { get; set; } = false;
    public int AutoTimeIntervalSeconds { get; set; } = 5;
    public int AutoTimeIncrementMinutes { get; set; } = 30;
    public bool PauseOnCouncil { get; set; } = true;
    public bool PauseOnDeath { get; set; } = true;
    public bool PauseOnPlayerAction { get; set; } = false;
}

public class AutoTimeConfigRequest
{
    public bool? AutoTimeEnabled { get; set; }
    public int? AutoTimeIntervalSeconds { get; set; }
    public int? AutoTimeIncrementMinutes { get; set; }
    public bool? PauseOnCouncil { get; set; }
    public bool? PauseOnDeath { get; set; }
    public bool? PauseOnPlayerAction { get; set; }
}
```

---

## 🎨 Frontend Implementation

### Polling System

```javascript
let autoTimeInterval = null;

function startAutoTimePolling() {
    if (autoTimeInterval) {
        clearInterval(autoTimeInterval);
    }
    
    // Poll every 1 second
    autoTimeInterval = setInterval(async () => {
        if (!gameState?.autoTime?.autoTimeEnabled) {
            stopAutoTimePolling();
            return;
        }
        
        const response = await fetch(`${API_BASE}/game/auto-time/should-advance`);
        const result = await response.json();
        
        if (result.shouldAdvance) {
            await autoAdvanceTime();
        }
    }, 1000);
}

function stopAutoTimePolling() {
    if (autoTimeInterval) {
        clearInterval(autoTimeInterval);
        autoTimeInterval = null;
    }
}
```

### Auto-Advance Function

```javascript
async function autoAdvanceTime() {
    try {
        const response = await fetch(`${API_BASE}/game/auto-time/advance`, {
            method: 'POST'
        });
        
        if (!response.ok) throw new Error(await response.text());
        gameState = await response.json();
        
        // Check if paused due to death
        if (!gameState.autoTime.autoTimeEnabled) {
            stopAutoTimePolling();
        }
        
        updateUI();
    } catch (error) {
        console.error('Error auto-advancing time:', error);
    }
}
```

### Button State Management

```javascript
function updateAutoTimeButton() {
    const button = document.getElementById('autoTimeToggle');
    if (!button) return;
    
    if (gameState?.autoTime?.autoTimeEnabled) {
        button.textContent = '⏸️ Pause Auto';
        button.style.background = '#00aa00';
    } else {
        button.textContent = '▶️ Auto-Time';
        button.style.background = '#8b0000';
    }
}
```

---

## 🔄 State Flow Diagram

```
User Clicks "New Game"
    ↓
Configuration Modal Opens
    ↓
User Configures Settings
    ↓
POST /api/game/new (with config)
    ↓
GameState Created with Auto-Time Settings
    ↓
Frontend Receives Game State
    ↓
If AutoTimeEnabled:
    ↓
    Start Polling (every 1 second)
        ↓
        GET /api/game/auto-time/should-advance
        ↓
        If shouldAdvance == true:
            ↓
            POST /api/game/auto-time/advance
            ↓
            Update GameState
            ↓
            Check Pause Conditions
            ↓
            If Death Detected && PauseOnDeath:
                ↓
                Set AutoTimeEnabled = false
                ↓
                Stop Polling
                ↓
                Show Notification
            ↓
            Update UI
        ↓
        Loop back to polling
```

---

## 🧪 Testing Checklist

### Backend Tests

- [ ] Create game with auto-time enabled
- [ ] Create game with auto-time disabled
- [ ] Toggle auto-time on/off
- [ ] Configure auto-time settings
- [ ] Check should-advance logic
- [ ] Execute auto-advance
- [ ] Verify pause on council
- [ ] Verify pause on death
- [ ] Verify value clamping (min/max)
- [ ] Handle invalid requests

### Frontend Tests

- [ ] Modal opens on "New Game" click
- [ ] Modal closes on X click
- [ ] Configuration values are sent correctly
- [ ] Polling starts when enabled
- [ ] Polling stops when disabled
- [ ] Button state updates correctly
- [ ] Auto-advance executes properly
- [ ] Pause conditions work
- [ ] Manual override works
- [ ] UI updates after auto-advance

### Integration Tests

- [ ] Full game flow with auto-time
- [ ] Council pause behavior
- [ ] Death pause behavior
- [ ] Resume after pause
- [ ] Multiple pause/resume cycles
- [ ] Game over with auto-time active
- [ ] Browser tab inactive behavior
- [ ] Page refresh persistence

---

## 🐛 Common Issues & Solutions

### Issue 1: Polling Doesn't Stop

**Symptom:** Auto-time continues after toggling off

**Solution:**
```javascript
// Ensure stopAutoTimePolling() clears interval
function stopAutoTimePolling() {
    if (autoTimeInterval) {
        clearInterval(autoTimeInterval);
        autoTimeInterval = null; // Important!
    }
}
```

### Issue 2: Button State Out of Sync

**Symptom:** Button shows wrong state after actions

**Solution:**
```javascript
// Call updateAutoTimeButton() in updateUI()
function updateUI() {
    // ... other updates ...
    updateAutoTimeButton(); // Add this
}
```

### Issue 3: Pause Conditions Not Working

**Symptom:** Game doesn't pause during council/death

**Solution:**
```csharp
// Check pause logic in ShouldAutoAdvance
if (PauseOnCouncil && CurrentPhase == GamePhase.VillageCouncil)
    return false;

// Check death detection in AutoAdvanceTime
var previousAliveCount = game.NPCs.Count(n => n.Status == NPCStatus.Alive);
// ... advance time ...
var currentAliveCount = game.NPCs.Count(n => n.Status == NPCStatus.Alive);
if (PauseOnDeath && currentAliveCount < previousAliveCount)
{
    game.AutoTimeEnabled = false;
}
```

### Issue 4: Rapid Polling Overhead

**Symptom:** High CPU usage from polling

**Solution:**
```javascript
// Use 1-second interval (not faster)
setInterval(async () => {
    // Lightweight check first
    if (!gameState?.autoTime?.autoTimeEnabled) {
        stopAutoTimePolling();
        return;
    }
    // Then fetch
}, 1000); // Don't go below 1000ms
```

---

## 🚀 Performance Considerations

### Backend

- **Lightweight Checks:** `ShouldAutoAdvance` only checks timestamps and flags
- **No Heavy Computation:** Actual simulation only runs in `AutoAdvanceTime`
- **Singleton State:** In-memory state access is fast

### Frontend

- **1-Second Polling:** Balance between responsiveness and overhead
- **Conditional Fetching:** Only advance when threshold met
- **Efficient Updates:** Only update UI when state changes

### Optimization Tips

1. **Batch Updates:** Consider batching multiple small advances
2. **Debounce UI:** Don't update UI on every poll, only on state change
3. **Lazy Loading:** Load heavy data (evidence, rumors) only when tab is active
4. **Web Workers:** Consider moving polling to web worker (future enhancement)

---

## 🔮 Future Enhancements

### Database Persistence

```csharp
// Add to DbContext
public DbSet<GameState> GameStates { get; set; }

// Save auto-time settings
await _context.SaveChangesAsync();
```

### SignalR Real-Time Updates

```csharp
// Push updates instead of polling
await Clients.All.SendAsync("GameStateUpdated", gameState);
```

### Advanced Pause Conditions

```csharp
public bool PauseOnEvidenceFound { get; set; }
public bool PauseOnRumorSpread { get; set; }
public bool PauseOnAllianceFormed { get; set; }
public List<string> PauseOnPhases { get; set; } // Specific phases
```

### Variable Speed

```csharp
public Dictionary<GamePhase, int> PhaseSpeedMultipliers { get; set; }
// e.g., Night: 2x, Day: 1x, Council: 0.5x
```

---

## 📚 Code References

### Key Methods

**Backend:**
- `GameController.CreateNewGame()` - Line ~50
- `GameController.ConfigureAutoTime()` - Line ~350
- `GameController.ToggleAutoTime()` - Line ~380
- `GameController.ShouldAutoAdvance()` - Line ~400
- `GameController.AutoAdvanceTime()` - Line ~430

**Frontend:**
- `showNewGameModal()` - Line ~1450
- `startNewGameWithConfig()` - Line ~1460
- `toggleAutoTime()` - Line ~1500
- `startAutoTimePolling()` - Line ~1530
- `autoAdvanceTime()` - Line ~1560
- `updateAutoTimeButton()` - Line ~1590

---

## 🎓 Best Practices

### For Developers

1. **Always Validate Input:** Clamp values to valid ranges
2. **Handle Edge Cases:** Game over, no active game, etc.
3. **Clear Intervals:** Always clean up polling intervals
4. **Sync State:** Keep button state in sync with game state
5. **Error Handling:** Gracefully handle network errors
6. **User Feedback:** Show notifications for state changes

### For Testers

1. **Test All Configurations:** Try different interval/increment combinations
2. **Test Pause Conditions:** Verify each pause condition works
3. **Test Edge Cases:** Game over, server restart, network issues
4. **Test Performance:** Monitor CPU/memory with auto-time active
5. **Test UX:** Ensure smooth, intuitive user experience

---

## 📞 Support

For questions or issues with the Auto-Time feature:

1. Check this documentation
2. Review `AUTO_TIME_FEATURE.md` for user-facing details
3. Examine code comments in modified files
4. Test with different configurations
5. Check browser console for errors

---

**Implementation Version:** 1.0  
**Last Updated:** June 2026  
**Status:** ✅ Production Ready
