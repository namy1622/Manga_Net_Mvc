namespace Manga.Home.Models
{
    public class ReadHistoryModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }

        public DateTime LastReadTime { get; set; }

        public List<DetailsMangaModel> DetailsManga { get; set; }
    }
}
