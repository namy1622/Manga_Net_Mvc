namespace Manga.Home.Models
{
    public class ReadingHistoryModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }

        public string IdManga { get; set; }
        public string NameChapter { get; set; }

        public string LinkChapter { get; set; }

        public DateTime LastReadTime { get; set; }
    }
}
