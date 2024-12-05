using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Manga.Models;

namespace Manga.Home.Models
{
    public class ReadingHistoryModel
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("mangaUser")]
        [DisplayName("Id nngười đọc")]
        public string UserID { get; set; }

        public string IdManga { get; set; }
        public string NameChapter { get; set; }

        public string LinkChapter { get; set; }

        public virtual MangaUser mangaUser{set; get;}

        public DateTime LastReadTime { get; set; }
    }
}
