using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BTL_WebManga.Areas.Manga.Models
{
    public class ComicModel
    {
        [JsonPropertyName("_id")]
        public string IdComic { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("thumb_url")]
        public string Thumb_url { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime Date { get; set; }

        [JsonPropertyName("category")]
        public List<CategoryModel> CategoryList { get; set; }
    }

    public class Data_Comic
    {
        [JsonPropertyName("items")]
        public List<ComicModel> InfoMangaList { set; get; }

        // [JsonPropertyName("og_image")]
        // public List<ImageMangaModels> ImageMangaList{set; get;}

        // [JsonPropertyName("seoOnPage")]
        // public SeoOnPage SeoOnPage { get; set; }
    }
    public class ApiResponse_InfoManga
    {

        [JsonPropertyName("data")]
        public Data_Comic Data { get; set; }
    }
}
