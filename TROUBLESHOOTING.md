# Troubleshooting Guide

## Common Issues and Solutions

### 1. Game Won't Start - "Address already in use"

**Error Message:**
```
System.IO.IOException: Failed to bind to address http://127.0.0.1:5151: address already in use.
```

**Cause:** Another instance of the game (or another application) is already using the port.

**Solutions:**

#### Option A: Use the startup script (Easiest)
```bash
./start-game.sh
```
The script automatically detects and handles port conflicts.

#### Option B: Manual fix
```bash
# 1. Find the process using the port
lsof -i :5000
# or
lsof -i :5151

# 2. Kill the process (replace 12345 with actual PID)
kill -9 12345

# 3. Start the game
cd src/VillageOfAshes.API
dotnet run --urls "http://localhost:5000"
```

---

### 2. Port Mismatch - Frontend Can't Connect to Backend

**Symptoms:**
- Server starts successfully but game doesn't load
- Browser console shows connection errors
- "No active game" errors

**Cause:** The server is running on port 5151 but the frontend (index.html) expects port 5000.

**Solution:**
Always start the server with the `--urls` parameter:
```bash
dotnet run --urls "http://localhost:5000"
```

Or use the startup script:
```bash
./start-game.sh
```

**Why this happens:**
The `launchSettings.json` file configures the default port as 5151, but the frontend HTML file has the API endpoint hardcoded to port 5000. Using the `--urls` parameter overrides the launch settings.

---

### 3. CORS Errors

**Error in browser console:**
```
Access to fetch at 'http://localhost:5000/api/game/new' from origin 'null' has been blocked by CORS policy
```

**Solution:**
- Make sure the API server is running
- The API already has CORS configured to allow all origins
- Try accessing via `http://localhost:5000` instead of opening the HTML file directly

---

### 4. No NPCs Showing

**Symptoms:**
- Game starts but villagers panel is empty
- Evidence and rumors panels are empty

**Solutions:**
1. Click the "Refresh" button
2. Check browser console (F12) for JavaScript errors
3. Verify API is accessible: `curl http://localhost:5000/api/game/state`
4. Restart the server

---

### 5. Build Errors

**Error:**
```
error CS0246: The type or namespace name 'X' could not be found
```

**Solution:**
```bash
# Restore NuGet packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

---

### 6. Game State Not Updating

**Symptoms:**
- Time advances but nothing changes
- NPCs don't die during night phase
- No evidence generated

**Possible Causes:**
1. Night simulation service not executing
2. Game state not being updated properly

**Solutions:**
1. Check server logs for errors
2. Restart the game (click "New Game")
3. Check that you're advancing time past 9:00 PM for night simulation

---

## Quick Diagnostic Commands

### Check if server is running
```bash
curl http://localhost:5000/api/game/state
```

### Check what's using port 5000
```bash
lsof -i :5000
```

### View server logs
Check the terminal where you ran `dotnet run`

### Test API endpoints
```bash
# Create new game
curl -X POST http://localhost:5000/api/game/new

# Get game state
curl http://localhost:5000/api/game/state

# Advance time
curl -X POST http://localhost:5000/api/game/advance-time \
  -H "Content-Type: application/json" \
  -d "60"
```

---

## Getting Help

If you're still experiencing issues:

1. Check the server terminal for error messages
2. Check browser console (F12) for JavaScript errors
3. Verify .NET version: `dotnet --version` (should be 10.x or higher)
4. Try a clean rebuild:
   ```bash
   dotnet clean
   dotnet build
   ./start-game.sh
   ```

---

## Prevention Tips

1. **Always use the startup script** - It handles port conflicts automatically
2. **Don't run multiple instances** - Only one game server should run at a time
3. **Check ports before starting** - Use `lsof -i :5000` to verify the port is free
4. **Use Ctrl+C to stop** - Always stop the server gracefully before starting a new instance
