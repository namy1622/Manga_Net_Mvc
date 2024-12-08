using BTL_WebManga.Models;
using BTL_WebManga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<IActionResult> Index(List<string> selectedCategories, int currentPage, int pagesize, string slug = "")
        {
            var categoryList = _categoryService.GetCategoryList();
            ViewBag.CategoryList = categoryList;
            ViewBag.SelectedCategories = selectedCategories;

            // Gọi API bất đồng bộ và lấy danh sách manga
            var response = await httpClient.GetFromJsonAsync<List<ComicModel>>("/Home/GetMangaList");
            if (response == null)
            {
                ViewBag.MangaList = null;
                return View();
            }

            if (System.IO.File.Exists(jsonFilePath))
            {
                var jsonString = System.IO.File.ReadAllText(jsonFilePath);
                var apiResponse = JsonSerializer.Deserialize<APIResponse_Category>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation($"===== pagesize = {pagesize} ====");

                if (apiResponse?.Data?.CategoryItems != null)
                {

                    if (!string.IsNullOrEmpty(slug))
                    {
                        var comicsByCategory = response.Where(ci => ci.CategoryList.Any(c => c.SlugCategory == slug)).ToList();
                        if (!comicsByCategory.Any())
                        {
                            ViewBag.MangaList = null;
                            return View();
                        }
                        ViewBag.MangaList = comicsByCategory;

                        int totalManga = comicsByCategory.Count();
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

                        var MangasInPage = comicsByCategory
                                                .Skip((currentPage - 1) * pagesize)
                                                .Take(pagesize)
                                                .ToList();
                        ViewBag.MangasInPage = MangasInPage;
                        //-- end Phân trang --/=
                        _logger.LogInformation($"===== currentPage = {currentPage} ====");
                        _logger.LogInformation($"===== pagesize = {pagesize} ====");

                        return View();
                    }
                    else if (selectedCategories != null && selectedCategories.Any())
                    {
                        slug = "";
                        var filteredComics = response.Where(manga => manga.CategoryList.Any(c => selectedCategories.Contains(c.SlugCategory))).ToList();
                        if (!filteredComics.Any())
                        {
                            ViewBag.MangaList = null;
                            return View();
                        }
                        ViewBag.MangaList = filteredComics;
                        //Hello

                        int totalManga = filteredComics.Count();
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

                        var MangasInPage = filteredComics
                                                .Skip((currentPage - 1) * pagesize)
                                                .Take(pagesize)
                                                .ToList();
                        ViewBag.MangasInPage = MangasInPage;
                        //-- end Phân trang --/=
                        _logger.LogInformation($"===== currentPage = {currentPage} ====");
                        _logger.LogInformation($"===== pagesize = {pagesize} ====");
                        //endhello
                        return View(filteredComics);
                    }
                    else
                    {
                        var sortedComics = response.OrderByDescending(ci => ci.Date).ToList();
                        ViewBag.MangaList = sortedComics;
                        return View(sortedComics);
                    }
                }

            }
            ViewBag.MangaList = null;
            return View();
        }


    }

}
