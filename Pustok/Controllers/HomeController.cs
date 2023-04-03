using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.ViewModels.HomeViewModels;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //ViewBag.Authors = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync(),
                MostViewed = await _context.Products.Where(c => c.IsDeleted == false && c.IsMostViewed)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false).ToListAsync(),
                NewArrival = await _context.Products.Where(c => c.IsDeleted == false && c.IsNewArrival)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false).ToListAsync(),
                Featured = await _context.Products.Where(c => c.IsDeleted == false && c.IsFeatured)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false).ToListAsync(),
                Deals = await _context.Deals.Where(c => c.IsDeleted == false).ToListAsync(),
                BestSellers = await _context.Products.Where(c => c.IsDeleted == false)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
                .OrderByDescending(a => a.TotalSold).ToListAsync(),
                ChildrensBook = await _context.Products.Where(c => c.IsDeleted == false && c.Category.ParentId == 7)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false).ToListAsync(),
                ArtPhotography = await _context.Products.Where(c => c.IsDeleted == false && c.Category.ParentId == 10)
                .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false).ToListAsync(),
            };

            return View(homeVM);
        }
    }
}
