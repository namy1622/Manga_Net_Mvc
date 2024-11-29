using Microsoft.AspNetCore.Mvc;

namespace Areas.Manga.Controllers
{
     [Area("Manga")]
    [Route("/readmanga/[action]")]
    public class ReadMangaController : Controller
    {
        // GET: ReadManga
        public ActionResult Read()
        {
            return View();
        }

    }
}
