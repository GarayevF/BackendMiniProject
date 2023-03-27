using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DataAccessLayer;
using Pustok.Interfaces;
using Pustok.Models;
using Pustok.ViewModels.BasketViewModels;
using Pustok.ViewModels.WishlistViewModels;

namespace Pustok.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<List<BasketVM>> GetBaskets()
        {
            AppUser appUser = null;
            List<Basket> baskets = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated &&
                _httpContextAccessor.HttpContext.User.IsInRole("Member"))
            {
                appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                    .FirstOrDefaultAsync(u => u.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

                baskets = appUser.Baskets;
            }

            string cookie = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<BasketVM> basketVMs = null;
                if (baskets != null && baskets.Count > 0)
                {
                    basketVMs = new List<BasketVM>();
                    foreach (Basket basket in baskets)
                    {
                        Product product = basket.Product;

                        if (product != null)
                        {
                            BasketVM basketVM = new BasketVM();

                            basketVM.Id = product.Id;
                            basketVM.Count = product.Count;
                            basketVM.Title = product.Title;
                            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM.Image = product.MainImage;
                            basketVM.ExTax = product.ExTax;

                            basketVMs.Add(basketVM);
                        }
                    }
                }
                else
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                    foreach (BasketVM basketVM1 in basketVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM1.Id);

                        if (product != null)
                        {
                            basketVM1.Title = product.Title;
                            basketVM1.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM1.Image = product.MainImage;
                            basketVM1.ExTax = product.ExTax;
                        }
                    }
                }

                return basketVMs;
            }

            return new List<BasketVM>();
        }

		public async Task<List<WishlistVM>> GetWishLists()
		{
			AppUser appUser = null;
			List<Wishlist> wishlists = null;
			if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated &&
				_httpContextAccessor.HttpContext.User.IsInRole("Member"))
			{
				appUser = await _userManager.Users
					.Include(u => u.Wishlists.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
					.FirstOrDefaultAsync(u => u.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

				wishlists = appUser.Wishlists;
			}

			string cookie = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];

			if (!string.IsNullOrWhiteSpace(cookie))
			{
				List<WishlistVM> wishlistVMs = null;
				if (wishlists != null && wishlists.Count > 0)
				{
					wishlistVMs = new List<WishlistVM>();
					foreach (Wishlist wishlist in wishlists)
					{
						Product product = wishlist.Product;

						if (product != null)
						{
							WishlistVM wishlistVM = new WishlistVM();

							wishlistVM.Id = product.Id;
							wishlistVM.Title = product.Title;
							wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
							wishlistVM.Image = product.MainImage;
							wishlistVM.ExTax = product.ExTax;

							wishlistVMs.Add(wishlistVM);
						}
					}
				}
				else
				{
					wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);
					foreach (WishlistVM wishlistVM in wishlistVMs)
					{
						Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishlistVM.Id);

						if (product != null)
						{
							wishlistVM.Title = product.Title;
							wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
							wishlistVM.Image = product.MainImage;
							wishlistVM.ExTax = product.ExTax;
						}
					}
				}

				return wishlistVMs;
			}

			return new List<WishlistVM>();
		}

		public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Children).Where(c => c.IsDeleted == false)
                .Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}
