using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KT14.Data;
using KT14.Models;

namespace KT14.Controllers;

public class StudentsController(SchoolContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Students.Include(s => s.Courses).OrderBy(s => s.LastName).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var student = await db.Students
            .Include(s => s.Courses).ThenInclude(c => c.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);
        return student is null ? NotFound() : View(student);
    }

    public IActionResult Create() => View(new Student { BirthDate = new DateTime(2005, 1, 1) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,BirthDate")] Student student)
    {
        await CheckEmailAsync(student);
        if (!ModelState.IsValid) return View(student);

        db.Students.Add(student);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await db.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,BirthDate")] Student student)
    {
        if (id != student.Id || !await db.Students.AnyAsync(s => s.Id == id)) return NotFound();

        await CheckEmailAsync(student);
        if (!ModelState.IsValid) return View(student);

        db.Update(student);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Каскад: вместе со студентом удаляются его записи на курсы (строки StudentCourses).
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await db.Students.FindAsync(id);
        if (student is not null)
        {
            db.Students.Remove(student);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckEmailAsync(Student s)
    {
        if (await db.Students.AnyAsync(x => x.Email == s.Email && x.Id != s.Id))
            ModelState.AddModelError(nameof(Student.Email), "Этот email уже используется");
    }
}
