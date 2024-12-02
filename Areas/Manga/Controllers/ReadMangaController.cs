using System.Text.Json;
using Areas.Manga.Models.ViewModels;
using Manga.Data;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Areas.Manga.Controllers
{
     [Area("Manga")]
    public class ReadMangaController : Controller
    {
         private readonly HttpClient httpClient;
        private readonly ILogger<ReadMangaController> _logger;

        private readonly MangaContext _mangaContext;


        private readonly IMemoryCache _cache;  

        // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
        private readonly string baseAPI_read = "https://sv1.otruyencdn.com/";

        private readonly string jsonAPI_Details = "https://otruyenapi.com/v1/api/truyen-tranh/";

        // public List<Chapter> server_chap{set; get;}
        public ReadMangaController(HttpClient _httpClient, ILogger<ReadMangaController> logger, IMemoryCache cache, MangaContext mangaContext)
        {
            httpClient = _httpClient;
            _logger = logger;
            _cache = cache;
            _mangaContext = mangaContext;
        }

        // GET: ReadManga
        [HttpGet]
        // [Route("/read/{linkchap?}")]
        public async Task<ActionResult> Read(string linkchap, string comicId, string chapterName)
        {
            var viewModel_Read = new ReadManga_ViewModel();

            var readManga = await GetImagePageManga(linkchap);
            _logger.LogInformation($"===== {readManga}====");

            var imageChap = readManga.Chapter_Image;
            _logger.LogInformation($"===== {imageChap}====");

            var path = readManga.Chapter_Path;
            _logger.LogInformation($"===== {path}====");

            viewModel_Read.data_Read = readManga;
            viewModel_Read.Chapter_path = path;
            viewModel_Read.chapter_Images = imageChap;
            var userName = User.Identity.Name;

            // Thêm và cập nhật danh sách lịch sử đọc truyện
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return NotFound();

            var history = await _mangaContext.ReadingHistory
                                        .FirstOrDefaultAsync(r => r.UserID == user.Id && r.IdManga == comicId);

            if (history == null)
            {
                history = new ReadingHistoryModel
                {
                    UserID = user.Id,
                    IdManga = comicId,
                    NameChapter = chapterName,
                    LastReadTime = DateTime.Now
                };
                _mangaContext.ReadingHistory.Add(history);
            }
            else
            {
                history.NameChapter = chapterName;
                history.LastReadTime = DateTime.Now;
                _mangaContext.ReadingHistory.Update(history);
            }
            viewModel_Read.CurrentChapter = history.NameChapter;


            await _mangaContext.SaveChangesAsync();
            return View(viewModel_Read);
        }

        public async Task<Read_Items> GetImagePageManga(string linkchap)
        {
             _logger.LogInformation("===== Goi API Chapter====");
             _logger.LogInformation($"===== {linkchap}====");

            var response = await httpClient.GetAsync(linkchap);
            _logger.LogInformation($"===== {response}====");
            if(response.IsSuccessStatusCode)
            {
                var jsonString_Read = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse_Reads>(jsonString_Read, new JsonSerializerOptions
                {
                     PropertyNameCaseInsensitive = true,
                        DefaultBufferSize = 4096, // Tăng kích thước bộ đệm
                        WriteIndented = false,    // Tắt định dạng dễ đọc (cho tốc độ nhanh hơn)
                        MaxDepth = 64             // Đặt độ sâu tối đa của JSON
                });

                var mangaRead = apiResponse?.data_Read?.Item;

                return mangaRead;
            }

             else{
                  _logger.LogError("Không lấy được dữ liệu chi tiết manga từ API.");
                  return null;
               }
             
        }


    }
}
