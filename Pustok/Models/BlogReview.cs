using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class BlogReview : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public int? BlogId { get; set; }
        public Blog? Blog { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
