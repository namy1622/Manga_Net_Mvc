// using Manga.Models.Post;
using Manga.Home.Models;
using Manga.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Manga.Data
{
    // Manga.Models.MangaContext
    public class MangaContext : IdentityDbContext<MangaUser>
    {
        public MangaContext(DbContextOptions<MangaContext> options) : base(options)
        {

        }
        public DbSet<FavouriteComicModel> UserFavouriteComic { get; set; } 
        public DbSet<ReadingHistoryModel> ReadingHistory { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            // Thi?t l?p quan h? gi?a FavouritesManga v� MangaUser
            modelBuilder.Entity<FavouriteComicModel>()
                .HasOne(f => f.User)
                .WithMany() // M?t user c� th? c� nhi?u favourite
                .HasForeignKey(f => f.UserID)
                //.OnDelete(DeleteBehavior.Cascade); // X�a user s? x�a c? favourite
                .OnDelete(DeleteBehavior.NoAction); 

modelBuilder.Entity<ReadingHistoryModel>()
                .HasOne(f => f.mangaUser)
                .WithMany() // M?t user c� th? c� nhi?u favourite
                .HasForeignKey(f => f.UserID)
                //.OnDelete(DeleteBehavior.Cascade); // X�a user s? x�a c? favourite
                .OnDelete(DeleteBehavior.NoAction); 

        }



    }
}
