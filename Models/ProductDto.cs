namespace aspnet_qrmenu.Models;

public class ProductDto
{
    public int Id { get; set; }
    public required string ProductName { get; set; }
    public required string Image { get; set; }
    public decimal Price { get; set; }
}