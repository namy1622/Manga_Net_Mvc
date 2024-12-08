using System.Text.Json.Serialization;

namespace BTL_WebManga.Models
{
    public class CategoryModel
    {
        [JsonPropertyName("_id")]
        public string IdCatecogy { get; set; }

        [JsonPropertyName("slug")]
        public string SlugCategory { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        
    }

    public class Data_Category
    {
        [JsonPropertyName("items")]
        public List<CategoryModel> CategoryItems { get; set; }
    }

    public class APIResponse_Category
    {
        public Data_Category Data { get; set; }
    }
}

