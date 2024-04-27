using contr7.Models;
using contr7.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public IActionResult Index(string name, string autor, string status, BookSortState sortState = BookSortState.NameAsc, int page =1)
    {
        ViewBag.Statuses = new List<string>() { "В наличии", "Выдана"};
        List<Book> books = _context.Books.ToList();
        if (!string.IsNullOrEmpty(name))
        {
            books = books.Where(b => b.Name==name).ToList();
        }
        if (!string.IsNullOrEmpty(status))
        {
            books = books.Where(b => b.Status==status).ToList();
        }
        if (!string.IsNullOrEmpty(autor))
        {
            books = books.Where(b => b.Autor == autor).ToList();
        }
        ViewBag.NameSort = sortState==BookSortState.NameAsc ? BookSortState.NameDesc : BookSortState.NameAsc;
        ViewBag.AutorSort = sortState == BookSortState.AutorAsc ? BookSortState.AutorDesc: BookSortState.AutorAsc;
        ViewBag.StatusSort = sortState == BookSortState.StatusAsc ? BookSortState.StatusDesc : BookSortState.StatusAsc;
        switch (sortState)
        {
            case BookSortState.NameAsc:
                books = books.OrderBy(t => t.Name).ToList();
                break;
            case BookSortState.NameDesc:
                books = books.OrderByDescending(t => t.Name).ToList();
                break;
            case BookSortState.AutorAsc:
                books = books.OrderBy(t => SortFields(t.Autor)).ToList();
                break;
            case BookSortState.AutorDesc:
                books = books.OrderByDescending(t => SortFields(t.Autor)).ToList();
                break;
            case BookSortState.StatusAsc:
                books = books.OrderBy(t => SortFields(t.Status)).ToList();
                break;
            case BookSortState.StatusDesc:
                books = books.OrderByDescending(t => SortFields(t.Status)).ToList();
                break;
            default:
                books = books.OrderBy(t => t.Name).ToList();
                break;
        }
        int pageSize = 2;
        int count = books.Count();
        var items = books.Skip((page - 1) * pageSize).Take(pageSize);
        PageViewModel pvm = new PageViewModel(count, page, pageSize);
        BookIndexViewModel bivm = new BookIndexViewModel()
        {
            PageViewModel = pvm,
            Books = items
        };
        return View(bivm);
    }

    public IActionResult Given(string name, string status, string autor, BookSortState sortState = BookSortState.NameAsc, int page=1)
    {
        ViewBag.Statuses = new List<string>() { "В наличии", "Выдана"};
        List<Book> books = new List<Book>();
        if (!string.IsNullOrEmpty(name))
        {
            books = books.Where(b => b.Name==name).ToList();
        }
        if (!string.IsNullOrEmpty(status))
        {
            books = books.Where(b => b.Status==status).ToList();
        }
        if (!string.IsNullOrEmpty(autor))
        {
            books = books.Where(b => b.Autor == autor).ToList();
        }
        List<Order> orders = _context.Orders.Include(u => u.User).Include(b => b.Book).ToList();
        foreach (var order in orders)
            books.Add(order.Book);
        ViewBag.NameSort = sortState==BookSortState.NameAsc ? BookSortState.NameDesc : BookSortState.NameAsc;
        ViewBag.AutorSort = sortState == BookSortState.AutorAsc ? BookSortState.AutorDesc: BookSortState.AutorAsc;
        ViewBag.StatusSort = sortState == BookSortState.StatusAsc ? BookSortState.StatusDesc : BookSortState.StatusAsc;
        switch (sortState)
        {
            case BookSortState.NameAsc:
                books = books.OrderBy(t => t.Name).ToList();
                break;
            case BookSortState.NameDesc:
                books = books.OrderByDescending(t => t.Name).ToList();
                break;
            case BookSortState.AutorAsc:
                books = books.OrderBy(t => SortFields(t.Autor)).ToList();
                break;
            case BookSortState.AutorDesc:
                books = books.OrderByDescending(t => SortFields(t.Autor)).ToList();
                break;
            case BookSortState.StatusAsc:
                books = books.OrderBy(t => SortFields(t.Status)).ToList();
                break;
            case BookSortState.StatusDesc:
                books = books.OrderByDescending(t => SortFields(t.Status)).ToList();
                break;
            default:
                books = books.OrderBy(t => t.Name).ToList();
                break;
        }
        int pageSize = 2;
        int count = books.Count();
        var items = books.Skip((page - 1) * pageSize).Take(pageSize);
        PageViewModel pvm = new PageViewModel(count, page, pageSize);
        BookGaveIndexViewModel bivm = new BookGaveIndexViewModel()
        {
            PageViewModel = pvm,
            Orders = orders
        };
        return View(bivm);
    }
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Book? book, IFormFile? uploadedFile)
    {
        ViewBag.Categories = _context.Categories.ToList();
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

    public IActionResult Edit(int id)
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View(_context.Books.FirstOrDefault(b => b.Id == id));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Book? book, IFormFile? uploadedFile)
    {
        ViewBag.Categories = _context.Categories.ToList();
        book.AddedDate = book.AddedDate.Value.ToUniversalTime();
        book.EditedDate = DateTime.UtcNow;
        if (ModelState.IsValid)
        {
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
            _context.Books.Update(book);
            _context.SaveChanges();
            return RedirectToAction("Book", new {id = book.Id});
        }
        return View(book);
    }

    public IActionResult Book(int id, string error)
    {
        ViewBag.ErrorMessage = error;
        Book book = _context.Books.Include(c => c.Category).FirstOrDefault(b => b.Id == id);
        return View(book);
    }
    private int SortFields(string field)
    {
        switch (field.ToLower())
        {
            case "В наличии":
                return 2;
            case "Выдана":
                return 1;
            default:
                return 0;
        }
    }
}