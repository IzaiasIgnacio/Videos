using System.Web.Mvc;
using Videos.Models.Repository;
using Videos.Models.ViewModel;
using Videos.Models.Entity;
using Videos.Models.Services;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;

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
            foreach (video_artista video_artista in video.video_artista.Where(va=>va.principal == false)) {
                videoDataView.Artistas.Add(new artista {
                    id = video_artista.artista.id,
                    nome = video_artista.artista.nome
                });
            }
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
            videoDataView.Tamanho = Convert.ToInt32(video.tamanho);
            videoDataView.FormatoVideo = video.formato_video;
            videoDataView.Fps = video.fps;
            videoDataView.CanaisAudio = video.canais_audio;
            videoDataView.FormatoAudio = video.formato_audio;
            videoDataView.Thumbs = videoDataService.getThumbs(video.id);

            return PartialView("FormVideoView", videoDataView);
        }

        public ActionResult ExibirModalPlaylists(int id) {
            PlaylistRepository playlistRepository = new PlaylistRepository();
            PlaylistsView playlistsView = new PlaylistsView();

            playlistsView.ListaPlaylists = playlistRepository.Listar<playlist>().Where(pl => pl.tipo == "custom").ToList();
            playlistsView.id_video = id;

            return PartialView("ModalPlaylistsView", playlistsView);
        }

        private BaseVideoView getView(string model) {
            BaseVideoView view;

            if (model == "VideosView") {
                view = VideosView.GetVideosView();
            }
            else {
                view = VideoDataView.GetVideoDataView();
            }

            return view;
        }

        public ActionResult AdicionarArtistaPrincipalJquery(int id, string nome_artista, string model) {
            BaseVideoView view = getView(model);

            if (!view.ArtistaPrincipal.Any(a => a.id == id)) {
                view.ArtistaPrincipal.Add(new artista { id = id, nome = nome_artista });
            }

            return PartialView("ArtistaPrincipalListView", view);
        }

        public ActionResult RemoverArtistaPrincipalJquery(int id, string model) {
            BaseVideoView view = getView(model);

            view.ArtistaPrincipal.RemoveAll(a => a.id == id);

            return PartialView("ArtistaPrincipalListView", view);
        }

        public ActionResult AdicionarArtistaJquery(int id, string nome_artista, string model) {
            BaseVideoView view = getView(model);

            if (!view.Artistas.Any(a => a.id == id)) {
                view.Artistas.Add(new artista { id = id, nome = nome_artista });
            }

            return PartialView("ArtistaListView", view);
        }

        public ActionResult RemoverArtistaJquery(int id, string model) {
            BaseVideoView view = getView(model);

            view.Artistas.RemoveAll(a => a.id == id);

            return PartialView("ArtistaListView", view);
        }

        public ActionResult AdicionarMusicaJquery(string titulo_musica, string model) {
            BaseVideoView view = getView(model);

            MusicaRepository musicaRepository = new MusicaRepository();
            musica musica = musicaRepository.GetMusicaByTitulo(titulo_musica);

            if (!view.Musicas.Any(m => m.id == musica.id)) {
                view.Musicas.Add(musica);
            }

            return PartialView("MusicaListView", view);
        }

        public ActionResult RemoverMusicaJquery(string titulo_musica, string model) {
            BaseVideoView view = getView(model);

            MusicaRepository musicaRepository = new MusicaRepository();
            musica musica = musicaRepository.GetMusicaByTitulo(titulo_musica);

            if (musica != null) {
                view.Musicas.RemoveAll(m => m.id == musica.id);
            }

            return PartialView("MusicaListView", view);
        }

        public ActionResult AdicionarTagJquery(string nome_tag, string model) {
            BaseVideoView view = getView(model);

            TagRepository tagRepository = new TagRepository();
            tag tag = tagRepository.GetTagByNome(nome_tag);

            if (!view.Tags.Any(t => t.id == tag.id)) {
                view.Tags.Add(tag);
            }

            return PartialView("TagListView", view);
        }

        public ActionResult RemoverTagJquery(string nome_tag, string model) {
            BaseVideoView view = getView(model);
            TagRepository tagRepository = new TagRepository();
            tag tag = tagRepository.GetTagByNome(nome_tag);

            if (tag != null) {
                view.Tags.RemoveAll(t => t.id == tag.id);
            }

            return PartialView("TagListView", view);
        }

        public ActionResult AdicionarTipoJquery(int id, string descricao) {
            BaseVideoView view = getView("VideosView");
            view.Tipos.Add(new tipo { id = id, descricao = descricao });

            return PartialView("TipoListView", view);
        }

        public ActionResult RemoverTipoJquery(int id) {
            BaseVideoView view = getView("VideosView");
            view.Tipos.Remove(view.Tipos.Single(t => t.id == id));

            return PartialView("TipoListView", view);
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
            if (view.ArtistaPrincipal.Count > 0) {
                lista = lista.Where(v => v.video_artista.Any(va => view.ArtistaPrincipal.Select(t => t.id).ToArray().Contains(va.id_artista) && va.principal == true)).ToList();
            }
            if (view.Artistas.Count > 0) {
                foreach (var a in view.Artistas) {
                    lista = lista.Where(v => v.video_artista.Any(ar => ar.id_artista == a.id && ar.principal == false)).ToList();
                }
            }
            if (view.Musicas.Count > 0) {
                lista = lista.Where(v => v.video_musica.Any(vm => view.Musicas.Select(t => t.id).ToArray().Contains(vm.id_musica))).ToList();
            }
            if (view.Tags.Count > 0) {
                foreach (var t in view.Tags) {
                    lista = lista.Where(v => v.video_tag.Any(ta=>ta.id_tag == t.id)).ToList();
                }
            }
            if (view.Tipos.Count > 0) {
                lista = lista.Where(v => view.Tipos.Select(t=>t.id).ToArray().Contains(v.id_tipo)).ToList();
            }

            videosView.ListaVideos = lista.Distinct().OrderByDescending(v => v.id).ToList();
            videosView.totalVideos = videosView.ListaVideos.Count();

            return PartialView("VideoListView", videosView);
        }

        public void GerarPlaylistJquery(VideosView view) {
            List<int> ids = view.Playlist.Split(',').Select(Int32.Parse).ToList();
            VideoRepository videoRepository = new VideoRepository();
            
            var lista = new List<video>();

            lista = videoRepository.listarVideos();
            lista = lista.Where(v => ids.Contains(v.id)).ToList();
            
            Random rnd = new Random();
            lista = lista.Distinct().OrderBy(v => rnd.Next()).ToList();

            System.IO.File.WriteAllLines(@"K:\\Vídeos\\kpop\\"+view.NomePlaylist+ ".m3u", lista.Select(l=>l.caminho).ToArray());
        }
        
        public void AtualizarVideosJquery() {
            List<string> listaArquivos = new List<string>();
            string caminho = @"K:\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                string[] arquivos = Directory.GetFiles(pasta);
                foreach (string arquivo in arquivos) {
                    FileInfo dados = new FileInfo(arquivo);
                    string[] extensoes = { ".mp4", ".mkv", ".ts", ".tp", ".avi", ".vob", ".m2t", ".m2ts" };
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

            foreach (video video in videoRepository.Listar<video>()) {
                if (!System.IO.File.Exists(video.caminho)) {
                    videoRepository.excluir(video);
                }
            }
        }

        public void ExcluirVideoJquery(int id) {
            VideoRepository videoRepository = new VideoRepository();
            video video = videoRepository.getVideoById(id);
            videoRepository.excluirArquivo(video);
            videoRepository.excluir(video);
        }

        public void PlayVideo(int id) {
            VideoRepository videoRepository = new VideoRepository();
            video video = videoRepository.getVideoById(id);

            using (FileStream file = new FileStream(@"K:\\Vídeos\\kpop\\play.m3u", FileMode.Create)) {
                byte[] linha = Encoding.UTF8.GetBytes(Environment.NewLine);
                byte[] bytes = Encoding.UTF8.GetBytes(video.caminho);
                file.Write(bytes, 0, bytes.Length);
                file.Write(linha, 0, linha.Length);
            }
        }

        public void AtualizarPlaylist(int id) {
            VideoRepository videoRepository = new VideoRepository();
            PlaylistRepository playlistRepository = new PlaylistRepository();
            var playlist = playlistRepository.getPlaylistById(id);
            
            var lista = videoRepository.listarVideos();

            switch (playlist.tipo) {
                case "auto":
                    var artistas = playlist.playlist_filtros.Where(f => f.tipo == "artista").ToList();
                    if (artistas.Count > 0) {
                        lista = lista.Where(v => v.video_artista.Any(va => artistas.Select(t => Int32.Parse(t.valor)).ToArray().Contains(va.id_artista) && va.principal == true)).ToList();
                    }

                    var musicas = playlist.playlist_filtros.Where(f => f.tipo == "musica").ToList();
                    if (musicas.Count > 0) {
                        foreach (var m in musicas) {
                            lista = lista.Where(v => v.video_musica.Any(mu => mu.id_musica == Int32.Parse(m.valor))).ToList();
                        }
                    }

                    var tags = playlist.playlist_filtros.Where(f => f.tipo == "tag").ToList();
                    if (tags.Count > 0) {
                        foreach (var tag in tags) {
                            lista = lista.Where(v => v.video_tag.Any(ta => ta.id_tag == Int32.Parse(tag.valor))).ToList();
                        }
                    }

                    var tipos = playlist.playlist_filtros.Where(f => f.tipo == "tipo").ToList();
                    if (tipos.Count > 0) {
                        lista = lista.Where(v => tipos.Select(t => Int32.Parse(t.valor)).ToArray().Contains(v.id_tipo)).ToList();
                    }
                break;
                case "favoritos":
                    lista = lista.Where(v => v.favorito == true).ToList();
                break;
                case "recentes":
                    lista = lista
                            .Where(v => v.id_tipo == 1 || v.id_tipo == 2)
                            .Where(v => DateTime.Compare(v.data.Value, DateTime.Today.AddMonths(-6)) >= 0)
                            .ToList();
                break;
                case "custom":
                    var videos = playlist.playlist_filtros.Where(f => f.tipo == "video").ToList();
                    if (videos.Count > 0) {
                        lista = lista.Where(v => videos.Select(t => Int32.Parse(t.valor)).ToArray().Contains(v.id)).ToList();
                    }
                break;
                default:
                    return;
            }
            
            Random rnd = new Random();
            lista = lista.Distinct().OrderBy(v => rnd.Next()).ToList();

            System.IO.File.WriteAllLines(@"K:\\Vídeos\\kpop\\" + playlist.nome + ".m3u", lista.Select(l => l.caminho).ToArray());
        }

        public void SetVideoFavorito(int id, bool favorito) {
            VideoRepository videoRepository = new VideoRepository();
            videoRepository.setFavorito(id, !favorito);
        }

        public void AdicionarVideoPlaylist(int id_playlist, int id_video) {
            PlaylistRepository playlistRepository = new PlaylistRepository();
            playlistRepository.AdicionarVideoPlaylist(id_playlist, id_video);
        }

    }
    
}