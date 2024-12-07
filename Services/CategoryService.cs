using System.Text.Json;
using Manga.Models;

namespace Manga.Services
{
    public class CategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Json", "Category.json");

        public CategoryService(ILogger<CategoryService> logger)
        {
            _logger = logger;
        }

        public List<CategoryListModel> CategoryList()
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

                        // TempData["CategoryList"] = apiResponse.Data.CategoryItems;
                        return apiResponse.Data.CategoryItems;
                    }
                }
                _logger.LogWarning("Không có dữ liệu trong JSON.");
               
                return new List<CategoryListModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xử lý JSON: {ex.Message}");
                return new List<CategoryListModel>();
            }
        }
    }

}