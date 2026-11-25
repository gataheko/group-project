using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusinessInventory.Data;
using SmallBusinessInventory.Models;

namespace SmallBusinessInventory.Controllers;

public class CustomersController(InventoryDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var customers = db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            customers = customers.Where(c => c.Name.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.Phone != null && c.Phone.Contains(search)));
        }
        ViewBag.Search = search;
        return View(await customers.OrderBy(c => c.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? NotFound() : View(customer);
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address")] Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        TempData["Message"] = "Customer created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Address")] Customer input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        customer.Name = input.Name;
        customer.Email = input.Email;
        customer.Phone = input.Phone;
        customer.Address = input.Address;
        await db.SaveChangesAsync();
        TempData["Message"] = "Customer updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        db.Customers.Remove(customer);
        await db.SaveChangesAsync();
        TempData["Message"] = "Customer deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
