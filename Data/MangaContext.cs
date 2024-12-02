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
        public DbSet<FavouriteComicModel> UserFavouriteComic { get; set; } = default!;
        public DbSet<ReadingHistoryModel> ReadingHistory { get; set; } = default!;

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
        }



    }
}
