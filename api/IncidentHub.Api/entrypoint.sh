#!/bin/sh
set -e

echo "🚀 Starting IncidentHub API without EF migrations..."
exec dotnet IncidentHub.Api.dll
