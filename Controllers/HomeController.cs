using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_WebManga.Models;
using System.Text.Json;
using System.Net.Http;
using BTL_WebManga.Services;

namespace BTL_WebManga.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient httpClient;
    private readonly ILogger<HomeController> _logger;
    private readonly CategoryService _categoryService;

    // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
    private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Json", "ListComics.json");

    public HomeController(HttpClient _httpClient, ILogger<HomeController> logger, CategoryService categoryService)
    {
        httpClient = _httpClient;
        _logger = logger;
        _categoryService = categoryService;
    }

    // [Route("/api/home")]
    [HttpGet]
    // [Route("")]
    public async Task<IActionResult> Index(int currentPage, int pagesize)
    {

        try
        {
            _logger.LogInformation("===== Goi API 1 ====");

            // var response = await httpClient.GetAsync(api_Home);
            var categoryList = _categoryService.GetCategoryList();
            ViewBag.CategoryList = categoryList;
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
                    return View(new List<ComicModel>());
                }

            }
            else
            {
                _logger.LogError($"Không tìm thấy tệp JSON tại đường dẫn {jsonFilePath}");
                ViewBag.Error = "Không tìm thấy tệp JSON.";
                return View(new List<ComicModel>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi gọi API: {ex.Message}");
            ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
            return View(new List<ComicModel>());
        }

    }
    public IActionResult GetMangaList()
    {
        var jsonString = System.IO.File.ReadAllText(jsonFilePath);
        var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
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
            // Trả về dữ liệu JSON
            return Json(mangaList);
        }
        else
        {
            _logger.LogInformation("=========== Không có dữ liệu trong tệp JSON.");
            ViewBag.Error = "Không có dữ liệu trong tệp JSON.";
            return View(new List<ComicModel>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
