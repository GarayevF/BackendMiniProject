namespace Pustok.Models
{
    public class ProductAuthor : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
