# User Guide

1. Start the application and open the local URL shown in the terminal/browser.
2. Use the dashboard to review inventory totals and low-stock indicators.
3. Open Products to search and sort the inventory list.
4. Choose Create Product to enter a name, unique SKU, price, quantity, reorder level, category, and supplier. Required fields and numeric ranges are validated.
5. Use Edit to correct product details. Use archive to hide retired products without erasing their history.
6. Enter a barcode and use the Open Food Facts lookup if available; verify returned information before saving.
7. Use Reports for low-stock and inventory-value summaries.

Categories and suppliers must exist before assigning them to a product. The seeded demo data is for testing only. The initial app has product CRUD/archiving; the extra order and audit tables establish the required schema but do not yet have complete dedicated screens, so do not claim full UI CRUD for every table until those screens are implemented.
