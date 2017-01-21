using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class ArtistaRepository : BaseRepository {

        public List<String> listarPastasArtistas() {
            List<String> listaPastas = new List<string>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho);

            foreach (var pasta in pastas) {
                DirectoryInfo info = new DirectoryInfo(pasta);
                listaPastas.Add(info.Name);
            }

            return listaPastas;
        }

        public Dictionary<int, string> listarArtistas() {
            Dictionary<int, string> ListaArtista;
            ListaArtista = db.Artista.Select(a => new { a.id, a.artista }).OrderBy(a => a.artista)
                   .ToDictionary(a => a.id, a => a.artista);
            return ListaArtista;
        }

        public void inserirArtista(string pasta) {
            string nomeArtista = pasta;
            ArtistaEntity artistaEntity;

            var db = new VideosEntities();
            artistaEntity = db.Artista.FirstOrDefault(a => a.artista == nomeArtista);
            if (artistaEntity == null) {
                artistaEntity = new ArtistaEntity();
                db.Artista.Add(artistaEntity);
                artistaEntity.artista = nomeArtista;
                db.SaveChanges();
            }
        }

    }
}