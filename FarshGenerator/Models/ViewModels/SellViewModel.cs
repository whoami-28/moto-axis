using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FarshGenerator.Models.ViewModels
{
    public class SellViewModel
    {
        [Required(ErrorMessage = "Выберите марку")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Выберите класс мотоцикла")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Укажите модель")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина модели должна быть от 2 до 50 символов")]
        public string BikeModel { get; set; }

        [Required(ErrorMessage = "Укажите год выпуска")]
        [Range(1900, 2027, ErrorMessage = "Некорректный год выпуска")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Укажите объем двигателя")]
        [Range(1, 3000, ErrorMessage = "Объем двигателя должен быть от 1 до 3000 куб.см.")]
        public int EngineDisplacement { get; set; }

        [Required(ErrorMessage = "Укажите мощность")]
        [Range(1, 500, ErrorMessage = "Мощность должна быть от 1 до 500 л.с.")]
        public decimal EnginePower { get; set; }

        [Required(ErrorMessage = "Укажите пробег")]
        [Range(0, 1000000, ErrorMessage = "Пробег не может быть отрицательным")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Выберите тип трансмиссии")]
        public string Transmission { get; set; }

        [Required(ErrorMessage = "Выберите тип привода")]
        public string DriveType { get; set; }

        [Required(ErrorMessage = "Укажите цену")]
        [Range(1, 1000000, ErrorMessage = "Цена должна быть от 1 BYN до 1 000 000 BYN")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Добавьте описание")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Описание должно быть от 10 до 2000 символов")]
        public string Description { get; set; }

        public IFormFile? ImageUpload { get; set; }

        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}