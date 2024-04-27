using System.ComponentModel.DataAnnotations;

namespace contr7.Models;

public class User
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Name { get; set; }
    public string LastName { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Введите номер телефона в формате: 0123456789")]
    public string Phone { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Password { get; set; }
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    
}