using System.Text.Json;
using BTL_WebManga.Models;

namespace BTL_WebManga.Services
{
    public class CategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Json", "Category.json");

        public CategoryService(ILogger<CategoryService> logger)
        {
            _logger = logger;
        }

        public List<CategoryModel> GetCategoryList()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    var jsonString = File.ReadAllText(jsonFilePath);
                    var apiResponse = JsonSerializer.Deserialize<APIResponse_Category>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Data?.CategoryItems != null)
                    {
                        return apiResponse.Data.CategoryItems;
                    }
                }
                _logger.LogWarning("Không có dữ liệu trong JSON.");
                return new List<CategoryModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xử lý JSON: {ex.Message}");
                return new List<CategoryModel>();
            }
        }
    }

}
