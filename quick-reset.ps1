# Quick Database Reset Script

Write-Host "Dropping database..." -ForegroundColor Yellow
dotnet ef database drop -p EventsManagement.Infrastructure -s EventsManagement.API --force

Write-Host "Removing old migrations..." -ForegroundColor Yellow
Remove-Item -Path "EventsManagement.Infrastructure\Migrations\*" -Force -Recurse -ErrorAction SilentlyContinue

Write-Host "Creating new migration..." -ForegroundColor Cyan
dotnet ef migrations add InitialCreate -p EventsManagement.Infrastructure -s EventsManagement.API

Write-Host "Done! Now run the API with: dotnet run --project EventsManagement.API" -ForegroundColor Green
