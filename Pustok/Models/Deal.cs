namespace Pustok.Models
{
    public class Deal : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

    }
}
