# Entity Relationship Diagram

```mermaid
erDiagram
    CATEGORIES ||--o{ PRODUCTS : classifies
    SUPPLIERS ||--o{ PRODUCTS : supplies
    CUSTOMERS o|--o{ PURCHASE_ORDERS : places
    PURCHASE_ORDERS ||--|{ PURCHASE_ORDER_DETAILS : contains
    PRODUCTS ||--o{ PURCHASE_ORDER_DETAILS : ordered_as
    PRODUCTS ||--o{ STOCK_MOVEMENTS : records
    PRODUCTS ||--o{ INVENTORY_ADJUSTMENTS : adjusted_by
    CATEGORIES { int Id PK string Name UK }
    SUPPLIERS { int Id PK string Name UK }
    PRODUCTS { int Id PK string Sku UK int CategoryId FK int SupplierId FK decimal Price int Quantity }
    CUSTOMERS { int Id PK string Name }
    PURCHASE_ORDERS { int Id PK string OrderNumber UK int CustomerId FK string Status }
    PURCHASE_ORDER_DETAILS { int Id PK int PurchaseOrderId FK int ProductId FK int Quantity decimal UnitPrice }
    STOCK_MOVEMENTS { int Id PK int ProductId FK int QuantityChange string MovementType }
    INVENTORY_ADJUSTMENTS { int Id PK int ProductId FK int PreviousQuantity int NewQuantity string Reason }
```

The model has eight tables. Foreign keys preserve relationships. Product SKU, category name, supplier name, and order number are unique. Product prices and order-line prices are stored on their respective entities; order-line price records the price at the time of the order. Customer details are kept separately from orders to avoid repeating them. Stock movements and inventory adjustments are separate audit records. These choices support 3NF because non-key attributes describe their own table's entity and do not depend on other non-key attributes.
