using System.Security.Claims;
using contr7.Models;
using contr7.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace contr7.Controllers;

public class AccountController : Controller
{
    private BookContext _context;

    public AccountController(BookContext context)
    {
        _context = context;
        
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Give(int id)
    {
        var userName = User.Identity.Name;
        User user = _context.Users.FirstOrDefault(u => u.Name == userName);
        List<Book> books = new List<Book>();
        List<Order> orders = _context.Orders.Include(o => o.Book).ToList();
        foreach (var orderV in orders)
            if (orderV.UserId == user.Id)
                books.Add(orderV.Book);
        Book book = books.FirstOrDefault(b => b.Id ==id);
        Order order = _context.Orders.FirstOrDefault(o => o.BookId == book.Id && o.UserId == user.Id);
        _context.Orders.Remove(order);
        book.Status = "В наличии";
        _context.SaveChanges();
        return RedirectToAction("Cabinet");
    }
    public IActionResult Cabinet(string name, string autor, string status, BookSortState sortState = BookSortState.NameAsc, int page=1)
    {
        ViewBag.Statuses = new List<string>() { "В наличии", "Выдана"};
        var userName = User.Identity.Name;
        User user = _context.Users.FirstOrDefault(u => u.Name == userName);
        List<Book> books = new List<Book>();
        List<Order> orders = _context.Orders.Include(o => o.Book).ToList();
        foreach (var orderV in orders)
            if (orderV.UserId == user.Id)
                books.Add(orderV.Book);
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User? user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
            if (user != null)
            {
                await Authenticate(user); 
                return RedirectToAction("Index", "Book");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]   public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                User newUser = new User { Email = model.Email, Password = model.Password, 
                    Name = model.Name, LastName =model.LastName, Phone = model.Phone, 
                    RoleId = 2, Role = role};
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                await Authenticate(newUser);
                return RedirectToAction("Index", "Book");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
        }
        return View(model);
    }
    
    private async Task Authenticate(User user) {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
        };
        ClaimsIdentity id = new ClaimsIdentity( claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(id),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
            });
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
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