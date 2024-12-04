

using Manga.Home.Models;

namespace Areas.Manga.Models.ViewModels
{
    public class ReadManga_ViewModel{
        public Read_Items data_Read{set; get;}

        public IEnumerable<Chapter_Images> chapter_Images{set; get;}

        public IEnumerable<InfoMangaModels>? Mangas { set; get; }

        public string Chapter_path {set; get;}


    }
}    