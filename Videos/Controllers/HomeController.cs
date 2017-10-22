using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Videos.Models.Entity;
using Videos.Models.Repository;
using Videos.Models.ViewModel;

namespace Videos.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {
            return View();
        }

        public ActionResult Download(string titulo, string path, string audio) {
            using (var client = new WebClient()) {
                string c = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(path));
                client.DownloadFile(c, "C:/Users/Izaias/Downloads/teste.mp4");
                string a = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(audio));
                client.DownloadFile(a, "C:/Users/Izaias/Downloads/audio.mp4");
                Process.Start("ffmpeg -i teste.mp4 -i audio.mp4 -c copy merged_output.mp4");
            }
            return null;
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
            videosView.ListaVideos = lista.Distinct().OrderByDescending(v=>v.id).ToList();
            videosView.ListaArtistas = videoRepository.Listar<artista>().OrderBy(a => a.nome).ToList();
            videosView.ListaMusicas = videoRepository.Listar<musica>().OrderBy(a => a.titulo).ToList();
            videosView.ListaTipos = videoRepository.Listar<tipo>().OrderBy(a => a.descricao).ToList();
            videosView.ListaTags = videoRepository.Listar<tag>().OrderBy(a => a.nome).ToList();
            
            return View(videosView);
        }

        public ActionResult Playlists() {
            PlaylistsView view = new PlaylistsView();
            PlaylistRepository playlistRepository = new PlaylistRepository();

            view.ListaPlaylists = playlistRepository.Listar<playlist>().OrderBy(p => p.nome).ToList();

            return View(view);
        }
    }
}
 