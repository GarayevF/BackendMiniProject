using Pustok.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.ViewModels.BlogViewModels
{
    public class BlogVM
    {
        public IEnumerable<Blog>? Blogs { get; set; }
        public List<BlogTag>? BlogTags { get; set; }
        [NotMapped]
        public IEnumerable<int>? TagIds { get; set; }
        public IEnumerable<Tag>? Tags { get; set; }
        public IEnumerable<Blog>? RecentBlogs { get; set; }
    }
}
