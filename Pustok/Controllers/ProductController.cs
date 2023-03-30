using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;
using Pustok.ViewModels.ProductViewModels;
using System.Linq;

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

            Product product = _context.Products.OrderBy(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
            double minValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);

            product = _context.Products.OrderByDescending(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
            double maxValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);

            ProductVM productVM = new ProductVM
            {
                Products = PageNatedList<Product>.Create(products, pageIndex, 12, 7),
                Categories = categories,
                Authors = authors,
                AllProducts = products/*.ToList()*/,
				SortSelect = 0,
				MinimumPrice = minValue,
				MaximumPrice = maxValue
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

		public async Task<IActionResult> Filter(string? categoriesText, string? authorsText, double? min, double? max, int? sortby, int pageIndex = 1)
        {
			List<Product>? newProducts = new List<Product>();
			double minValue = -1;
			double maxValue = -1;
			int sortSelect = -1;

			List<int> categoryIds = new List<int>();

            if (!string.IsNullOrWhiteSpace(categoriesText))
			{
				string[] categoriesTexts = categoriesText.Split(',');
				foreach (string ctg in categoriesTexts)
				{
					if(int.TryParse(ctg, out int res))
					{
						if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == res))
						categoryIds.Add(res);
					}
				}
			}

			List<int>? authorIds = new List<int>();

            if (!string.IsNullOrWhiteSpace(authorsText))
            {
                string[] authorsTexts = authorsText.Split(',');
                foreach (string ctg in authorsTexts)
                {
                    if (int.TryParse(ctg, out int res))
                    {
                        if (await _context.Authors.AnyAsync(c => c.IsDeleted == false && c.Id == res)) ;
                        authorIds.Add(res);
                    }
                }
            }


            List<Product> products = await _context.Products
						.Include(p => p.ProductAuthors.Where(a => a.IsDeleted == false))
						.ThenInclude(pa => pa.Author).Where(a => a.IsDeleted == false)
						.Include(p => p.Reviews.Where(r => r.IsDeleted == false))
						.Include(p => p.Category).Where(a => a.IsDeleted == false).ToListAsync();


			if (categoryIds != null && categoryIds.Count() > 0)
            {
				//products = products.Where(p => (p.Category.IsMain ? categoryIds.Contains((int)p.Category.ParentId) : categoryIds.Contains((int)p.CategoryId)));

				for (int i = 0; i < categoryIds.Count(); i++)
				{
					if (!await _context.Categories.AnyAsync(c => c.Id == categoryIds[i])) continue;

					if (await _context.Categories.AnyAsync(c => c.Id == categoryIds[i] && c.IsMain))
					{
                        newProducts.AddRange(products.Where(p => p.Category.ParentId == categoryIds[i]));
					}
					else
					{
                        newProducts.AddRange(products.Where(p => p.CategoryId == categoryIds[i]));
					}
				}
			}

			if (authorIds != null && authorIds.Count() > 0)
			{
				for (int i = 0; i < authorIds.Count(); i++)
				{
					if (!await _context.Authors.AnyAsync(c => c.Id == authorIds[i])) continue;

                    newProducts.AddRange(products.Where(p => p.ProductAuthors.Any(pa => pa.AuthorId == authorIds[i])));
				}
			}

			if(min != null && min > -1)
			{
                newProducts.AddRange(products.Where(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price) > min));
			}
			else
			{
				Product product = _context.Products.OrderBy(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
				minValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);
			}

			if (max != null && max > -1)
			{
                newProducts.AddRange(products.Where(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price) < max));
			}
			else
			{
				Product product = _context.Products.OrderByDescending(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).First();
				maxValue = (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price);
			}

			if ((categoriesText == null) && (authorsText == null) &&
				!(min != null && min > -1) && !(max != null && max > -1))
			{
				newProducts = products;
			}

			if (sortby != null && sortby > 0)
			{
				
				switch (sortby)
				{
					case 1:
						newProducts = newProducts.OrderBy(p => p.Title).ToList();
						sortSelect = 1;
						break;
					case 2:
						newProducts = newProducts.OrderByDescending(p => p.Title).ToList();
						sortSelect = 2;
						break;
					case 3:
						newProducts = newProducts.OrderBy(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).ToList();
						sortSelect = 3;
						break;
					case 4:
						newProducts = newProducts.OrderByDescending(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price)).ToList();
						sortSelect = 4;
						break;
					case 5:
						newProducts = newProducts.OrderBy(p => p.Reviews?.Average(r => r.Star)).ToList();
						sortSelect = 5;
						break;
					case 6:
						newProducts = newProducts.OrderByDescending(p => p.Reviews?.Average(r => r.Star)).ToList();
						sortSelect = 6;
						break;
				}
			}


			IEnumerable<Category> categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain)
				.Include(c => c.Children.Where(ct => ct.IsDeleted == false && ct.IsMain == false))
				.Include(c => c.Products).ToListAsync();

			IEnumerable<Author> authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

			ProductVM productVM = new ProductVM
			{
				Products = PageNatedList<Product>.Create(newProducts.AsQueryable(), pageIndex, 12, 7),
				Categories = categories,
				Authors = authors,
				AllProducts = products/*.ToList()*/,
				SortSelect = (sortSelect == -1 ? 0 : sortSelect),
				MinimumPrice = min,
				MaximumPrice = max
			};

			return PartialView("_ShopPaginationPartial", productVM);
		}
	}
}
