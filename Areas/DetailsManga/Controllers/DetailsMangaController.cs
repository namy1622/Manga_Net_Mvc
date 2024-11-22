using System.Text.Json;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;

namespace Areas.DetailsManga
{
    [Area("DetailsManga")]
    // [Route("/details/index")]
    public class DetailsMangaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DetailsMangaController> _logger;

        // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        public DetailsMangaController(HttpClient _httpClient, ILogger<DetailsMangaController> logger)
        {
            httpClient = _httpClient;
            _logger = logger;
        }

        // [Route("/api/home")]
        [HttpGet]
        // GET: DetailsMangaController
        public ActionResult Index()
        {
            try
            {
                _logger.LogInformation("===== Goi API 1 ====");

                // var response = await httpClient.GetAsync(api_Home);

                // Đọc dữ liệu từ tệp JSON
                if (System.IO.File.Exists(jsonFilePath))
                {
                    var jsonString = System.IO.File.ReadAllText(jsonFilePath);

                    // Giải mã (deserialize) dữ liệu JSON
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation($"Đã nhận dữ liệu từ tệp JSON: {jsonString}");

                    // if (apiResponse?.Data.InfoMangaList != null && apiResponse?.Data.SeoOnPage != null)
                    if (apiResponse?.Data.InfoMangaList != null)
                    {
                        _logger.LogInformation("===== Đọc dữ liệu thành công từ tệp JSON, đang truyền vào View ====");

                        // Lấy danh sách manga
                        var mangaList = apiResponse.Data.InfoMangaList;
                        _logger.LogInformation("===== Thành công lấy mangaList ====");

                        // Lấy danh sách og_image
                        //var ogImages = apiResponse.Data.SeoOnPage?.OgImages;
                        _logger.LogInformation("===== Thành công lấy seoOnPage ====");

                        // Truyền dữ liệu vào View
                        ViewBag.MangaList = mangaList;
                        // ViewBag.OgImages = ogImages;



                        return View();
                    }
                    else
                    {
                        _logger.LogInformation("=========== Không có dữ liệu trong tệp JSON.");
                        ViewBag.Error = "Không có dữ liệu trong tệp JSON.";
                        return View(new List<InfoMangaModels>());
                    }

                }
                else
                {
                    _logger.LogError($"Không tìm thấy tệp JSON tại đường dẫn {jsonFilePath}");
                    ViewBag.Error = "Không tìm thấy tệp JSON.";
                    return View(new List<InfoMangaModels>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gọi API: {ex.Message}");
                ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
                return View(new List<InfoMangaModels>());
            }
        }

    }
}
