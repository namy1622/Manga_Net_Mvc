using Microsoft.AspNetCore.Mvc;

namespace Areas.DetailsManga
{
    [Area("DetailsManga")]
    // [Route("/details/index")]
    public class DetailsMangaController : Controller
    {
        // GET: DetailsMangaController
        public ActionResult Index()
        {
            return View();
        }

    }
}
