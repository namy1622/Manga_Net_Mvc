using Manga.Data;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BTL_WebManga.Areas.Manga.Controllers
{
    [Area("Manga")]
    public class ReadingHistoryController : Controller
    {
        private readonly MangaContext _mangaContext;
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        public ReadingHistoryController(MangaContext mangaContext)
        {
            _mangaContext = mangaContext;
        }


        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return Ok("Bạn phải đăng nhập mới dùng được chức năng này");

            // Lấy danh sách theo dõi từ database theo UserID
            var listReadingHistory = await _mangaContext.ReadingHistory
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

            // Lọc danh sách manga đã xem từ JSON
            var history = mangaList
                .Where(m => listReadingHistory.Any(f => f.IdManga == m.Id))
                .Distinct() // Loại bỏ các bản ghi trùng lặp
                .ToList();

            return View(history);
        }

        
    }
}

