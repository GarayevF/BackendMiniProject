using Pustok.Models;
using Pustok.ViewModels.BasketViewModels;

namespace Pustok.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBaskets();
    }
}
