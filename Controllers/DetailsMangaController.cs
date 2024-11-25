using BTL_WebManga.Models;
using BTL_WebManga.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebManga.Controllers
{
    public class DetailsMangaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DetailsMangaController> _logger;
        private readonly CategoryService _categoryService;

        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Json", "Category.json");

        public DetailsMangaController(HttpClient _httpClient, ILogger<DetailsMangaController> logger, CategoryService categoryService)
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7143");
            httpClient = _httpClient;
            _logger = logger;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var response = await httpClient.GetFromJsonAsync<List<ComicModel>>("/Home/GetMangaList");

            // Truyền dữ liệu manga vào View
            ViewBag.MangaList = response;

            return View();
        }


    }
}
