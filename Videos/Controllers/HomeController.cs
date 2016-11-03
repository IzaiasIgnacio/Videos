using System.Collections.Generic;
using System.Web.Mvc;
using Videos.Services;

namespace Videos.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {
            return View();
        }

        public ActionResult Artistas(string id) {
            ArtistaService artistaService = new ArtistaService();

            if (id == "Atualizar") {
                List<string> pastasArtistas = artistaService.listarPastasArtistas();
                foreach (string pasta in pastasArtistas) {
                    artistaService.inserirArtista(pasta);
                }
            }

            Dictionary<int, string> listaArtistas = artistaService.listarArtistas();
            ViewBag.artistas = listaArtistas;

            return View();
        }

        public ActionResult Videos() {
            VideoService videoService = new VideoService();

            List<VideoEntity> listaVideos = videoService.listarVideos();
            ViewBag.videos = listaVideos;

            return View();
        }
    }
}
 