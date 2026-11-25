"""Verify schema constraints on a fresh SQL or EF-created demonstration database.

Run without arguments for the raw SQL schema, or pass a populated demo SQLite path.
All test writes are rolled back, even if a check fails.
"""
import sqlite3
import sys
from pathlib import Path

root = Path(__file__).resolve().parents[1]
if len(sys.argv) > 2:
    raise SystemExit("Usage: python3 tests/database_integrity.py [demo-database-path]")
if len(sys.argv) == 2:
    path = Path(sys.argv[1]).resolve()
    if not path.is_file():
        raise SystemExit(f"Database does not exist: {path}")
    conn = sqlite3.connect(str(path))
else:
    conn = sqlite3.connect(":memory:")
    conn.executescript((root / "Database/01_schema.sql").read_text())
    conn.executescript((root / "Database/02_sample_data.sql").read_text())
conn.execute("PRAGMA foreign_keys = ON")


def reject(label, sql, params=()):
    try:
        conn.execute(sql, params)
    except sqlite3.IntegrityError:
        print(f"PASS: {label}")
    else:
        raise AssertionError(f"Constraint did not reject: {label}")


try:
    conn.execute("BEGIN")
    expected = {"Categories", "Suppliers", "Products", "Customers", "PurchaseOrders",
                "PurchaseOrderDetails", "StockMovements", "InventoryAdjustments"}
    actual = {r[0] for r in conn.execute("SELECT name FROM sqlite_master WHERE type='table'")}
    assert expected <= actual, f"Missing tables: {expected - actual}"
    assert not conn.execute("PRAGMA foreign_key_check").fetchall(), "Existing foreign key violations"
    print("PASS: all eight tables exist and foreign_key_check is clean")
    product, category, supplier, sku = conn.execute(
        "SELECT Id, CategoryId, SupplierId, Sku FROM Products ORDER BY Id LIMIT 1").fetchone()
    missing_category = conn.execute("SELECT COALESCE(MAX(Id), 0)+1 FROM Categories").fetchone()[0]
    reject("invalid category foreign key", "UPDATE Products SET CategoryId=? WHERE Id=?", (missing_category, product))
    reject("negative product price", "UPDATE Products SET Price=-1 WHERE Id=?", (product,))
    reject("zero product price", "UPDATE Products SET Price=0 WHERE Id=?", (product,))
    reject("negative stock", "UPDATE Products SET Quantity=-1 WHERE Id=?", (product,))
    reject("negative reorder level", "UPDATE Products SET ReorderLevel=-1 WHERE Id=?", (product,))
    reject("referenced category deletion", "DELETE FROM Categories WHERE Id=?", (category,))
    reject("referenced supplier deletion", "DELETE FROM Suppliers WHERE Id=?", (supplier,))
    reject("duplicate SKU", "INSERT INTO Products (Name,Sku,Price,Quantity,ReorderLevel,CategoryId,SupplierId,IsArchived,CreatedAt) VALUES ('Test',?,1,1,1,?,?,0,CURRENT_TIMESTAMP)", (sku,category,supplier))
    customer = conn.execute("INSERT INTO Customers (Name) VALUES ('Integrity test')").lastrowid
    order = conn.execute("INSERT INTO PurchaseOrders (OrderNumber,OrderDate,Status,CustomerId) VALUES ('INTEGRITY-' || hex(randomblob(8)),CURRENT_TIMESTAMP,'Draft',?)", (customer,)).lastrowid
    detail_sql = "INSERT INTO PurchaseOrderDetails (PurchaseOrderId,ProductId,Quantity,UnitPrice) VALUES (?,?,?,?)"
    reject("zero order line quantity", detail_sql, (order, product, 0, 1))
    reject("zero order line price", detail_sql, (order, product, 1, 0))
    conn.execute(detail_sql, (order, product, 1, 1))
    reject("duplicate order/product line", detail_sql, (order, product, 1, 1))
    adjustment_sql = "INSERT INTO InventoryAdjustments (ProductId,AdjustmentDate,PreviousQuantity,NewQuantity,Reason) VALUES (?,CURRENT_TIMESTAMP,?,?, 'Test')"
    reject("negative previous adjustment quantity", adjustment_sql, (product, -1, 1))
    reject("negative new adjustment quantity", adjustment_sql, (product, 1, -1))
    conn.execute("DELETE FROM Customers WHERE Id=?", (customer,))
    assert conn.execute("SELECT CustomerId FROM PurchaseOrders WHERE Id=?", (order,)).fetchone() == (None,)
    print("PASS: customer deletion sets order foreign key to NULL")
    conn.execute("DELETE FROM PurchaseOrders WHERE Id=?", (order,))
    assert conn.execute("SELECT COUNT(*) FROM PurchaseOrderDetails WHERE PurchaseOrderId=?", (order,)).fetchone() == (0,)
    print("PASS: order deletion cascades to lines")
finally:
    conn.rollback()
    conn.close()
