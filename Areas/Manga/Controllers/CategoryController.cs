using BTL_WebManga.Areas.Manga.Models;
using BTL_WebManga.Services;
using Manga.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BTL_WebManga.Areas.Manga.Controllers
{
    [Area("Manga")]
    public class CategoryController : Controller
    {

        private readonly HttpClient httpClient;
        private readonly ILogger<CategoryController> _logger;
        private readonly CategoryService _categoryService;

        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        public CategoryController(HttpClient _httpClient, ILogger<CategoryController> logger, CategoryService categoryService)
        {
            httpClient = _httpClient;
            _logger = logger;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(List<string> selectedCategories, int currentPage, int pagesize, string slug = "")
        {
            var categoryList = _categoryService.GetCategoryList();
            ViewBag.CategoryList = categoryList;
            ViewBag.SelectedCategories = selectedCategories;

            if (System.IO.File.Exists(jsonFilePath))
            {
                var jsonString = System.IO.File.ReadAllText(jsonFilePath);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation($"===== pagesize = {pagesize} ====");

                if (apiResponse?.Data?.InfoMangaList != null)
                {
                    var mangaList = apiResponse.Data.InfoMangaList;

                    if (!string.IsNullOrEmpty(slug))
                    {
                        mangaList = mangaList.Where(ci => ci.CategoryList.Any(c => c.SlugCategory == slug)).ToList();
                        // if (!Enumerable.Any<ComicModel>(mangaList))
                        // {
                        //     ViewBag.MangaList = null;
                        //     return base.View();
                        // }

                        if(!mangaList.Any())
                        {
                            ViewBag.MangaList = null;
                            ViewBag.pagingModel = null;
                            return base.View();
                        }

                        if (!selectedCategories.Contains(slug))
                        {
                            selectedCategories.Add(slug);
                        }

                        ViewBag.MangaList = mangaList;
                    }
                    else if (selectedCategories != null && selectedCategories.Any())
                    {
                        mangaList = mangaList.Where(manga => manga.CategoryList.Any(c => selectedCategories.Contains(c.SlugCategory))).ToList();
                        // if (!Enumerable.Any<ComicModel>(mangaList))
                        // {
                        //     ViewBag.MangaList = null;
                        //     return base.View();
                        // }
                        if(!mangaList.Any())
                        {
                            ViewBag.MangaList = null;
                            ViewBag.pagingModel = null;
                            return base.View();
                        }


                        ViewBag.MangaList = mangaList;

                    }
                    else if (selectedCategories == null || !selectedCategories.Any() || slug == null)
                    {
                        ViewBag.MangaList = mangaList;

                    }
                    //Phan trang
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
                        generateUrl = (pageNumber) => Url.Action("Index", new
                        {
                            currentPage = pageNumber,
                            pagesize
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
                }

            }
            ViewBag.MangaList = null;
            return View();
        }

    }

}
