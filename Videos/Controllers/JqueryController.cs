using System.Web.Mvc;
using Videos.Models.Repository;
using Videos.Models.ViewModel;
using Videos.Models.Entity;
using Videos.Models.Services;
using System.Linq;

namespace Videos.Controllers {
    public class JqueryController : Controller {

        public ActionResult ExibirFormVideo(int id) {
            VideoRepository videoRepository = new VideoRepository();
            ArtistaRepository artistaRepository = new ArtistaRepository();
            MusicaRepository musicaRepository = new MusicaRepository();
            TagRepository tagRepository = new TagRepository();
            VideoDataView videoDataView = VideoDataView.init();
            VideoDataService videoDataService = new VideoDataService();

            video video = videoRepository.getVideoById(id);

            videoDataView.Id = id;
            videoDataView.Titulo = video.titulo;
            videoDataView.ArtistaPrincipal = video.video_artista.Where(a => a.principal == true).FirstOrDefault().artista.nome;
            videoDataView.ListaArtistas = artistaRepository.Listar<artista>().OrderBy(a => a.nome).ToList();
            videoDataView.ListaMusicas = musicaRepository.Listar<musica>().OrderBy(m => m.titulo).ToList();
            foreach (video_musica video_musica in video.video_musica) {
                videoDataView.Musicas.Add(new musica {
                    id = video_musica.musica.id,
                    titulo = video_musica.musica.titulo
                });
            }
            videoDataView.ListaTags = tagRepository.Listar<tag>().OrderBy(a => a.nome).ToList();
            foreach (video_tag video_tag in video.video_tag) {
                videoDataView.Tags.Add(new tag {
                    id = video_tag.tag.id,
                    nome = video_tag.tag.nome
                });
            }
            videoDataView.Duracao = video.duracao;
            videoDataView.Resolucao = video.resolucao;
            videoDataView.FormatoVideo = video.formato_video;
            videoDataView.Fps = video.fps;
            videoDataView.CanaisAudio = video.canais_audio;
            videoDataView.FormatoAudio = video.formato_audio;
            videoDataView.Thumbs = videoDataService.getThumbs(video.id);

            return PartialView("FormVideoView", videoDataView);
        }

        public ActionResult AdicionarArtistaJquery(int id, string nome_artista) {
            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Artistas.Add(new artista { id = id, nome = nome_artista });

            return PartialView("ArtistaListView", videoDataView);
        }

        public ActionResult AdicionarMusicaJquery(string titulo_musica) {
            MusicaRepository musicaRepository = new MusicaRepository();
            musica musica = musicaRepository.GetMusicaByTitulo(titulo_musica);

            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Musicas.Add(musica);

            return PartialView("MusicaListView", videoDataView);
        }

        public ActionResult AdicionarTagJquery(string nome_tag) {
            TagRepository tagRepository = new TagRepository();
            tag tag = tagRepository.GetTagByNome(nome_tag);

            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Tags.Add(tag);

            return PartialView("TagListView", videoDataView);
        }

        public void SalvarVideoJquery(VideoDataView dados) {
            VideoRepository videoRepository = new VideoRepository();
            videoRepository.salvarVideo(dados);
        }
        public ActionResult AtualizarMetadataJquery(int id) {
            VideoDataService videoDataService = new VideoDataService();
            VideoDataView videoDataView = videoDataService.getVideoMetaData(id);

            return PartialView("VideoMetaDataView", videoDataView);
        }
    }   
}
 