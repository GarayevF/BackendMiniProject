using Pustok.Models;

namespace Pustok.ViewModels.AccountViewModels
{
    public class ProfileVM
    {
        public IEnumerable<Address>? Addresses { get; set; }
        public Address? Address { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public AccountVM AccountVM { get; set; }
    }
}
