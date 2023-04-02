using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Areas.Manage.Controllers
{
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Setting> query = _context.Settings
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Setting>.Create(query, pageIndex, 3, 8));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Setting setting)
        {
            if (!ModelState.IsValid) return View();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu adda {category.Name} category movcuddu");
                return View(category);
            }

            if (category.IsMain)
            {
                if (category.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Uygun type deyil");
                    return View();
                }
                if ((category.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "Size uygun deyil");
                    return View();
                }

                category.Image = await category.File.CreateFileAsync(_env, "assets", "images");
                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }

                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == category.ParentId && c.IsMain))
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }

                category.Image = null;
            }

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
