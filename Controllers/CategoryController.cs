using System;
using System.Collections.Generic;
using System.Linq;
using aspnet_qrmenu.Models;
using aspnet_qrmenu.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qrmenu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.AsNoTracking().OrderBy(c => c.CategoryName).Select(c => new
            {
                c.Id,
                c.CategoryName,
                c.Slug
            }).ToListAsync();
            return Ok(categories);
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<CategoryDetailDto>> GetCategory(string slug)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Slug == slug) // '=' yerine '==' kullanıldı
                .Select(c => new CategoryDetailDto
                {
                    Id = c.Id,
                    CategoryImage = c.CategoryImage,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    Products = c.Products
                        .OrderBy(p => p.ProductName)
                        .Select(p => new ProductDto
                        {
                            Id = p.Id,
                            ProductName = p.ProductName,
                            Image = p.Image,
                            Price = p.Price
                        })
                        .ToList() // Projeksiyon içinde ToList() olmadan da EF Core liste olarak doldurur
                })
                .FirstOrDefaultAsync(); // Sorguyu çalıştıran asenkron metod eklendi ve ';' koyuldu

            if (category is null)
            {
                return NotFound();
            }

            return Ok(category); // Satır sonuna ';' eklendi
        }

    }
}