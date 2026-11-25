# Developer Guide

## Stack
- C# / ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8 with SQLite
- Open Food Facts HTTP API

## Run locally
1. Install the .NET 8 SDK or Visual Studio 2022 with ASP.NET and web development.
2. Open `SmallBusinessInventory.csproj`.
3. Run `dotnet restore`, `dotnet build`, then `dotnet run` from this directory.
4. For a new schema, back up and remove `inventory.db` before starting; `EnsureCreated()` is not a migration system.

## Structure
- `Controllers/`: MVC request handling
- `Models/`: validation and entity definitions
- `Data/InventoryDbContext.cs`: tables, indexes, and relationships
- `Data/SeedData.cs`: initial demonstration records
- `Services/OpenFoodFactsService.cs`: external barcode lookup
- `Database/`: portable SQLite schema, sample data, and SQL examples
- `docs/`: design and project documentation

## Integrity and security notes
Foreign keys are configured in EF Core; delete restrictions protect related inventory history. Never store secrets in source control. Validate API output before persistence. Review `Database/01_schema.sql` before executing it against any real business data.
