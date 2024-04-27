namespace contr7.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public User? User { get; set; }
    public Book? Book { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? GaveDate { get; set; }
}