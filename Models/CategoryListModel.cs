using System.Text.Json.Serialization;

namespace Manga.Models
{
    public class CategoryListModel
    {
        [JsonPropertyName("_id")]
        public string IdCatecogy { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        
    }

    public class Data_Category
    {
        [JsonPropertyName("items")]
        public List<CategoryListModel> CategoryItems { get; set; }
    }

    public class APIResponse_Category
    {
        public Data_Category Data { get; set; }
    }
}