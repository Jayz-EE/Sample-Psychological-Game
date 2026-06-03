#!/bin/bash

# Test script for dialogue system
# This tests both casual talk and council dialogue

API_BASE="http://localhost:5000/api"

echo "=== Starting Dialogue System Test ==="
echo ""

# Step 1: Initialize a new game
echo "1. Creating new game..."
GAME_RESPONSE=$(curl -s -X POST "$API_BASE/game/initialize")
echo "Game created: $(echo $GAME_RESPONSE | jq -r '.currentDay // "Error"')"
echo ""

# Step 2: Get game state to find an NPC
echo "2. Getting game state..."
STATE=$(curl -s "$API_BASE/game/state")
NPC_ID=$(echo $STATE | jq -r '.npCs[0].id')
NPC_NAME=$(echo $STATE | jq -r '.npCs[0].name')
echo "Selected NPC: $NPC_NAME ($NPC_ID)"
echo ""

# Step 3: Test casual talk dialogue
echo "3. Testing casual talk dialogue..."
DIALOGUE=$(curl -s "$API_BASE/dialogue/npc/$NPC_ID")
echo "Dialogue response structure:"
echo $DIALOGUE | jq '{npcId: .NpcId, question: .Question, optionsCount: (.Options | length)}'
echo ""
echo "First dialogue option:"
echo $DIALOGUE | jq '.Options[0] | {Id, Text: .Text // .PlayerLine}'
echo ""

# Step 4: Test council dialogue options
echo "4. Testing council dialogue options..."
COUNCIL_OPTIONS=$(curl -s "$API_BASE/game/council/dialogue-options")
echo "Council options response:"
echo $COUNCIL_OPTIONS | jq '{optionsCount: (.options | length)}'
echo ""
echo "First 3 council options:"
echo $COUNCIL_OPTIONS | jq '.options[0:3]'
echo ""

# Step 5: Advance to council phase
echo "5. Advancing to council phase..."
curl -s -X POST "$API_BASE/game/time-advance?hours=1" > /dev/null
STATE=$(curl -s "$API_BASE/game/state")
PHASE=$(echo $STATE | jq -r '.currentPhase')
echo "Current phase: $PHASE"
echo ""

# Step 6: Test making council statement
if [ "$PHASE" == "1" ]; then
    echo "6. Making council statement..."
    STATEMENT="I think we need to work together."
    RESULT=$(curl -s -X POST "$API_BASE/game/council/player-statement" \
        -H "Content-Type: application/json" \
        -d "{\"statement\":\"$STATEMENT\"}")
    echo "Statement result: $(echo $RESULT | jq -r '.success // "Error"')"
    echo "Statement recorded: $(echo $RESULT | jq -r '.statement // "Error"')"
else
    echo "6. Skipped council statement (not in council phase)"
fi

echo ""
echo "=== Test Complete ==="
