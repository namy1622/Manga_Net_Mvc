using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_WebManga.Models;
using Manga.Home.Models;
using System.Text.Json;
using Manga.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Index(int currentPage, int pagesize, string searchName)
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
                    var mangaList = apiResponse.Data.InfoMangaList;

                    // Tìm kiếm 
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        mangaList = mangaList
                        .Where(s =>
                            (s.Name != null && s.Name.ToUpper().Contains(searchName.ToUpper())) ||
                            (s.Author != null && s.Author.ToUpper().Contains(searchName.ToUpper()))
                        )
                        .DistinctBy(s=> s.Name)
                        .ToList();
                    }

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
                    return View(mangaList);
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
