namespace FarshGenerator.Models
{
    public class Motorcycle
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Displacement { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Condition { get; set; }
        public string Category { get; set; }
        public double? Power { get; set; }
        public int? Torque { get; set; }
        public int? Weight { get; set; }
    }
}