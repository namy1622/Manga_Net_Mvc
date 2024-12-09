using System.Collections.Generic;
using Manga.Models;

namespace App.Areas.Identity.Models.UserViewModels
{
        public class UserListModel
        {
            public DateTime LockoutEnd {set; get;}
            public int totalUsers { get; set; }
            public int countPages { get; set; }

            public int ITEMS_PER_PAGE { get; set; } = 10;

            public int currentPage { get; set; }

            public List<UserAndRole> users { get; set; }

            //  public UserVisit userVisit {set;get;}

        }

        public class UserAndRole : MangaUser
        {
            public string RoleNames { get; set; }

            public string ProviderLogin {set; get;}

            //  public UserVisit visit{set; get;}
            public int VisitCount {set; get;}
            public DateTime LastVisit {set; get;}
        }





        //public class Visited : UserVisi


}