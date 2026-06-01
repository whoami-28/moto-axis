namespace FarshGenerator.Models
{
    public class MotorcycleSpec
    {
        public int Id { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Model { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string Condition { get; set; }

        public int? EngineDisplacement { get; set; }
        public decimal? EnginePower { get; set; }
        public string? EngineType { get; set; }
        public string? Transmission { get; set; }
        public string? DriveType { get; set; }
        public string? Color { get; set; }
        public string? VIN { get; set; }

        public Listing? Listing { get; set; }
    }
}