using System.Collections.Generic;
using System.Web.Mvc;
using Videos.Models.Entity;
using Videos.Models.Repository;
using Videos.Models.ViewModel;

namespace Videos.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {
            return View();
        }

        public ActionResult Artistas(string id) {
            ArtistaRepository artistaRepository = new ArtistaRepository();

            if (id == "Atualizar") {
                List<string> pastasArtistas = artistaRepository.listarPastasArtistas();
                foreach (string pasta in pastasArtistas) {
                    artistaRepository.inserirArtista(pasta);
                }
            }

            ArtistasView artistasView = new ArtistasView();
            artistasView.listaArtistas = artistaRepository.listarArtistas();

            return View(artistasView);
        }

        public ActionResult Videos() {
            VideoRepository videoRepository = new VideoRepository();

            VideosView videosView = new VideosView();
            videosView.listaVideos = videoRepository.listarVideos();

            return View(videosView);
        }
    }
}
 