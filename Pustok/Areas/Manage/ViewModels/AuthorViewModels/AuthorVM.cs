using Pustok.Models;

namespace Pustok.Areas.Manage.ViewModels.AuthorViewModels
{
    public class AuthorVM
    {
        public IEnumerable<Product>? Products { get; set; }
        public Author Author { get; set; }
    }
}
