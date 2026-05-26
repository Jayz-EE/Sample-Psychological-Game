#!/bin/bash

# Village of Ashes - Game Startup Script
# This script starts the game server on the correct port

echo "🏚️  Starting Village of Ashes..."
echo ""

# Check if port 5000 is already in use
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1 ; then
    echo "⚠️  Port 5000 is already in use!"
    echo "Finding the process..."
    PID=$(lsof -Pi :5000 -sTCP:LISTEN -t)
    echo "Process ID: $PID"
    read -p "Do you want to kill this process and continue? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        kill -9 $PID
        echo "✓ Process killed"
    else
        echo "❌ Startup cancelled"
        exit 1
    fi
fi

echo "🚀 Starting server on http://localhost:5000"
echo "📂 Navigate to http://localhost:5000 in your browser"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""

cd src/VillageOfAshes.API
dotnet run --urls "http://localhost:5000"
