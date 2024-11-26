using Manga.Home.Models;

namespace Areas.Manga.Models.ViewModels
{
    public class DetailsManga_ViewModel
    {
        public DetailsManga MangaDetails {get;set;}
        // public IEnumerable<InfoMangaModels> RelatedMangas  {set; get;}
        public IEnumerable<InfoMangaModels> RelatedMangas  {set; get;}
        public IEnumerable<ChapterData> ChapterData {set; get;}

        public IEnumerable<Category> Categories {set; get;}
    }
}