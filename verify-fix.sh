#!/bin/bash

echo "🔍 Verifying Role Display Fix..."
echo ""

# Check if server is running
if ! lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1 ; then
    echo "❌ Server is not running on port 5000"
    echo "   Run: ./start-game.sh"
    exit 1
fi

echo "✅ Server is running on port 5000"
echo ""

# Check if HTML has role mappings
if grep -q "const RoleNames" src/VillageOfAshes.API/wwwroot/index.html; then
    echo "✅ HTML file contains RoleNames mapping"
else
    echo "❌ HTML file missing RoleNames mapping"
    exit 1
fi

# Check if server is serving the updated file
if curl -s http://localhost:5000/ | grep -q "const RoleNames"; then
    echo "✅ Server is serving updated HTML with role mappings"
else
    echo "❌ Server is NOT serving updated HTML"
    echo "   Try restarting the server"
    exit 1
fi

echo ""
echo "📊 Testing API response..."

# Create a new game and check response
RESPONSE=$(curl -s -X POST http://localhost:5000/api/game/new)

if echo "$RESPONSE" | grep -q '"role":'; then
    echo "✅ API returns role data"
    
    # Extract a sample role number
    ROLE_NUM=$(echo "$RESPONSE" | grep -o '"role":[0-9]' | head -1 | grep -o '[0-9]')
    echo "   Sample role number from API: $ROLE_NUM"
    
    case $ROLE_NUM in
        0) echo "   → Should display as: Detective" ;;
        1) echo "   → Should display as: Doctor" ;;
        2) echo "   → Should display as: Butcher" ;;
        3) echo "   → Should display as: Vagabond" ;;
        4) echo "   → Should display as: Farmer" ;;
        5) echo "   → Should display as: Shopkeeper" ;;
    esac
else
    echo "❌ API response doesn't contain role data"
    exit 1
fi

echo ""
echo "✅ All checks passed!"
echo ""
echo "📝 If you still see numbers instead of role names:"
echo "   1. Hard refresh your browser (Ctrl+F5 or Cmd+Shift+R)"
echo "   2. Clear browser cache"
echo "   3. Use Incognito/Private mode"
echo "   4. Open Developer Tools (F12) and check 'Disable cache'"
echo ""
echo "   See BROWSER_CACHE_FIX.md for detailed instructions"
