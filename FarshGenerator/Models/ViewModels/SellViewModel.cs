using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarshGenerator.Models.ViewModels
{
    public class SellViewModel
    {
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int EngineDisplacement { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public IFormFile? ImageUpload { get; set; }

        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}