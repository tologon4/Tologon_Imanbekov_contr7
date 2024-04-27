using System.ComponentModel.DataAnnotations;

namespace contr7.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Введите номер телефона в формате: 0123456789")]
    public string Phone { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}