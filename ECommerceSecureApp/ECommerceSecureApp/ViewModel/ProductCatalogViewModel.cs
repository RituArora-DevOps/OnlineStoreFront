using ECommerceSecureApp.Models;
using ECommerceSecureApp.Models.Utility;

namespace ECommerceSecureApp.ViewModel
{
    public class ProductCatalogViewModel
    {
        public PagedResult<Product> PagedProducts { get; set; } = new();
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
