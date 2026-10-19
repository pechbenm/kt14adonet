using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT14.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Укажите название")]
    [StringLength(100)]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Range(1, 30, ErrorMessage = "Кредиты: от 1 до 30")]
    [Display(Name = "Кредиты")]
    public int Credits { get; set; } = 3;

    // Внешний ключ на преподавателя (обязательная связь)
    [Range(1, int.MaxValue, ErrorMessage = "Выберите преподавателя")]
    [Display(Name = "Преподаватель")]
    [ForeignKey(nameof(Teacher))]
    public int TeacherId { get; set; }

    // Nullable, чтобы MVC не требовал навигационное свойство при привязке формы
    public Teacher? Teacher { get; set; }

    // Многие ко многим со Student
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
