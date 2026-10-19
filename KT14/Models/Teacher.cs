using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT14.Models;

public class Teacher
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(50)]
    [Display(Name = "Имя")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите фамилию")]
    [StringLength(50)]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Кафедра")]
    public string? Department { get; set; }

    [NotMapped]
    public string FullName => $"{LastName} {FirstName}";

    // Один преподаватель — много курсов
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
