using Microsoft.EntityFrameworkCore;
using KT14.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// SQLite — работает без установки сервера БД.
// Для SQL Server замените на: options.UseSqlServer(...) и пакет Microsoft.EntityFrameworkCore.SqlServer.
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchoolContext>();
    db.Database.EnsureCreated();   // при переходе на миграции замените на db.Database.Migrate()
    SeedData.Initialize(db);
}

app.UseRouting();
app.MapControllerRoute("default", "{controller=Courses}/{action=Index}/{id?}");

app.Run();
