# Project Proposal: Small Business Inventory System

## Problem
Small shops may track products, stock counts, suppliers, and reorders manually. This can cause inconsistent product records and missed low-stock items.

## Proposed solution
A web-based inventory tool for maintaining a searchable product catalogue, monitoring stock levels, viewing inventory value, and looking up product details by barcode through Open Food Facts.

## Intended users
Small business owners and staff responsible for receiving, counting, and updating stock.

## Scope
The current implementation includes product management, category/supplier references, search/sorting, validation, reporting, SQLite persistence, and barcode lookup. The schema also defines customers, purchase orders and details, stock movements, and inventory adjustments as the foundation for order and audit workflows.

## Success criteria
- App builds and starts on .NET 8.
- Product data persists in SQLite.
- Foreign keys and uniqueness rules prevent invalid relationships and duplicate SKUs.
- Search, sorting, validation, low-stock reporting, and API lookup can be demonstrated.
- SQL scripts and ER documentation describe the eight-table normalized schema.

## Risks
API availability varies; users must be able to enter data manually. The starter MVC UI does not yet expose every new order/audit entity, so those screens are future implementation work.
