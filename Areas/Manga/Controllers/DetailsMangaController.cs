using System.Text.Json;
using Areas.Manga.Models.ViewModels;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;

namespace Areas.Manga.C
{
    [Area("Manga")]
    [Route("/details/[action]")]
    public class DetailsMangaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DetailsMangaController> _logger;

        // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        private readonly string jsonAPI_Details = "https://otruyenapi.com/v1/api/truyen-tranh/";

        // public List<Chapter> server_chap{set; get;}
        public DetailsMangaController(HttpClient _httpClient, ILogger<DetailsMangaController> logger)
        {
            httpClient = _httpClient;
            _logger = logger;
        }
        //-----------------------------------------------------------------------------------------------------------        
        [Route("/details/{slug?}")]
        [HttpGet]
        public async Task<ActionResult> Details(string? slug)
        {
            var viewModel = new DetailsManga_ViewModel();

            // 
            var mangaDetails = await GetMangaDetails(slug);
            viewModel.MangaDetails = mangaDetails;

            var relatedMangas = GetRelatedMangas(slug);
            viewModel.RelatedMangas = relatedMangas;

            if (mangaDetails != null)
            {
                viewModel.ChapterData = mangaDetails.Chapters.FirstOrDefault().ChapterData;
                viewModel.Categories = mangaDetails.Category;
            }

            return View(viewModel);
        }


        private IEnumerable<InfoMangaModels> GetRelatedMangas(string? slug)
        {
            // show Truyen tương tự 
            try
            {
                _logger.LogInformation("===== Goi API Manga ====");

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

                    if (apiResponse?.Data.InfoMangaList != null)
                    {
                        return apiResponse.Data.InfoMangaList.Where(item => item.Slug == slug);
                    }
                    else
                    {
                        _logger.LogInformation("=========== Không có dữ liệu trong tệp JSON.");
                    }
                }
                else
                {
                    _logger.LogError($"Không tìm thấy tệp JSON tại đường dẫn {jsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gọi API: {ex.Message}");
            }
            return new List<InfoMangaModels>();

        }

        public async Task<DetailsManga> GetMangaDetails(string? slug)
        {

            try
            {
                _logger.LogInformation("===== Goi API Chapter====");

                var jsonAPI = $"{jsonAPI_Details}{slug}";
                var response = await httpClient.GetAsync(jsonAPI);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var apiResponse =  JsonSerializer.Deserialize<ApiResponse_Details>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return apiResponse?.data_Details?.DetailsManga;

                }

                _logger.LogError("Không lấy được dữ liệu chi tiết manga từ API.");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gọi API Details: {ex.Message}");

            }

            return null;
        }

    }
}



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