using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;
using SmallBusinessInventory.Models;
using SmallBusinessInventory.Services;

namespace SmallBusinessInventory.Controllers;

public class ProductsController(InventoryDbContext db, OpenFoodFactsService foodFacts, ILogger<ProductsController> logger) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "name", bool lowStock = false)
    {
        var query = db.Products.Include(p => p.Category).Include(p => p.Supplier)
            .Where(p => !p.IsArchived);
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search) ||
                                     (p.Barcode != null && p.Barcode.Contains(search)));
        }
        if (lowStock) query = query.Where(p => p.Quantity <= p.ReorderLevel);
        query = sort switch
        {
            "sku" => query.OrderBy(p => p.Sku),
            "stock" => query.OrderBy(p => p.Quantity),
            _ => query.OrderBy(p => p.Name)
        };
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.LowStock = lowStock;
        var products = await query.AsNoTracking().ToListAsync();
        // EF Core 8's SQLite provider cannot translate decimal ordering reliably.
        // Sort the filtered catalog in memory without converting prices to floating point.
        if (sort == "price") products = products.OrderBy(p => p.Price).ThenBy(p => p.Name).ToList();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new Product { ReorderLevel = 5 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Sku,Barcode,Description,Price,Quantity,ReorderLevel,CategoryId,SupplierId")] Product product)
    {
        await ValidateProduct(product);
        if (ModelState.IsValid)
        {
            db.Products.Add(product);
            try
            {
                await db.SaveChangesAsync();
                TempData["Message"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Could not create product.");
                ModelState.AddModelError(string.Empty, "The product could not be saved. Check that its SKU is unique and the selected category and supplier still exist.");
            }
        }
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null || product.IsArchived) return NotFound();
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Sku,Barcode,Description,Price,Quantity,ReorderLevel,CategoryId,SupplierId")] Product product)
    {
        if (id != product.Id) return BadRequest();
        var current = await db.Products.FindAsync(id);
        if (current is null || current.IsArchived) return NotFound();
        await ValidateProduct(product, id);
        if (ModelState.IsValid)
        {
            current.Name = product.Name; current.Sku = product.Sku; current.Barcode = product.Barcode;
            current.Description = product.Description; current.Price = product.Price;
            current.Quantity = product.Quantity; current.ReorderLevel = product.ReorderLevel;
            current.CategoryId = product.CategoryId; current.SupplierId = product.SupplierId;
            try
            {
                await db.SaveChangesAsync();
                TempData["Message"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Could not update product {ProductId}.", id);
                ModelState.AddModelError(string.Empty, "The product could not be saved. Check that its SKU is unique and the selected category and supplier still exist.");
            }
        }
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    private async Task ValidateProduct(Product product, int? existingId = null)
    {
        product.Name = (product.Name ?? string.Empty).Trim();
        product.Sku = (product.Sku ?? string.Empty).Trim();
        product.Barcode = string.IsNullOrWhiteSpace(product.Barcode) ? null : product.Barcode.Trim();
        product.Description = string.IsNullOrWhiteSpace(product.Description) ? null : product.Description.Trim();
        if (string.IsNullOrWhiteSpace(product.Name))
            ModelState.AddModelError(nameof(product.Name), "Enter a product name.");
        if (string.IsNullOrWhiteSpace(product.Sku))
            ModelState.AddModelError(nameof(product.Sku), "Enter a SKU.");
        else if (await db.Products.AnyAsync(p => p.Sku == product.Sku && p.Id != (existingId ?? 0)))
            ModelState.AddModelError(nameof(product.Sku), "That SKU already exists.");
        if (decimal.Round(product.Price, 2) != product.Price)
            ModelState.AddModelError(nameof(product.Price), "Enter a price with no more than two decimal places.");
        if (product.CategoryId > 0 && !await db.Categories.AnyAsync(c => c.Id == product.CategoryId))
            ModelState.AddModelError(nameof(product.CategoryId), "Select an existing category.");
        if (product.SupplierId > 0 && !await db.Suppliers.AnyAsync(s => s.Id == product.SupplierId))
            ModelState.AddModelError(nameof(product.SupplierId), "Select an existing supplier.");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.IsArchived = true;
        await db.SaveChangesAsync();
        TempData["Message"] = "Product archived.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> LookupBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return BadRequest(new { message = "Enter a barcode first." });
        try
        {
            var product = await foodFacts.FindByBarcodeAsync(barcode);
            return product is null ? NotFound(new { message = "No product found for that barcode." }) : Json(product);
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { message = "The product lookup service could not be reached." });
        }
        catch (System.Text.Json.JsonException)
        {
            return StatusCode(502, new { message = "The product lookup returned invalid data." });
        }
    }

    private async Task LoadLists(int? categoryId = null, int? supplierId = null)
    {
        ViewBag.CategoryId = new SelectList(await db.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId);
        ViewBag.SupplierId = new SelectList(await db.Suppliers.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", supplierId);
    }
}
