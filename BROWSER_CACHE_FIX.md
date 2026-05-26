# Browser Cache Fix - Role Display Issue

## Problem
After updating the HTML file, the browser still shows role numbers (e.g., "Role (4)") instead of role names (e.g., "Farmer").

## Root Cause
**Browser caching** - Your browser cached the old version of the HTML file and isn't loading the updated version.

## Solutions (Try in Order)

### Solution 1: Hard Refresh (Fastest)
**Windows/Linux:**
- Press `Ctrl + F5` or `Ctrl + Shift + R`

**Mac:**
- Press `Cmd + Shift + R`

**Alternative:**
- Press `F12` to open Developer Tools
- Right-click the refresh button
- Select "Empty Cache and Hard Reload"

---

### Solution 2: Clear Browser Cache
1. Open browser settings
2. Find "Clear browsing data" or "Clear cache"
3. Select "Cached images and files"
4. Clear cache
5. Reload the page

---

### Solution 3: Use Incognito/Private Mode
1. Open a new Incognito/Private window
2. Navigate to `http://localhost:5000`
3. This bypasses the cache entirely

---

### Solution 4: Disable Cache in Developer Tools
1. Press `F12` to open Developer Tools
2. Go to the "Network" tab
3. Check "Disable cache" checkbox
4. Keep Developer Tools open
5. Refresh the page

---

### Solution 5: Check Server is Serving Updated File
```bash
# Verify the server has the role mappings
curl -s http://localhost:5000/ | grep "RoleNames"

# Should output:
# const RoleNames = {
#     0: 'Detective',
#     1: 'Doctor',
#     ...
```

---

## How to Verify the Fix Worked

After clearing cache, you should see:

### ✅ CORRECT Display:
- "Detective (Good)" 
- "Butcher (Evil)"
- "Farmer (Neutral)"
- "Vagabond (Neutral)"
- Phase: "Morning Discovery"

### ❌ WRONG Display (cached version):
- "0 (0)"
- "2 (1)" 
- "4 (2)"
- Phase: "1"

---

## Version Check
Look at the page title or header. The updated version shows:
- **Title:** "Village of Ashes - v1.1"
- **Header:** "🏚️ VILLAGE OF ASHES 🏚️ v1.1"

If you don't see "v1.1", you're viewing the cached version.

---

## Prevention

To avoid cache issues in the future:

1. **Always use Developer Tools during development**
   - Keep F12 open with "Disable cache" checked

2. **Use Incognito mode for testing**
   - Ensures fresh page load every time

3. **Hard refresh after server restarts**
   - Use Ctrl+F5 instead of regular F5

---

## Technical Details

The updated HTML file includes:

```javascript
// Role enum mapping
const RoleNames = {
    0: 'Detective',
    1: 'Doctor',
    2: 'Butcher',
    3: 'Vagabond',
    4: 'Farmer',
    5: 'Shopkeeper'
};

// Alignment enum mapping
const AlignmentNames = {
    0: 'Good',
    1: 'Evil',
    2: 'Neutral'
};
```

These mappings convert the numeric enum values from the API into readable names.

---

## Still Not Working?

If you've tried all solutions and still see numbers:

1. **Verify server is running:**
   ```bash
   curl http://localhost:5000/
   ```

2. **Check browser console for errors:**
   - Press F12
   - Go to "Console" tab
   - Look for JavaScript errors

3. **Verify the file was updated:**
   ```bash
   grep "RoleNames" src/VillageOfAshes.API/wwwroot/index.html
   ```

4. **Restart everything:**
   ```bash
   # Kill server
   pkill -9 dotnet
   
   # Restart
   ./start-game.sh
   
   # Clear browser cache
   # Hard refresh (Ctrl+F5)
   ```

---

## Quick Test

Open this test file in your browser to verify JavaScript mapping works:
```
file:///path/to/Village/test-roles.html
```

If the test shows "✅ MAPPING WORKS!" then the issue is definitely browser cache.
