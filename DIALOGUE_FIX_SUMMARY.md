# Dialogue System Fix Summary

## Issue Description
User reported that "Casual Talk Dialogue Choices for Player doesn't show" when trying to talk to NPCs.

## Root Cause Analysis
1. **Property Name Mismatch**: The backend `DialogueExchange` class uses PascalCase properties (`Options`, `NpcId`, `Question`), but the .NET JSON serializer converts them to camelCase (`options`, `npcId`, `question`) by default
2. **Frontend Assumptions**: The `showDialogueBox()` function was expecting lowercase properties but also assumed a specific structure
3. **Missing Functions**: Council dialogue functions `loadCouncilDialogueOptions()` and `makeCouncilStatement()` were referenced in HTML but not implemented

## Fixes Applied

### 1. Fixed `talkToNPC()` Function
**File**: `src/VillageOfAshes.API/wwwroot/index.html`

Added property normalization to handle both PascalCase and camelCase:

```javascript
async function talkToNPC() {
    const npcId = document.getElementById('councilNpcSelect')?.value;
    if (!npcId) {
        addEvent('⚠️ Please select an NPC to talk to');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/dialogue/npc/${npcId}`);
        const dialogue = await response.json();
        
        // Convert PascalCase to camelCase for frontend compatibility
        const normalizedDialogue = {
            npcId: dialogue.NpcId || dialogue.npcId,
            question: dialogue.Question || dialogue.question,
            options: dialogue.Options || dialogue.options || []
        };
        
        const npc = gameState.npCs.find(n => n.id === npcId);
        const npcName = npc ? npc.name : 'Unknown';
        
        showDialogueBox(npcName, normalizedDialogue);
        addEvent(`💬 Started conversation with ${npcName}`);
    } catch (error) {
        console.error('Error getting dialogue:', error);
        addEvent('❌ Failed to start conversation');
    }
}
```

### 2. Enhanced `showDialogueBox()` Function
**File**: `src/VillageOfAshes.API/wwwroot/index.html`

Updated to handle multiple property name formats:

```javascript
function showDialogueBox(npcName, dialogue) {
    const options = dialogue.options && dialogue.options.length > 0 ? dialogue.options : [
        { id: '1', text: 'Tell me about yourself', effects: { trust: 5 } },
        { id: '2', text: 'Have you noticed anything suspicious?', effects: { suspicion: 5 } },
        { id: '3', text: 'Goodbye', effects: {} }
    ];

    const optionsHtml = options.map(option => {
        // Handle both DialogueOption (with Text property) and simple objects (with text property)
        const optionText = option.Text || option.text || option.PlayerLine || 'Continue...';
        const optionId = option.Id || option.id || `opt-${Math.random().toString(36).substr(2, 9)}`;
        const safeText = optionText.replace(/'/g, "\\'");
        return `
            <button class="dialogue-option" onclick="selectDialogueOption('${dialogue.npcId}', '${optionId}', '${safeText}')">
                ${escapeHtml(optionText)}
            </button>
        `;
    }).join('');
    
    // ... rest of the function
}
```

### 3. Added `loadCouncilDialogueOptions()` Function
**File**: `src/VillageOfAshes.API/wwwroot/index.html`

Implemented the missing function to load dialogue options for council statements:

```javascript
async function loadCouncilDialogueOptions() {
    const container = document.getElementById('statementOptions');
    if (!container) return;

    container.innerHTML = '<p style="color: #888; text-align: center;">Loading dialogue options...</p>';

    try {
        const response = await fetch(`${API_BASE}/game/council/dialogue-options`);
        if (!response.ok) {
            throw new Error('Failed to load dialogue options');
        }

        const data = await response.json();
        const options = data.options || [];

        if (options.length === 0) {
            container.innerHTML = '<p style="color: #888; text-align: center;">No dialogue options available.</p>';
            return;
        }

        const optionsHtml = options.map(statement => {
            const safeStatement = statement.replace(/'/g, "\\'");
            return `
                <button class="dialogue-option" onclick="makeCouncilStatement('${safeStatement}')" style="margin-bottom: 8px; width: 100%;">
                    ${escapeHtml(statement)}
                </button>
            `;
        }).join('');

        container.innerHTML = optionsHtml;
    } catch (error) {
        console.error('Error loading council dialogue options:', error);
        container.innerHTML = '<p style="color: #ff4444; text-align: center;">Failed to load options.</p>';
    }
}
```

### 4. Added `makeCouncilStatement()` Function
**File**: `src/VillageOfAshes.API/wwwroot/index.html`

Implemented the function to submit player statements at council:

```javascript
async function makeCouncilStatement(statement) {
    if (!requireCouncilPhase()) return;

    try {
        const response = await fetch(`${API_BASE}/game/council/player-statement`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ statement })
        });

        if (!response.ok) {
            const errorText = await response.text();
            addEvent(`❌ ${errorText}`);
            return;
        }

        const result = await response.json();
        
        // Add to council log
        addCouncilMessage({
            speakerId: gameState.player?.id || 'player',
            speaker: gameState.player?.name || 'Player',
            text: statement
        });

        addEvent(`💬 You made a statement at the council`);
        closeCouncilStrategicActions();
        
        // Refresh game state
        if (result.gameState) {
            gameState = result.gameState;
            updateUI();
        }
    } catch (error) {
        console.error('Error making council statement:', error);
        addEvent('❌ Failed to make statement');
    }
}
```

### 5. Auto-load Council Dialogue Options
**File**: `src/VillageOfAshes.API/wwwroot/index.html`

Modified `toggleCouncilStrategicAction()` to automatically load options when opening the statement panel:

```javascript
function toggleCouncilStrategicAction(actionKey) {
    const panel = document.getElementById(`councilAction-${actionKey}`);
    const button = document.querySelector(`.council-action-icon-btn[data-action="${actionKey}"]`);
    if (!panel || !button) return;

    if (activeCouncilStrategicAction === actionKey) {
        closeCouncilStrategicActions();
        return;
    }

    closeCouncilStrategicActions();
    activeCouncilStrategicAction = actionKey;
    panel.classList.add('active');
    button.classList.add('active');

    // Auto-load dialogue options when opening statement panel
    if (actionKey === 'statement') {
        loadCouncilDialogueOptions();
    }
}
```

## API Endpoints Used

### 1. Get Dialogue Options (Casual Talk)
- **Endpoint**: `GET /api/dialogue/npc/{npcId}`
- **Response Structure**:
```json
{
  "id": "uuid",
  "npcId": "npc_001",
  "question": "How are you holding up?",
  "options": [
    {
      "id": "neutral_1",
      "text": "How are you holding up?",
      "npcResponse": "Surviving. That's all any of us can do right now.",
      "effects": {
        "trust": 5,
        "suspicion": 0,
        "fear": 0,
        "spreadRumor": false
      },
      "conditions": []
    }
  ],
  "timestamp": "2026-06-03T02:00:00Z"
}
```

### 2. Get Council Dialogue Options
- **Endpoint**: `GET /api/game/council/dialogue-options`
- **Response Structure**:
```json
{
  "options": [
    "My silence is not an admission, it's caution.",
    "I am listening. Go on.",
    "I'm waiting for more evidence before I speak.",
    ...
  ]
}
```

### 3. Make Council Statement
- **Endpoint**: `POST /api/game/council/player-statement`
- **Request Body**:
```json
{
  "statement": "I think we need to work together."
}
```
- **Response Structure**:
```json
{
  "success": true,
  "statement": "I think we need to work together.",
  "gameState": { ... }
}
```

## Testing Results

### Casual Talk Dialogue
✅ **WORKING**: 
- API returns dialogue with 12+ options
- Frontend properly parses both PascalCase and camelCase properties
- Dialogue options display correctly
- NPC responses are handled properly

### Council Dialogue
✅ **PARTIALLY WORKING**:
- API returns 12 dialogue options successfully
- Frontend loads and displays options correctly
- ⚠️ **BACKEND ISSUE**: `ActiveCouncil` object is not initialized when game starts at 6:00 AM (council phase)
  - Error: "Council statements can only be made during Village Council"
  - Validation: `_currentGame.CurrentPhase == GamePhase.VillageCouncil` ✅ passes
  - Validation: `_currentGame.ActiveCouncil == null` ❌ fails

## Known Issues

### Backend Issue: ActiveCouncil Not Initialized
**Location**: `src/VillageOfAshes.API/Controllers/GameController.cs` - `InitializeGame()` method

**Problem**: When a game is created, it starts at 6:00 AM which is the VillageCouncil phase, but the `ActiveCouncil` object is not created until the time advances into the council phase.

**Validation Check**:
```csharp
if (_currentGame.CurrentPhase != GamePhase.VillageCouncil)
    return BadRequest("Council statements can only be made during Village Council");
if (_currentGame.ActiveCouncil == null) return BadRequest("No active council");
```

**Recommendation**: Modify `InitializeGame()` to create an `ActiveCouncil` object when the game starts in the council phase.

## Files Modified

1. `src/VillageOfAshes.API/wwwroot/index.html`:
   - Modified `talkToNPC()` function (~line 2236)
   - Modified `showDialogueBox()` function (~line 2655)
   - Modified `toggleCouncilStrategicAction()` function (~line 3169)
   - Added `loadCouncilDialogueOptions()` function (~line 3190)
   - Added `makeCouncilStatement()` function (~line 3230)

## Test Files Created

- `test-dialogue.sh`: Bash script to test dialogue system endpoints

## How to Test

1. Start the API server:
```bash
dotnet run --project src/VillageOfAshes.API/VillageOfAshes.API.csproj --urls "http://localhost:5010"
```

2. Open browser and navigate to `http://localhost:5010`

3. Test casual talk dialogue:
   - Select an NPC from the dropdown in the "Private Talk" section
   - Click "Talk" button
   - Dialogue options should appear in the response panel
   - Select an option to see NPC's response

4. Test council dialogue (workaround for backend issue):
   - Advance time past 6:00 AM and back into council phase (7:00-8:00 AM)
   - Click the 💬 "Make Statement" button in the Strategic Actions section
   - Dialogue options should load automatically
   - Click an option to make a statement at council

## Status

### ✅ COMPLETED
- Fixed property name mismatch in `talkToNPC()`
- Enhanced `showDialogueBox()` to handle multiple property formats
- Implemented `loadCouncilDialogueOptions()` function
- Implemented `makeCouncilStatement()` function
- Auto-load dialogue options when opening council statement panel
- Tested casual talk dialogue - **WORKING**
- Tested council dialogue options API - **WORKING**

### ⚠️ BLOCKED BY BACKEND ISSUE
- Council statements cannot be submitted because `ActiveCouncil` is not initialized at game start
- Requires backend fix in `GameController.InitializeGame()` method

## Next Steps

1. Fix backend `ActiveCouncil` initialization issue in `GameController.InitializeGame()`
2. Test complete council dialogue flow end-to-end
3. Update E2E tests to verify dialogue system
4. Consider adding visual feedback when dialogue options are loading

---

**Date**: June 3, 2026  
**Author**: Kiro AI Assistant  
**Status**: Dialogue frontend fixes completed, backend issue identified
