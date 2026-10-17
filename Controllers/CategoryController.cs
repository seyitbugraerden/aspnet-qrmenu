using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}