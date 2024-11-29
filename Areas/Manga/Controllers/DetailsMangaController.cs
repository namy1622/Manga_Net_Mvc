using System.Text.Json;
using Areas.Manga.Models.ViewModels;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Protocol;

namespace Areas.Manga.C
{
    [Area("Manga")]
    [Route("/details/[action]")]
    public class DetailsMangaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DetailsMangaController> _logger;

        private readonly IMemoryCache _cache;

        // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        private readonly string jsonAPI_Details = "https://otruyenapi.com/v1/api/truyen-tranh/";

        // public List<Chapter> server_chap{set; get;}
        public DetailsMangaController(HttpClient _httpClient, ILogger<DetailsMangaController> logger, IMemoryCache cache)
        {
            httpClient = _httpClient;
            _logger = logger;
            _cache = cache;
        }


        public IActionResult Splash(string slug)
        {
             ViewData["AreaName"] = "Manga";      
            ViewData["ControllerName"] = "DetailsManga"; // Controller
            ViewData["ActionName"] = "Details"; // Action

            ViewData["Route"] = "details";
            ViewData["Id"] = slug; // ID hoặc các tham số khác
            return NotFound("////");
            
        }
        //-----------------------------------------------------------------------------------------------------------        
        // [Route("/details/{slug?}")]
        [HttpGet]
        public async Task<ActionResult> Details(string? slug)
        {
            var viewModel = new DetailsManga_ViewModel();


            // var mangaDetails = await GetMangaDetails(slug);
            // viewModel.MangaDetails = mangaDetails;

            // var relatedMangas = await GetRelatedMangas(slug);
            // viewModel.RelatedMangas = relatedMangas;

            // Gọi các API đồng thời
            var mangaDetailsTask =  GetMangaDetails(slug);
            var relatedMangasTask =  GetRelatedMangas(slug);

            // Chờ tất cả các API hoàn thành
            await Task.WhenAll(mangaDetailsTask, relatedMangasTask);

            // Lấy kết quả từ các task
            var mangaDetails = await mangaDetailsTask;
            var relatedMangas = await relatedMangasTask;

            var filter_related = relatedMangas.Where(p => p.Category.Any(c => c.slug_category == "fantasy")).ToList();
            // Gán dữ liệu vào ViewModel
            viewModel.MangaDetails = mangaDetails;
            viewModel.RelatedMangas = filter_related;

            if (mangaDetails != null)
            {
                viewModel.ChapterData = mangaDetails.Chapters.FirstOrDefault().ChapterData;
                viewModel.Categories = mangaDetails.Category;
            }



            return View(viewModel);
        }


        private async Task<IEnumerable<InfoMangaModels>?> GetRelatedMangas(string? slug)
        {
            // show Truyen tương tự 
            // try
            // {
                _logger.LogInformation("===== Goi API Manga ====");

                // Đọc dữ liệu từ tệp JSON
                if (System.IO.File.Exists(jsonFilePath))
                {
                    var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);

                    // Giải mã (deserialize) dữ liệu JSON
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultBufferSize = 4096, // Tăng kích thước bộ đệm
                        WriteIndented = false,    // Tắt định dạng dễ đọc (cho tốc độ nhanh hơn)
                        MaxDepth = 64             // Đặt độ sâu tối đa của JSON
                    });

                    _logger.LogInformation($"Đã nhận dữ liệu từ tệp JSON: {jsonString}");

                    if (apiResponse?.Data.InfoMangaList != null)
                    {

                        // return apiResponse.Data.InfoMangaList.Where(item => item.Slug == slug);
                        return apiResponse.Data.InfoMangaList;
                    }
                    else
                    {
                        _logger.LogInformation("=========== Không có dữ liệu trong tệp JSON.");
                        return null;
                    }
                }
                else
                {
                    _logger.LogError($"Không tìm thấy tệp JSON tại đường dẫn {jsonFilePath}");
                    return null;
                }
            }
            // catch (Exception ex)
            // {
            //     _logger.LogError($"Lỗi khi gọi API: {ex.Message}");
            // }
            // return new List<InfoMangaModels>();

        // }

        public async Task<DetailsManga> GetMangaDetails(string? slug)
        {
            var cacheKey = $"MangaDetails_{slug}";

            if (_cache.TryGetValue(cacheKey, out DetailsManga cachedManga))
            {
                _logger.LogInformation("Returning cached MangaDetails.");
                return cachedManga;
            }

            // try
            // {

                _logger.LogInformation("===== Goi API Chapter====");

                var jsonAPI =  $"{jsonAPI_Details}{slug}";
                var response = await httpClient.GetAsync(jsonAPI);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonSerializer.Deserialize<ApiResponse_Details>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultBufferSize = 4096, // Tăng kích thước bộ đệm
                        WriteIndented = false,    // Tắt định dạng dễ đọc (cho tốc độ nhanh hơn)
                        MaxDepth = 64             // Đặt độ sâu tối đa của JSON
                    });

                    // return apiResponse?.data_Details?.DetailsManga;

                    var mangaDetails = apiResponse?.data_Details?.DetailsManga;

                    // Cache the manga details for future requests
                    if (mangaDetails != null)
                    {
                        _cache.Set(cacheKey, mangaDetails, TimeSpan.FromMinutes(3000)); // Cache for 30 minutes
                    }

                    return mangaDetails;
                }
               else{
                  _logger.LogError("Không lấy được dữ liệu chi tiết manga từ API.");
                  return null;
               }
                

            }
            // catch (Exception ex)
            // {
            //     _logger.LogError($"Lỗi khi gọi API Details: {ex.Message}");

            // }

            
        }

    }
// }



//  if (apiResponse?.Data.InfoMangaList != null)
//                     {
//                         // _logger.LogInformation("===== Đọc dữ liệu thành công từ tệp JSON, đang truyền vào View ====");

//                         // // Lấy danh sách manga
//                         // var mangaList = apiResponse.Data.InfoMangaList;
//                         // _logger.LogInformation("===== Thành công lấy mangaList ====");

//                         // // Lấy danh sách og_image
//                         // //var ogImages = apiResponse.Data.SeoOnPage?.OgImages;
//                         // _logger.LogInformation("===== Thành công lấy seoOnPage ====");

//                         // // Truyền dữ liệu vào View
//                         // ViewBag.MangaList = mangaList;
//                         // // ViewBag.OgImages = ogImages;

//                         // // truy vấn lấy ra 1 truyện = slug
//                         // var slugFilter = mangaList.Where(item => item.Slug == slug).ToList();
//                         // ViewBag.DetailManga = slugFilter;
//                         // _logger.LogInformation($"=====*** {ViewBag.DetailManga}====");

//                         // // var category = slug.
//                         // return View();
//                     }