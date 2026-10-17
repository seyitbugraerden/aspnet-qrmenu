using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnet_qrmenu.Models;
using aspnet_qrmenu.Data;
using Microsoft.EntityFrameworkCore;

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
}
