using GameZone.Data;
using GameZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using static GameZone.Constants.ModelConstants.Game;

namespace GameZone.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly GameZoneDbContext context;

        public GameController(GameZoneDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new GameViewModel();
            model.Genres = await context.Genres
                .Select(g => new Genre
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            DateTime releasedOn;

            if (!DateTime.TryParseExact(model.ReleasedOn, ReleasedOnFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out releasedOn))
            {
                ModelState.AddModelError(nameof(model.ReleasedOn), "Invalid date format.");

                return View(model);
            }

            Game game = new Game
            {
                Title = model.Title,
                ImageUrl = model.ImageUrl,
                Description = model.Description,
                ReleasedOn = releasedOn,
                GenreId = model.GenreId,
                PublisherId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await context.Games.AddAsync(game);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await context.Games
                .Select(g => new GameInfoViewModel
                {
                    Id = g.Id,
                    Title = g.Title,
                    ImageUrl = g.ImageUrl,
                    Genre = g.Genre.Name,
                    ReleasedOn = g.ReleasedOn.ToString(ReleasedOnFormat),
                    Publisher = g.Publisher.UserName ?? string.Empty
                })
                .AsNoTracking() 
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new GameViewModel();
            // Get the game from the database

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update the game in the database

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MyZone()
        {
            return View(new List<GameInfoViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> AddToMyZone(int id)
        {
            // Add the game to the user's zone

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StrikeOut(int id)
        {
            // Add the game to the user's zone

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Delete the game from the database

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the game from the database

            return View();
        }
    }
}
