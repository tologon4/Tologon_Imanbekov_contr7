using System.Security.Claims;
using contr7.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
               return RedirectToAction("Index", "Home");
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
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Register(RegisterViewModel model)
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
               return RedirectToAction("Index", "Home");
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
}