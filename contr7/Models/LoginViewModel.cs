using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace contr7.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}