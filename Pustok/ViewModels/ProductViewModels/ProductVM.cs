using Pustok.Models;

namespace Pustok.ViewModels.ProductViewModels
{
	public class ProductVM
	{
        public PageNatedList<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Author>? Authors { get; set; }
        public IEnumerable<Product>? AllProducts { get; set; }
    }
}
