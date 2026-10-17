using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_qrmenu.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public required string CategoryName { get; set; }
        public required string CategoryImage { get; set; }
        public required string Slug { get; set; }
    }
}