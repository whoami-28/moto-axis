using FarshGenerator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarshGenerator.Models.ViewModels
{
    public class BrowseViewModel
    {
        public string? SearchQuery { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? MinDisplacement { get; set; }
        public int? MaxDisplacement { get; set; }
        public decimal? MinPower { get; set; }
        public int? MaxMileage { get; set; }
        public string? DriveType { get; set; }
        public string? Transmission { get; set; }

        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }

        public IEnumerable<Listing> Listings { get; set; } = new List<Listing>();
        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}