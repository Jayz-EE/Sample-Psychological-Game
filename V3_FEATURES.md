# Village of Ashes v3.0 - Phase-Based Actions & Economy System

## 🎮 Major New Features

### 1. ✅ Phase-Based Actions
Actions now only appear during appropriate game phases:
- **Council Phase (7-8 AM):** Conversations only
- **Day Phase (8 AM-6 PM):** Role-specific actions, shopkeeper, investigations
- **Night Phase (9 PM-6 AM):** Night role actions (track, protect, kill)
- **Other Phases:** Limited/no actions

### 2. ✅ Shopkeeper Economy System
- Visit shopkeeper during day phase
- Buy/sell crops, meat, items
- Role-specific shop interactions
- Anonymous transactions (Butcher's identity protected)

### 3. ✅ Resource Management
- **Crops & Meat:** Consumed nightly
- **Health System:** Lose health without food
- **Inventory Tracking:** Visual resource display

### 4. ✅ Role-Specific Actions
Each role has unique day and night actions

---

## 📅 Phase-Based Action System

### Council Phase (7:00 AM - 8:00 AM)
**Available Actions:**
- 🗣️ **Talk to NPCs** - Engage in council discussions
- 💬 **Participate in Conversations** - Choose dialogue responses

**Purpose:** Social interaction, accusations, building alliances

---

### Day Phase (8:00 AM - 6:00 PM)
**Always Available:**
- 🏪 **Visit Shopkeeper** - Buy/sell resources and items

**Role-Specific Actions:**

#### Detective 🔍
- **Track Suspicious NPC** - Gather evidence about movements
- **Investigate Behavior** - Analyze NPC patterns
- **Buy Information** - Purchase intel from shopkeeper (sold by Vagabond)

#### Doctor 💉
- **Heal NPC** - Restore health to injured villagers
- **Investigate** - Identify threats
- **Buy Medical Supplies** - Purchase items if available

#### Butcher 🔪
- **Sell Meat** - Sell victim meat to shopkeeper (3 meat per victim)
- **Investigate** - Scout potential targets
- **Spread Rumors** - Frame innocents

#### Vagabond 📜
- **Sell Information** - Sell gathered intel to shopkeeper
- **Investigate** - Gather information
- **Spread Rumors** - Manipulate social dynamics

#### Farmer 🌾
- **Harvest Crops** - Gather 5 crops (once per day)
- **Investigate** - Protect your interests
- **Spread Rumors** - Defend yourself

**General Actions (All Roles):**
- 🔍 **Investigate NPC** - Analyze behavior
- 📢 **Spread Rumor** - Create gossip

---

### Night Phase (9:00 PM - 6:00 AM)
**Role-Specific Night Actions:**

#### Detective 🔍
- **Set Track Target** - Choose NPC to track tonight
- Tracks movements during night simulation
- Gathers evidence about activities

#### Doctor 💉
- **Set Protection Target** - Choose NPC to protect tonight
- Prevents death from Butcher attack
- Can save one person per night

#### Butcher 🔪
- **Set Kill Target** - Choose victim for tonight
- Executes during night simulation
- Generates 3 meat per successful kill
- Cannot target Shopkeeper (protected for 7 days)

#### Other Roles
- No specific night actions
- Rest and prepare for tomorrow
- Consume food (automatic)

---

## 🏪 Shopkeeper Economy System

### Visiting the Shopkeeper

**How to Access:**
1. Wait for Day Phase (8 AM - 6 PM)
2. Click "🏪 Visit Shopkeeper" button
3. Shop modal opens with available options

### What's Available

#### For Everyone:
**🌾 Crops**
- Essential for survival
- Consumed nightly (1 per night)
- Without crops/meat, health decreases
- Source: Farmer harvests, shopkeeper stock

**🥩 Meat**
- Nutritious food source
- Consumed nightly (1 per night)
- Alternative to crops
- Source: Butcher sells (anonymous), shopkeeper stock

#### For Detective:
**📜 Information**
- Sold by Vagabond to shopkeeper
- Costs 1 crop
- Reveals suspicious activities
- Helps identify the Butcher

#### For All (When Available):
**🎒 Items**
- Special items stolen by Thief (future feature)
- Sold to shopkeeper
- Various effects
- Availability varies

### Selling to Shopkeeper

#### Butcher - Sell Meat 🥩
- Sell meat from victims
- 3 meat per victim
- **Identity Protected:** Shopkeeper never reveals source
- Detectives cannot trace meat back to Butcher

#### Vagabond - Sell Information 📜
- Sell gathered intelligence
- Makes info available for Detectives
- Earn resources
- Strategic information trading

#### Thief - Sell Items 🎒 (Future Feature)
- Steal items from houses
- Sell to shopkeeper
- Items become available for purchase
- Random distribution system

---

## 🎒 Resource Management System

### Resource Types

#### 🌾 Crops
- **Source:** Farmer harvests 5/day, shopkeeper
- **Use:** Consumed nightly (1 per night)
- **Effect:** Prevents health loss
- **Storage:** Unlimited in inventory

#### 🥩 Meat
- **Source:** Butcher sells (from victims), shopkeeper
- **Use:** Consumed nightly (1 per night)
- **Effect:** Prevents health loss
- **Storage:** Unlimited in inventory

#### 🎒 Items
- **Source:** Thief steals and sells (future)
- **Use:** Role-specific effects
- **Effect:** Varies by item
- **Storage:** Unlimited in inventory

### Nightly Consumption

**Every Night (Automatic):**
1. Each NPC/Player consumes 1 food (crop or meat)
2. Priority: Crops first, then meat
3. If no food available: **Health -20**
4. If health reaches 0: **Death**

**Strategy:**
- Always maintain food supply
- Farmer role is crucial for village survival
- Butcher can sustain themselves through kills
- Trade/buy food if running low

### Resource Display

**Main UI:**
- Your Role panel shows current resources
- 🌾 Crops count
- 🥩 Meat count
- 🎒 Items count
- ⚠️ Warning about nightly consumption

**Shopkeeper Modal:**
- Real-time resource counts
- Updated after each transaction
- Shows what you can afford

---

## 🎭 Role-Specific Strategies

### Detective 🔍

**Day Actions:**
- Track suspicious NPCs
- Buy information from shopkeeper
- Investigate behavior patterns
- Spread accurate rumors to guide suspicion

**Night Actions:**
- Set track target before night
- Gather evidence during simulation
- Identify Butcher through patterns

**Resource Strategy:**
- Maintain crop supply
- Spend crops on information
- Information is key to winning

---

### Doctor 💉

**Day Actions:**
- Heal injured NPCs
- Build trust through healing
- Investigate threats
- Maintain food supply

**Night Actions:**
- Set protection target
- Save potential victims
- Protect key allies

**Resource Strategy:**
- Keep healthy food supply
- Focus on survival
- Healing builds alliances

---

### Butcher 🔪

**Day Actions:**
- Sell meat anonymously
- Spread false rumors
- Investigate targets
- Build trust facade

**Night Actions:**
- Set kill target
- Execute during simulation
- Gain 3 meat per kill

**Resource Strategy:**
- Self-sustaining through kills
- Sell excess meat for cover
- Shopkeeper protects identity

---

### Vagabond 📜

**Day Actions:**
- Sell information to shopkeeper
- Gather intelligence
- Stay low profile
- Minimal interactions

**Night Actions:**
- No specific actions
- Survive and observe

**Resource Strategy:**
- Trade information for food
- Maintain survival resources
- Escape after 5 nights

---

### Farmer 🌾

**Day Actions:**
- Harvest 5 crops daily
- Sell/trade excess crops
- Build community trust
- Defend against accusations

**Night Actions:**
- No specific actions
- Consume crops

**Resource Strategy:**
- Harvest daily
- Feed yourself first
- Trade excess for security
- Survive 7 days

---

## 🎯 Gameplay Flow

### Morning (6-7 AM) - Morning Discovery
- Check for deaths
- Review new evidence
- **Limited Actions:** Advance time to council

### Council (7-8 AM) - Village Council
- **Available:** Talk to NPCs
- Participate in discussions
- Choose dialogue responses
- Build/break alliances

### Day (8 AM-6 PM) - Day Actions
- **Available:** All day actions
- Visit shopkeeper
- Role-specific actions
- Investigations
- Spread rumors

### Evening (6-9 PM) - Limited Movement
- **Limited Actions:** Prepare for night
- Advance time to night

### Night (9 PM-6 AM) - Night Simulation
- **Available:** Night role actions
- Set targets (Detective, Doctor, Butcher)
- Simulation executes when advancing to morning
- Food consumed automatically
- Health decreases if no food

---

## 🔄 Economy Cycle

### Farmer Cycle
1. Harvest 5 crops daily
2. Keep some for self
3. Sell/trade excess
4. Repeat

### Butcher Cycle
1. Kill victim at night
2. Gain 3 meat
3. Consume 1 meat nightly
4. Sell excess anonymously
5. Repeat

### Vagabond Cycle
1. Gather information
2. Sell to shopkeeper
3. Earn resources
4. Survive 5 nights
5. Escape

### Detective Cycle
1. Maintain food supply
2. Buy information
3. Investigate suspects
4. Identify Butcher
5. Win game

---

## ⚠️ Important Rules

### Shopkeeper Protection
- Shopkeeper cannot be killed for first 7 days
- Protects village economy
- After 7 days, vulnerable to Butcher

### Anonymous Transactions
- Butcher's meat sales are anonymous
- Shopkeeper never reveals source
- Detectives cannot trace meat
- Maintains Butcher's cover

### Food Consumption
- Automatic every night
- Cannot be prevented
- Crops consumed before meat
- No food = -20 health

### Health System
- Starts at 100
- Decreases by 20 per night without food
- Can be healed by Doctor
- Death at 0 health

---

## 🎮 How to Play v3.0

### 1. Start Game
```
Click "New Game"
Check your role
Note your starting resources (usually 0)
```

### 2. Survive First Night
```
If Farmer: Harvest crops immediately
If not: Visit shopkeeper, buy 1 crop
Advance to night
Food consumed automatically
```

### 3. Use Phase-Based Actions
```
Council Phase: Talk to NPCs
Day Phase: Role actions + shopkeeper
Night Phase: Set night targets
```

### 4. Manage Resources
```
Check resource display regularly
Maintain food supply
Trade strategically
Don't starve!
```

### 5. Complete Objective
```
Good: Identify and eliminate Butcher
Evil: Survive and eliminate good roles
Neutral: Complete specific objective
```

---

## 🐛 Known Limitations

### Current Implementation
- Food consumption is client-side (not persisted)
- Shopkeeper inventory is unlimited
- Thief role not yet implemented
- Item system placeholder only
- Night actions don't affect backend simulation yet

### Future Enhancements (v4.0)
- Backend resource persistence
- Limited shopkeeper inventory
- Thief role implementation
- Item effects system
- Night action integration with simulation
- Trading between players
- Resource scarcity mechanics

---

## 🚀 Getting Started

```bash
# Start server
./start-game.sh

# Open browser
http://localhost:5000

# Verify version
Look for "v3.0" in title

# Hard refresh if needed
Ctrl+F5 (Windows/Linux)
Cmd+Shift+R (Mac)
```

---

**Version:** 3.0  
**Status:** ✅ Phase-Based Actions & Economy  
**Last Updated:** 2026-05-26

**Enjoy the enhanced survival mechanics! 🏚️💀🎮**
