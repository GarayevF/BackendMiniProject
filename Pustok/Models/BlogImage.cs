namespace Pustok.Models
{
    public class BlogImage : BaseEntity
    {
        public string Image { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
