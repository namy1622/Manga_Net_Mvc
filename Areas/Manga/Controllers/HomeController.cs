using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_WebManga.Models;
using Manga.Home.Models;
using System.Text.Json;
using Manga.Models;

namespace Areas.Manga.Controllers;

[Area("Manga")]
// [ApiController]
// [Route("/api/[Controller]")]\
// [Route("[area]/[controller]/[action]")]
public class HomeController : Controller
{
    private readonly HttpClient httpClient;
    private readonly ILogger<HomeController> _logger;

    // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
    private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

    public HomeController(HttpClient _httpClient, ILogger<HomeController> logger)
    {
        httpClient = _httpClient;
        _logger = logger;
    }

    //
    public IActionResult Splash()
    {
        return View();
        
    }

    //

    // [Route("/api/home")]
    [HttpGet]
    // [Route("")]
    public async Task<IActionResult> Index(int currentPage, int pagesize)
    {

        try
        {
            _logger.LogInformation("===== Goi API 1 ====");

            // var response = await httpClient.GetAsync(api_Home);

            // Đọc dữ liệu từ tệp JSON
            if (System.IO.File.Exists(jsonFilePath))
            {
                var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);

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

                    //-- Phân trang--
                    int totalManga = mangaList.Count();
                    _logger.LogInformation($"===== {totalManga} ====");

                    if (pagesize <= 0) pagesize = 30;
                    int countPages = (int)Math.Ceiling((double)totalManga / pagesize);

                    if (currentPage > countPages) currentPage = countPages;
                    if (currentPage < 1) currentPage = 1;

                    var pagingModel = new PagingModel()
                    {
                        countpages = countPages,
                        currentpage = currentPage,

                        //delegate phat sinh ra url
                        generateUrl = (pageNumber) => Url.Action("Index", new
                        {
                            currentPage = pageNumber,
                            pagesize = pagesize
                        })
                    };

                    ViewBag.pagingModel = pagingModel;
                    ViewBag.totalManga = totalManga;

                    ViewBag.postIndex = (currentPage - 1) * pagesize;

                    var MangasInPage = mangaList
                                            .Skip((currentPage - 1) * pagesize)
                                            .Take(pagesize)
                                            .ToList();
                    ViewBag.MangasInPage = MangasInPage;
                    //-- end Phân trang --/=
                    _logger.LogInformation($"===== currentPage = {currentPage} ====");
                    _logger.LogInformation($"===== pagesize = {pagesize} ====");
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

    [Route("/api/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/api/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


// _logger.LogInformation("===== Truoc Success ====");

// if (response.IsSuccessStatusCode)
// {
//     _logger.LogInformation("===== Sau Success ====");

//     var jsonString = await response.Content.ReadAsStringAsync();

//     var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
//     {
//         PropertyNameCaseInsensitive = true
//     });

//     _logger.LogInformation($"Received response with status code: {response.StatusCode}");
//     _logger.LogInformation($"API Response: {jsonString}");


//     _logger.LogInformation("===== Truoc 3dk ====");
//     _logger.LogInformation($"===== {apiResponse.Data.SeoOnPage} ====");
//     _logger.LogInformation($"===== {apiResponse.Data.InfoMangaList} ====");
//     _logger.LogInformation($"===== {apiResponse} ====");

//     // if (apiResponse != null && apiResponse.InfoMangaList != null && apiResponse.SeoOnPage != null)
//    if (apiResponse?.Data.InfoMangaList != null && apiResponse?.Data.SeoOnPage != null)
//     {
//         _logger.LogInformation("===== Goi API thanh cong, tai da len View ====");

//         // Lấy danh sách manga
//         var mangaList = apiResponse.Data.InfoMangaList;
//         _logger.LogInformation("===== thanh cong mangaList ====");

//         // Lấy danh sách og_image
//         var ogImages = apiResponse.Data.SeoOnPage?.OgImages;
//         _logger.LogInformation("===== thanh cong seoOnPage ====");
//         ViewBag.MangaList = mangaList;
//         ViewBag.OgImages = ogImages;
//         _logger.LogInformation("===== finish ====");
//         return View();
//     }
//     else
//     {
//         _logger.LogInformation("=========== Không có dữ liệu trả về từ API.");
//         ViewBag.Error = "Không có dữ liệu trả về từ API.";
//         return View(new List<InfoMangaModels>());
//     }
// }
// else
// {
//     _logger.LogError($"API call failed with status code {response.StatusCode}");
//     ViewBag.Error = "Không thể lấy dữ liệu từ API.";
//     return View(new List<InfoMangaModels>());
// }