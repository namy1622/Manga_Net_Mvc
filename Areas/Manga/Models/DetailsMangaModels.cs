using System.Text.Json.Serialization;

namespace Manga.Home.Models
{
   public class DetailsMangaModel
   {
      [JsonPropertyName("_id")]
      public string Id { get; set; }

      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("slug")]
      public string Slug { get; set; }

      [JsonPropertyName("thumb_url")]
      public string Thumb_url { get; set; }

        // [JsonPropertyName("author")]
        // public string? Author { set; get; }

        [JsonPropertyName("category")]
      public List<Category> Category { set; get; }

      [JsonPropertyName("chapters")]
      public List<Chapter> Chapters {set; get;}

      [JsonPropertyName("updatedAt")]
      public string Update {set; get;}
   }

   public class Chapter{
      // [JsonPropertyName("server_name")]
      // public string server_name {set; get;}
      [JsonPropertyName("server_data")]
      public List<ChapterData> ChapterData{set; get;}
   }

  
   public class ChapterData
   {

      [JsonPropertyName("chapter_name")]
      public string num_Chapter { set; get; }

      [JsonPropertyName("chapter_api_data")]
      public string link_chap { get; set; }

        internal object Where(bool v)
        {
            throw new NotImplementedException();
        }
    }

    public class Data_Details{
      [JsonPropertyName("item")]
      public DetailsMangaModel DetailsManga{set; get;}
   }



   public class ApiResponse_Details{
      [JsonPropertyName("data")]
      public Data_Details DataDetails {set; get;}
   }

}