namespace contr7.Models;
using Microsoft.EntityFrameworkCore;

public class BookContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public BookContext(DbContextOptions<BookContext> options) : base(options){}
    
}