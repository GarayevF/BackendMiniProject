using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Author : BaseEntity
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? MiddleName { get; set; }
        [StringLength(100)]
        public string? Surname { get; set; }
    }
}
