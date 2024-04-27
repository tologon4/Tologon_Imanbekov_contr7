using contr7.Models;
using Microsoft.AspNetCore.Mvc;

namespace contr7.Controllers;

public class OrderController : Controller
{
    private BookContext _context;

    public OrderController(BookContext context)
    {
        _context = context;
    }
    [HttpPost]
    public IActionResult Order(string email, int id)
    {
        string error = "";
        if (!string.IsNullOrEmpty(email))
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            Book? book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (user != null)
            {
                if (book.Status != "Выдана" && OrderCheck(user.Id))
                {
                    Order order = new Order()
                    {
                        UserId = user.Id, User = user, BookId = book.Id,
                        Book = book, IssueDate = DateTime.UtcNow
                    };
                    _context.Orders.Add(order);
                    book.Status = "Выдана";
                    _context.SaveChanges();
                }
                else
                    error = "Книга уже выдана или вы уже превысили лимит!";
                return RedirectToAction("Book", "Book", new { id = id, error =error });
            }
            else
                error = "Такого пользователя не существует!";
        }
        return RedirectToAction("Book", "Book",new {id = id, error =error});
    }

    bool OrderCheck(int userId)
    {
        bool result = true;
        var orders=  _context.Orders.ToList();
        int count = 0;
        foreach (var orderV in orders)
            if (orderV.UserId == userId) 
                count++;
        if (count>=3)
            result = false;
        return result;
    }
}