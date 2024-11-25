using BTL_WebManga.Models;
using BTL_WebManga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace BTL_WebManga.Controllers
{
    public class CategoryController : Controller
    {

        private readonly HttpClient httpClient;
        private readonly ILogger<CategoryController> _logger;
        private readonly CategoryService _categoryService;

        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Json", "Category.json");

        public CategoryController(HttpClient _httpClient, ILogger<CategoryController> logger, CategoryService categoryService)
        {
            httpClient = _httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7143");
            _logger = logger;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()  // Đổi IActionResult thành Task<IActionResult>
        {
            // Lấy danh sách thể loại từ dịch vụ hoặc cơ sở dữ liệu
            var categoryList = _categoryService.GetCategoryList();
            ViewBag.CategoryList = categoryList;

            // Gọi API bất đồng bộ và lấy danh sách manga
            var response = await httpClient.GetFromJsonAsync<List<ComicModel>>("/Home/GetMangaList");

            // Truyền dữ liệu manga vào View
            ViewBag.MangaList = response;

            // Trả về view
            return View();
        }
    }
}
