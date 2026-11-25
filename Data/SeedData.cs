using SmallBusinessInventory.Models;

namespace SmallBusinessInventory.Data;

public static class SeedData
{
    public static void Initialize(InventoryDbContext db)
    {
        // Only seed an empty database; do not overwrite existing business records.
        using var transaction = db.Database.BeginTransaction();
        if (db.Categories.Any() || db.Suppliers.Any() || db.Products.Any() ||
            db.Customers.Any() || db.PurchaseOrders.Any() || db.PurchaseOrderDetails.Any() ||
            db.StockMovements.Any() || db.InventoryAdjustments.Any()) return;

        var categories = new[]
        {
            new Category { Name = "Beverages", Description = "Drinks and bottled beverages" },
            new Category { Name = "Snacks", Description = "Packaged snacks and treats" },
            new Category { Name = "Household", Description = "Everyday household items" }
        };
        db.Categories.AddRange(categories);
        db.SaveChanges();

        var suppliers = new[]
        {
            new Supplier { Name = "Peach State Wholesale", ContactName = "Morgan Lee", Email = "orders@peachstate.example", Phone = "706-555-0101" },
            new Supplier { Name = "Everyday Goods Co.", ContactName = "Taylor James", Email = "sales@everyday.example", Phone = "706-555-0102" }
        };
        db.Suppliers.AddRange(suppliers);
        db.SaveChanges();

        db.Products.AddRange(
            new Product { Name = "Spring Water 500ml", Sku = "BEV-001", Barcode = "3017620422003", Description = "Bottled spring water", Price = 1.25m, Quantity = 42, ReorderLevel = 12, CategoryId = categories[0].Id, SupplierId = suppliers[0].Id },
            new Product { Name = "Potato Chips", Sku = "SNK-001", Description = "Classic salted chips", Price = 2.49m, Quantity = 7, ReorderLevel = 10, CategoryId = categories[1].Id, SupplierId = suppliers[1].Id },
            new Product { Name = "Paper Towels", Sku = "HOM-001", Description = "Two-roll pack", Price = 3.99m, Quantity = 18, ReorderLevel = 5, CategoryId = categories[2].Id, SupplierId = suppliers[1].Id }
        );
        db.SaveChanges();
        transaction.Commit();
    }
}
