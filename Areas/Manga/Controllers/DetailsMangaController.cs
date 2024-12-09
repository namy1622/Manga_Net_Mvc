using System.Text.Json;
using Areas.Manga.Models.ViewModels;
using Manga.Data;
using Manga.Home.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Areas.Manga.Controllers
{
    [Area("Manga")]
    [Route("/details/[action]")]
    public class DetailsMangaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DetailsMangaController> _logger;
        private readonly MangaContext _mangaContext;

        private readonly IMemoryCache _cache;

        // private readonly string api_Home = "https://otruyenapi.com/v1/api/home";
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "API", "Home.json");

        private readonly string jsonAPI_Details = "https://otruyenapi.com/v1/api/truyen-tranh/";

        // public List<Chapter> server_chap{set; get;}
        public DetailsMangaController(HttpClient _httpClient, ILogger<DetailsMangaController> logger, IMemoryCache cache, MangaContext mangaContext)
        {
            httpClient = _httpClient;
            _logger = logger;
            _cache = cache;
            _mangaContext = mangaContext;
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

            if (string.IsNullOrEmpty(slug))
            {
                _logger.LogError("Slug bị null hoặc rỗng.");
                return NotFound("Slug không hợp lệ.");
            }

   
    

            // Gọi các API đồng thời
            var mangaDetailsTask = GetMangaDetails(slug);
            var relatedMangasTask = GetRelatedMangas(slug);

            // Chờ tất cả các API hoàn thành
            await Task.WhenAll(mangaDetailsTask, relatedMangasTask);

            // Lấy kết quả từ các task
            var mangaDetails = await mangaDetailsTask;
            var relatedMangas = await relatedMangasTask;

        // Lưu toàn bộ ChapterData vào TempData
        // TempData["ChapterData"] = Newtonsoft.Json.JsonConvert.SerializeObject(mangaDetails.Chapters.FirstOrDefault()?.ChapterData);


        // TempData["id_Manga"] = mangaDetails.Id;

        // Lưu ChapterData vào Session
HttpContext.Session.SetString("ChapterData", Newtonsoft.Json.JsonConvert.SerializeObject(mangaDetails.Chapters.FirstOrDefault()?.ChapterData));
HttpContext.Session.SetString("id_Manga", mangaDetails.Id);


            var filter_related = relatedMangas.Where(p => p.Category.Any(c => c.slug_category == "fantasy")).ToList();
            viewModel.MangaDetails = mangaDetails;
            viewModel.RelatedMangas = filter_related;

            if (mangaDetails != null)
            {
                viewModel.ChapterData = mangaDetails.Chapters.FirstOrDefault()?.ChapterData;

                // ViewData["data_chap"] = viewModel.ChapterData;
                viewModel.Categories = mangaDetails.Category;
            }

            // Kiểm tra xem người dùng đã đọc truyện này chưa
            var username = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user != null && viewModel.MangaDetails != null)
            {
                // Kiểm tra trong bảng ReadingHistory xem có bản ghi đọc truyện này chưa
                var readingHistory = await _mangaContext.ReadingHistory
                    .FirstOrDefaultAsync(rh => rh.UserID == user.Id && rh.IdManga == mangaDetails.Id);

                // Nếu có lịch sử đọc, hiển thị nút "Đọc tiếp"
                viewModel.HasReadingHistory = readingHistory != null;

                // Kiểm tra xem manga có trong danh sách yêu thích của người dùng không
                viewModel.IsFavourite = await _mangaContext.UserFavouriteComic
                    .AnyAsync(f => f.UserID == user.Id && f.IdManga == viewModel.MangaDetails.Id);
            }

            return View(viewModel);
        }


        // catch (Exception ex)
        // {
        //     _logger.LogError($"Lỗi khi gọi API: {ex.Message}");
        // }
        // return new List<InfoMangaModels>();

        // }

        [HttpPost]
        public async Task<IActionResult> Favourite(string comicId)
        {
            
            var username = User.Identity.Name;
            // var userID = User.Identity.
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            // Đọc dữ liệu từ file JSON
            if (!System.IO.File.Exists(jsonFilePath))
                return NotFound("Không tìm thấy tệp dữ liệu JSON.");

            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var mangaList = apiResponse?.Data.InfoMangaList;

            if (mangaList == null)
                return NotFound("Không tìm thấy danh sách manga trong tệp JSON.");

            var manga = mangaList.FirstOrDefault(m => m.Id == comicId);

            if (manga == null)
                return NotFound("Không tìm thấy manga với ComicId được cung cấp.");

            // Thêm vào danh sách yêu thích
            var favouriteComic = new FavouriteComicModel
            {
                UserID = user.Id,
                IdManga = comicId,
                FollowedDate = DateTime.Now
            };

            _mangaContext.UserFavouriteComic.Add(favouriteComic);
            await _mangaContext.SaveChangesAsync();

            return RedirectToAction("Details", new { slug = manga.Slug });
        }

        [HttpPost]
        public async Task<IActionResult> UnFavourite(string ComicId)
        {
            var username = User.Identity.Name;
            var user = await _mangaContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            // Đọc dữ liệu từ file JSON
            if (!System.IO.File.Exists(jsonFilePath))
                return NotFound("Không tìm thấy tệp dữ liệu JSON.");

            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            var apiResponse = Deserialize<ApiResponse_InfoManga>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var mangaList = apiResponse?.Data.InfoMangaList;

            if (mangaList == null)
                return NotFound("Không tìm thấy danh sách manga trong tệp JSON.");

            var manga = mangaList.FirstOrDefault(m => m.Id == ComicId);

            if (manga == null)
                return NotFound("Không tìm thấy manga với ComicId được cung cấp.");

            var existingFavourite = await _mangaContext.UserFavouriteComic
                .FirstOrDefaultAsync(f => f.UserID == user.Id && f.IdManga == ComicId);

            // Xóa khỏi danh sách yêu thích
            _mangaContext.UserFavouriteComic.Remove(existingFavourite);
            await _mangaContext.SaveChangesAsync();

            return RedirectToAction("Details", new { slug = manga.Slug });
        }

        public async Task<DetailsMangaModel> GetMangaDetails(string? slug)
        {
            var cacheKey = $"MangaDetails_{slug}";

            if (_cache.TryGetValue(cacheKey, out DetailsMangaModel cachedManga))
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

                    var mangaDetails = apiResponse?.DataDetails?.DetailsManga;

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