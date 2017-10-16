using System;
using System.Linq;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class MusicaRepository : BaseRepository {
        public musica GetMusicaByTitulo(string titulo) {
            titulo = System.Web.HttpUtility.HtmlDecode(titulo);
            musica musica = db.musica.Where(m => m.titulo == titulo).FirstOrDefault();
            if (musica == null) {
                musica = AdicionarMusica(titulo);
            }
            return musica;
        }

        private musica AdicionarMusica(string titulo) {
            musica musica = new musica { titulo = titulo };
            db.musica.Add(musica);
            db.SaveChanges();

            return musica;
        }
    }
}