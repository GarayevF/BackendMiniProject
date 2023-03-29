using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;
using Pustok.ViewModels.ProductViewModels;

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

        public async Task<IActionResult> Index()
        {
            int pageIndex = 1;

			IQueryable<Product> products = _context.Products
                        .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                        .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
                        .Include(p => p.Category).Where(a => a.IsDeleted == false);

            IEnumerable<Category> categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain)
                .Include(c => c.Children.Where(ct => ct.IsDeleted == false && ct.IsMain == false))
                .Include(c => c.Products).ToListAsync();

            IEnumerable<Author> authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

            ProductVM productVM = new ProductVM
            {
                Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                Categories = categories,
                Authors = authors,
                AllProducts = products/*.ToList()*/
            };

            return View(productVM);
        }

		public async Task<IActionResult> ChangePage(int pageIndex = 1)
		{
			IQueryable<Product> products = _context.Products
						.Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
						.ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
						.Include(p => p.Category).Where(a => a.IsDeleted == false);

			IEnumerable<Category> categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain)
				.Include(c => c.Children.Where(ct => ct.IsDeleted == false && ct.IsMain == false))
				.Include(c => c.Products).ToListAsync();

			IEnumerable<Author> authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

            ProductVM productVM = new ProductVM
            {
                Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                Categories = categories,
                Authors = authors,
                AllProducts = products/*.ToList()*/
			};

			return PartialView("_ShopPaginationPartial", productVM);
		}
	}
}
