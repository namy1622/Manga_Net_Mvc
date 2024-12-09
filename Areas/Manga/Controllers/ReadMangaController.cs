using System.Drawing.Drawing2D;
using System.Text.Json;
using Areas.Manga.Models.ViewModels;
using Manga.Data;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Protocol;


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
        public async Task<ActionResult> Read(string linkchap, string comicId, string chapterName , string mangaId = null)
        {
            var viewModel_Read = new ReadManga_ViewModel();

            // //////////////////////
            // Nếu linkchap rỗng và có mangaId, thực hiện chức năng "Đọc tiếp"
            if (string.IsNullOrEmpty(linkchap) && !string.IsNullOrEmpty(mangaId))
            {
                var userName_continue = User.Identity.Name;
                var user_continue = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName_continue);

                if (user_continue == null)
                    return Ok("Bạn phải đăng nhập mới dùng được chức năng này");

                var mangaHistory = await _mangaContext.ReadingHistory
                    .FirstOrDefaultAsync(m => m.UserID == user_continue.Id && m.IdManga == mangaId);

                if (mangaHistory == null)
                    return NotFound("Không tìm thấy lịch sử đọc cho manga này");

                linkchap = mangaHistory.LinkChapter;
                chapterName = mangaHistory.NameChapter;
                comicId = mangaHistory.IdManga;
            }


            // =======================

            var chapterDataJson = HttpContext.Session.GetString("ChapterData");
            var id_manga = HttpContext.Session.GetString("id_Manga");


            if (!string.IsNullOrEmpty(chapterDataJson))
            {
                var chapterData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChapterData>>(chapterDataJson);
                // Gán dữ liệu ChapterData vào ViewModel
                viewModel_Read.ChapterData = chapterData;


                try
                {
                    var sortedChapters = chapterData
                                   .Select(c => new { Chapter = c, ChapterNumber = double.Parse(c.num_Chapter) })
                                   .OrderBy(c => c.ChapterNumber)
                                   .ToList();

                    var testChap = chapterName;
                    var next_chap = chapterData.LastOrDefault();
                    var prev_chap = chapterData.FirstOrDefault(c => int.Parse(c.num_Chapter) == int.Parse(chapterName));
                    if (chapterName != chapterData.FirstOrDefault().num_Chapter.ToString())
                    {
                        prev_chap = chapterData.FirstOrDefault(c => int.Parse(c.num_Chapter) == int.Parse(chapterName) - 1);
                        var prev_check = prev_chap.num_Chapter;

                        viewModel_Read.prev_chap = prev_chap;
                    }

                    else
                    {
                        viewModel_Read.prev_chap = prev_chap;
                    }

                    if (chapterName != chapterData.LastOrDefault().num_Chapter.ToString())
                    {
                        next_chap = chapterData.FirstOrDefault(c => double.Parse(c.num_Chapter) == double.Parse(chapterName) + 1);
                        viewModel_Read.next_chap = next_chap;
                    }
                    else
                    {
                        viewModel_Read.next_chap = next_chap;
                    }

                }
                catch (Exception e)
                {
                    return NotFound($"Loi he thong: {e.ToString}");
                }

            }

            var readManga = await GetImagePageManga(linkchap);
            _logger.LogInformation($"===== {readManga}====");

            var imageChap = readManga.Chapter_Image;
            _logger.LogInformation($"===== {imageChap}====");

            var path = readManga.Chapter_Path;
            _logger.LogInformation($"===== {path}====");

            // var details_chap = new DetailsManga_ViewModel();
            // ViewData["Details_Chap"] = ViewData["data_chap"];

            viewModel_Read.data_Read = readManga;
            viewModel_Read.Chapter_path = path;
            viewModel_Read.chapter_Images = imageChap;
            viewModel_Read.id_manga = id_manga;

            var userName = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);


            // Thêm và cập nhật danh sách lịch sử đọc truyện
            if (user != null)
            {
                var history = await _mangaContext.ReadingHistory
                                        .FirstOrDefaultAsync(r => r.UserID == user.Id && r.IdManga == comicId);

                if (history == null)
                {
                    history = new ReadingHistoryModel
                    {
                        UserID = user.Id,
                        IdManga = comicId,
                        NameChapter = chapterName,
                        LinkChapter = linkchap,
                        LastReadTime = DateTime.Now
                    };
                    _mangaContext.ReadingHistory.Add(history);
                }
                else
                {
                    history.NameChapter = chapterName;
                    history.LinkChapter = linkchap;
                    history.LastReadTime = DateTime.Now;
                    _mangaContext.ReadingHistory.Update(history);
                }
                await _mangaContext.SaveChangesAsync();
            }

            ViewData["numChap"] = chapterName;
            return View(viewModel_Read);
        }

        //Get
       
       

        public async Task<Read_Items> GetImagePageManga(string linkchap)
        {
            _logger.LogInformation("===== Goi API Chapter====");
            _logger.LogInformation($"===== {linkchap}====");

            var response = await httpClient.GetAsync(linkchap);
            _logger.LogInformation($"===== {response}====");
            if (response.IsSuccessStatusCode)
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

            else
            {
                _logger.LogError("Không lấy được dữ liệu chi tiết manga từ API.");
                return null;
            }

        }
    }
}
