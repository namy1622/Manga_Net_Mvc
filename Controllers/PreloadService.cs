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
         stoppingToken.ThrowIfCancellationRequested();

         try{

            string url = "https://otruyenapi.com/v1/api/truyen-tranh/van-co-toi-cuong-tong";
            var response = await _httpClient.GetAsync(url, stoppingToken);

            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse_Details>(jsonString);

                if (apiResponse?.data_Details?.DetailsManga != null)
                {
                    _cache.Set("PreloadedMangaList", apiResponse.data_Details.DetailsManga, TimeSpan.FromMinutes(30));
                    _logger.LogInformation("Preloaded data successfully.");
                }
            }
         }
         catch(Exception ex)
         {
            _logger.LogError($"Error while preloading data: {ex.Message}");
         }
    }
}