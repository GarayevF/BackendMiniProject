using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class Blog : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        
        [StringLength(255)]
        public string Title { get; set; }
        public string Desc { get; set; }
		public List<BlogTag>? BlogTags { get; set; }
		[NotMapped]
		public List<int>? TagIds { get; set; }
		public List<BlogReview>? BlogReviews { get; set; }
        public List<BlogImage>? BlogImages { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
