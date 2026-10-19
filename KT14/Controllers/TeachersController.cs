using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KT14.Data;
using KT14.Models;

namespace KT14.Controllers;

public class TeachersController(SchoolContext db) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Teachers.Include(t => t.Courses).OrderBy(t => t.LastName).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var teacher = await db.Teachers
            .Include(t => t.Courses).ThenInclude(c => c.Students)
            .FirstOrDefaultAsync(t => t.Id == id);
        return teacher is null ? NotFound() : View(teacher);
    }

    public IActionResult Create() => View(new Teacher());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Department")] Teacher teacher)
    {
        await CheckEmailAsync(teacher);
        if (!ModelState.IsValid) return View(teacher);

        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await db.Teachers.FindAsync(id);
        return teacher is null ? NotFound() : View(teacher);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Department")] Teacher teacher)
    {
        if (id != teacher.Id || !await db.Teachers.AnyAsync(t => t.Id == id)) return NotFound();

        await CheckEmailAsync(teacher);
        if (!ModelState.IsValid) return View(teacher);

        db.Update(teacher);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Каскад: вместе с преподавателем удаляются его курсы и записи студентов на них.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await db.Teachers.FindAsync(id);
        if (teacher is not null)
        {
            db.Teachers.Remove(teacher);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckEmailAsync(Teacher t)
    {
        if (await db.Teachers.AnyAsync(x => x.Email == t.Email && x.Id != t.Id))
            ModelState.AddModelError(nameof(Teacher.Email), "Этот email уже используется");
    }
}
