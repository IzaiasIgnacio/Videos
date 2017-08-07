using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;
using Videos.Models.Repository;

namespace Videos.Models.ViewModel {
    public class VideosView {
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