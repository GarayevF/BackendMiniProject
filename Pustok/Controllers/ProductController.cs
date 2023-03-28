using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Product> products = _context.Products
                        .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                        .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
                        .Include(p => p.Category).Where(a => a.IsDeleted == false);

            IEnumerable<Category> categories = _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false);


			return View(PageNatedList<Product>.Create(products, pageIndex, 12, 7));
        }
    }
}
