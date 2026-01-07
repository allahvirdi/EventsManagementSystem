# Fix ErrorMessage nullable migration
dotnet ef migrations add FixErrorMessageNullable -p EventsManagement.Infrastructure -s EventsManagement.API
dotnet ef database update -p EventsManagement.Infrastructure -s EventsManagement.API
