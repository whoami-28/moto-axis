using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarshGenerator.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public decimal Rating { get; set; } = 0;
        public int ReviewsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}