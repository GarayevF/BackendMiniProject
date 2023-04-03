using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels.AuthorViewModels;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Author> query = _context.Authors.Where(a => a.IsDeleted == false)
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Author>.Create(query, pageIndex, 3, 8));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            IEnumerable<Product> products = await _context.Products
                .Include(p => p.ProductAuthors.Where(pa => pa.IsDeleted == false)).ThenInclude(pa => pa.Author)
                .Where(a => a.IsDeleted == false && a.Id == id)
                .ToListAsync();
                

            Author author = await _context.Authors
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (author == null) return NotFound();

            AuthorVM authorVM = new AuthorVM
            {
                Products = products,
                Author = author,
            };

            return View(authorVM);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) return View();

            if (await _context.Authors.AnyAsync(c => c.IsDeleted == false && 
            c.Name.ToLower() == author.Name.Trim().ToLower() &&
            c.MiddleName.ToLower() == author.MiddleName.Trim().ToLower() &&
            c.Surname.ToLower() == author.Surname.Trim().ToLower()
            
            ))
            {
                ModelState.AddModelError("Name", $"Bu adda {author.Name} {author.MiddleName} {author.Surname} category movcuddu");
                return View(author);
            }

            author.Name = author.Name.Trim();
            author.MiddleName = author.MiddleName.Trim();
            author.Surname = author.Surname.Trim();
            author.CreatedAt = DateTime.UtcNow.AddHours(4);
            author.CreatedBy = "System";

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            Author author = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (author == null) return NotFound();

            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id, Author author)
        {
            if (!ModelState.IsValid) return View(author);

            if (id == null) return BadRequest();

            if (id != author.Id) return BadRequest();

            Author dbAuthor = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (author == null) return NotFound();

            if (await _context.Authors.AnyAsync(c => c.IsDeleted == false &&
            c.Name.ToLower() == author.Name.Trim().ToLower() &&
            c.MiddleName.ToLower() == author.MiddleName.Trim().ToLower() &&
            c.Surname.ToLower() == author.Surname.Trim().ToLower()

            ))
            {
                ModelState.AddModelError("Name", $"Bu adda {author.Name} {author.MiddleName} {author.Surname} author movcuddu");
                return View(author);
            }

            dbAuthor.Name = author.Name.Trim();
            dbAuthor.MiddleName = author.MiddleName.Trim();
            dbAuthor.Surname = author.Surname.Trim();
            dbAuthor.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbAuthor.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Author author = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (author == null) return NotFound();

            author.IsDeleted = true;
            author.DeletedBy = "System";
            author.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
