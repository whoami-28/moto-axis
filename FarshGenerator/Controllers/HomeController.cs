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
            ViewBag.Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();
            ViewBag.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();

            var featuredListings = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .Where(l => l.Status == "Active")
                .OrderByDescending(l => l.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(featuredListings);
        }

        [HttpGet]
        public async Task<IActionResult> Browse(BrowseViewModel filters)
        {
            var query = _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .Where(l => l.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchQuery))
            {
                query = query.Where(l => l.Title.Contains(filters.SearchQuery) ||
                                         l.Spec.Model.Contains(filters.SearchQuery) ||
                                         l.Description.Contains(filters.SearchQuery));
            }

            if (filters.BrandId.HasValue)
                query = query.Where(l => l.Spec.BrandId == filters.BrandId.Value);

            if (filters.CategoryId.HasValue)
                query = query.Where(l => l.Spec.CategoryId == filters.CategoryId.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(l => l.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(l => l.Price <= filters.MaxPrice.Value);

            if (filters.MinYear.HasValue)
                query = query.Where(l => l.Spec.Year >= filters.MinYear.Value);

            if (filters.MaxYear.HasValue)
                query = query.Where(l => l.Spec.Year <= filters.MaxYear.Value);

            if (filters.MinDisplacement.HasValue)
                query = query.Where(l => l.Spec.EngineDisplacement >= filters.MinDisplacement.Value);

            if (filters.MaxDisplacement.HasValue)
                query = query.Where(l => l.Spec.EngineDisplacement <= filters.MaxDisplacement.Value);

            if (filters.MinPower.HasValue)
                query = query.Where(l => l.Spec.EnginePower >= filters.MinPower.Value);

            if (filters.MaxMileage.HasValue)
                query = query.Where(l => l.Spec.Mileage <= filters.MaxMileage.Value);

            if (!string.IsNullOrEmpty(filters.DriveType))
                query = query.Where(l => l.Spec.DriveType == filters.DriveType);

            if (!string.IsNullOrEmpty(filters.Transmission))
                query = query.Where(l => l.Spec.Transmission == filters.Transmission);

            int totalItems = await query.CountAsync();
            int pageSize = 6;

            if (filters.PageNumber < 1) filters.PageNumber = 1;
            filters.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            filters.Listings = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((filters.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            filters.Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();
            filters.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();

            return View(filters);
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
        public async Task<IActionResult> Sell(SellViewModel viewModel)
        {
            if (viewModel.ImageUpload != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(viewModel.ImageUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageUpload", "Допустимы только файлы форматов .jpg, .jpeg, .png");
                }

                if (viewModel.ImageUpload.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageUpload", "Максимальный размер изображения не должен превышать 5 МБ");
                }
            }

            if (!ModelState.IsValid)
            {
                viewModel.Brands = await _context.Brands.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();
                viewModel.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();
                return View(viewModel);
            }

            var sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var spec = new MotorcycleSpec
            {
                BrandId = viewModel.BrandId,
                CategoryId = viewModel.CategoryId,
                Model = viewModel.BikeModel,
                Year = viewModel.Year,
                EngineDisplacement = viewModel.EngineDisplacement,
                EnginePower = viewModel.EnginePower,
                Mileage = viewModel.Mileage,
                Transmission = viewModel.Transmission,
                DriveType = viewModel.DriveType,
                Condition = viewModel.Mileage == 0 ? "New" : "Used"
            };

            _context.MotorcycleSpecs.Add(spec);
            await _context.SaveChangesAsync();

            var listing = new Listing
            {
                SellerId = sellerId,
                SpecId = spec.Id,
                Title = $"Продается {viewModel.BikeModel}",
                Description = viewModel.Description,
                Price = viewModel.Price,
                Currency = "BYN",
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();

            string imageUrl = "https://images.unsplash.com/photo-1558981403-c5f9899a28bc?w=800&q=80";

            if (viewModel.ImageUpload != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "listings");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageUpload.CopyToAsync(fileStream);
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

        public async Task<IActionResult> SellerProfile(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var listings = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .Where(l => l.SellerId == id && l.Status == "Active")
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            ViewBag.SellerName = user.Username;
            return View(listings);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}