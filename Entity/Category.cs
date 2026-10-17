using aspnet_qrmenu.Entity;

namespace aspnet_qrmenu.Entity;

public class Category
{
    public int Id { get; set; }
    public required string CategoryName { get; set; }
    public required string CategoryImage { get; set; }
    public required string Slug { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
