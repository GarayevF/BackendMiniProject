using Pustok.Models;

namespace Pustok.ViewModels.OrderViewModels
{
	public class OrderVM
	{
		public Order Order { get; set; }
		public IEnumerable<Basket> Baskets { get; set; }
	}
}
