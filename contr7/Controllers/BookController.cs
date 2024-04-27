using contr7.Models;
using Microsoft.AspNetCore.Mvc;

namespace contr7.Controllers;

public class BookController : Controller
{
    private BookContext _context;
    private IWebHostEnvironment _environment;

    public BookController(BookContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    // GET
    public IActionResult Index()
    {
        return View(_context.Books.ToList());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Book? book, IFormFile? uploadedFile)
    {
        
        if (ModelState.IsValid)
        {
            book.AddedDate = DateTime.UtcNow;
            book.Status = "В наличии";
            if (uploadedFile != null)
            {
                string newFileName = Path.ChangeExtension(book.Name.Trim(), Path.GetExtension(uploadedFile.FileName));
                string path= "/imageFile/" + newFileName.Trim();
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                book.ImagePath = path;
            }
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(book);
    }

    public IActionResult Book(int? id)
    {
        return View(_context.Books.FirstOrDefault(b => b.Id == id));
    }
}