#!/bin/bash

echo "============================================"
echo "Starting Both API and Blazor on Same Port"
echo "============================================"
echo ""

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR/AppointmentManagementSystem.API"

echo "🚀 Starting API (Blazor will auto-start on same port)..."
echo ""
echo "============================================"
echo "✅ Starting..."
echo "============================================"
echo "📊 API: http://localhost:5089/api"
echo "📚 Swagger: http://localhost:5089/swagger"
echo "🌐 Blazor UI: http://localhost:5089"
echo "============================================"
echo ""
echo "⏳ Blazor compiling may take 10-30 seconds on first load"
echo ""

dotnet run
