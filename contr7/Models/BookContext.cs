namespace contr7.Models;
using Microsoft.EntityFrameworkCore;

public class BookContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public BookContext(DbContextOptions<BookContext> options) : base(options){}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(new Role {Id = 1, Name = "admin"});
        modelBuilder.Entity<Role>().HasData(new Role {Id = 2, Name = "user"});
        modelBuilder.Entity<User>().HasData(new User {Id = 1, Email = "admin@admin.admin", Name = "tologon",LastName = "imanbekov", Phone = "0997180903", Password = "admin", RoleId = 1});
    }
    
}