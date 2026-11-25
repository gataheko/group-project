PRAGMA foreign_keys = ON;
INSERT OR IGNORE INTO Categories (Name, Description) VALUES
 ('Beverages','Drinks and bottled beverages'), ('Snacks','Packaged snacks and treats'), ('Household','Everyday household items');
INSERT OR IGNORE INTO Suppliers (Name, ContactName, Email, Phone) VALUES
 ('Peach State Wholesale','Morgan Lee','orders@peachstate.example','706-555-0101'),
 ('Everyday Goods Co.','Taylor James','sales@everyday.example','706-555-0102');
INSERT OR IGNORE INTO Products (Name,Sku,Barcode,Description,Price,Quantity,ReorderLevel,CategoryId,SupplierId)
SELECT 'Spring Water 500ml','BEV-001','3017620422003','Bottled spring water',1.25,42,12,c.Id,s.Id FROM Categories c, Suppliers s WHERE c.Name='Beverages' AND s.Name='Peach State Wholesale';
INSERT OR IGNORE INTO Products (Name,Sku,Description,Price,Quantity,ReorderLevel,CategoryId,SupplierId)
SELECT 'Potato Chips','SNK-001','Classic salted chips',2.49,7,10,c.Id,s.Id FROM Categories c, Suppliers s WHERE c.Name='Snacks' AND s.Name='Everyday Goods Co.';
INSERT OR IGNORE INTO Customers (Name,Email,Phone,Address) VALUES ('Sample Customer','customer@example.com','706-555-0110','Augusta, GA');
INSERT OR IGNORE INTO PurchaseOrders (OrderNumber,Status,CustomerId)
SELECT 'PO-1001','Draft',Id FROM Customers WHERE Email='customer@example.com';
INSERT OR IGNORE INTO PurchaseOrderDetails (PurchaseOrderId,ProductId,Quantity,UnitPrice)
SELECT po.Id,p.Id,2,p.Price FROM PurchaseOrders po,Products p WHERE po.OrderNumber='PO-1001' AND p.Sku='BEV-001';
