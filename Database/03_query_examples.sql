-- UPDATE example (run against a copy of the database)
UPDATE Products SET Price = 1.35 WHERE Sku = 'BEV-001';

-- DELETE example: removes only a sample draft order; cascading removes its details.
DELETE FROM PurchaseOrders WHERE OrderNumber = 'PO-DELETE-EXAMPLE' AND Status = 'Draft';

-- Search + JOIN: product, category, supplier
SELECT p.Sku, p.Name, c.Name AS Category, s.Name AS Supplier, p.Price, p.Quantity
FROM Products p JOIN Categories c ON c.Id=p.CategoryId JOIN Suppliers s ON s.Id=p.SupplierId
WHERE p.Name LIKE '%water%' ORDER BY p.Name ASC;

-- Aggregate: value and count by category
SELECT c.Name, COUNT(p.Id) AS ProductCount, COALESCE(SUM(p.Price*p.Quantity),0) AS StockValue
FROM Categories c LEFT JOIN Products p ON p.CategoryId=c.Id AND p.IsArchived=0
GROUP BY c.Id,c.Name ORDER BY StockValue DESC;

-- Low-stock report
SELECT Sku, Name, Quantity, ReorderLevel FROM Products
WHERE IsArchived=0 AND Quantity <= ReorderLevel ORDER BY Quantity ASC;

-- Order line totals
SELECT po.OrderNumber, po.OrderDate, COALESCE(c.Name,'Walk-in customer') AS Customer,
       p.Name AS Product, d.Quantity, d.UnitPrice, d.Quantity*d.UnitPrice AS LineTotal
FROM PurchaseOrders po JOIN PurchaseOrderDetails d ON d.PurchaseOrderId=po.Id
JOIN Products p ON p.Id=d.ProductId LEFT JOIN Customers c ON c.Id=po.CustomerId
ORDER BY po.OrderDate DESC;

-- Stock movement history
SELECT p.Sku,p.Name,m.MovementDate,m.MovementType,m.QuantityChange,m.Notes
FROM StockMovements m JOIN Products p ON p.Id=m.ProductId ORDER BY m.MovementDate DESC;
