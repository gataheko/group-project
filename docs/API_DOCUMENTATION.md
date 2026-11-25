# External API Documentation — Open Food Facts

**Purpose:** Product/barcode lookup helps inventory staff fill product information from a barcode instead of manually retyping it. This is an operational workflow, not a decorative API display.

**Service:** Open Food Facts public product endpoint, `https://world.openfoodfacts.org/api/v2/product/{barcode}.json`.

**Authentication:** No API key is required for the public lookup used by this project. Internet access is required. Treat returned fields as untrusted external data and review them before saving. The API may return no match, incomplete data, or rate-limit responses; the application should display a useful message and allow manual entry.

**Typical response fields used:** product name, brands, and barcode/product code when present. The service class is `Services/OpenFoodFactsService.cs`; the product form provides the barcode lookup interaction.

**Privacy:** Do not send customer information to this service. Only the product barcode is sent.
