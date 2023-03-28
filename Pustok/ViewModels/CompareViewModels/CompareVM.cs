using Pustok.Models;

namespace Pustok.ViewModels.CompareViewModels
{
    public class CompareVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public double ExTax { get; set; }
        public bool IsAvailable { get; set; }
        public List<ProductAuthor>? ProductAuthors { get; set; }
        public Category? Category { get; set; }
    }
}
