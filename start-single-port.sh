#!/bin/bash

echo "============================================"
echo "Single Port Deployment - Build and Run"
echo "============================================"
echo ""

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo "🔧 Step 1: Restoring packages..."
dotnet restore
echo ""

echo "🏗️ Step 2: Building solution..."
dotnet build --no-restore
echo ""

echo "📦 Step 3: Publishing Blazor..."
cd AppointmentManagementSystem.BlazorUI
dotnet publish -c Release -o ../AppointmentManagementSystem.API/wwwroot/blazor
cd ..
echo ""

echo "🚀 Step 4: Starting API (Blazor included)..."
echo ""
echo "============================================"
echo "✅ Ready!"
echo "============================================"
echo "📊 API: http://localhost:5089/api"
echo "📚 Swagger: http://localhost:5089/swagger"
echo "🌐 Blazor UI: http://localhost:5089"
echo "============================================"
echo ""

cd AppointmentManagementSystem.API
dotnet run
