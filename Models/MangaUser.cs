using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Manga.Home.Models;
using Microsoft.AspNetCore.Identity;

namespace Manga.Models 
{
    public class MangaUser: IdentityUser 
    {
    //    public virtual FavouriteComicModel FavouriteComicModel {set; get;}
    }
}
