

using Manga.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manga.Home.Models
{
    [Table("FavouritesManga")]
    public class FavouriteComicModel
    {
        [Key]
        public int ID { get; set; }


        [ForeignKey("User")]
        [DisplayName("Id nngười đọc")]
        public string UserID { set; get; }

        
        public virtual MangaUser? User {  get; set; }

        [DisplayName("ID truyện")]
        public string IdManga { get; set; }

        [DisplayName("Ngày thêm")]
        public DateTime FollowedDate { get; set; }
    }
}
