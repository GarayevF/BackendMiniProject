using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels.BasketViewModels;
using Pustok.ViewModels.CompareViewModels;

namespace Pustok.Controllers
{
    public class CompareController : Controller
    {
        private readonly AppDbContext _context;

        public CompareController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string cookie = HttpContext.Request.Cookies["compare"];
            List<CompareVM> compareVMs = null;

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                foreach (CompareVM compareVM in compareVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

                    if (product != null)
                    {
                        compareVM.Title = product.Title;
                        compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        compareVM.Image = product.MainImage;
                        compareVM.ExTax = product.ExTax;
                    }
                }
            }

            return View(compareVMs);
        }
    }
}
