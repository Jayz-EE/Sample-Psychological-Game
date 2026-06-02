#!/bin/bash

# Village of Ashes - E2E Test Runner
# This script runs the comprehensive End-to-End tests

set -e

echo "=========================================="
echo "Village of Ashes - E2E Test Suite"
echo "=========================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}Building projects...${NC}"
dotnet build src/VillageOfAshes.API/VillageOfAshes.API.csproj

echo ""
echo -e "${BLUE}Running E2E Tests...${NC}"
echo ""

# Run tests with detailed output
dotnet test tests/VillageOfAshes.E2ETests/VillageOfAshes.E2ETests.csproj \
    --logger "console;verbosity=normal" \
    --configuration Debug

echo ""
echo -e "${GREEN}=========================================="
echo -e "E2E Tests Complete!"
echo -e "==========================================${NC}"
