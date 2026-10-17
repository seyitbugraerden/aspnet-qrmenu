using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnet_qrmenu.Models;
using aspnet_qrmenu.Data;
using Microsoft.EntityFrameworkCore;
using aspnet_qrmenu.Entity;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace aspnet_qrmenu.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                CategoryImage = c.CategoryImage,
                Slug = c.Slug
            })
            .ToListAsync();

        return View(categories);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var category = await _context.Categories.AsNoTracking().Where(c => c.Slug == slug).Select(c => new CategoryDetailDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            CategoryImage = c.CategoryImage,
            Slug = c.Slug,
            Products = c.Products.OrderBy(p => p.ProductName).Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Image = p.Image,
                Price = p.Price
            }).ToList()
        }).SingleOrDefaultAsync();
        if (category is null)
        {
            return NotFound();
        }
        return View(category);
    }

}
