using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public string FullName
        {
            get
            {
                var nameParts = new List<string>();
                if (!string.IsNullOrEmpty(Name))
                {
                    nameParts.Add(Name);
                }
                if (!string.IsNullOrEmpty(MiddleName))
                {
                    nameParts.Add(MiddleName);
                }
                if (!string.IsNullOrEmpty(Surname))
                {
                    nameParts.Add(Surname);
                }
                return string.Join(" ", nameParts).Trim();
            }
        }
    }
}
