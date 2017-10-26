using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Videos.Models.Entity;
using Videos.Models.Repository;

namespace Videos.Models.ViewModel {

    public class BaseVideoView {
        public string pastaCapturas;
        private List<artista> artistaPrincipal;
        public List<artista> ArtistaPrincipal {
            get {
                if (artistaPrincipal == null) {
                    artistaPrincipal = new List<artista>();
                }
                return artistaPrincipal;
            }
            set {
                artistaPrincipal = value;
            }
        }
        public List<artista> ListaArtistas { get; set; }
        private List<artista> artistas;
        public List<artista> Artistas {
            get {
                if (artistas == null) {
                    artistas = new List<artista>();
                }
                return artistas;
            }
            set {
                artistas = value;
            }
        }
        public List<musica> ListaMusicas { get; set; }
        private List<musica> musicas { get; set; }
        public List<musica> Musicas {
            get {
                if (musicas == null) {
                    musicas = new List<musica>();
                }
                return musicas;
            }
            set {
                musicas = value;
            }
        }
        public List<tag> ListaTags { get; set; }
        private List<tag> tags { get; set; }
        public List<tag> Tags {
            get {
                if (tags == null) {
                    tags = new List<tag>();
                }
                return tags;
            }
            set {
                tags = value;
            }
        }
        public List<tipo> ListaTipos { get; set; }
        private List<tipo> tipos { get; set; }
        public List<tipo> Tipos {
            get {
                if (tipos == null) {
                    tipos = new List<tipo>();
                }
                return tipos;
            }
            set {
                tipos = value;
            }
        }

        public BaseVideoView() {
            pastaCapturas = AppDomain.CurrentDomain.BaseDirectory.Replace("bin", "") + "capturas\\";
        }
    }

    public class VideoDataView : BaseVideoView {
        private static VideoDataView view;

        public static VideoDataView GetVideoDataView() {
            if (view == null) {
                view = new VideoDataView();
            }
            return view;
        }

        public static VideoDataView init() {
            view = new VideoDataView();

            return view;
        }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Duracao { get; set; }
        public string Resolucao { get; set; }
        public string FormatoVideo { get; set; }
        public string Fps { get; set; }
        public string CanaisAudio { get; set; }
        public string FormatoAudio { get; set; }
        public List<string> Thumbs { get; set; }
        public string ArtistaPrincipal { get; set; }
    }

    public class VideoInfoView {
        public int Id { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
    }

    public class VideosView : BaseVideoView {
        private static VideosView view;

        public static VideosView GetVideosView() {
            if (view == null) {
                view = new VideosView();
            }
            return view;
        }

        public static VideosView init() {
            view = new VideosView();

            return view;
        }
        public string pastaArtistas {
            get {
                return AppDomain.CurrentDomain.BaseDirectory.Replace("bin", "") + "artistas\\";
            }
        }
        public string caminhoArtistas {
            get {
                return ConfigurationManager.AppSettings["pastaArtistas"];
            }
        }

        public int totalVideos { get; set; }
        public bool favorito { get; set; }
        public List<video> ListaVideos { get; set; }
        public string Playlist { get; set; }
        public string NomePlaylist { get; set; }
        public string getLink(video video) {
            string artista = video.video_artista.Where(a => a.principal == true).FirstOrDefault().artista.nome;
            string tipo = video.tipo.descricao;
            if (tipo.ToLower() == "live" || tipo.ToLower() == "mv") {
                tipo += "s";
            }
            string arquivo = video.titulo + video.extensao;
            return "http://127.0.0.1:8887/"+artista + "/" + tipo + "/" + arquivo;
        }
    }
}