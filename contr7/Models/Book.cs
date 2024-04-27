using System.ComponentModel.DataAnnotations;

namespace contr7.Models;

public class Book
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Autor { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public IFormFile Image { get; set; }
    public DateTime ReleaseYear { get; set; }
    public string Description { get; set; }
    public DateTime AddedDate { get; set; }
}