using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;

// Allows the controller to reference the Supplier Class. ~Rduehr
using SmallBusinessInventory.Models;

namespace SmallBusinessInventory.Controllers;

public class SuppliersController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Suppliers.Include(s => s.Products).OrderBy(s => s.Name).ToListAsync());

    public IActionResult Create() {
        return View(new Supplier());
    }

    //Restrict the submitted data Name, ContactName, Email, and Phone. ~Rduehr
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ContactName,Email,Phone")] Supplier supplier) {

        // Name normalization. ~Rduehr
        supplier.Name = supplier.Name?.Trim() ?? "";

        if (!ModelState.IsValid)
            return View(supplier);

        // Make sure there are no duplicate names. ~Rduehr
        if (await db.Suppliers.AnyAsync(s => s.Name == supplier.Name)) {
            ModelState.AddModelError(nameof(Supplier.Name), "A supplier with this name already exists.");

            return View(supplier);
        }

// Most of the below is edited and copied from Category Management. ~Rduehr
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();
        TempData["Message"] = "Supplier created successfully.";

        return RedirectToAction(nameof(Index));
    }

    // Edit action. Requires a valid ID and entry. ~rduehr
    public async Task<IActionResult> Edit(int id) {
        var supplier = await db.Suppliers.FindAsync(id);

        // Make sure the entity actually exists. ~rduehr
        if (supplier is null)
            return NotFound();

        return View(supplier);
    }

    [HttpPost, ValidateAntiForgeryToken]
    // Later added [FromRoute] to make sure the id comes from the URL and not from the same field. Not sure if it's necessary, but it doesn't hurt. ~rduehr
    public async Task<IActionResult> Edit([FromRoute] int id,[Bind("Id,Name,ContactName,Email,Phone")] Supplier input) {

        // ID validation to make sure it matches the one submitted by the form. ~rduehr
        if (id != input.Id)
            return BadRequest();

        input.Name = input.Name?.Trim() ?? "";

        if (!ModelState.IsValid)
            return View(input);

        var supplier = await db.Suppliers.FindAsync(id);

        if (supplier is null)
            return NotFound();

         // Made sure duplicate checking excludes the current supplier. ~rduehr
        if (await db.Suppliers.AnyAsync(s => s.Name == input.Name && s.Id != id)) {
            ModelState.AddModelError(nameof(Supplier.Name), "A supplier with this name already exists.");

            return View(input);
        }

        supplier.Name = input.Name;
        supplier.ContactName = input.ContactName;
        supplier.Email = input.Email;
        supplier.Phone = input.Phone;

        await db.SaveChangesAsync();

        TempData["Message"] = "Supplier updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // Delete action. I originally had this a GET action, but I removed it so I could just have a button popup for deletion instead of creating a new delete page. ~rduehr
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id) {
        var supplier = await db.Suppliers.FindAsync(id);

        if (supplier is null)
            return NotFound();

        // Stopped someone from deleting a supplier with a product to have invalid orphaned entries since the alternative was far more annoying. ~rduehr
        if (await db.Products.AnyAsync(p => p.SupplierId == id)) {
            TempData["Message"] = "Cannot delete a supplier that has associated products.";

            return RedirectToAction(nameof(Index));
        }

        db.Suppliers.Remove(supplier);

        try {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException) {
            TempData["Message"] = "Unable to delete this supplier. It may still be referenced by other records.";

            return RedirectToAction(nameof(Index));
        }

        TempData["Message"] = "Supplier deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}
