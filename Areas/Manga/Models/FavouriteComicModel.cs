

namespace Manga.Home.Models
{
    public class FavouriteComicModel
    {
        public int ID { get; set; }
        public string UserID {  get; set; }

        public string IdManga { get; set; }

        public DateTime FollowedDate { get; set; }
    }
}
