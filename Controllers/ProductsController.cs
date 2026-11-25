using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;
using SmallBusinessInventory.Models;
using SmallBusinessInventory.Services;

namespace SmallBusinessInventory.Controllers;

public class ProductsController(InventoryDbContext db, OpenFoodFactsService foodFacts) : Controller
{
    public async Task<IActionResult> Index(string? search, string sort = "name", bool lowStock = false)
    {
        var query = db.Products.Include(p => p.Category).Include(p => p.Supplier)
            .Where(p => !p.IsArchived);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search) ||
                                     (p.Barcode != null && p.Barcode.Contains(search)));
        if (lowStock) query = query.Where(p => p.Quantity <= p.ReorderLevel);
        query = sort switch
        {
            "sku" => query.OrderBy(p => p.Sku),
            "stock" => query.OrderBy(p => p.Quantity),
            "price" => query.OrderBy(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.LowStock = lowStock;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new Product { ReorderLevel = 5 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (await db.Products.AnyAsync(p => p.Sku == product.Sku))
            ModelState.AddModelError(nameof(product.Sku), "That SKU already exists.");
        if (ModelState.IsValid)
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            TempData["Message"] = "Product added successfully.";
            return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        if (await db.Products.AnyAsync(p => p.Sku == product.Sku && p.Id != id))
            ModelState.AddModelError(nameof(product.Sku), "That SKU already exists.");
        if (ModelState.IsValid)
        {
            var current = await db.Products.FindAsync(id);
            if (current is null || current.IsArchived) return NotFound();
            current.Name = product.Name; current.Sku = product.Sku; current.Barcode = product.Barcode;
            current.Description = product.Description; current.Price = product.Price;
            current.Quantity = product.Quantity; current.ReorderLevel = product.ReorderLevel;
            current.CategoryId = product.CategoryId; current.SupplierId = product.SupplierId;
            await db.SaveChangesAsync();
            TempData["Message"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
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
