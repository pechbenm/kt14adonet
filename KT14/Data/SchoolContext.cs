using Microsoft.EntityFrameworkCore;
using KT14.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KT14.Data;

public class SchoolContext(DbContextOptions<SchoolContext> options) : DbContext(options)
{
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ограничения длины, обязательность и т.п. заданы атрибутами в моделях.
        // Здесь — то, что удобнее выражать через Fluent API: индексы, связи, каскады.

        modelBuilder.Entity<Teacher>(e =>
        {
            e.ToTable("Teachers");
            e.HasIndex(t => t.Email).IsUnique();

            // Teacher 1 —— * Course. Удаление преподавателя удаляет его курсы
            // (а вместе с ними — записи студентов на эти курсы).
            e.HasMany(t => t.Courses)
             .WithOne(c => c.Teacher)
             .HasForeignKey(c => c.TeacherId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.ToTable("Students");
            e.HasIndex(s => s.Email).IsUnique();

            // Student * —— * Course через связующую таблицу StudentCourses.
            // Удаление студента или курса удаляет соответствующие строки связующей таблицы.
            e.HasMany(s => s.Courses)
             .WithMany(c => c.Students)
             .UsingEntity<Dictionary<string, object>>(
                 "StudentCourse",
                 right => right.HasOne<Course>().WithMany()
                               .HasForeignKey("CourseId")
                               .OnDelete(DeleteBehavior.Cascade),
                 left => left.HasOne<Student>().WithMany()
                              .HasForeignKey("StudentId")
                              .OnDelete(DeleteBehavior.Cascade),
                 join =>
                 {
                     join.ToTable("StudentCourses");
                     join.HasKey("StudentId", "CourseId");
                 });
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.ToTable("Courses");
            e.HasIndex(c => c.Title);
        });
    }
}
