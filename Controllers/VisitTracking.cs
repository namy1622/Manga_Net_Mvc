using System.Security.Claims;
using Manga.Data;
using Manga.Models;

namespace Manga.Controllers
{
    public class VisitTracking
    {
        private readonly RequestDelegate _next;

        public VisitTracking(RequestDelegate next)
        {
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context, MangaContext dbContext)
        {
            // Chỉ xử lý các yêu cầu không phải file tĩnh
            // if (context.Request.Path.StartsWithSegments("/api") ||
            //     context.Request.Path.HasValue && !context.Request.Path.Value.Contains("."))
            // {
                if (context.User.Identity.IsAuthenticated)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (userId != null)
                    {
                        var userVisit = dbContext.UserVisits.FirstOrDefault(uv => uv.UserId == userId);

                        if (userVisit == null)
                        {
                            userVisit = new UserVisit
                            {
                                UserId = userId,
                                VisitCount = 1,
                                LastVisit = DateTime.UtcNow
                            };
                            dbContext.UserVisits.Add(userVisit);
                        }
                        else
                        {
                            userVisit.VisitCount = userVisit.VisitCount + 1;
                            userVisit.LastVisit = DateTime.UtcNow;
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }

                await _next(context);
            // }
        }
    }
}