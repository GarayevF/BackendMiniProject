using Pustok.Models;
using Pustok.ViewModels.BasketViewModels;
using Pustok.ViewModels.CompareViewModels;
using Pustok.ViewModels.WishlistViewModels;

namespace Pustok.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBaskets();
        Task<List<WishlistVM>> GetWishLists();
        Task<List<CompareVM>> GetCompares();
    }
}
