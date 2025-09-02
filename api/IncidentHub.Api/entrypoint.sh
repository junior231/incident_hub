#!/bin/sh
set -e

echo "🔄 Applying EF Core migrations..."
dotnet ef database update --no-build --connection "$ConnectionStrings__DefaultConnection" || echo "⚠️ Migration failed, starting API anyway..."

echo "🚀 Starting IncidentHub API..."
dotnet IncidentHub.Api.dll
