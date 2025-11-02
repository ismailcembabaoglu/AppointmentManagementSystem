#!/bin/bash

echo "============================================"
echo "Starting Appointment Management System"
echo "============================================"
echo ""

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# API directory
API_DIR="$SCRIPT_DIR/AppointmentManagementSystem.API"

# Blazor directory
BLAZOR_DIR="$SCRIPT_DIR/AppointmentManagementSystem.BlazorUI"

echo "🔧 Restoring packages..."
dotnet restore
echo ""

echo "🏗️ Building solution..."
dotnet build --no-restore
echo ""

echo "🚀 Starting API..."
cd "$API_DIR"
gnome-terminal --title="API Server" -- bash -c "dotnet run; exec bash" 2>/dev/null || \
xterm -T "API Server" -e "dotnet run; bash" 2>/dev/null || \
osascript -e "tell application \"Terminal\" to do script \"cd '$API_DIR' && dotnet run\"" 2>/dev/null &

echo "⏳ Waiting for API to start (3 seconds)..."
sleep 3

echo "🌐 Starting Blazor UI..."
cd "$BLAZOR_DIR"
export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=""
gnome-terminal --title="Blazor UI" -- bash -c "dotnet run; exec bash" 2>/dev/null || \
xterm -T "Blazor UI" -e "dotnet run; bash" 2>/dev/null || \
osascript -e "tell application \"Terminal\" to do script \"cd '$BLAZOR_DIR' && export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES='' && dotnet run\"" 2>/dev/null &

echo ""
echo "============================================"
echo "✅ Both services started!"
echo "============================================"
echo "📊 API: http://localhost:5089"
echo "📊 Swagger: http://localhost:5089/swagger"
echo "🌐 Blazor: http://localhost:5090"
echo "============================================"
echo ""
