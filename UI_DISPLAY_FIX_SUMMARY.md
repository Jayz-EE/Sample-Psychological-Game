# UI Display Fix Summary

## Overview
Fixed the UI display logic to show actions only when appropriate conditions are met, based on phases, game state, and role-specific requirements.

## Changes Made

### 1. Hunter Tracking Actions (IdentifyTraces)
**Problem:** Hunter could see "Track" action even when no traces were available.

**Solution:**
- Modified `renderRoleActionControls()` to filter out `IdentifyTraces` action when no traces exist
- Added visual indicator showing trace availability status for Hunters
- Only shows traces of types: Footprints, ClawMarks, DragMarks, DisturbedDirt
- Added descriptive text: "Identify who left this trace"

**Code Location:** `index.html` - `renderRoleActionControls()` and `updateRoleSpecificActions()`

### 2. Shopkeeper Selling Actions
**Problem:** Selling actions (crops, meat, items, info) were showing in the main action panel instead of only in the shopkeeper modal.

**Solution:**
- Removed selling actions from `RoleDayActions` constant:
  - Farmer: Removed `SellProduce` (only in shop)
  - Butcher: Removed `SellMeat` (only in shop)
  - Alchemist: Removed `SellRemedies` (only in shop)
  - Thief: Removed `TradeStolenGoods` (only in shop)
  - Voyeur: Removed `SellInformation` (only in shop)
  - Shopkeeper: Removed `TradeResources`, `SellClues`, `SpreadRumors` (only in shop modal)

- Modified `renderRoleActionControls()` to filter out all selling actions from role action panels
- These actions now only appear in the shopkeeper modal when visiting the shop

**Code Location:** `index.html` - `RoleDayActions` constant and `renderRoleActionControls()`

### 3. Action Consumption at Shopkeeper
**Problem:** Action consumption timing was unclear when visiting the shopkeeper.

**Solution:**
- Modified `closeShopkeeper()` to consume action point when leaving the shop
- Added visual warning in shop modal: "⚠️ Action point will be consumed when you leave the shop"
- All shop transactions (buying/selling) are now free during the visit
- Action is consumed only once when closing the shop modal

**Code Location:** `index.html` - `closeShopkeeper()` and `loadShopContent()`

### 4. Shopkeeper Availability Check
**Problem:** Visit Shopkeeper button was always visible even if shopkeeper was dead.

**Solution:**
- Added `updateShopkeeperVisitButton()` function to check shopkeeper status
- Shows "Visit Shopkeeper" button only if shopkeeper is alive (status === 0)
- Shows "💀 The Shopkeeper is no longer available" notice when shopkeeper is dead
- Added validation in `visitShopkeeper()` to prevent visiting if shopkeeper is unavailable

**Code Location:** `index.html` - `updateShopkeeperVisitButton()` and `visitShopkeeper()`

### 5. Phase-Based Action Display
**Problem:** Actions were showing regardless of current game phase.

**Solution:**
- Added phase validation in `renderRoleActionControls()`:
  - Day actions only show during Day Phase (phase === 3)
  - Night actions only show during Night Phase (phase === 0)
  - PublicAccusation only shows during Council Phase (phase === 2)
- Updated `updatePhaseActions()` to properly hide/show action sections based on current phase

**Code Location:** `index.html` - `renderRoleActionControls()` and `updatePhaseActions()`

### 6. Shopkeeper Role Actions in Modal
**Problem:** Shopkeeper role actions weren't clearly separated from trading actions.

**Solution:**
- Added dedicated "Shopkeeper Role Actions" section in shop modal
- Only visible when player is the Shopkeeper (role === 5)
- Clearly labeled as role-specific actions
- Includes: TradeResources, SellClues, SpreadRumors

**Code Location:** `index.html` - `loadShopContent()`

### 7. Enhanced Action Targeting
**Problem:** Some night actions weren't properly requiring targets.

**Solution:**
- Updated `actionNeedsTarget()` regex to include more action types:
  - Added: Protect, Emergency, Secret
- Ensures all actions that need a target have proper selection UI

**Code Location:** `index.html` - `actionNeedsTarget()`

### 8. Visual Feedback Improvements
**Added:**
- Hunter trace availability indicator (green checkmark when traces found, red X when none)
- Shopkeeper dead notice with skull emoji
- Action consumption warning in shop modal
- Descriptive text for IdentifyTraces action
- Better empty state messages for roles with no available actions

## Testing Checklist

### Hunter Role
- [ ] IdentifyTraces only shows when traces (Footprints, ClawMarks, DragMarks, DisturbedDirt) exist
- [ ] Visual indicator shows trace count when available
- [ ] Visual indicator shows "no traces" message when unavailable
- [ ] HuntAnimals action always available during day phase

### Shopkeeper Interactions
- [ ] Visit Shopkeeper button only shows when shopkeeper is alive
- [ ] Dead shopkeeper shows appropriate notice
- [ ] Selling crops/meat only available in shop modal
- [ ] Buying supplies works in shop modal
- [ ] Action point consumed only when leaving shop
- [ ] Warning message displays in shop modal

### Shopkeeper Role
- [ ] Shopkeeper role actions (TradeResources, SellClues, SpreadRumors) only in shop modal
- [ ] No role actions show in main Actions tab for Shopkeeper
- [ ] Shopkeeper can perform role actions in their own shop

### Phase-Based Actions
- [ ] Day actions only show during Day Phase (8 AM - 6 PM)
- [ ] Night actions only show during Night Phase (9 PM - 6 AM)
- [ ] Council actions show during Council Phase (7 AM - 8 AM)
- [ ] PublicAccusation only available during Council Phase
- [ ] Other phases show appropriate "limited actions" message

### Other Roles
- [ ] Farmer: SellProduce only in shop, other actions in main panel
- [ ] Butcher: SellMeat only in shop, CleanTools in main panel
- [ ] Alchemist: SellRemedies only in shop, BrewPotions in main panel
- [ ] Thief: TradeStolenGoods only in shop, ScoutHouses in main panel
- [ ] Voyeur: SellInformation only in shop, ListenToRumors in main panel

## Files Modified
- `/home/classify/Documents/Misc/Practice/Village/src/VillageOfAshes.API/wwwroot/index.html`

## Benefits
1. **Clearer UI**: Actions only show when they can actually be performed
2. **Better UX**: Players understand when and where to perform actions
3. **Reduced Confusion**: No more trying to track NPCs without traces
4. **Logical Flow**: Selling happens at the shop, not randomly during the day
5. **Phase Awareness**: Players understand what actions are available in each phase
6. **Resource Management**: Clear indication of when action points are consumed

## Future Enhancements
- Add tooltips explaining why certain actions are unavailable
- Add sound effects when traces are found (for Hunter)
- Add animation when shopkeeper becomes unavailable
- Add phase transition notifications
- Add action history log showing what was done in the shop
