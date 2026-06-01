using System;

namespace FarshGenerator.Models
{
    public class ListingImage
    {
        public int Id { get; set; }

        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        public string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}