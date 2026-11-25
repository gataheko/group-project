# Database Schema and Normalization

The application uses SQLite and Entity Framework Core. The schema contains eight tables:

1. **Categories** — product classification; unique category name.
2. **Suppliers** — supplier contact information; unique supplier name.
3. **Products** — SKU, descriptive data, price, current quantity, category and supplier foreign keys.
4. **Customers** — customer contact information.
5. **PurchaseOrders** — order number, date, status, and optional customer foreign key.
6. **PurchaseOrderDetails** — line items linking orders and products, with quantity and sale-time unit price.
7. **StockMovements** — dated inventory increases/decreases and explanatory notes.
8. **InventoryAdjustments** — before/after quantity and reason for a counted adjustment.

## Keys and integrity
Each table has an integer primary key. Foreign keys connect products to categories/suppliers, orders to customers, order details to orders/products, and stock audit records to products. Unique indexes prevent duplicate category names, supplier names, SKUs, and order numbers. Restrict deletes where deletion would orphan product history; order details cascade only when their parent order is deleted.

## 3NF rationale
Each table represents one entity or relationship. Attributes are atomic (1NF); each table uses a single-column primary key and non-key attributes depend on the whole key (2NF); non-key attributes are not used to determine other non-key attributes in the same table (3NF). Customer contact data is not copied into each order, and category/supplier names are stored in their own tables. `PurchaseOrderDetails.UnitPrice` intentionally snapshots the unit price for that order line rather than relying on the product's current price.

## Files
- `Database/01_schema.sql`: create tables and indexes.
- `Database/02_sample_data.sql`: insert demonstration rows.
- `Database/03_query_examples.sql`: update, delete, search, joins, and aggregate examples.

For a fresh database in the application, stop the app and back up/delete `inventory.db` before restarting. `EnsureCreated()` creates missing schema only when the database is new; it does not migrate an existing database.
