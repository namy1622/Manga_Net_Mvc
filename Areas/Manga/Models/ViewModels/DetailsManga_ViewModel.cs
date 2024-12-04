using Manga.Home.Models;
using Manga.Data;

namespace Areas.Manga.Models.ViewModels
{
    public class DetailsManga_ViewModel
    {
        public  DetailsMangaModel? MangaDetails {get;set;}
        // public IEnumerable<InfoMangaModels> RelatedMangas  {set; get;}
        public IEnumerable<InfoMangaModels>? RelatedMangas  {set; get;}
        public IEnumerable<ChapterData>? ChapterData {set; get;}

        public bool HasReadingHistory { get; set; }
        public bool IsFavourite { get; set; }

        public IEnumerable<Category>? Categories {set; get;}

        
    }

    
}