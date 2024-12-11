using System.Text.Json;
using Manga.Home.Models;
using Microsoft.Extensions.Caching.Memory;

public class PreloadService : BackgroundService{
    private readonly ILogger<PreloadService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

     public PreloadService(ILogger<PreloadService> logger, HttpClient httpClient, IMemoryCache cache)
    {
        _logger = logger;
        _httpClient = httpClient;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         _logger.LogInformation("PreloadService is starting.");
        //  stoppingToken.ThrowIfCancellationRequested();

         try{

            string url = "https://otruyenapi.com/v1/api/truyen-tranh/van-co-toi-cuong-tong";
            var response = await _httpClient.GetAsync(url);

            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse_Details>(jsonString);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // Dữ liệu tồn tại trong 30 phút
                    Priority = CacheItemPriority.High, // Ưu tiên giữ dữ liệu trong bộ nhớ
                    Size = 1 // Thiết lập kích thước cho entry này (nếu sử dụng giới hạn kích thước)
                };

                // if (apiResponse?.DataDetails?.DetailsManga != null)
                // {
                    // _cache.Set("PreloadedMangaList", apiResponse.DataDetails.DetailsManga, TimeSpan.FromMinutes(30));
                    _cache.Set("PreloadedMangaList", apiResponse.DataDetails.DetailsManga, cacheEntryOptions);
                    _logger.LogInformation("Preloaded data successfully.");
                // }
            }
            else
            {
                _logger.LogError($"Failed to retrieve data from API. Status code: {response.StatusCode}");
            }
         }
         catch(Exception ex)
         {
            _logger.LogError($"Error while preloading data: {ex.Message}");
         }
    }
}