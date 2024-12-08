using BTL_WebManga.Areas.Manga.Models;
using BTL_WebManga.Models;
using BTL_WebManga.Services;
using Manga.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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


                    if (selectedCategories == null || !selectedCategories.Any())
                    {
                        ViewBag.MangaList = mangaList;

                        // Pagination
                        int totalManga = mangaList.Count();
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

                        return View();
                    }

                    if (!string.IsNullOrEmpty(slug))
                    {
                        var comicsByCategory = mangaList.Where(ci => ci.CategoryList.Any(c => c.SlugCategory == slug)).ToList();
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
                            generateUrl = (pageNumber) => Url.Action("Index", new
                            {
                                currentPage = pageNumber,
                                pagesize
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

                        return View();
                    }
                    else if (selectedCategories != null && selectedCategories.Any())
                    {
                        var filteredComics = mangaList.Where(manga => manga.CategoryList.Any(c => selectedCategories.Contains(c.SlugCategory))).ToList();
                        if (!filteredComics.Any())
                        {
                            ViewBag.MangaList = null;
                            return View();
                        }

                        ViewBag.MangaList = filteredComics;

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
                            generateUrl = (pageNumber) => Url.Action("Index", new
                            {
                                currentPage = pageNumber,
                                pagesize
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

                        return View();
                    }
                }

            }
            ViewBag.MangaList = null;
            return View();
        }



    }

}
