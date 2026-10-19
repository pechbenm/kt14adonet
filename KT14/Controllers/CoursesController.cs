using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KT14.Data;
using KT14.Models;

namespace KT14.Controllers;

public class CoursesController(SchoolContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Courses.Include(c => c.Teacher).Include(c => c.Students).OrderBy(c => c.Title).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var course = await db.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course is null) return NotFound();
        
        // Студенты, которых ещё можно записать на курс
        var enrolledIds = course.Students.Select(s => s.Id).ToList();
        var available = await db.Students.Where(s => !enrolledIds.Contains(s.Id)).OrderBy(s => s.LastName).ToListAsync();
        ViewBag.AvailableStudents = new SelectList(available, nameof(Student.Id), nameof(Student.FullName));

        return View(course);
    }

    public async Task<IActionResult> Create()
    {
        await LoadTeachersAsync();
        return View(new Course());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Credits,TeacherId")] Course course)
    {
        if (!ModelState.IsValid)
        {
            await LoadTeachersAsync(course.TeacherId);
            return View(course);
        }

        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = course.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return NotFound();

        await LoadTeachersAsync(course.TeacherId);
        return View(course);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Credits,TeacherId")] Course course)
    {
        if (id != course.Id || !await db.Courses.AnyAsync(c => c.Id == id)) return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadTeachersAsync(course.TeacherId);
            return View(course);
        }

        db.Update(course);   // записи студентов на курс не затрагиваются
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    // Каскад: вместе с курсом удаляются записи студентов на него.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is not null)
        {
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Записать студента на курс
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int id, int studentId)
    {
        var course = await db.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == id);
        var student = await db.Students.FindAsync(studentId);
        if (course is null || student is null) return NotFound();

        if (course.Students.All(s => s.Id != studentId))
        {
            course.Students.Add(student);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // Отчислить студента с курса
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unenroll(int id, int studentId)
    {
        var course = await db.Courses.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == id);
        if (course is null) return NotFound();

        var student = course.Students.FirstOrDefault(s => s.Id == studentId);
        if (student is not null)
        {
            course.Students.Remove(student);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadTeachersAsync(int? selectedId = null)
    {
        var teachers = await db.Teachers.OrderBy(t => t.LastName).ToListAsync();
        ViewBag.Teachers = new SelectList(teachers, nameof(Teacher.Id), nameof(Teacher.FullName), selectedId);
    }
}
