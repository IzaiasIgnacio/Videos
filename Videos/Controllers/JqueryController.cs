using System.Web.Mvc;
using Videos.Models.Repository;
using Videos.Models.ViewModel;
using Videos.Models.Entity;
using Videos.Models.Services;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

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

        public ActionResult RemoverArtistaJquery(int id) {
            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Artistas.Remove(videoDataView.Artistas.Single(a => a.id == id));

            return PartialView("ArtistaListView", videoDataView);
        }

        public ActionResult AdicionarMusicaJquery(string titulo_musica) {
            MusicaRepository musicaRepository = new MusicaRepository();
            musica musica = musicaRepository.GetMusicaByTitulo(titulo_musica);

            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Musicas.Add(musica);

            return PartialView("MusicaListView", videoDataView);
        }

        public ActionResult RemoverMusicaJquery(int id) {
            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Musicas.Remove(videoDataView.Musicas.Single(m => m.id == id));

            return PartialView("MusicaListView", videoDataView);
        }

        public ActionResult AdicionarTagJquery(string nome_tag) {
            TagRepository tagRepository = new TagRepository();
            tag tag = tagRepository.GetTagByNome(nome_tag);

            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Tags.Add(tag);

            return PartialView("TagListView", videoDataView);
        }

        public ActionResult RemoverTagJquery(int id) {
            VideoDataView videoDataView = VideoDataView.GetVideoDataView();
            videoDataView.Tags.Remove(videoDataView.Tags.Single(t => t.id == id));

            return PartialView("TagListView", videoDataView);
        }

        public ActionResult AdicionarTipoJquery(int id, string descricao) {
            VideosView videosView = VideosView.GetVideosView();
            videosView.Tipos.Add(new tipo { id = id, descricao = descricao });

            return PartialView("TipoListView", videosView);
        }

        public ActionResult RemoverTipoJquery(int id) {
            VideosView videosView = VideosView.GetVideosView();
            videosView.Tipos.Remove(videosView.Tipos.Single(t => t.id == id));

            return PartialView("TipoListView", videosView);
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

        public ActionResult FiltrarVideosJquery(VideosView view) {
            VideoRepository videoRepository = new VideoRepository();
            VideosView videosView = new VideosView();
            var lista = new List<video>();

            lista = videoRepository.listarVideos();
            if (!view.ArtistaPrincipal.Equals(0)) {
                lista = lista.Where(v => v.video_artista.FirstOrDefault().id_artista == view.ArtistaPrincipal).ToList();
            }
            if (view.Artistas.Count > 0) {
                lista = lista.Where(v => v.video_artista.Any(va => view.Artistas.Select(t => t.id).ToArray().Contains(va.id_artista))).ToList();
            }
            if (view.Musicas.Count > 0) {
                lista = lista.Where(v => v.video_musica.Any(vm => view.Musicas.Select(t => t.id).ToArray().Contains(vm.id_musica))).ToList();
            }
            if (view.Tags.Count > 0) {
                lista = lista.Where(v => v.video_tag.Any(vt => view.Tags.Select(t => t.id).ToArray().Contains(vt.id_tag))).ToList();
            }
            if (view.Tipos.Count > 0) {
                lista = lista.Where(v => view.Tipos.Select(t=>t.id).ToArray().Contains(v.id_tipo)).ToList();
            }

            videosView.ListaVideos = lista.Distinct().OrderBy(v => v.titulo).ToList();

            return PartialView("VideoListView", videosView);
        }

        public void GerarPlaylistJquery(VideosView view) {
            VideoRepository videoRepository = new VideoRepository();
            VideosView videosView = new VideosView();
            var lista = new List<video>();

            lista = videoRepository.listarVideos();
            if (!view.ArtistaPrincipal.Equals(0)) {
                lista = lista.Where(v => v.video_artista.FirstOrDefault().id_artista == view.ArtistaPrincipal).ToList();
            }
            if (view.Artistas.Count > 0) {
                lista = lista.Where(v => v.video_artista.Any(va => view.Artistas.Select(t => t.id).ToArray().Contains(va.id_artista))).ToList();
            }
            if (view.Musicas.Count > 0) {
                lista = lista.Where(v => v.video_musica.Any(vm => view.Musicas.Select(t => t.id).ToArray().Contains(vm.id_musica))).ToList();
            }
            if (view.Tags.Count > 0) {
                lista = lista.Where(v => v.video_tag.Any(vt => view.Tags.Select(t => t.id).ToArray().Contains(vt.id_tag))).ToList();
            }
            if (view.Tipos.Count > 0) {
                lista = lista.Where(v => view.Tipos.Select(t => t.id).ToArray().Contains(v.id_tipo)).ToList();
            }
            
            Random rnd = new Random();
            lista = lista.Distinct().OrderBy(v => rnd.Next()).ToList();

            System.IO.File.WriteAllLines(@"K:\\ICI\\Vídeos\\kpop\\playlist.m3u", lista.Select(l=>l.caminho).ToArray());
        }

        public void AtualizarVideosJquery() {
            List<string> listaArquivos = new List<string>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                string[] arquivos = Directory.GetFiles(pasta);
                foreach (string arquivo in arquivos) {
                    FileInfo dados = new FileInfo(arquivo);
                    string[] extensoes = { ".mp4", ".mkv", ".ts", ".tp", ".avi", ".vob" };
                    if (extensoes.Contains(dados.Extension.ToLower())) {
                        listaArquivos.Add(arquivo);
                    }
                }
            }

            VideoRepository videoRepository = new VideoRepository();
            foreach (var arquivo in listaArquivos) {
                video video = videoRepository.findByCaminho(arquivo);
                if (video == null) {
                    videoRepository.salvar(arquivo);
                }
            }
        }

        public void PlayVideo(int id) {
            VideoRepository videoRepository = new VideoRepository();
            video video = videoRepository.getVideoById(id);
            System.Diagnostics.Process.Start(video.caminho);
        }

    }
}