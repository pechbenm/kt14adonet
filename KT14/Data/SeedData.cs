using KT14.Models;

namespace KT14.Data;

public static class SeedData
{
    public static void Initialize(SchoolContext db)
    {
        if (db.Teachers.Any()) return;

        var petrov = new Teacher { FirstName = "Иван", LastName = "Петров", Email = "petrov@school.local", Department = "Информатика" };
        var sidorova = new Teacher { FirstName = "Анна", LastName = "Сидорова", Email = "sidorova@school.local", Department = "Математика" };

        var s1 = new Student { FirstName = "Мария", LastName = "Иванова", Email = "ivanova@student.local", BirthDate = new DateTime(2005, 3, 14) };
        var s2 = new Student { FirstName = "Олег", LastName = "Кузнецов", Email = "kuznetsov@student.local", BirthDate = new DateTime(2004, 11, 2) };
        var s3 = new Student { FirstName = "Елена", LastName = "Смирнова", Email = "smirnova@student.local", BirthDate = new DateTime(2005, 7, 21) };

        db.Courses.AddRange(
            new Course { Title = "Основы C#", Description = "Синтаксис, ООП, LINQ.", Credits = 5, Teacher = petrov, Students = new List<Student> { s1, s2 } },
            new Course { Title = "ASP.NET Core", Description = "MVC, EF Core, REST API.", Credits = 6, Teacher = petrov, Students = new List<Student> { s2, s3 } },
            new Course { Title = "Дискретная математика", Credits = 4, Teacher = sidorova, Students = new List<Student> { s1 } });

        db.SaveChanges();
    }
}
