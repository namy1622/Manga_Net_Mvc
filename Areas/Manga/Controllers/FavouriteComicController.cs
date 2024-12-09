using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_WebManga.Models;
using Manga.Home.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Manga.Data;

namespace Areas.Manga.Controllers
{
    [Area("Manga")]
    public class FavouriteComicController : Controller
    {
        private readonly MangaContext _mangaContext;
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        public FavouriteComicController(MangaContext mangaContext)
        {
            _mangaContext = mangaContext;
        }


        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return RedirectToAction("Login","Account", new {area="Identity"});

            var favouriteComics = await _mangaContext.UserFavouriteComic
                .Where(f => f.UserID == user.Id)
                .ToListAsync();

            // Đọc dữ liệu từ file JSON
            if (!System.IO.File.Exists(jsonFilePath))
                return NotFound("Không tìm thấy tệp dữ liệu JSON.");

            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var mangaList = apiResponse?.Data.InfoMangaList
            .GroupBy(m => m.Id)
            .Select(g => g.First()) 
            .ToList(); 

            if (mangaList == null)
                return NotFound("Không tìm thấy danh sách manga trong tệp JSON.");

            // Lọc danh sách manga yêu thích từ JSON
            var favouriteMangaDetails = mangaList
                .Where(m => favouriteComics.Any(f => f.IdManga == m.Id))
                .Distinct() // Loại bỏ các bản ghi trùng lặp
                .ToList();

            return View(favouriteMangaDetails);
        }

    }
}

