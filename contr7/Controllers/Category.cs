using contr7.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace contr7.Controllers;

public class Category : Controller
{
    private BookContext _context;

    public Category(BookContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult Delete(int id)
    {
        _context.Categories.Remove(_context.Categories.FirstOrDefault(c => c.Id == id));
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult Index()
    {
        ViewBag.CategoryList = _context.Categories.ToList();
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")] 
    public IActionResult Index(Models.Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        ViewBag.CategoryList = _context.Categories.ToList();
        return View();
    }
}