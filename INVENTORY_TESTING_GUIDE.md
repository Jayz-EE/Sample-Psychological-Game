# Inventory System Testing Guide

## Pre-Testing Setup

### 1. Start a New Game
```
1. Open the game in browser
2. Click "New Game" button
3. Verify player starts with 5 coins
4. Check Actions tab → Your Resources section shows: 🪙 5
```

### 2. Check Initial State
```
Expected Inventory:
- Coins: 5
- Crops: 0
- Meat: 0
- Items: 0
```

---

## Test Scenarios

### Scenario 1: Farmer Selling Crops

**Setup:**
1. Start new game as Farmer (or wait until you get Farmer role)
2. Advance to Day Phase (8 AM - 6 PM)

**Test Steps:**
```
1. Perform action: Harvest Crops
   Expected: +5 crops in inventory
   
2. Check inventory:
   - Coins: 5
   - Crops: 5
   - Meat: 0
   
3. Visit Shopkeeper
   
4. In shop modal, check "Sell Goods" section
   Expected: "Crops (5) [Sell All]" button visible
   
5. Click "Sell All" for crops
   Expected: 
   - Crops removed from inventory (0 crops)
   - Coins added (5 + 5 = 10 coins)
   - Event log: "💰 Successfully sold goods: Sell Produce"
   
6. Verify inventory in shop modal:
   - Coins: 10
   - Crops: 0
   
7. Close shop (action consumed)
```

**Expected Result:** ✅ Crops properly deducted, coins properly added

---

### Scenario 2: Hunter Selling Meat

**Setup:**
1. Start new game as Hunter
2. Advance to Day Phase

**Test Steps:**
```
1. Perform action: Hunt Animals
   Expected: +2 meat in inventory
   
2. Check inventory:
   - Coins: 5
   - Meat: 2
   
3. Visit Shopkeeper
   
4. In shop modal, check "Sell Goods" section
   Expected: "Meat (2) [Sell All]" button visible
   
5. Click "Sell All" for meat
   Expected:
   - Meat removed (0 meat)
   - Coins added (5 + 2 = 7 coins)
   
6. Verify inventory:
   - Coins: 7
   - Meat: 0
```

**Expected Result:** ✅ Meat properly deducted, coins properly added

---

### Scenario 3: Alchemist Selling Potions

**Setup:**
1. Start new game as Alchemist
2. Advance to Day Phase

**Test Steps:**
```
1. Perform action: Brew Potions
   Expected: +1 potion in inventory
   
2. Perform action: Brew Potions again
   Expected: +1 potion (total 2 potions)
   
3. Check inventory:
   - Coins: 5
   - Items: 2 (potions)
   
4. Visit Shopkeeper
   
5. In shop modal, check "Sell Goods" section
   Expected: "Remedies [Sell]" button visible
   
6. Click "Sell" for remedies
   Expected:
   - Potions removed (0 potions)
   - Coins added (5 + 4 = 9 coins) [2 potions × 2 coins each]
   
7. Verify inventory:
   - Coins: 9
   - Items: 0
```

**Expected Result:** ✅ Potions properly deducted, coins properly added (2x value)

---

### Scenario 4: Buying Crops

**Setup:**
1. Start new game (any role)
2. Advance to Day Phase
3. Ensure you have at least 1 coin

**Test Steps:**
```
1. Visit Shopkeeper
   
2. Check "Buy Supplies" section
   Expected: "1 Crop - 1 🪙 [Buy]" button visible
   
3. Click "Buy" for crop
   Expected:
   - 1 coin removed (5 - 1 = 4 coins)
   - 1 crop added (0 + 1 = 1 crop)
   - Event log: "🌾 Bought 1 crop from shopkeeper (paid 1 coin)"
   
4. Verify inventory in shop modal:
   - Coins: 4
   - Crops: 1
   
5. Click "Buy" for crop 4 more times
   Expected: 
   - Coins: 0
   - Crops: 5
   
6. Try to buy another crop
   Expected: Error message "❌ Not enough coins to buy crops (need 1 coin)"
```

**Expected Result:** ✅ Coins properly deducted, crops properly added, can't buy without coins

---

### Scenario 5: Buying Meat

**Setup:**
1. Start new game (any role)
2. Advance to Day Phase
3. Ensure you have at least 2 coins

**Test Steps:**
```
1. Visit Shopkeeper
   
2. Check "Buy Supplies" section
   Expected: "1 Meat - 2 🪙 [Buy]" button visible
   
3. Click "Buy" for meat
   Expected:
   - 2 coins removed (5 - 2 = 3 coins)
   - 1 meat added (0 + 1 = 1 meat)
   - Event log: "🥩 Bought 1 meat from shopkeeper (paid 2 coins)"
   
4. Verify inventory:
   - Coins: 3
   - Meat: 1
   
5. Try to buy another meat
   Expected:
   - Coins: 1
   - Meat: 2
   
6. Try to buy another meat (only 1 coin left)
   Expected: Error message "❌ Not enough coins to buy meat (need 2 coins)"
```

**Expected Result:** ✅ Coins properly deducted, meat properly added, can't buy without enough coins

---

### Scenario 6: Buying Intelligence (Detective Only)

**Setup:**
1. Start new game as Detective
2. Advance to Day Phase
3. Ensure you have at least 3 coins

**Test Steps:**
```
1. Visit Shopkeeper
   
2. Check "Buy Supplies" section
   Expected: "Intelligence - 3 🪙 [Buy]" button visible (Detective only)
   
3. Click "Buy" for intelligence
   Expected:
   - 3 coins removed (5 - 3 = 2 coins)
   - Event log shows random evidence clue
   - Example: "📜 Bought information: Footprints found at Forest"
   
4. Verify inventory:
   - Coins: 2
   
5. Try to buy another intelligence
   Expected: Error message "❌ Not enough coins to buy information (need 3 coins)"
```

**Expected Result:** ✅ Coins properly deducted, intelligence provided

---

### Scenario 7: Thief Trading Stolen Goods

**Setup:**
1. Start new game as Thief
2. Advance to Night Phase
3. Perform night action to steal resources

**Test Steps:**
```
1. Night Phase: Perform "Steal Resources" action
   Expected: +coins or +scrap in inventory
   
2. Advance to Day Phase
   
3. Visit Shopkeeper
   
4. In shop modal, check "Sell Goods" section
   Expected: "Stolen Goods [Trade]" button visible (if you have coins/scrap)
   
5. Click "Trade" for stolen goods
   Expected:
   - Stolen items removed
   - Legitimate coins added
   
6. Verify inventory updated
```

**Expected Result:** ✅ Stolen goods properly laundered into coins

---

### Scenario 8: Voyeur Selling Information

**Setup:**
1. Start new game as Voyeur
2. Advance to Day Phase
3. Perform "Listen to Rumors" action to gather knowledge

**Test Steps:**
```
1. Perform "Listen to Rumors" action multiple times
   Expected: Knowledge added to KnownFacts
   
2. Visit Shopkeeper
   
3. In shop modal, check "Sell Goods" section
   Expected: "Information [Sell]" button visible
   
4. Click "Sell" for information
   Expected:
   - Coins added (based on knowledge count)
   - Knowledge NOT removed (can sell again)
   
5. Verify coins increased
```

**Expected Result:** ✅ Information sold for coins, knowledge retained

---

### Scenario 9: Shopkeeper Selling Clues

**Setup:**
1. Start new game as Shopkeeper
2. Advance to Day Phase
3. Ensure evidence exists in game

**Test Steps:**
```
1. Visit Shopkeeper (your own shop)
   
2. Check "Shopkeeper Role Actions" section
   Expected: "Sell Clues [Button]" visible
   
3. Click "Sell Clues"
   Expected:
   - Coins added (based on evidence count)
   - Event log confirms sale
   
4. Verify coins increased
```

**Expected Result:** ✅ Clues sold for coins

---

### Scenario 10: Complete Economic Cycle

**Setup:**
1. Start new game as Farmer
2. Full day cycle test

**Test Steps:**
```
Day 1 Morning (6 AM):
- Starting inventory: 5 coins, 0 crops, 0 meat

Day 1 Day Phase (8 AM):
1. Harvest Crops → +5 crops
2. Harvest Crops again → +5 crops (total 10 crops)
   Inventory: 5 coins, 10 crops, 0 meat

Day 1 Afternoon (2 PM):
3. Visit Shopkeeper
4. Sell all crops (10) → +10 coins
   Inventory: 15 coins, 0 crops, 0 meat
5. Buy 3 crops (3 coins) → -3 coins, +3 crops
   Inventory: 12 coins, 3 crops, 0 meat
6. Buy 2 meat (4 coins) → -4 coins, +2 meat
   Inventory: 8 coins, 3 crops, 2 meat
7. Close shop (1 action consumed)

Day 1 Evening (6 PM):
- Inventory: 8 coins, 3 crops, 2 meat

Day 1 Night (9 PM):
- Food consumed automatically (1 crop or 1 meat)
- Inventory: 8 coins, 2-3 crops, 1-2 meat

Day 2 Morning (6 AM):
- Verify inventory persisted correctly
```

**Expected Result:** ✅ Complete economic cycle works correctly

---

## Edge Cases to Test

### Edge Case 1: Selling with Empty Inventory
```
1. Visit shopkeeper with no items to sell
2. Check "Sell Goods" section
Expected: "Nothing to sell currently" message
```

### Edge Case 2: Buying with 0 Coins
```
1. Spend all coins
2. Try to buy anything
Expected: Error message for each item
```

### Edge Case 3: Multiple Transactions in One Visit
```
1. Visit shopkeeper
2. Buy crop (1 coin)
3. Buy meat (2 coins)
4. Sell crop (1 coin)
5. Buy crop again (1 coin)
Expected: All transactions work, only 1 action consumed on exit
```

### Edge Case 4: Closing Shop Without Transactions
```
1. Visit shopkeeper
2. Don't buy or sell anything
3. Close shop
Expected: 1 action still consumed
```

### Edge Case 5: Inventory Display Updates
```
1. Perform any transaction
2. Check both displays:
   - Shop modal resource display
   - Main actions tab resource display
Expected: Both show same values, update in real-time
```

---

## Regression Testing

### Test Previous Functionality Still Works

1. **Action Consumption**
   - [ ] Actions still consume action points correctly
   - [ ] Max 2 actions per phase enforced
   - [ ] Shop visit consumes 1 action on exit

2. **Phase Transitions**
   - [ ] Phase changes reset action counts
   - [ ] Actions available based on phase
   - [ ] Time advancement works

3. **Role Actions**
   - [ ] All role actions still work
   - [ ] Hunter tracking conditional on traces
   - [ ] Shopkeeper actions in shop modal

4. **UI Display**
   - [ ] All tabs work correctly
   - [ ] Evidence displays
   - [ ] Rumors display
   - [ ] NPC list displays

---

## Performance Testing

### Test with Large Inventories
```
1. Use console to add 100 crops to inventory
2. Visit shopkeeper
3. Sell all crops
Expected: No lag, all items removed, coins added correctly
```

### Test Rapid Transactions
```
1. Visit shopkeeper
2. Rapidly click buy/sell buttons
Expected: All transactions process correctly, no duplicates
```

---

## Browser Compatibility

Test in:
- [ ] Chrome/Edge
- [ ] Firefox
- [ ] Safari
- [ ] Mobile browsers

---

## Bug Report Template

If you find issues, report using this format:

```
**Bug Title:** [Brief description]

**Steps to Reproduce:**
1. [Step 1]
2. [Step 2]
3. [Step 3]

**Expected Behavior:**
[What should happen]

**Actual Behavior:**
[What actually happened]

**Inventory Before:**
- Coins: X
- Crops: X
- Meat: X
- Items: X

**Inventory After:**
- Coins: X
- Crops: X
- Meat: X
- Items: X

**Console Errors:**
[Any JavaScript errors from browser console]

**Screenshots:**
[If applicable]
```

---

## Success Criteria

All tests pass when:
- ✅ All selling actions properly deduct inventory
- ✅ All buying actions properly deduct coins
- ✅ Inventory displays update correctly
- ✅ Can't buy without sufficient coins
- ✅ Economic cycle works end-to-end
- ✅ No console errors
- ✅ No inventory duplication bugs
- ✅ No negative inventory values
- ✅ Previous functionality still works

---

## Quick Smoke Test (5 minutes)

For rapid verification:

```
1. Start new game
   ✓ Check: 5 coins displayed

2. Harvest crops (if Farmer) or Hunt (if Hunter)
   ✓ Check: Items added to inventory

3. Visit shopkeeper
   ✓ Check: Sell button appears for items you have

4. Sell items
   ✓ Check: Items removed, coins added

5. Buy 1 crop
   ✓ Check: 1 coin removed, 1 crop added

6. Close shop
   ✓ Check: 1 action consumed

7. Check inventory in Actions tab
   ✓ Check: Matches shop modal inventory
```

If all 7 checks pass, basic functionality is working! ✅
