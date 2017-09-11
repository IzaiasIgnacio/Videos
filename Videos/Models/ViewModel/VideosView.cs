using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;
using Videos.Models.Repository;

namespace Videos.Models.ViewModel {

    public class BaseVideoView {
        public string pastaCapturas;

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
    }

    public class VideoInfoView {
        public int Id { get; set; }
        public string descricao { get; set; }
        public string tipo { get; set; }
    }

    public class VideosView : BaseVideoView {
        public List<video> listaVideos;
        
        public tipo getTipoByCaminho(string caminho) {
            string[] split = caminho.Split('\\');
            string[] tipos = { "Lives", "MVs" };
            TipoRepository tipo = new TipoRepository();
            if (split.Length > 5) {
                if (tipos.Contains(split[5])) {
                    return tipo.GetTipoByDescricao(split[5]);
                }
            }
            return tipo.GetTipoByDescricao("misc"); 
        }

        public artista getArtistaByCaminho(string caminho) {
            ArtistaRepository artista = new ArtistaRepository();
            string[] split = caminho.Split('\\');
            return artista.GetArtistaByNome(split[4]);
        }

    }
}