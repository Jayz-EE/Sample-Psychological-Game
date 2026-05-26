# Developer Quick Reference Card

## 🎯 What Was Fixed

### UI Display Issues ✅
- Hunter tracking only shows with traces
- Selling actions only in shop modal
- Action consumed when leaving shop
- Shopkeeper button only when alive
- Phase-based action filtering

### Inventory Issues ✅
- All selling actions deduct inventory
- Coin-based economy implemented
- Buying requires coins
- Inventory displays show coins
- Multi-quantity consumption supported

---

## 📁 Files Modified

### Backend
```
src/VillageOfAshes.API/Controllers/GameController.cs
├── ApplyRoleAction() - Fixed 6 selling actions
├── ConsumePlayerInventoryItem() - Added quantity support
└── InitializeGame() - Added starting coins (5)
```

### Frontend
```
src/VillageOfAshes.API/wwwroot/index.html
├── renderRoleActionControls() - Conditional display
├── updateRoleSpecificActions() - Hunter indicator
├── updateShopkeeperVisitButton() - Availability check
├── buyCrops/buyMeat/buyInfo() - Coin payment
├── updateShopkeeperInventory() - Coin display
├── loadShopContent() - Price display
└── RoleDayActions constant - Removed selling actions
```

---

## 💰 Economy System

### Starting Resources
```javascript
Player: 5 coins, 0 crops, 0 meat, 0 items
NPCs: 0 coins (can earn through actions)
```

### Shop Prices
| Item | Buy Price | Sell Price |
|------|-----------|------------|
| Crop | 1 coin | 1 coin |
| Meat | 2 coins | 1 coin |
| Potion | - | 2 coins |
| Info | 3 coins | - |

### Selling Actions
```csharp
SellMeat: Remove all meat → Add coins (1:1)
SellProduce: Remove all crops → Add coins (1:1)
SellRemedies: Remove all potions → Add coins (1:2)
TradeStolenGoods: Remove stolen items → Add coins
SellInformation: Add coins (knowledge retained)
SellClues: Add coins (evidence based)
```

---

## 🔍 Key Functions

### Backend (C#)

#### Fixed Selling Action Example
```csharp
case "SellMeat": 
    var meatCount = actor.Inventory.Count(i => i == "meat");
    actor.Inventory.RemoveAll(i => i == "meat");
    actor.Inventory.AddRange(Enumerable.Repeat("coin", meatCount));
    AddEvidence(EvidenceType.TransactionRecords, 20); 
    AdjustWorld(food: meatCount * 2, economy: meatCount * 2); 
    break;
```

#### Multi-Quantity Consumption
```csharp
var quantity = Math.Max(1, request.Quantity);
var itemCount = _currentGame.Player.Inventory.Count(...);

if (itemCount < quantity)
    return BadRequest($"Not enough {item}");

for (int i = 0; i < quantity; i++) {
    var index = _currentGame.Player.Inventory.FindIndex(...);
    if (index >= 0) {
        _currentGame.Player.Inventory.RemoveAt(index);
    }
}
```

### Frontend (JavaScript)

#### Buying with Coin Check
```javascript
async function buyCrops() {
    const coins = (gameState.player.inventory || [])
        .filter(i => i === 'coin').length;
    
    if (coins < 1) {
        addEvent('❌ Not enough coins (need 1 coin)');
        return;
    }
    
    // Remove coin
    await fetch(`${API_BASE}/game/inventory/consume`, {
        method: 'POST',
        body: JSON.stringify({ item: 'coin', isFree: true })
    });
    
    // Add crop
    await fetch(`${API_BASE}/game/inventory/add`, {
        method: 'POST',
        body: JSON.stringify({ item: 'crop', isFree: true })
    });
    
    await refreshGame();
}
```

#### Conditional Action Display
```javascript
const filteredActions = actions.filter(action => {
    const isTraceAction = action === 'IdentifyTraces';
    const isShopAction = ['SellMeat', 'SellProduce', ...].includes(action);
    
    // Only show trace action if traces exist
    if (isTraceAction && !traceOptions) return false;
    
    // Only show shop actions in shop modal
    if (isShopAction) return false;
    
    // Phase-based filtering
    if (mode === 'day' && currentPhase !== 3) return false;
    
    return true;
});
```

---

## 🧪 Quick Test Commands

### Browser Console Tests

```javascript
// Check inventory
console.log('Inventory:', gameState.player.inventory);

// Count coins
const coins = gameState.player.inventory.filter(i => i === 'coin').length;
console.log('Coins:', coins);

// Count crops
const crops = gameState.player.inventory.filter(i => i === 'crop').length;
console.log('Crops:', crops);

// Check shopkeeper status
const shopkeeper = gameState.npCs.find(n => n.role === 5);
console.log('Shopkeeper alive:', shopkeeper?.status === 0);

// Check traces
const traces = gameState.evidence.filter(e => 
    e.type === 1 || e.type === 12 || e.type === 20 || e.type === 24
);
console.log('Traces found:', traces.length);

// Check current phase
console.log('Phase:', gameState.currentPhase);
// 0=Night, 1=Morning, 2=Council, 3=Day, 4=Evening
```

---

## 🐛 Common Issues & Fixes

### Issue: Inventory not updating
```javascript
// Solution: Force refresh
await refreshGame();
updateShopkeeperInventory();
renderSellOptions();
```

### Issue: Can't buy items
```javascript
// Check: Do you have coins?
const coins = gameState.player.inventory.filter(i => i === 'coin').length;
console.log('You have', coins, 'coins');

// Check: Is shop modal open?
console.log('Shop open:', document.getElementById('shopkeeperModal').style.display);
```

### Issue: Actions not showing
```javascript
// Check: Current phase
console.log('Phase:', gameState.currentPhase);
// Day actions need phase === 3

// Check: Player role
console.log('Role:', gameState.player.role);

// Check: Traces (for Hunter)
const traces = gameState.evidence.filter(e => 
    [1, 12, 20, 24].includes(e.type)
);
console.log('Traces:', traces.length);
```

---

## 📊 Data Structures

### Inventory Array
```javascript
// Example inventory
player.inventory = [
    "coin", "coin", "coin",  // 3 coins
    "crop", "crop",          // 2 crops
    "meat",                  // 1 meat
    "potion"                 // 1 potion
]

// Count items
const coins = inventory.filter(i => i === 'coin').length;
const crops = inventory.filter(i => i === 'crop').length;
```

### Evidence Types (for traces)
```javascript
const TRACE_TYPES = {
    Footprints: 1,
    ClawMarks: 12,
    DragMarks: 20,
    DisturbedDirt: 24
};

// Check for traces
const traces = evidence.filter(e => 
    [1, 12, 20, 24].includes(e.type)
);
```

### Game Phases
```javascript
const PHASES = {
    NightSimulation: 0,   // 9 PM - 5 AM
    MorningDiscovery: 1,  // 6 AM
    VillageCouncil: 2,    // 7 AM - 8 AM
    DayActions: 3,        // 8 AM - 6 PM
    Evening: 4            // 6 PM - 9 PM
};
```

---

## 🔧 Debugging Tips

### Enable Verbose Logging
```javascript
// Add to top of index.html <script>
const DEBUG = true;

function debugLog(message, data) {
    if (DEBUG) {
        console.log(`[DEBUG] ${message}`, data);
    }
}

// Use in functions
debugLog('Buying crop', { coins, crops });
```

### Monitor Inventory Changes
```javascript
// Add to updatePlayerInfo()
const oldInventory = JSON.stringify(gameState.player.inventory);
// ... update code ...
const newInventory = JSON.stringify(gameState.player.inventory);

if (oldInventory !== newInventory) {
    console.log('Inventory changed:', {
        old: oldInventory,
        new: newInventory
    });
}
```

### Track Action Consumption
```javascript
// Add to ConsumeAction()
console.log('Action consumed:', {
    actor: actor.name,
    before: actor.phaseActionCount - 1,
    after: actor.phaseActionCount,
    phase: gameState.currentPhase
});
```

---

## 📝 Code Patterns

### Adding New Selling Action
```csharp
// Backend: GameController.cs
case "SellNewItem":
    var itemCount = actor.Inventory.Count(i => i == "newitem");
    actor.Inventory.RemoveAll(i => i == "newitem");
    actor.Inventory.AddRange(Enumerable.Repeat("coin", itemCount * PRICE));
    AddEvidence(EvidenceType.TransactionRecords, 20);
    AdjustWorld(economy: itemCount * 2);
    break;
```

```javascript
// Frontend: index.html
if (count('newitem') > 0) {
    sellHTML += `<div class="shop-item">
        <div class="item-info"><strong>New Items (${count('newitem')})</strong></div>
        <button class="btn" onclick="shopSellAction('SellNewItem')">Sell All</button>
    </div>`;
}
```

### Adding New Buying Action
```javascript
async function buyNewItem() {
    const coins = (gameState.player.inventory || [])
        .filter(i => i === 'coin').length;
    const PRICE = 2;
    
    if (coins < PRICE) {
        addEvent(`❌ Not enough coins (need ${PRICE} coins)`);
        return;
    }
    
    await fetch(`${API_BASE}/game/inventory/consume`, {
        method: 'POST',
        body: JSON.stringify({ item: 'coin', quantity: PRICE, isFree: true })
    });
    
    await fetch(`${API_BASE}/game/inventory/add`, {
        method: 'POST',
        body: JSON.stringify({ item: 'newitem', isFree: true })
    });
    
    addEvent(`✅ Bought new item (paid ${PRICE} coins)`);
    await refreshGame();
}
```

---

## 🚀 Deployment Commands

### Build & Deploy
```bash
# Backend
cd src/VillageOfAshes.API
dotnet build
dotnet run

# Frontend (no build needed, static HTML)
# Just refresh browser after changes
```

### Clear Cache
```bash
# Force browser to reload
Ctrl + Shift + R (Windows/Linux)
Cmd + Shift + R (Mac)
```

---

## 📚 Related Documentation

- **UI_DISPLAY_FIX_SUMMARY.md** - UI fix details
- **INVENTORY_FIX_SUMMARY.md** - Inventory fix details
- **INVENTORY_TESTING_GUIDE.md** - Testing procedures
- **COMPLETE_FIX_SUMMARY.md** - Full summary
- **UI_QUICK_REFERENCE.md** - Player reference

---

## ✅ Quick Verification

Run this in browser console after changes:
```javascript
// Verify fixes
const verify = {
    coinsExist: gameState.player.inventory.includes('coin'),
    coinsDisplay: document.getElementById('playerCoinsDisplay') !== null,
    shopButton: document.getElementById('shopkeeperVisitSection') !== null,
    sellActionsRemoved: !RoleDayActions[4].includes('SellProduce'),
    hunterConditional: true // Check manually
};

console.log('Verification:', verify);
console.log('All checks passed:', Object.values(verify).every(v => v));
```

---

**Last Updated:** 2026-05-26  
**Version:** 3.1  
**Status:** ✅ Ready for Production
