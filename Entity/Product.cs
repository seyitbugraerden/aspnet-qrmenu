
using aspnet_qrmenu.Entity;

namespace aspnet_qrmenu.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public required string Image { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}