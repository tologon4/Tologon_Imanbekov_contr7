using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace contr7.Models;

public class Book
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Name { get; set; }
    public string? Status { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")]
    public string Autor { get; set; }
    public string? ImagePath { get; set; }
    [Required(ErrorMessage = "Заполните ячейку")] 
    [Range(int.MinValue, 2024, ErrorMessage = "Год не может быть в будущем!")]
    public int ReleaseYear { get; set; }
    public string? Description { get; set; }
    public DateTime? AddedDate { get; set; }
}