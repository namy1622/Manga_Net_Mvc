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

            var historyChapter = mangaList
            .Where(m => listReadingHistory.Any(f => f.IdManga == m.Id))
            .Select(m => {
                var readingBook = listReadingHistory.FirstOrDefault(r => r.IdManga == m.Id);
                if (readingBook != null)
                {
                    m.CurrentChapter = readingBook.NameChapter; 
                }
                return m;
            })
            .Distinct() // Loại bỏ các bản ghi trùng lặp
            .ToList();

            if (mangaList == null)
                return NotFound("Không tìm thấy danh sách manga trong tệp JSON.");

            // Lọc danh sách manga đã xem từ JSON
            var history = mangaList
                .Where(m => listReadingHistory.Any(f => f.IdManga == m.Id))
                .Distinct() 
                .ToList();

            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHistory(string mangaId)
        {
            var userName = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return Unauthorized("Bạn phải đăng nhập để sử dụng chức năng này.");

            var history = await _mangaContext.ReadingHistory
                .FirstOrDefaultAsync(r => r.UserID == user.Id && r.IdManga == mangaId);

            if (history == null)
                return NotFound("Không tìm thấy lịch sử đọc cần xóa.");

            // Xóa bản ghi
            _mangaContext.ReadingHistory.Remove(history);
            await _mangaContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}

