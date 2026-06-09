using System;
using System.Collections.Generic;

namespace FarshGenerator.Models
{
    public class Listing
    {
        public int Id { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public int SpecId { get; set; }
        public MotorcycleSpec Spec { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "Br";
        public string Status { get; set; } = "Active";
        public string? Location { get; set; }
        public int ViewsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
    }
}