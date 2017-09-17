using System.Collections.Generic;
using System.Linq;
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
            VideoDataView.init();
            VideoRepository videoRepository = new VideoRepository();

            VideosView videosView = VideosView.init();
            
            List<video> lista = videoRepository.listarVideos();
            videosView.ListaVideos = lista.Distinct().ToList();
            videosView.ListaArtistas = videoRepository.Listar<artista>().OrderBy(a => a.nome).ToList();
            videosView.ListaMusicas = videoRepository.Listar<musica>().OrderBy(a => a.titulo).ToList();
            videosView.ListaTipos = videoRepository.Listar<tipo>().OrderBy(a => a.descricao).ToList();
            videosView.ListaTags = videoRepository.Listar<tag>().OrderBy(a => a.nome).ToList();
            
            return View(videosView);
        }
    }
}
 