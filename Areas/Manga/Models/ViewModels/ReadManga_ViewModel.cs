

using System.Text.Json.Serialization;
using Manga.Home.Models;

namespace Areas.Manga.Models.ViewModels
{
    public class ReadManga_ViewModel{
        public Read_Items data_Read{set; get;}

        public IEnumerable<Chapter_Images> chapter_Images{set; get;}

        public IEnumerable<InfoMangaModels>? Mangas { set; get; }

        public string Chapter_path {set; get;}

public IEnumerable<ChapterData>? ChapterData {set; get;}

    public string id_manga {set; get;}

    public ChapterData prev_chap{get; set;}
    public ChapterData next_chap{get; set;}
    }

//       public class ChapterData
//    {

//       [JsonPropertyName("chapter_name")]
//       public string num_Chapter { set; get; }

//       [JsonPropertyName("chapter_api_data")]
//       public string link_chap { get; set; }

        
//     }
}    