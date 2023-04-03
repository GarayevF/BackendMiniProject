using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Extensions;
using Pustok.Helpers;
using Pustok.Models;
using Pustok.ViewModels;

namespace Pustok.Areas.Manage.Controllers
{
	[Area("Manage")]
	public class BlogController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public BlogController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
		{

			IQueryable<Blog>? queries = _context.Blogs
				.Where(p => p.IsDeleted == false);

			return View(PageNatedList<Blog>.Create(queries, pageIndex, 3, 5));
		}

		[HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create()
		{
			ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create(Blog blog)
		{
			ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

			if (!ModelState.IsValid)
			{
				return View(blog);
			}

			if (blog.Files != null && blog.Files.Count() > 6)
			{
				ModelState.AddModelError("Files", "maksimum 6 sekil yukleye bilersiniz");
				return View(blog);
			}

			if (blog.TagIds != null && blog.TagIds.Count() > 0)
			{
				List<BlogTag> blogTags = new List<BlogTag>();

				foreach (int tagId in blog.TagIds)
				{
					if (!await _context.Tags.AnyAsync(c => c.IsDeleted == false && c.Id == tagId))
					{
						ModelState.AddModelError("TagIds", $"{tagId} id deyeri yanlisdir");
						return View(blog);
					}

					BlogTag blogTag = new BlogTag
					{
						TagId = tagId,
						BlogId = blog.Id,
						CreatedAt = DateTime.UtcNow.AddHours(4),
						CreatedBy = "System"
					};

					blogTags.Add(blogTag);
				}

				blog.BlogTags = blogTags;

			}

			//Multi file upload
			if (blog.Files != null && blog.Files.Count() > 0)
			{
				List<BlogImage> blogImages = new List<BlogImage>();

				foreach (IFormFile file in blog.Files)
				{
					if (file.CheckFileContenttype("image/jpeg"))
					{
						ModelState.AddModelError("Files", $"{file.FileName} adli fayl novu duzgun deyil");
						return View(blog);
					}

					if (file.CheckFileLength(300))
					{
						ModelState.AddModelError("Files", $"{file.FileName} adli fayl hecmi coxdur");
						return View(blog);
					}

					BlogImage blogImage = new BlogImage
					{
						Image = await file.CreateFileAsync(_env, "assets", "image", "others"),
						CreatedAt = DateTime.UtcNow.AddHours(4),
						CreatedBy = "System"
					};

					blogImages.Add(blogImage);
				}

				blog.BlogImages = blogImages;
			}
			else
			{
				ModelState.AddModelError("Files", "Sekil mutleq secilmelidir");
				return View(blog);
			}

			blog.CreatedAt = DateTime.UtcNow.AddHours(4);
			blog.CreatedBy = "System";


			await _context.Blogs.AddAsync(blog);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
		{
			if (id == null) return BadRequest();

			Blog blog = await _context.Blogs
				.Include(p => p.BlogTags.Where(pt => pt.IsDeleted == false))
				.Include(p => p.BlogImages.Where(pi => pi.IsDeleted == false))
				.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

			if (blog == null) return NotFound();
			ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

			blog.TagIds = blog.BlogTags?.Select(x => x.TagId).ToList();

			return View(blog);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id, Blog blog)
		{
			ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

			if (!ModelState.IsValid)
			{
				return View(blog);
			}

			if (id == null) return BadRequest();

			if (id != blog.Id) return BadRequest();

			Blog dbblog = await _context.Blogs
				.Include(p => p.BlogTags.Where(pt => pt.IsDeleted == false))
				.Include(p => p.BlogImages.Where(pi => pi.IsDeleted == false))
				.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

			if (dbblog == null) return NotFound();

			if (blog.TagIds != null && blog.TagIds.Count() > 0)
			{

				_context.BlogTags.RemoveRange(dbblog.BlogTags);

				List<BlogTag> blogTags = new List<BlogTag>();

				foreach (int tagId in blog.TagIds)
				{
					if (!await _context.Tags.AnyAsync(c => c.IsDeleted == false && c.Id == tagId))
					{
						ModelState.AddModelError("TagIds", $"{tagId} id deyeri yanlisdir");
						return View(blog);
					}

					BlogTag blogTag = new BlogTag
					{
						TagId = tagId,
						BlogId = blog.Id,
						CreatedAt = DateTime.UtcNow.AddHours(4),
						CreatedBy = "System"
					};

					blogTags.Add(blogTag);
				}

				dbblog.BlogTags.AddRange(blogTags);

			}

			int canUpload = 6 - (blog.BlogImages != null ? blog.BlogImages.Count() : 0);

			if (blog.Files != null && canUpload < blog.Files.Count())
			{
				ModelState.AddModelError("Files", $"Maksimum {canUpload} qeder fayl upload edebilersiniz");
				dbblog.TagIds = blog.TagIds;
				return View(dbblog);
			}

			if (blog.Files != null && blog.Files.Count() > 0)
			{
				List<BlogImage> blogImages = new List<BlogImage>();

				foreach (IFormFile file in blog.Files)
				{
					if (file.CheckFileContenttype("image/jpeg"))
					{
						ModelState.AddModelError("Files", $"{file.FileName} adli fayl novu duzgun deyil");
						return View(blog);
					}

					if (file.CheckFileLength(300))
					{
						ModelState.AddModelError("Files", $"{file.FileName} adli fayl hecmi coxdur");
						return View(blog);
					}

					BlogImage blogImage = new BlogImage
					{
						Image = await file.CreateFileAsync(_env, "assets", "image", "others"),
						CreatedAt = DateTime.UtcNow.AddHours(4),
						CreatedBy = "System"
					};

					blogImages.Add(blogImage);
				}

				dbblog.BlogImages.AddRange(blogImages);
			}

			dbblog.Title = blog.Title;
			dbblog.Desc = blog.Desc;

			dbblog.UpdatedBy = "System";
			dbblog.UpdatedAt = DateTime.UtcNow.AddHours(4);

			await _context.SaveChangesAsync();


			return RedirectToAction(nameof(Index));
		}

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(c => c.Id == id);

            if (blog == null) return NotFound();

            blog.IsDeleted = true;
            blog.DeletedBy = "System";
            blog.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Blog blog = await _context.Blogs
                .Include(p => p.BlogImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (blog == null) return NotFound();

            if (!blog.BlogImages.Any(pi => pi.Id == imageId)) return BadRequest();

            if (blog.BlogImages.Count <= 1)
            {
                return BadRequest();
            }

            blog.BlogImages.FirstOrDefault(p => p.Id == imageId).IsDeleted = true;
            blog.BlogImages.FirstOrDefault(p => p.Id == imageId).DeletedBy = "System";
            blog.BlogImages.FirstOrDefault(p => p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(blog.BlogImages.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "image", "others");

            return PartialView("_ProductImagePartial", blog.BlogImages.Where(pi => pi.IsDeleted == false).ToList());
        }

    }
}
