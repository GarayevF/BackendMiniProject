using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels.BasketViewModels;
using Pustok.ViewModels.CompareViewModels;
using Pustok.ViewModels.WishlistViewModels;

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
                    Product product = await _context.Products
                        .Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
                        .ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
                        .Include(p => p.Category).Where(a => a.IsDeleted == false)
                        .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

                    if (product != null)
                    {
                        compareVM.Title = product.Title;
                        compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        compareVM.Image = product.MainImage;
                        compareVM.ExTax = product.ExTax;
                        //foreach (int AuthorId in product.AuthorIds)
                        //{
                        //    Author author = await _context.Authors.FirstOrDefaultAsync(a => a.IsDeleted == false && a.Id == AuthorId);
                        //    string fullName = "";
                        //    if(author.Name != null)
                        //    {
                        //        fullName += author.Name;
                        //    }
                        //    if (author.MiddleName != null)
                        //    {
                        //        fullName += " " + author.MiddleName;
                        //    }
                        //    if (author.Surname != null)
                        //    {
                        //        fullName += " " + author.Surname;
                        //    }

                        //    compareVM.Authors.Add(fullName);
                        //}
                        compareVM.ProductAuthors = product.ProductAuthors;
                        compareVM.Category = product.Category;
                    }
                }
            }

            return View(compareVMs);
        }

        public async Task<IActionResult> AddCompare(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string cookie = HttpContext.Request.Cookies["compare"];

            List<CompareVM> compareVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                compareVMs = new List<CompareVM>()
                {
                    new CompareVM
                    {
                        Id = (int)id,
                    }
                };
            }
            else
            {
                compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

                if(!(compareVMs.Count() < 3))
                {
                    //toastr error mesaji elave ele ki 3 dene mehsul var uje comparede
                    return Ok();
                }

                if (!compareVMs.Exists(p => p.Id == id))
                {
                    compareVMs.Add(new CompareVM { Id = (int)id });
                }

            }

            cookie = JsonConvert.SerializeObject(compareVMs);
            HttpContext.Response.Cookies.Append("compare", cookie);

            //foreach (CompareVM compareVM in compareVMs)
            //{
            //    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

            //    if (product != null)
            //    {
            //        compareVM.Title = product.Title;
            //        compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            //        compareVM.Image = product.MainImage;
            //        compareVM.ExTax = product.ExTax;
            //        compareVM.IsAvailable = (product.Count > 0);
            //    }
            //}

            //succes toastr mesaji elave et
            return Ok();
        }

        public async Task<IActionResult> DeleteCompare(int? id)
        {
            return Ok();
        }
    }
}
