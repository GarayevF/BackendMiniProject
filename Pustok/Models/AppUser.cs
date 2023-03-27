using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        public bool IsActive { get; set; }
        public List<Basket>? Baskets { get; set; }
        public List<Wishlist>? Wishlists { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
