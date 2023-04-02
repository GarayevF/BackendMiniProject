using Pustok.DataAccessLayer;
using Pustok.Extensions;
using Pustok.Helpers;
using Pustok.Models;
using Pustok.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductAuthors.Where(pt => pt.IsDeleted == false))
                .ThenInclude(pt => pt.Author)
                .Where(p => p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(queries, pageIndex, 3, 5));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Category Mutleq secilmelidir");
                return View(product);
            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun category secin");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContenttype("image.jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli fayl novu duzgun deyil");
                    return View(product);
                }

                if (product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli fayl hecmi coxdur");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("MainFile", "Mainfile mutleqdir");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContenttype("image.jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli fayl novu duzgun deyil");
                    return View(product);
                }

                if (product.HoverFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli fayl hecmi coxdur");
                    return View(product);
                }

                product.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("HoverFile", "HoverFile mutleqdir");
                return View(product);
            }

            //Many-to-Many Create
            if (product.AuthorIds != null && product.AuthorIds.Count() > 0)
            {
                List<ProductAuthor> productAuthors = new List<ProductAuthor>();

                foreach (int authorId in product.AuthorIds)
                {
                    if (!await _context.Authors.AnyAsync(c => c.IsDeleted == false && c.Id == authorId))
                    {
                        ModelState.AddModelError("AuthorIds", $"{authorId} id deyeri yanlisdir");
                        return View(product);
                    }

                    ProductAuthor productAuthor = new ProductAuthor
                    {
                        AuthorId = authorId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productAuthors.Add(productAuthor);
                }

                product.ProductAuthors = productAuthors;

            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                ModelState.AddModelError("Files", "maksimum 6 sekil yukleye bilersiniz");
                return View(product);
            }

            //Multi file upload
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContenttype("image.jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli fayl novu duzgun deyil");
                        return View(product);
                    }

                    if (file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli fayl hecmi coxdur");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "images", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }

                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("Files", "Sekil mutleq secilmelidir");
                return View(product);
            }

            string seria = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);
            seria += _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);
            seria = seria.ToLower();

            int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            product.Seria = seria;
            product.Code = code;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = "System";


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductAuthors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

            product.AuthorIds = product.ProductAuthors?.Select(x => x.AuthorId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (id == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(p => p.ProductAuthors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbproduct == null) return NotFound();

            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Category Mutleq secilmelidir");
                return View(product);
            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun category secin");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContenttype("image.jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli fayl novu duzgun deyil");
                    return View(product);
                }

                if (product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli fayl hecmi coxdur");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.MainImage, _env, "assets", "images", "products");

                dbproduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContenttype("image.jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli fayl novu duzgun deyil");
                    return View(product);
                }

                if (product.HoverFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli fayl hecmi coxdur");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.HoverImage, _env, "assets", "images", "products");

                dbproduct.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.AuthorIds != null && product.AuthorIds.Count() > 0)
            {

                _context.ProductAuthors.RemoveRange(dbproduct.ProductAuthors);

                List<ProductAuthor> productAuthors = new List<ProductAuthor>();

                foreach (int authorId in product.AuthorIds)
                {
                    if (!await _context.Authors.AnyAsync(c => c.IsDeleted == false && c.Id == authorId))
                    {
                        ModelState.AddModelError("AuthorIds", $"{authorId} id deyeri yanlisdir");
                        return View(product);
                    }

                    ProductAuthor productAuthor = new ProductAuthor
                    {
                        AuthorId = authorId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productAuthors.Add(productAuthor);
                }

                dbproduct.ProductAuthors.AddRange(productAuthors);

            }

            int canUpload = 6 - (product.ProductImages != null ? product.ProductImages.Count() : 0);

            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} qeder fayl upload edebilersiniz");
                dbproduct.AuthorIds = product.AuthorIds;
                return View(dbproduct);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContenttype("image.jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli fayl novu duzgun deyil");
                        return View(product);
                    }

                    if (file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli fayl hecmi coxdur");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "images", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }

                dbproduct.ProductImages.AddRange(productImages);
            }

            dbproduct.Title = product.Title;
            dbproduct.Description = product.Description;
            dbproduct.Price = product.Price;
            dbproduct.DiscountedPrice = product.DiscountedPrice;
            dbproduct.ExTax = product.ExTax;
            dbproduct.Count = product.Count;
            dbproduct.IsMostViewed = product.IsMostViewed;
            dbproduct.IsNewArrival = product.IsNewArrival;
            dbproduct.IsFeatured = product.IsFeatured;

            dbproduct.UpdatedBy = "System";
            dbproduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (!product.ProductImages.Any(pi => pi.Id == imageId)) return BadRequest();

            if (product.ProductImages.Count <= 1)
            {
                return BadRequest();
            }

            product.ProductImages.FirstOrDefault(p => p.Id == imageId).IsDeleted = true;
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedBy = "System";
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages?.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "images", "products");

            return PartialView("_ProductImagePartial", product.ProductImages.Where(pi => pi.IsDeleted == false).ToList());
        }


    }
}
