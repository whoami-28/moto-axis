using FarshGenerator.Models;
using FarshGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FarshGenerator.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userListings = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .Include(l => l.Spec).ThenInclude(s => s.Category)
                .Include(l => l.Images)
                .Where(l => l.SellerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return View(userListings);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listing = await _context.Listings
                .Include(l => l.Spec).ThenInclude(s => s.Brand)
                .FirstOrDefaultAsync(l => l.Id == id && l.SellerId == userId);

            if (listing == null) return NotFound();

            var viewModel = new EditListingViewModel
            {
                Id = listing.Id,
                Price = listing.Price,
                Description = listing.Description,
                Status = listing.Status
            };

            ViewBag.TitleName = $"{listing.Spec.Brand.Name} {listing.Spec.Model}";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditListingViewModel viewModel)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == viewModel.Id && l.SellerId == userId);

            if (listing == null) return NotFound();

            listing.Price = viewModel.Price;
            listing.Description = viewModel.Description;
            listing.Status = viewModel.Status;
            listing.UpdatedAt = DateTime.Now;

            _context.Listings.Update(listing);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listing = await _context.Listings
                .Include(l => l.Images)
                .Include(l => l.Spec)
                .FirstOrDefaultAsync(l => l.Id == id && l.SellerId == userId);

            if (listing == null) return NotFound();

            _context.ListingImages.RemoveRange(listing.Images);
            _context.Listings.Remove(listing);
            _context.MotorcycleSpecs.Remove(listing.Spec);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}