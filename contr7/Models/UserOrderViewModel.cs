namespace contr7.Models;

public class UserOrderViewModel
{
    public User User { get; set; }
    public Order Order { get; set; }
    public List<Book> Books { get; set; }
}