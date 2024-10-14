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
            model.Genres = await GetGenres();

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
                PublisherId = GetCurrentUserId() ?? string.Empty
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
            var model = await context.Games
                .Where(g => g.Id == id)
                .AsNoTracking()
                .Select(g => new GameViewModel
                {
                    Title = g.Title,
                    ImageUrl = g.ImageUrl,
                    Description = g.Description,
                    ReleasedOn = g.ReleasedOn.ToString(ReleasedOnFormat),
                    GenreId = g.GenreId
                })
                .FirstOrDefaultAsync();

            model.Genres = await GetGenres();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GameViewModel model, int id)
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

            Game? game = await context.Games.FindAsync(id);

            game.Title = model.Title;
            game.ImageUrl = model.ImageUrl;
            game.Description = model.Description;
            game.ReleasedOn = releasedOn;
            game.GenreId = model.GenreId;
           

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> MyZone()
        {
            string CurrentUserId = GetCurrentUserId() ?? string.Empty;

            var model = await context.Games
                .Where(gg => gg.GamersGames.Any(g => g.GamerId == CurrentUserId))
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
        public async Task<IActionResult> StrikeOut(int id)
        {
            Game? game = await context.Games
                .Where(g => g.Id == id)
                .Include(g => g.GamersGames)
                .FirstOrDefaultAsync();

            if (game == null)
            {
                throw new ArgumentException("Invalid game");
            }

            string CurrentUserId = GetCurrentUserId() ?? string.Empty;

            GamerGame? gamerGame = game.GamersGames.FirstOrDefault(g => g.GamerId == CurrentUserId);

            if (gamerGame != null)
            {
                game.GamersGames.Remove(gamerGame);

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyZone));
        }

        [HttpGet]
        public async Task<IActionResult> AddToMyZone(int id)
        {
            Game? game = await context.Games
               .Where(g => g.Id == id)
               .Include(g => g.GamersGames)
               .FirstOrDefaultAsync();

            if (game == null)
            {
                throw new ArgumentException("Invalid game");
            }

            string CurrentUserId = GetCurrentUserId() ?? string.Empty;

            if (game.GamersGames.Any(g => g.GamerId == CurrentUserId))
            {
                return RedirectToAction(nameof(MyZone));
            }

            game.GamersGames.Add(new GamerGame()
            {
                GamerId = GetCurrentUserId() ?? string.Empty,
                GameId = game.Id
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(MyZone));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await context.Games
                .Where(g => g.Id == id)
                .AsNoTracking()
                .Select(g => new GameDetailsViewModel()
                {
                    Id = g.Id,
                    Title = g.Title,
                    ImageUrl = g.ImageUrl,
                    Description = g.Description,
                    ReleasedOn = g.ReleasedOn.ToString(ReleasedOnFormat),
                    Genre = g.Genre.Name,
                    Publisher = g.Publisher.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Games
                .Where(g => g.Id == id)
                .Select(g => new DeleteViewModel()
                {
                    Id = g.Id,
                    Title = g.Title,
                    Publisher = g.Publisher.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteViewModel model)
        {
            Game? game = await context.Games
                .Where(g => g.Id == model.Id)
                .FirstOrDefaultAsync();

            if (game != null)
            {
                context.Games.Remove(game);

                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(All));
        }

        private async Task<List<Genre>> GetGenres()
        {
            return await context.Genres
                .ToListAsync();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
