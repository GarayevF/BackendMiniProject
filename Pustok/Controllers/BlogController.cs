using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DataAccessLayer;
using Pustok.Models;
using Pustok.ViewModels;
using Pustok.ViewModels.BlogViewModels;
using Pustok.ViewModels.ProductViewModels;
using System.Linq;

namespace Pustok.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            IEnumerable<Blog>? blogs = await _context.Blogs.Where(b => b.IsDeleted == false)
                .Include(b => b.BlogImages.Where(bi => bi.IsDeleted == false))
                .ToListAsync();

            List<Tag>? tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            BlogVM blogVM = new BlogVM
            {
                Blogs = blogs,
                Tags = tags,
                RecentBlogs = blogs.OrderByDescending(a => a.CreatedAt).Take(5)
            };
            
            return View(blogVM);
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = _context.Blogs
                .Include(b => b.BlogImages.Where(bi => bi.IsDeleted == false))
                .Include(b => b.BlogReviews)
                .FirstOrDefault(b => b.Id == id && b.IsDeleted == false);

            if(blog == null) return NotFound();

            BlogDetailVM blogDetailVM = new BlogDetailVM
            {
                Blog = blog
            };

            return View(blogDetailVM);
        }

        public async Task<IActionResult> Filter(int? tagId)
        {

            List<Blog> blogs = await _context.Blogs
                        .Include(p => p.BlogImages.Where(a => a.IsDeleted == false))
                        .Include(p => p.BlogTags.Where(a => a.IsDeleted == false))
                        .ThenInclude(a => a.Tag).Where(a => a.IsDeleted == false)
                        .ToListAsync();

            List<Tag>? tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();
            
            List<Blog>? newProducts = blogs;

            if (tagId != null && tagId > 0)
            {
                List<Blog>? tempList = new List<Blog>();

                tempList.AddRange(blogs?.Where(p => p.BlogTags.Any(pa => pa.TagId == tagId)).ToList());

                newProducts = newProducts.Where(p => tempList.Contains(p)).ToList();
            }

            //BlogVM blogVM = new BlogVM
            //{
            //    Blogs = newProducts,
            //    Tags = tags,
            //    RecentBlogs = blogs.OrderByDescending(a => a.CreatedAt).Take(5)
            //};

            return PartialView("_BlogListPartial", blogs);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReview(BlogReview review)
        {
            if (review.BlogId == null) return BadRequest();

            Blog blog = await _context.Blogs
                .Include(p => p.BlogReviews.Where(p => p.IsDeleted == false)).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.BlogId);

            if (blog == null) return NotFound();

            if (blog.BlogReviews.Any(r => r.User.UserName == User.Identity.Name)) return BadRequest();

            if (!ModelState.IsValid)
            {
                BlogDetailVM detailVM = new BlogDetailVM
                {
                    Blog = blog,
                };

                return View("Detail", detailVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.UserId = appUser.Id;

            await _context.BlogReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new { id = review.BlogId });


        }
    }
}
