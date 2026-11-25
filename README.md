# Small Business Inventory System (C#)

A C# ASP.NET Core MVC inventory application for Visual Studio. It uses SQLite through Entity Framework Core and can look up product information from the public Open Food Facts API.

## Requirements
- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET 8 SDK
- Internet connection for Open Food Facts lookups

## Open and run
1. Extract the ZIP file.
2. Open `SmallBusinessInventory.csproj` in Visual Studio 2022 (or open the folder).
3. Allow Visual Studio to restore NuGet packages.
4. Select the project as the startup project.
5. Press **F5** or click **Run**.
6. The browser opens the local web application. The SQLite database `inventory.db` is created automatically.

If Visual Studio asks to trust the HTTPS development certificate, accept the prompt for local development.

## Features
- Inventory dashboard and total inventory value
- Add, edit, archive, and search products
- SKU and barcode fields; duplicate SKU validation
- Categories and suppliers
- Low-stock report and inventory valuation report
- Barcode lookup using Open Food Facts
- SQLite database with foreign keys and seeded sample data

## API key
No API key is required for the public Open Food Facts product endpoint. The lookup requires an internet connection.

## Database tables
The normalized schema defines eight tables: `Categories`, `Suppliers`, `Products`, `Customers`, `PurchaseOrders`, `PurchaseOrderDetails`, `StockMovements`, and `InventoryAdjustments`. Foreign keys and unique indexes enforce relationships and prevent duplicate SKUs, category names, supplier names, and order numbers. See `docs/ER_DIAGRAM.md` and `docs/SCHEMA_AND_NORMALIZATION.md`.

## SQL deliverables
- `Database/01_schema.sql` — create tables and indexes
- `Database/02_sample_data.sql` — insert sample data
- `Database/03_query_examples.sql` — update, delete, JOIN, search, and aggregate examples

## Documentation
Proposal, ER diagram, schema/normalization notes, API documentation, wireframes, user guide, developer guide, sprint plan template, meeting notes template, and final reflection prompts are in `docs/`. Replace the templates with your team's real records; do not claim meetings, reviews, or contributions that did not happen.

## Scope note
The existing UI primarily manages products, categories, and suppliers. The additional customer/order/stock-audit tables are implemented in the EF Core model and SQL schema, but complete user-interface CRUD screens for every new entity are not included yet. Test the project and complete any remaining screens before representing every requirement as finished.

## Notes for coursework
This is a working starter application. Before submitting, run it in your own Visual Studio installation, verify the screens, personalize the sample data, and add your actual group meeting notes, sprint evidence, repository link, and final reflection if your assignment requires them. Do not claim a GitHub workflow or team meeting happened unless it actually did.
