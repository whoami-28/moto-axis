using FarshGenerator.Models;
using FarshGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace FarshGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var featuredListings = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .OrderByDescending(l => l.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(featuredListings);
        }

        public async Task<IActionResult> Browse()
        {
            var allListings = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return View(allListings);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Sell()
        {
            var viewModel = new SellViewModel
            {
                Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync(),
                Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Sell(SellViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();
                model.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();
                return View(model);
            }

            var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var spec = new MotorcycleSpec
            {
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                Model = model.Model,
                Year = model.Year,
                EngineDisplacement = model.EngineDisplacement,
                Mileage = 0,
                Condition = "Used",
                EnginePower = model.EngineDisplacement * 0.15m,
                Transmission = "Manual",
                DriveType = "Chain"
            };

            _context.MotorcycleSpecs.Add(spec);
            await _context.SaveChangesAsync();

            var listing = new Listing
            {
                SellerId = sellerId,
                SpecId = spec.Id,
                Title = $"Продается {model.Model}",
                Description = model.Description,
                Price = model.Price,
                Currency = "USD",
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();

            string imageUrl = "https://images.unsplash.com/photo-1558981403-c5f9899a28bc?w=800&q=80";

            if (model.ImageUpload != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "listings");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUpload.CopyToAsync(fileStream);
                }
                imageUrl = "/images/listings/" + uniqueFileName;
            }

            var defaultImage = new ListingImage
            {
                ListingId = listing.Id,
                ImageUrl = imageUrl,
                IsMain = true,
                UploadedAt = DateTime.Now
            };

            _context.ListingImages.Add(defaultImage);
            await _context.SaveChangesAsync();

            return RedirectToAction("TechSpecs", new { id = listing.Id });
        }

        public async Task<IActionResult> TechSpecs(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return NotFound();

            return View(listing);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}