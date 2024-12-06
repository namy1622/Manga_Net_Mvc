using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manga.Models{
    public class UserVisit{
         [Key]
        public int Id {set; get;}
        [Required]
       
        public string UserId{set; get;}

        [ForeignKey("UserId")]
        public virtual MangaUser User {set; get;}

        public int VisitCount {set; get;}
        public DateTime LastVisit {set;get;}
    }
}