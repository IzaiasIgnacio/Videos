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