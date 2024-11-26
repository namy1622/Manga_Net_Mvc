using System.Drawing;
using System.Text.Json.Serialization;

namespace Manga.Home.Models{
    public class InfoMangaModels {
         [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("thumb_url")]
        public string Thumb_url { get; set; }

         [JsonPropertyName("author")]
         public string? Author {set; get;}

        [JsonPropertyName("category")]
        public  List<Category> Category {set; get;}
    }

    public class Category{
        [JsonPropertyName("name")]
        public string name{set; get;}
        [JsonPropertyName("slug")]
        public string slug_category{set; get;}
    }
    // public class SeoOnPage{
    //     [JsonPropertyName("og_image")]
    //     public List<string> OgImages { get; set; }
    // }

    // public class ApiResponse_InfoManga{
    public class Data{
        [JsonPropertyName("items")]
        public List<InfoMangaModels> InfoMangaList {set; get;}

        // [JsonPropertyName("og_image")]
        // public List<ImageMangaModels> ImageMangaList{set; get;}

        // [JsonPropertyName("seoOnPage")]
        // public SeoOnPage SeoOnPage { get; set; }
    }

    public class ApiResponse_InfoManga
    {
       

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }
}