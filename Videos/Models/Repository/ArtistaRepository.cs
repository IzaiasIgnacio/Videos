using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class ArtistaRepository : BaseRepository {

        public artista GetArtistaByNome(string nome) {
            artista artista = db.artista.Where(n => n.nome == nome).FirstOrDefault();
            return artista;
        }

        public List<String> listarPastasArtistas() {
            List<String> listaPastas = new List<string>();
            string caminho = @"K:\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho);

            foreach (var pasta in pastas) {
                DirectoryInfo info = new DirectoryInfo(pasta);
                listaPastas.Add(info.Name);
            }

            return listaPastas;
        }

        public List<artista> listarArtistas() {
            return db.artista.OrderBy(a => a.nome).ToList();
        }

        public void inserirArtista(string pasta) {
            string nomeArtista = pasta;
            artista artista;

            var db = new VideosEntities();
            artista = db.artista.FirstOrDefault(a => a.nome == nomeArtista);
            if (artista == null) {
                artista = new artista();
                db.artista.Add(artista);
                artista.nome = nomeArtista;
                db.SaveChanges();
            }
        }

    }
}