using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string MainTitle { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(255)]
        public string Link { get; set; }
        [StringLength(255)]
        public string LinkText { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
    }
}
