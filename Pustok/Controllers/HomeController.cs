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
                MostViewed = await _context.Products.Where(c => c.IsDeleted == false && c.IsMostViewed).ToListAsync(),
                NewArrival = await _context.Products.Where(c => c.IsDeleted == false && c.IsNewArrival).ToListAsync(),
                Featured = await _context.Products.Where(c => c.IsDeleted == false && c.IsFeatured).ToListAsync(),
                Deals = await _context.Deals.Where(c => c.IsDeleted == false).ToListAsync()
            };

            return View(homeVM);
        }
    }
}
